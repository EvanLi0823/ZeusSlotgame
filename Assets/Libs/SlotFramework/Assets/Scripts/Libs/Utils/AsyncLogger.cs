using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using Libs;
/// <summary>
/// Async logger.
/// 这里有两种实现思路：
/// 第一种是直接用C#的Thread实现，在导出Android或者IOS时，会转换为相应的平台代码
/// 第二种是如果线程这块无法进行转换的话，则采用Native Plugins的方式，将此块代码移植到相应的平台代码中，然后通过Native Plugin方式来传递参数或者调用方法
/// 采用的技术:doubleBufferQueue,Thread
/// </summary>
public class AsyncLogger {
	public const string TIME_FORMAT = "MM/dd/yyyy HH:mm:ss.fff";
	public LOG_LEVEL currentLogLevel = LOG_LEVEL.LOG_LEVEL_VERBOSE;
	public enum LOG_LEVEL
	{
		LOG_LEVEL_MIN = 0,
		LOG_LEVEL_VERBOSE = 1,
		LOG_LEVEL_DEBUG = 2,
		LOG_LEVEL_INFO = 3,
		LOG_LEVEL_WARNING = 4,
		LOG_LEVEL_ERROR = 5,
		LOG_LEVEL_ASSERT = 6,
		LOG_LEVEL_MAX = 7
	}
	/*线程中申请需要的参数*/
	private Queue<string> evtQ1;
	private Queue<string> evtQ2;
	private volatile Queue<string> curMessageQueue;
	private string savePath;
	private Dictionary<string,LogMessage> eventInfo;

	private bool EnableDebug = true;
	private string ConvertData(string eventName){
		LogMessage LM = eventInfo [eventName];
		Dictionary<string,object> eventLogInfo = new Dictionary<string, object> ();
		eventLogInfo.Add ("eventName",LM.eventName);
		eventLogInfo.Add ("startTime",LM.startTime.ToString(TIME_FORMAT));
		eventLogInfo.Add ("endTime",LM.endTime.ToString(TIME_FORMAT));
		eventLogInfo.Add ("duration",LM.duration);
		eventLogInfo.Add ("maxThreshold",LM.maxThreshold);
		eventLogInfo.Add ("eventLevel",LM.eventLevel);
		return MiniJSON.Json.Serialize (eventLogInfo);
	}

	public class LogMessage
	{
		public string eventName;
		public DateTime startTime;
		public DateTime endTime;
		public float duration;
		public float maxThreshold; 
		public LOG_LEVEL eventLevel;
	}

	public void StartTraceLog(string eventName,float delayThreshold=0f,LOG_LEVEL evtLvl = LOG_LEVEL.LOG_LEVEL_VERBOSE)
	{

		return;
		
		if (EnableLOG()) {
			if (evtLvl >= currentLogLevel) {
				if (!eventInfo.ContainsKey(eventName)) {
					LogMessage LM = new LogMessage ();
					LM.eventName = eventName;
					LM.startTime = DateTime.Now;
					LM.maxThreshold = delayThreshold;
					LM.eventLevel = evtLvl;
					eventInfo.Add (eventName,LM);
				} 
				else {
					//Debug.LogError ("eventName:"+eventName+" is repeat!!!");
				}
			}
		}
		if (EnableDebug) {
			Debug.Log (MiniJSON.Json.Serialize("Start eventName:"+eventName+" LT:"+DateTime.Now.ToString(TIME_FORMAT)));
		}
	}


	public void EndTraceLog(string eventName,LOG_LEVEL evtLvl = LOG_LEVEL.LOG_LEVEL_VERBOSE)
	{
		return;
		if (EnableLOG()) {
			if (evtLvl >= currentLogLevel ) {
				if (!eventInfo.ContainsKey(eventName)) {
					LogMessage LM = new LogMessage ();
					LM.eventName = eventName;
					LM.startTime = DateTime.Now;
					LM.maxThreshold = 0;
					LM.eventLevel = currentLogLevel;
					eventInfo.Add (eventName,LM);
				} 
				else {
					eventInfo [eventName].endTime = DateTime.Now;
					eventInfo [eventName].duration = (float)(eventInfo [eventName].endTime - eventInfo [eventName].startTime).TotalSeconds;
				}
				AddLog (ConvertData(eventName));
				eventInfo.Remove (eventName);
			}
		}
		if (EnableDebug) {
			Debug.Log (MiniJSON.Json.Serialize("END eventName:"+eventName+" LT:"+DateTime.Now.ToString(TIME_FORMAT)));
		}
	}

	private void AddLog(string eventData){
		unblockHandlerEvent.WaitOne();
		handlerFinishedEvent.Reset();

		curMessageQueue.Enqueue(eventData);

		//Sets the state of the event to signaled, allowing one or more waiting threads to proceed.
		dataAvailableEvent.Set();
		handlerFinishedEvent.Set();
	}

	public void SaveData()
	{
		return;
		if (EnableLOG()) {
			waitEvent.Set ();
		}
	}
	/*线程处理方法*/
	private void HandleLogMessage(){
		Queue<string> tempWriteQueue;
		while (!isExitThread) {
			/*等待事件激活*/
			waitEvent.WaitOne ();

			dataAvailableEvent.WaitOne();

			unblockHandlerEvent.Reset(); // block the producer
			handlerFinishedEvent.WaitOne(); // wait for the producer to finish
			tempWriteQueue = curMessageQueue;
			curMessageQueue = (curMessageQueue == evtQ1) ? evtQ2 : evtQ1; // switch the write queue
			unblockHandlerEvent.Set(); // unblock the producer
			WriteLogToFile (tempWriteQueue);

		}
	}

	private void WriteLogToFile(Queue<string> messageQueue){
		if (messageQueue.Count>0) {
			try {
				string pathName = Path.GetDirectoryName (savePath);
				if (!Directory.Exists(pathName)) {
					Directory.CreateDirectory (pathName);
				}
				bool initInfo = false;
				if (!File.Exists(savePath)) {
					initInfo = true;
				}
				FileStream fs = new FileStream (savePath,FileMode.Append,FileAccess.Write);
				StreamWriter sw = new StreamWriter (fs);
				sw.AutoFlush = false;
				if (initInfo) {
					sw.WriteLine(initData);
				}
				while (messageQueue.Count>0) {
					sw.WriteLine (messageQueue.Dequeue());
				}
				sw.Flush ();
				sw.Close ();
				fs.Close ();
			} catch (System.Exception ex) {
				Debug.Log ("Init Write Log Exception:"+ex.Message);
			}
		}

		//Sets the state of the event to nonsignaled, causing threads to block.
		waitEvent.Reset ();
	}

	private static AsyncLogger instance;

	private Thread writeLogger = null;
	/*线程结束控制,软退出*/
	private bool isExitThread = false;

	/*线程控制量，等待申请事件*/
	private AutoResetEvent waitEvent;
	private ManualResetEvent handlerFinishedEvent;
	private ManualResetEvent unblockHandlerEvent;
	private AutoResetEvent dataAvailableEvent;

	public static AsyncLogger Instance {
		get{ 
			if (instance==null) {
				instance = new AsyncLogger ();
			}
			return instance;
		}
	}


	private AsyncLogger()
	{
		return;
		#if UNITY_EDITOR
		currentLogLevel = LOG_LEVEL.LOG_LEVEL_VERBOSE;
		EnableDebug = false;
		#elif DEBUG
		currentLogLevel = LOG_LEVEL.LOG_LEVEL_VERBOSE;
		EnableDebug = false;
		#else
		currentLogLevel = LOG_LEVEL.LOG_LEVEL_MAX;
		EnableDebug = false;
		#endif
		if (EnableLOG()) {
			InitData ();
			StartThread ();
		}
	}

	~AsyncLogger()
	{
		return;
		if (EnableLOG()) {
			EndThread ();
		}
	}

	public bool EnableLOG(){
		return currentLogLevel < LOG_LEVEL.LOG_LEVEL_MAX;
	}
	string initData;
	private void InitData(){
		evtQ1 = new Queue<string> ();
		evtQ2 = new Queue<string> ();
		curMessageQueue = evtQ1;
		eventInfo = new Dictionary<string, LogMessage> ();

		handlerFinishedEvent = new ManualResetEvent(true);
		unblockHandlerEvent = new ManualResetEvent(true);
		dataAvailableEvent = new AutoResetEvent(false);
		waitEvent = new AutoResetEvent(false);

		savePath = AssetsPathManager.GetMachineLocalLogFileSavePath ();
		Dictionary<string,object> info = new Dictionary<string, object> ();
		info.Add ("app_version",Application.version);
		info.Add ("local_time",TimeUtils.GetLocalTime(true));
		info.Add ("device_model",SystemInfo.deviceModel);
		//info.Add ("device_name",SystemInfo.deviceName);
		//info.Add ("device_type",SystemInfo.deviceType);
		info.Add ("platform",AssetsPathManager.GetPlatformName());
		initData = MiniJSON.Json.Serialize(info);
	

	}

	private void StartThread()
	{
		return;
		writeLogger = new Thread (new ThreadStart(HandleLogMessage));
		writeLogger.IsBackground = true;
		writeLogger.Priority = System.Threading.ThreadPriority.Normal;
		writeLogger.Start ();
	}

	private void EndThread(){
		if (writeLogger!=null&&writeLogger.IsAlive) {
			isExitThread = true;
		}
	}

}
