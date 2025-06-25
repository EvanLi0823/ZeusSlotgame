using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Utils;
using Libs;
using Classic;
public class JackPotIncreaseCommonConfig  {
    public long NeedBet = 0;
    //public int BetMini = 0;
    //public float BetMiniRate = 0f;
    //public int BetMinor = 0;
    //public float BetMinorRate = 0f;
    //public int BetMajor = 0;
    //public float BetMajorRate = 0;

    private long InitMin =0;
	private long InitMax =0;

	public int IncreaseMinEverySeconds = 0;
	public int IncreaseMaxEverySeconds = 0;

	private long UpMin =0;
	private long UpMax=0;
	private int ClearFrequencyNum=0;

	public int Weight =0;

	public int ID;//自增唯一标识符
	public JackPotIncreaseCommonConfig(Dictionary<string,object> infos,int jackpotlevel)
	{
		this.NeedBet = Utilities.GetLong (infos,BET_MIN, 0);
		this.InitMin = Utilities.GetLong (infos,INIT_MIN,0);
		this.InitMax = Utilities.GetLong (infos,INIT_MAX,0);
		this.IncreaseMinEverySeconds = Utilities.GetInt (infos,INCREASE_MIN_SECONDS,0);
		this.IncreaseMaxEverySeconds = Utilities.GetInt (infos,INCREASE_MAX_SENONDS,0);
		this.UpMin = Utilities.GetLong (infos,UP_MIN,0);
		this.UpMax = Utilities.GetLong (infos,UP_MAX,0);
		this.ClearFrequencyNum = Utilities.GetInt (infos,CLEAR_FREQUENCY,0);
		this.Weight = Utilities.GetInt (infos, WEIGHT, 0);
		this.ID = jackpotlevel;
		//解析jackpot档位数值
		//5000,0.5;100000,1;500000,2
		//string jackpotLevelConfig = Plugins.Configuration.GetInstance().GetValueWithPath<string>("Module/JackpotLevelConfig", "");
		////Debug.LogError("-------JackpotLevelConfig---------->" + jackpotLevelConfig);
		//if (!string.IsNullOrEmpty(jackpotLevelConfig))
		//{
		//    string[] levelStr = jackpotLevelConfig.Split(';');
		//    BetMini = int.Parse(levelStr[0].Split(',')[0]);
		//    BetMiniRate = float.Parse(levelStr[0].Split(',')[1]);
		//    BetMinor = int.Parse(levelStr[1].Split(',')[0]);
		//    BetMinorRate = float.Parse(levelStr[1].Split(',')[1]);
		//    BetMajor = int.Parse(levelStr[2].Split(',')[0]);
		//    BetMajorRate = float.Parse(levelStr[2].Split(',')[1]);
		//}

		//BetMini = 200; BetMiniRate = 0.5f;
		//BetMinor = 500; BetMinorRate = 1f;
		//BetMajor = 1000; BetMajorRate = 2f;
	}
	
    private double _CurrentPrizeValue;
	public double CurrentPrizeValue
	{
		set{
			_CurrentPrizeValue = value;
		}
		get{
			return _CurrentPrizeValue;
		}
	}

	public double GetCurrentPrizeValue()
	{
		if (BaseSlotMachineController.Instance != null) 
		{
			BaseSlotMachineController.Instance.SetJackpotData (true, CurrentPrizeValue,"NormalJackpot");
		}

		return CurrentPrizeValue;
	}

//	private float _BottemPrizeValue;
//	public float BottemPrizeValue
//	{
//		set{
//			this._BottemPrizeValue = value;
//		}
//		get{
//			return _BottemPrizeValue;
//		}
//	}

	private bool HasInit = false;

	private double _TopPrizeValue;
	public double TopPrizeValue
	{
		set{
			_TopPrizeValue = value;
		}
		get{
			return _TopPrizeValue;
		}
	}

	private float _IncreaseNumberSeconds;
	public float IncreaseNumberSeconds
	{
		set{
			_IncreaseNumberSeconds = value;
		}
		get{
			return _IncreaseNumberSeconds;
		}
	}

	private SlotMachineConfig _slotConfig = null;
    /// <summary>
    /// 配表数据初始化，用于初始一些动态计算的基础值。
    /// </summary>
    /// <param name="_config"></param>
	public void InitData(SlotMachineConfig _config)
	{
		if (_slotConfig == null) {
			_slotConfig = _config;
		}
		if (!HasInit) {
			//app 启动
			double v = GetLocalCurrentPrize();
			if (v == 0) {
				//第一次进入
				this.Reset (true);
				return;
			} else {
				this._CurrentPrizeValue = v;
				this._IncreaseNumberSeconds = GetLocalIncreaseSeconds();
				this._TopPrizeValue = GetTopPrize();
			}
			HasInit = true;
		} 

		
        //计算上次退出游戏到该次启动游戏的总时长
		long seconds = TimeUtils.SecondsBetween(UserManager.GetInstance().UserProfile().LastExitGameTime,System.DateTime.Now);
        //根据退出游戏总时长、每分钟递增数，计算出该时间段的补加奖池数据。
		_CurrentPrizeValue += seconds * this._IncreaseNumberSeconds;
        //如果大于最高值则重置
		if (_CurrentPrizeValue > this._TopPrizeValue) {
			this.Reset (true);
		}

		Messenger.Broadcast(JackPotManager.JACKPOT_DATA_COMMON_CHANGE, this._slotConfig,ID);

//		#if UNITY_EDITOR
//		Log.Trace ("jack:"+_CurrentPrizeValue);
//		Log.Trace (_IncreaseNumberSeconds);
//		Log.Trace (_TopPrizeValue);
//		#endif
	}
    
    #region

    public double GetLocalCurrentPrize()
    {
	    double v = Utils.Utilities.GetValueDoubleFromFloat(LocalTranslateStr(LOCAL_JACKPOT_CURRENT_PRIZE,true));
	    if (v==0)
	    {
		     v = Utils.Utilities.GetValueDoubleFromFloat(LocalTranslateStr(LOCAL_JACKPOT_CURRENT_PRIZE,false));
	    }

	    return v;
    }

    public float GetLocalIncreaseSeconds()
    {
	    float v = SharedPlayerPrefs.GetPlayerPrefsFloatValue (LocalTranslateStr(LOCAL_JACKPOT_INCREASE_SECONDS,true));
	    if (v==0)
	    {
		    v = SharedPlayerPrefs.GetPlayerPrefsFloatValue (LocalTranslateStr(LOCAL_JACKPOT_INCREASE_SECONDS,false));
	    }

	    return v;
    }

    public double GetTopPrize()
    {
	    double v =  Utils.Utilities.GetValueDoubleFromFloat (LocalTranslateStr(LOCAL_JACKPOT_TOP_PRIZE,true));
	    if (v==0)
	    {
		    v = Utils.Utilities.GetValueDoubleFromFloat (LocalTranslateStr(LOCAL_JACKPOT_TOP_PRIZE,false));
	    }

	    return v;
    }
    #endregion
	public void SaveData()
	{
		Utils.Utilities.SetValueDoubleFromFloat (LocalTranslateStr(LOCAL_JACKPOT_CURRENT_PRIZE,true),_CurrentPrizeValue);
		SharedPlayerPrefs.SetPlayerPrefsFloatValue (LocalTranslateStr(LOCAL_JACKPOT_INCREASE_SECONDS,true),_IncreaseNumberSeconds);
		Utils.Utilities.SetValueDoubleFromFloat  (LocalTranslateStr(LOCAL_JACKPOT_TOP_PRIZE,true),_TopPrizeValue);
		
		Utils.Utilities.SetValueDoubleFromFloat (LocalTranslateStr(LOCAL_JACKPOT_CURRENT_PRIZE,false),0);
		SharedPlayerPrefs.SetPlayerPrefsFloatValue (LocalTranslateStr(LOCAL_JACKPOT_INCREASE_SECONDS,false),0);
		Utils.Utilities.SetValueDoubleFromFloat  (LocalTranslateStr(LOCAL_JACKPOT_TOP_PRIZE,false),0);
//		Log.Trace ("save:"+_CurrentPrizeValue);
//		Log.Trace (_IncreaseNumberSeconds);
//		Log.Trace (_TopPrizeValue);
	}

	public void Reset(bool needChangeToMinRandomValue = false)
	{
//		Log.Trace ("reset");
		if(needChangeToMinRandomValue){
			this._CurrentPrizeValue = Utils.Utilities.GenLongRandomNumber (this.InitMin,this.InitMax) ;
		}
		else{
			this._CurrentPrizeValue =  this.InitMin ;
		}
		this._IncreaseNumberSeconds = UnityEngine.Random.Range(this.IncreaseMinEverySeconds,this.IncreaseMaxEverySeconds);
		this._TopPrizeValue = Utils.Utilities.GenLongRandomNumber(UpMin,UpMax);
	
		Messenger.Broadcast(JackPotManager.JACKPOT_DATA_COMMON_CHANGE, this._slotConfig,ID);
	}

	public void IncreasePerTime(int _seconds)
	{
		this._CurrentPrizeValue += this._IncreaseNumberSeconds *_seconds;
//		Messenger.Broadcast<SlotMachineConfig> (JackPotManager.JACKPOT_DATA_COMMON_CHANGE, this._slotConfig);
		if (this._CurrentPrizeValue > this._TopPrizeValue) {
			//others 其他人获得了奖励
			this.Reset ();
		}
	}

	public bool EnableSpinJackpot(long currentBet)
	{
		return CurrentBetOverNeedBet(currentBet) && MaxBetOverNeedBet();
	}

	public bool CurrentBetOverNeedBet(long currentBet)
	{
		return this.NeedBet <= currentBet;
	}

	public bool MaxBetOverNeedBet()
	{
		return UserManager.GetInstance ().MaximumBetting (UserManager.GetInstance ().UserProfile ().Level ()) >= this.NeedBet;
	}
	
	public bool HasOpenBetChangeDialog
	{
		set{
			SharedPlayerPrefs.SetPlayerPrefsBoolValue (LocalTranslateStr(HAS_OPEN_BET_CHANGE_DIALOG,false), true);
		}
		get{
			return SharedPlayerPrefs.GetPlayerBoolValue (LocalTranslateStr(HAS_OPEN_BET_CHANGE_DIALOG,false), false);
		}
	}
	/// <summary>
	/// 跟本地相关的字段
	/// Locals the machine string.
	/// </summary>
	/// <returns>The machine string.</returns>
	/// <param name="jackppot">Jackppot.</param>
	private string LocalTranslateStr(string jackpotStr,bool bNew)
	{
		if (bNew)
		{
			return string.Format ("{0}_{1}_{2}",jackpotStr,this._slotConfig.Name(),ID);
		}
		return string.Format ("{0}_{1}",jackpotStr,this._slotConfig.Name());

	}

//	private const string IndexStr = "Index"; 
	private const string BET_MIN = "BetMin";
	private const string INIT_MIN = "InitMin";
	private const string INIT_MAX = "InitMax";
	private const string INCREASE_MIN_SECONDS = "IncreaseMinEverySeconds";
	private const string INCREASE_MAX_SENONDS = "IncreaseMaxEverySeconds";
	private const string UP_MIN = "UpMin";
	private const string UP_MAX = "UpMax";
	private const string CLEAR_FREQUENCY = "ClearFrequency"; 
	private const string WEIGHT = "Weight"; 

	private const string LOCAL_JACKPOT_CURRENT_PRIZE = "LOCAL_JACKPOT_CURRENT_PRIZE"; 
	private const string LOCAL_JACKPOT_TOP_PRIZE = "LOCAL_JACKPOT_TOP_PRIZE"; 
	private const string LOCAL_JACKPOT_INCREASE_SECONDS = "LOCAL_JACKPOT_INCREASE_SECONDS"; 
	private const string HAS_OPEN_BET_CHANGE_DIALOG = "HAS_OPEN_BET_CHANGE_DIALOG"; 

}
