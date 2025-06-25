using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using Classic;
using LuaFramework;

public class StatisticsManager{

	#region structor
	public StatisticsManager(){
		Init ();
	}
	#endregion
	#region Field

	public const string MACHINE_REAL_RTP = "MACHINE_REAL_RTP";
	private double winCoinsNum;
	private long costCoinsNum;
	private string machineName;
	private int spinNum;
	#endregion
	#region Property
	public double WinCoinsNum { 
		get {
			return winCoinsNum;
		}
		set {
			winCoinsNum = value;
		}
	}


	public long CostCoinsNum {
		get {
			return costCoinsNum;
		}
		set {
			costCoinsNum = value;
		}
	}

	public int SpinNum {
		get {
			return spinNum;
		}
		set {
			spinNum = value;
		}
	}

	public int AutoSpinNum {
		set;
		get;
	}

	public System.DateTime StartTime {
		set;
		get;
	}
	public float CostMoneyInSession {
		get;
		set;
	}
	#endregion

	#region Method

	public void Send_Machine_Real_RTP(System.DateTime startTime,string strReason){
		machineName = SceneManager.GetActiveScene ().name;
		if (spinNum > 0)
		{
			float costTime = Mathf.Max ((long)(System.DateTime.Now - startTime).TotalSeconds, 0L);
			Dictionary<string,object> parms = new Dictionary<string, object>();
			parms.Add("CostCoinsNum", costCoinsNum);
			parms.Add("WinCoinsNum", winCoinsNum);
			parms.Add("SpinNum", spinNum);

			parms.Add("SessionCostTime",costTime );

			Analytics.GetInstance().LogEvent(MACHINE_REAL_RTP, parms);
			Dictionary<string,object> parms2 = new Dictionary<string, object>(parms);
			parms2.Add("Reason", strReason);
			BaseGameConsole.ActiveGameConsole().LogBaseEvent(MACHINE_REAL_RTP, parms2);
		}

		SendAutoSpinTimesEvent ();

		//保存spin的次数
		if (!string.IsNullOrEmpty (machineName) && spinNum >0) {
			UserManager.GetInstance ().SaveMachineSpinTimes (machineName, spinNum);
			UserManager.GetInstance ().SaveFirstPlayedSlotMachineSpinCount (spinNum);
		}
		Init ();
	}
	private void Init(){
		winCoinsNum  = 0;
		costCoinsNum = 0;
		spinNum      = 0;
		machineName = "";
		AutoSpinNum = 0;
		//StartTime = System.DateTime.Now;
	}


	public  void SendAutoSpinTimesEvent()
	{
		if (AutoSpinNum > 0) {
			Analytics.GetInstance ().LogEvent (Analytics.AutoSpinCounts, Analytics.AutoSpinCounts, AutoSpinNum); 
			AutoSpinNum = 0;
		}
	}
	#endregion
}
