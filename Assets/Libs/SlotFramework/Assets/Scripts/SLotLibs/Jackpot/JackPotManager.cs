using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Libs;
using System;

public class JackPotManager
{
	public static string JACKPOT_DATA_MACHINE_CHANGE = "JackPotDataMachineChange";
	public static string JACKPOT_DATA_COMMON_CHANGE = "JackPotDataCommonChange";
	private readonly string LOCAL_HIT_JACKPOT = "hasJackpotHitShow";

	private const string KEY_LOCAL_SHOWN_IDS = "jackpotHasShowIds";

	protected static JackPotManager _Instance;
	private List<SlotMachineConfig> NeedDoWithJackPotSlotConfigs = new List<SlotMachineConfig> ();
	private static object syncRoot = new object ();
	private int allWeight = 0;

	private int JackpotCalculationInterval = 10;
	

    private int _rspJackpotLevel = 0;
    //此属性只要调用到，就已经是合法值了，所以不需要单独在判定
    
    private WaitForSeconds m_intervalSeconds;

	private System.DateTime m_JackpotChangeTimeStamp;
	private bool m_hasInit = false;

	public static JackPotManager Instance {
		get {
			if (_Instance == null) {
				lock (syncRoot) {
					if (_Instance == null) {
						_Instance = new JackPotManager ();
					}
				}
			}
			return _Instance;
		}
	}
	
	public void Init ()
	{
		InitConfig();
		//Special Jackpot
		List<SlotMachineConfig> lists = BaseGameConsole.ActiveGameConsole ().SlotMachines ().FindAll (delegate(SlotMachineConfig obj) {
			return obj.JackPotIncreaseConfigs.Count > 0;	
		});

		this.NeedDoWithJackPotSlotConfigs = new List<SlotMachineConfig> (lists);
		if (this.NeedDoWithJackPotSlotConfigs.Count == 0) {
			return;
		}
		
		ReadFromLocal ();

		if (m_hasInit) {
			return;
		}
		m_hasInit = true;
		StartJackPot ();
	}

	private void InitConfig()
	{
		JackpotCalculationInterval = Utils.Utilities.CastValueInt(Plugins.Configuration.GetInstance().GetValueWithPath<int>("Module/Jackpot/JackpotCalculationInterval", 60));
		m_intervalSeconds = new WaitForSeconds(JackpotCalculationInterval);
	}
	public void StartJackPot ()
	{
		if (this.NeedDoWithJackPotSlotConfigs.Count > 0) {
			Libs.CoroutineUtil.Instance.StartCoroutine (IncreaseJackPot ());
		}
	}

	public void PauseJackPot ()
	{
		SaveToLocal ();
	}

	public void RestoreJackPot ()
	{
	
	}

	public int GetLastTime ()
	{
		long tmp = TimeUtils.SecondsBetween (m_JackpotChangeTimeStamp, System.DateTime.Now);
		int result = (int)tmp;
		result = Mathf.Max (0, Mathf.Min (result, JackpotCalculationInterval));
		return result;
	}
	

	private IEnumerator IncreaseJackPot ()
	{
		while (true) {
			m_JackpotChangeTimeStamp = System.DateTime.Now;
			SecondsMachine ();
			PushInterval ();
			yield return m_intervalSeconds;
		}
	}
	
	private void SecondsMachine ()
	{
		for (int i = 0; i < NeedDoWithJackPotSlotConfigs.Count; i++) {
			SlotMachineConfig slotMachineConfig = NeedDoWithJackPotSlotConfigs [i];
			List<JackPotIncreaseMachineConfig> allJackPotSlotConfigs = slotMachineConfig.JackPotIncreaseConfigs;
			for (int j = 0; j < allJackPotSlotConfigs.Count; j++) {
				JackPotIncreaseMachineConfig jackPotConfig = allJackPotSlotConfigs [j];
				int randomNum = UnityEngine.Random.Range (0, jackPotConfig.ClearFrequencyMaxNum / JackpotCalculationInterval);
				JackPotPrizePool prizePool = slotMachineConfig.GetJackPotPool(jackPotConfig.JackPotType);
                //只有IncreaseEverySeconds != 0，随时间自动增长的jackpot才会随机清零
                if (randomNum == 0 && jackPotConfig.IncreaseEverySeconds != 0)
                {
	                prizePool.SaveExtraAward(0);
					Messenger.Broadcast<SlotMachineConfig,int,double> (JACKPOT_DATA_MACHINE_CHANGE, slotMachineConfig, prizePool.JackPotIndex, 0);

				} else {
					//add every second
					if (jackPotConfig.IncreaseEverySeconds != 0) {
						prizePool.AddPoolAwardEachTime (jackPotConfig.IncreaseEverySeconds, JackpotCalculationInterval);
					}
				}
			}
		}
	}
	

	#region 发送jackpot推送相关

	private const int SavePushCount = 30;
	public Stack<WinPushData> m_JackpotPushDatas = new Stack<WinPushData> ();
	public List<string> m_CurrentShow_Strs = new List<string> ();

	private void PushInterval ()
	{
		if (App.SubSystems.Settings.GetInstance ().IsWinnerAnnounce ()) {
			WinPushData pushData = GetNextPushData ();
			if (pushData != null) {
				Messenger.Broadcast<WinPushData> (GameDialogManager.OPEN_WIN_PUSH, pushData);
				if (m_CurrentShow_Strs.Count > SavePushCount) {
					m_CurrentShow_Strs.RemoveAt (0);
				}
				m_CurrentShow_Strs.Add (pushData.Show_Str);
			}
		} else {
			m_JackpotPushDatas.Clear ();
		}
	}

	//从服务器请求回来的jackpot数据，用于推送玩家jackpot中奖信息，单机游戏使用不到
	public void PaseResponseData (Dictionary<string,object> responseDataDict)
	{
		//JACKPOT
		Dictionary<string,object> jackpotDict = Utils.Utilities.GetValue<Dictionary< string,object>> (responseDataDict, GameConstants.KEY_JACKPOT, null);

		if (jackpotDict != null) {
			long timeStamp = Utils.Utilities.GetValue<long> (jackpotDict, GameConstants.TIME_STAMP, 0);


			List<object> jackpotDatas = Utils.Utilities.GetValue<List<object>> (jackpotDict, GameConstants.data_Key, null);

			if (jackpotDatas != null) {
				ParseJackpotData (jackpotDatas);
			}
		}
	}

	private void ParseJackpotData (List<object> jackpotDatas)
	{
		for (int i = 0; i < jackpotDatas.Count; i++) {
			Dictionary<string,object> dict = jackpotDatas [i] as Dictionary<string,object>;
			if (dict == null) {
				continue;
			}

			WinPushData pushData = new WinPushData (dict);
			if (pushData.IsValidPush ()) {
				if (HasPushedData (pushData.Show_Str)) {
					continue;
				}

				m_JackpotPushDatas.Push (pushData);
			}

		}
	}

	private WinPushData GetNextPushData ()
	{
		WinPushData pushData_result = null;
		while (m_JackpotPushDatas.Count > 0) {
			WinPushData pushData = m_JackpotPushDatas.Pop ();
			if (pushData.IsValidPush () && !HasPushedData (pushData.Show_Str)) {
				pushData_result = pushData;
				break;
			}
		}
		return pushData_result;

	}

	private bool HasPushedData (string showStr)
	{
		return m_CurrentShow_Strs.Exists (delegate(string obj) {
			return obj.Equals (showStr);
		});
	}

	private void ReadFromLocal ()
	{
		string localStr = SharedPlayerPrefs.GetPlayerPrefsStringValue (KEY_LOCAL_SHOWN_IDS, "");
		if (string.IsNullOrEmpty (localStr)) {
			return;
		}
		string[] allStr = localStr.Split (',');
		m_CurrentShow_Strs = new List<string> (allStr);

	}

	private void SaveToLocal ()
	{
		if (m_CurrentShow_Strs.Count == 0) {
			return;
		}
		string joinNames = String.Join (",", m_CurrentShow_Strs.ToArray ());
		SharedPlayerPrefs.SetPlayerPrefsStringValue (KEY_LOCAL_SHOWN_IDS, joinNames);
	}

	#endregion
}

//{
//	"data": {
//		"jackpots": {
//			"data": [
//				{
//					"timestamp": 1234567890123,  # 中奖时间戳，毫秒
//					"mid": "",
//					"app_id": 703,
//					"win_coins": 1000,
//					"machine": "",  # 中奖机器
//					"avatar_url": ""，
//					"app_version": "2.5.3"
//				}
//			],
//			"timestamp": 1234567890，  # jackpots数据最新时间
//		}
//	}
//}


public class WinPushData
{
	const string KEY_WIN_COINS = "win_coins";
	const string KEY_MACHINE = "machine";
	const string KEY_JACKPOT_LEVEL = "jackpot_level";
	
	
	private const string KEY_JACKPOT_TYPE = "jackpot_type";
	public long TimeStamp;
	public string Mid;
	public long WinCoins;
	public string MachineName;
	public string HeadUrl;
	public string NickName;
	public string AppVersion;
	public int AppId;
	public string Show_Str;
	public int JackpotLevel;
	public string jackpotType;
	public SlotMachineConfig SlotConfig;

	public WinPushData(Dictionary<string, object> dict)
	{
		TimeStamp = Utils.Utilities.GetLong(dict, GameConstants.TIME_STAMP, 0);
		Mid = Utils.Utilities.GetValue<string>(dict, GameConstants.KEY_MAIN_ID, "");
		WinCoins = Utils.Utilities.GetLong(dict, KEY_WIN_COINS, 0);
		MachineName = Utils.Utilities.GetValue<string>(dict, KEY_MACHINE, string.Empty);
		HeadUrl = Utils.Utilities.GetValue<string>(dict, GameConstants.KEY_TOUR_AVATAR, "");
		NickName = Utils.Utilities.GetValue<string>(dict,GameConstants.KEY_TOUR_NAME, "");

		JackpotLevel = Utils.Utilities.GetInt(dict, KEY_JACKPOT_LEVEL, 0);
		jackpotType = Utils.Utilities.GetValue<string>(dict, KEY_JACKPOT_TYPE, "");

		if (!string.IsNullOrEmpty(MachineName))
		{
			SlotConfig = BaseGameConsole.singletonInstance.SlotMachines().Find(delegate(SlotMachineConfig obj)
			{
				return obj.Name().Equals(MachineName);
			});

		}

		if (IsValidPush())
		{
			Show_Str = SetPushShowStr();

		}
	}

	public bool IsValidPush()
	{
		if (string.IsNullOrEmpty(Mid) || string.IsNullOrEmpty(MachineName) || TimeStamp == 0)
			return false;

		if (SlotConfig == null)
		{
			return false;
		}
		return true;
	}
	

	private string SetPushShowStr ()
	{
		return string.Format ("{0}_{1}", Mid, TimeStamp);
	}
}
