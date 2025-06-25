using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Classic;
using Libs;
/// <summary>
/// statistics data module.
/// 包括命令统计和全局状态统计
/// 目前只是将与事件命令相关的移植过来，在随后的开发过程中逐步移植过来
/// added By qing.liu
/// </summary>
public class StatisticsDataModule {

	public static readonly string CMD_LOCAL_PERSISTENCE_DATA = "CmdLocalPersistenceData";
	public static readonly string CMD_RUNTIME_DATA = "CmdRuntimeData";
	/// <summary>
	/// The temp cmd state dict.
	/// 只有注册过的类型统计才会更新，没有注册的话，cmd统计状态是不会跟新的
	/// 必须在cmdItem创建时，就开始对其进行注册
	/// </summary>
	private Dictionary<string,Dictionary<string,object>> tempCmdStateDict = new Dictionary<string, Dictionary<string, object>>();
	private Dictionary<string,Dictionary<string,object>> savedCmdStateDict = new Dictionary<string, Dictionary<string, object>> ();
	private StatisticsDataModule(){
		
	}

	public static StatisticsDataModule Instance{
		get{ 
			return Singleton<StatisticsDataModule>.Instance;
		}
	}

	public void UpdateCmdState(string cmdName,Dictionary<string,object> data){
		if (cmdName==null||data==null) {
			return;
		}

		if (tempCmdStateDict.ContainsKey(cmdName)) {
			foreach (string type in data.Keys) {
				if (tempCmdStateDict[cmdName].ContainsKey(type)) {
					tempCmdStateDict [cmdName] [type] = data [type];
				}
			}
		}
		if (savedCmdStateDict.ContainsKey(cmdName)) {
			foreach (string type in data.Keys) {
				if (savedCmdStateDict[cmdName].ContainsKey(type)) {
					savedCmdStateDict [cmdName] [type] = data [type];
				}
			}
		}
	}

	public void RegisterCmd(string cmdName,Dictionary<string,Dictionary<string,object>> info){
		if (cmdName==null||info==null) {
			return;
		}
		if (info.ContainsKey(CMD_LOCAL_PERSISTENCE_DATA)&&info[CMD_LOCAL_PERSISTENCE_DATA]!=null) {
			if (!savedCmdStateDict.ContainsKey(cmdName)) {
				savedCmdStateDict.Add (cmdName,info[CMD_LOCAL_PERSISTENCE_DATA]);
			}
			else
				foreach (string type in info[CMD_LOCAL_PERSISTENCE_DATA].Keys) {
					if(!savedCmdStateDict[cmdName].ContainsKey(type)){
						Dictionary<string,object> dict = new Dictionary<string, object> ();
						dict.Add (type,info[CMD_LOCAL_PERSISTENCE_DATA][type]);
						savedCmdStateDict.Add (cmdName,dict);
					}
				}
		}
		if (info.ContainsKey(CMD_RUNTIME_DATA)&&info[CMD_RUNTIME_DATA]!=null) {
			if (!tempCmdStateDict.ContainsKey(cmdName)) {
				tempCmdStateDict.Add (cmdName,info[CMD_RUNTIME_DATA]);
			}
			else
				foreach (string type in info[CMD_RUNTIME_DATA].Keys) {
					if(!tempCmdStateDict[cmdName].ContainsKey(type)){
						Dictionary<string,object> dict = new Dictionary<string, object> ();
						dict.Add (type,info[CMD_RUNTIME_DATA][type]);
						tempCmdStateDict.Add (cmdName,dict);
					}
				}
		}
	}

	/// <summary>
	/// Saves the state of the cmd to Local.
	/// exit app,from frontground to background
	/// </summary>
	public void SaveCmdState(){
		if (savedCmdStateDict.Keys.Count > 0) {
			string savedData = MiniJSON.Json.Serialize (savedCmdStateDict);
			SharedPlayerPrefs.SetPlayerPrefsStringValue (CMD_LOCAL_PERSISTENCE_DATA,savedData);
			SharedPlayerPrefs.SavePlayerPreference ();
		}
	}
	/// <summary>
	/// Loads the state of the cmd From Local.
	/// launch app
	/// </summary>
	public void LoadCmdState(){
		savedCmdStateDict.Clear ();
		string savedCmdData = SharedPlayerPrefs.GetPlayerPrefsStringValue (CMD_LOCAL_PERSISTENCE_DATA,"");
		if (!string.IsNullOrEmpty(savedCmdData)) {
			Dictionary<string,object> data =  MiniJSON.Json.Deserialize (savedCmdData) as Dictionary<string,object>;
			if (data!=null) {
				foreach (string cmd in data.Keys) {
					if (data[cmd]!=null) {
						Dictionary<string,object> info = data [cmd] as Dictionary<string,object>;
						if (info!=null) {
							savedCmdStateDict.Add (cmd,info);
						}
					}
				}
			}
		}
	}

	public int GlobalSpinNum = 0;
}

public class StatDataType{
	public const string MACHINE_SESSION ="MachineSession";//bool
	public const string APP_SESSION="AppSession"; //bool
}