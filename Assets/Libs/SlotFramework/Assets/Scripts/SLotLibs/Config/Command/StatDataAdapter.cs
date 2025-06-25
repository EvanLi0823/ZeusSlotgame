using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Classic;
/// <summary>
/// Stat data adapter.
///must stat in logic end, middle or high layer abstract concept may be interrupt
///eg popup dialog
/// </summary>
public class StatDataAdapter {
	private StatDataAdapter(){
		InitStatState ();
		Messenger.AddListener<System.Action,object> (GameDialogManager.OpenSceneExchange,ResetMachineSessionState );
	}
	~StatDataAdapter(){
		Messenger.RemoveListener<System.Action,object> (GameDialogManager.OpenSceneExchange, ResetMachineSessionState);
	}
	public static StatDataAdapter Instance{
		get{ 
			return Singleton<StatDataAdapter>.Instance;
		}
	}

	public void InitStatState(){
		if (machineSessionEventActionCount==null) {
			machineSessionEventActionCount = new Dictionary<string, int> ();
		} 
		else {
			machineSessionEventActionCount.Clear ();
		}

		if (machineSessionEventLastSpinCount==null) {
			machineSessionEventLastSpinCount = new Dictionary<string, int> ();
		} 
		else {
			machineSessionEventLastSpinCount.Clear ();
		}

		if (globalEventActionCount==null) {
			globalEventActionCount = new Dictionary<string, int> ();
		} 
		else {
			globalEventActionCount.Clear ();
		}

		if (globalEventLastSpinCount==null) {
			globalEventLastSpinCount = new Dictionary<string, int> ();
		} 
		else {
			globalEventLastSpinCount.Clear ();
		}

		if (globalCMDActionCount==null) {
			globalCMDActionCount = new Dictionary<string, int> ();
		} else {
			globalCMDActionCount.Clear ();
		}

		if (globalCMDLastSpinCount==null) {
			globalCMDLastSpinCount = new Dictionary<string, int> ();
		} else {
			globalCMDLastSpinCount.Clear ();
		}
	}
	/// <summary>
	/// Updates the event stat.when excute the event action
	/// </summary>
	/// <param name="eventName">Event name.</param>
	public void UpdateEventStat(string eventName){
		if (!machineSessionEventActionCount.ContainsKey(eventName)) {
			machineSessionEventActionCount.Add (eventName,1);
		} else {
			machineSessionEventActionCount [eventName]++;
		}

		if (!machineSessionEventLastSpinCount.ContainsKey(eventName)) {
			machineSessionEventLastSpinCount.Add (eventName,GlobalSpinNum);
		} else {
			machineSessionEventLastSpinCount [eventName] = GlobalSpinNum;
		}

		if (!globalEventActionCount.ContainsKey(eventName)) {
			globalEventActionCount.Add (eventName, 1);
		} else {
			globalEventActionCount [eventName]++;
		}

		if (!globalEventLastSpinCount.ContainsKey(eventName)) {
			globalEventLastSpinCount.Add (eventName,GlobalSpinNum);
		} else {
			globalEventLastSpinCount [eventName] = GlobalSpinNum;
		}
	}
	/// <summary>
	/// Updates the cmd stat.when excute the cmd
	/// </summary>
	/// <param name="cmdToken">Cmd token.</param>
	public void UpdateCmdStat(string cmdToken){
		if (!globalCMDLastSpinCount.ContainsKey(cmdToken)) {
			globalCMDLastSpinCount.Add (cmdToken,1);
		} else {
			globalCMDLastSpinCount [cmdToken]++;
		}

		if (!globalCMDLastSpinCount.ContainsKey(cmdToken)) {
			globalCMDLastSpinCount.Add (cmdToken,GlobalSpinNum);
		} else {
			globalCMDLastSpinCount [cmdToken] = GlobalSpinNum;
		}

	}
	public int GetSessionSpecialEventActionCount(string eventName){
		if (string.IsNullOrEmpty(eventName)) {
			return 0;
		}
		if (machineSessionEventActionCount.ContainsKey(eventName)) {
			return machineSessionEventActionCount[eventName];
		} else {
			machineSessionEventActionCount.Add (eventName,0);
		}
		return 0;
	}
	public int GetSessionSpecialLastSpinCount(string eventName){
		if (string.IsNullOrEmpty(eventName)) {
			return 0;
		}
		if (machineSessionEventLastSpinCount.ContainsKey(eventName)) {
			return machineSessionEventLastSpinCount [eventName];
		} else {
			machineSessionEventLastSpinCount.Add (eventName, 0);
		}
		return 0;
	}
	public int GetGlobalSpecialEventActionCount(string eventName){
		if (string.IsNullOrEmpty(eventName)) {
			return 0;
		}
		if (globalEventActionCount.ContainsKey(eventName)) {
			return globalEventActionCount [eventName];
		} else {
			globalEventActionCount.Add (eventName, 0);
		}
		return 0;
	}

	public int GetGlobalSpecialLastSpinCount(string eventName){
		if (string.IsNullOrEmpty(eventName)) {
			return 0;
		}
		if (globalEventLastSpinCount.ContainsKey(eventName)) {
			return globalEventLastSpinCount [eventName];
		} else {
			globalEventLastSpinCount.Add (eventName, 0);
		}
		return 0;
	}

	public int GetGlobalSpecialCmdExecCount(string cmdToken){
		if (string.IsNullOrEmpty(cmdToken)) {
			return 0;
		}
		if (globalCMDActionCount.ContainsKey(cmdToken)) {
			return globalCMDActionCount [cmdToken];
		} else {
			globalCMDActionCount.Add (cmdToken, 0);
		}
		return 0;
	}
	public int GetGlobalSpecialCmdLastSpinCount(string cmdToken){
		if (string.IsNullOrEmpty(cmdToken)) {
			return 0;
		}
		if (globalCMDLastSpinCount.ContainsKey(cmdToken)) {
			return globalCMDLastSpinCount [cmdToken];
		} else {
			globalCMDLastSpinCount.Add (cmdToken, 0);
		}
		return 0;
	}
	private void ResetMachineSessionState(System.Action closeCallBack,object data){
		if (machineSessionEventActionCount!=null) {
			List<string> buffer = new List<string> (machineSessionEventLastSpinCount.Keys);
			foreach (string key in buffer) {
				machineSessionEventActionCount [key] = 0;
			}
		}

		if (machineSessionEventLastSpinCount!=null) {
			List<string> buffer = new List<string> (machineSessionEventLastSpinCount.Keys);
			foreach (string key in buffer) {
				machineSessionEventLastSpinCount [key] = 0;
			}
		}

	}

	public int GlobalSpinNum = 0;
	public string CurrentMoment="";

	private Dictionary<string,int> machineSessionEventActionCount;
	private Dictionary<string,int> machineSessionEventLastSpinCount;
	private Dictionary<string,int> globalEventActionCount;
	private Dictionary<string,int> globalEventLastSpinCount;
	private Dictionary<string,int> globalCMDActionCount;
	private Dictionary<string,int> globalCMDLastSpinCount;

}
