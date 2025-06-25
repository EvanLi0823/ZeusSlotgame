using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using App.SubSystems;
using RealYou.Core.UI;
using Core;
using Utils;

namespace Classic
{
	public class ServerBetData
	{
		private const string AVG_BET_KEY = "avg_bet";
		private const string INIT_BET_KEY = "init_bet";
		private const string OP_FEATURE_BET_KEY = "op_feature_bet";
		private const string RCMD_BET_KEY = "rcmd_bet";
		private const string USER_BET_LIST_KEY = "user_bet_list";
		private const string FEATURE_OPEN_BET = "feature_open_bet";
		private Dictionary<string, object> dict;
		public List<long> betListData;
		public List<long> featureOpenBetList;
		public long avgBet;
		public long initBet;
		public long opFeatureBet;
		public long rcmdBet;
		public bool dirty;
		public string trigger;
		public bool enableServerData;//用于开关ServerBet数据，特定代码段执行，不启用ServerBet

		public bool ReceiveBetData
		{
			get;
			private set;
		}
		public ServerBetData()
		{
			betListData = new List<long>();
			featureOpenBetList = new List<long>();
			avgBet = 0;
			initBet = 0;
			opFeatureBet = 0;
			rcmdBet = 0;
			dirty = false;
			trigger = string.Empty;
			enableServerData = true;
			ReceiveBetData = false;
		}

		private void Reset()
		{
			betListData.Clear();
			featureOpenBetList.Clear();
			avgBet = 0;
			initBet = 0;
			opFeatureBet = 0;
			rcmdBet = 0;
			dirty = false;

			trigger = string.Empty;
		}
		public void Init(Dictionary<string, object> dict,string trigger,bool resetData = true)
		{
			if(resetData)
				Reset();
			ParseData(dict, trigger);
		}

		private void ParseData(Dictionary<string, object> dict,string trigger)
		{
			this.dict = dict;
			if (dict.ContainsKey(USER_BET_LIST_KEY))
			{
				betListData.Clear();
				List<object> list = dict[USER_BET_LIST_KEY] as List<object>;
				if (list != null && list.Count > 0)
				{
					for (int i = 0; i < list.Count; i++)
					{
						long bet= Utilities.CastValueLong(list[i]);
						if (bet>0&&!betListData.Contains(bet))//大于0，且要去重复
						{
							betListData.Add(bet);
						}
					}
					betListData.Sort((bet1, bet2) => {
						if (bet1-bet2>=0)
						{
							return 1;
						}
						else
						{
							return -1;
						}
					});
				}
				
			}
			if (dict.ContainsKey(INIT_BET_KEY))
			{
				initBet = Utilities.CastValueLong(dict[INIT_BET_KEY]);
			}

			//对InitBet做自动纠正
			if (initBet==0||!betListData.Contains(initBet))
			{
				if (betListData.Count > 0)
				{
					initBet = Utilities.GetNearestTgtValue(initBet, betListData, NearestType.GreaterTgtValue);
				}
			}
			
			if (dict.ContainsKey(AVG_BET_KEY))
			{
				avgBet = Utilities.CastValueLong(dict[AVG_BET_KEY]);
			}

			if (dict.ContainsKey(OP_FEATURE_BET_KEY))
			{
				opFeatureBet = Utilities.CastValueLong(dict[OP_FEATURE_BET_KEY]);
			}

			if (dict.ContainsKey(RCMD_BET_KEY))
			{
				rcmdBet = Utilities.CastValueLong(dict[RCMD_BET_KEY]);
			}

			//解锁bet列表值可以比bet_list的值都高，高意味着不解锁 也可以低，低意味着全部解锁
			if (dict.ContainsKey(FEATURE_OPEN_BET))
			{
				featureOpenBetList.Clear();
				List<object> list = dict[FEATURE_OPEN_BET] as List<object>;
				if (list != null && list.Count > 0)
				{
					for (int i = 0; i < list.Count; i++)
					{
						long tmp = Utilities.CastValueLong(list[i]);
						if (!featureOpenBetList.Contains(tmp)&&tmp>0)
						{
							featureOpenBetList.Add(tmp);
						}
					}
					featureOpenBetList.Sort((bet1, bet2) => {
						if (bet1-bet2>=0)
						{
							return 1;
						}
						else
						{
							return -1;
						}
					});
				}
			}

			if (IsValid() && !ReceiveBetData) ReceiveBetData = true;
			this.trigger = trigger;
		}
	
	
		public void InsertBetFromFeatureOpenList2BetList(ref List<long> curBetlist)
		{
			if (curBetlist == null||curBetlist.Count==0||featureOpenBetList==null||featureOpenBetList.Count==0) return;
			long tempBet = 0;
			for (int i = 0; i < featureOpenBetList.Count; i++)
			{
				tempBet = featureOpenBetList[i];
				if (tempBet > 0 && !curBetlist.Contains(tempBet) && curBetlist.Exists((bet) => bet > tempBet))
				{
					curBetlist.Add(tempBet);
				}
			}
			curBetlist.Sort((bet1, bet2) =>
			{
				if (bet1-bet2>=0)
				{
					return 1;
				}
				else
				{
					return -1;
				}
			});
		}

		//AppSession切换清除数据标记
		public void Clear(){
			Reset();
			enableServerData = true;
			ReceiveBetData = false;
		}

		
		public bool IsValid()
		{
			return betListData != null && betListData.Count > 0;
		}

		public void EnableServerData(bool enable)
		{
			enableServerData = enable;
		}
		public string GetBetConfigString()
		{
			if (dict!=null)
			{
				return MiniJSON.Json.Serialize(dict);
			}

			return string.Empty;
		}

		public ServerBetData Copy()
		{
			ServerBetData data = new ServerBetData();
			data.betListData = this.betListData.ToList();
			data.initBet = this.initBet;
			data.rcmdBet = this.rcmdBet;
			data.avgBet = this.avgBet;
			data.opFeatureBet = this.opFeatureBet;
			data.trigger = this.trigger;
			data.featureOpenBetList = this.featureOpenBetList.ToList();

			return data;
		}

		public List<long> GetFeatureOpenBetList()
		{
			//1.优先使用解锁列表中的值
			//2.解锁列表中的值，不存在时，使用rcmdbet值+2的值作为解锁最低bet档位
			//3.前2个都不满足时，使用intbet+2的值作为解锁最低bet档位
			//4.如果前3个都不满足时，则使用betlist最大档位作为解锁档位
			if (featureOpenBetList.Count > 0)
			{
				return featureOpenBetList.ToList();
			}

			int index = 0;
			int startIndex = 0;
			int count = betListData.Count;

			if (rcmdBet > 0)
			{
				index = Utilities.GetNearestTgtValueIndex(rcmdBet, betListData, NearestType.GreaterTgtValue);
                 startIndex = index + 2;
                if (startIndex < count)
                {
                	return betListData.GetRange(startIndex,count-startIndex).ToList();
                }
			}

			if (initBet > 0)
			{
				index = Utilities.GetNearestTgtValueIndex(initBet, betListData, NearestType.GreaterTgtValue);
				startIndex = index + 2;
			
				if (startIndex < count)
				{
					return betListData.GetRange(startIndex,count-startIndex).ToList();
				}
			}
		
			return new List<long>() { betListData[count-1]};
		}

		public long GetMinBet()
		{
			return betListData[0];
		}
	}
	public class DataManager
	{

        private static DataManager _instance;
		public readonly static string CONFIG_FAVORITE_LOCAL_DAYS="FavoriteLocalDays";
		public static DataManager GetInstance ()
		{
			if (_instance == null) {
				_instance = new DataManager ();
			}
			return _instance;
		}


        /// <summary>
        /// The format is <Maxbet, Level>.
        /// 
        ///     Means if the player is at [Level], he/she can access 
        /// all the bet-levels that are no more than [Maxbet]
        ///     Added in 1.2.4 by Shenjia.Hu.
        /// </summary>
		//禁止外部使用BetConfig、MachineBetConfig BetDatas
        private Dictionary<long, int> BetConfig = new Dictionary<long, int> ();
		private Dictionary<long, int> MachineBetConfig = new Dictionary<long, int> ();
		private List<long> BetDatas = new List<long> ();
		//外部调用只能使用MachineBetDatas里的Bet数据---在处理大plist时分levelplan时，发现代码里就已经给禁用掉了 --动态bet暂时不care这块

		#region ClubSystem

		private List<long> machineBetData = new List<long>();
		private int minBet = 0;
		public List<long> MachineBetDatas
		{
			get
			{
				return machineBetData.ToList();
			}
			set { machineBetData = value; }
		}
		

		#endregion
		

        public int betDefaultDivisor = 20;
		public long CurPiggyBuyCoins{ get; set;}
		public string SevenDayAwardString = "";
		public Dictionary<string,object> dailyBonus = new Dictionary<string,object>();
		public List<List<int>> DiscountItems;
		public int LimitSpecialOfferSecs;
		public Dictionary<string,string> LobbyAssetBundleVersions = new Dictionary<string,string> ();

		public float VipLevelAddCount;
		public int BecomeVipShowLevel = 1;
		public int VIPExchangeRate = 0;
		public int MaxVIPLevel = 0;
        public DataMining DataMining = new DataMining();
        public Dictionary<string, object> DailyCoinsConfig = new Dictionary<string, object> ();
		public int FavoriteLocalDays{ get; set;}
        /// <summary>
        /// Used to evaluate coins' real money value.
        /// 1.2.5 shenjia
        /// </summary>
        public int OldPriceDivisor = 7500;


		public float shopPromotionDiscount = 0f;
		
		private long lastMaxBet = SlotMachineConfig.DEFAULT_MIN_BET;
		public bool isMaxBetChange = false;
		public bool needChangeCurrentBet = false;
	
		public void SetData (Dictionary<string,object> config)
		{
			InitBetDivisor(config);
		}

		/// <summary>
        /// Adjustable bet levels are initialized.
        /// 1.2.4 Shenjia
        /// </summary>
        public void InitLvPlanBet()
        {
	        List<object> configs = Plugins.Configuration.GetInstance()
		        .GetValueWithPath<List<object>>("Module/BetConfigArray", null);
	        int betConfigIndex = Plugins.Configuration.GetInstance()
		        .GetValueWithPath<int>("Module/BetConfigIndex", 0);;
	        if (configs == null || configs.Count <= betConfigIndex)
	        {
		        Debug.LogError($"lvPlan Bet Error configs.Count {configs.Count},betConfigIndex{betConfigIndex}");
		        return;
	        }
	        List<object> config = configs[betConfigIndex] as List<object>; 
            _InitBetLevel(config,ref BetConfig,ref BetDatas);
        }

		private void _InitBetLevel( List<object>betConfigData, ref Dictionary<long, int> betLevelDict, ref List<long> betList, bool forceConfig = true){
	          betLevelDict.Clear();

            for(int i = 0; i < betConfigData.Count; i++){
                object obj = betConfigData[i];

                Dictionary<string,object> dict = (Dictionary<string,object>)obj;

                int level = Utils.Utilities.CastValueInt (dict ["Level"]);
                long maxBet = Utils.Utilities.CastValueLong (dict ["Bet"]);

                if (!betLevelDict.ContainsKey (maxBet)) {//note that the primary key here is maxBet, not level
                    betLevelDict.Add (maxBet, level);
                } else {
                    Utils.Utilities.LogPlistError("Identical bet found, will be ignored. ");
                }
            }
            //sort. to be checked
            betLevelDict = betLevelDict.OrderBy (o => o.Key).ToDictionary (o => o.Key, p => p.Value);

            int prevLevel = -1;
            foreach(KeyValuePair<long, int> K in betLevelDict) {
                if (K.Value < prevLevel) {

                    Utils.Utilities.LogPlistError ("The betconfig of plist is abnormal. The program will run but may behave strange. \n");
                    break;
                }
            }
            betList.Clear();
            //1.2.4
            foreach (long K in betLevelDict.Keys)
            {
                betList.Add(K);
            }
        }
        private void InitBetDivisor(Dictionary<string, object> config){
	        string intName = "BetDefaultDivisor";

            if (!config.ContainsKey (intName)) {
                Utils.Utilities.LogPlistError ("Cannot found [" + intName + "] in the plist.  " +
                    "Are you using an obselete plist before 1.2.4?\n");
                return;
            }

            betDefaultDivisor = Utils.Utilities.CastValueInt (config [intName]);
            Debug.Log ("betDefaultDivisor = " + betDefaultDivisor);
        }
        
		#region ServerBet

	   private long _opFeatureBet = 0;
	   public long OpFeatureBet
	   {
		   get
		   {
			   return _opFeatureBet;
		   }
		   private set { _opFeatureBet = value; }
	   }
      
        //进入机器时刷新 bet
        public void RefreshBetOnEnterSlot(SlotMachineConfig _slotConfig = null)
        {
	        SetBetData(GetCurrentLevelBetList(_slotConfig));
	        lastMaxBet = GetCurrentLevelMaxBet();
#if UNITY_EDITOR
	        Log.LogYellowColor("DataManager RefreshBetOnEnterSlot:bet list:"+MiniJSON.Json.Serialize(MachineBetDatas));
#endif
        }

        public long GetCurrentBet(SlotMachineConfig _slotConfig)
        {
	        long result = 0;
	        int level = UserManager.GetInstance().UserProfile().Level();
	        switch (betDefaultDivisor) {
		        case 0:
			        //use last
			        result = Math.Max(BaseSlotMachineController.Instance.currentBetting,_slotConfig.MinBet);
			        break;
		        case 1:
			        //use max
			        result = GetLevelMaxBet (level);
			        break;
		        case 9999:
			        //use min
			        result = MachineBetDatas [0];
			        break;
		        default:
			        //use balance/factor, rounded down
			        double dividedBet = UserManager.GetInstance().UserProfile ().Balance() / 20;
			        result = Utilities.CastValueLong(dividedBet);
			        break;
	        }
	        result = Utilities.GetNearestTgtValue(result,MachineBetDatas, NearestType.GreaterTgtValue);
	        return result;
        }
        public bool RefreshBetOnLevelUp()
        {

	        //2.最终使用BetList应用刷新
	        SetBetData(GetCurrentLevelBetList());
	        
	        UpdateBetSetting();
	        
#if UNITY_EDITOR
	       Log.LogYellowColor("DataManager RefreshBetOnLevelUp:"+" bet list:"+MiniJSON.Json.Serialize(MachineBetDatas));
#endif
	        return false;
        }
        public void UpdateBetSetting()
        {
	        long tempMaxBet = GetCurrentLevelMaxBet();
	        if (UserManager.GetInstance().UserProfile().Level() >= Core.ApplicationConfig.GetInstance().EnaMaxBetChangeDialogLevel && lastMaxBet < tempMaxBet)
	        {
		        isMaxBetChange = true;
	        }

	        long currentBet = UserManager.GetInstance().UserProfile().CurrentBet;
	        if (CheckUserCurrentBetChangeLimitCondition()&&(lastMaxBet < tempMaxBet||currentBet < lastMaxBet))
	        {
		        needChangeCurrentBet = true;
		        if (isMaxBetChange) 
			        isMaxBetChange = false;//开启提示时，关闭MaxBet提示
	        }

	        //只有后续的最大Bet
	        if (lastMaxBet < tempMaxBet)
	        {
		        lastMaxBet = tempMaxBet;
	        }
        }

		public void UpdateMaxBetSetting()
        {
	        long tempMaxBet = GetCurrentLevelMaxBet();
	        if (UserManager.GetInstance().UserProfile().Level() >= Core.ApplicationConfig.GetInstance().EnaMaxBetChangeDialogLevel && lastMaxBet < tempMaxBet)
	        {
		        isMaxBetChange = true;
	        }
	        
	        //只有后续的最大Bet
	        if (lastMaxBet < tempMaxBet)
	        {
		        lastMaxBet = tempMaxBet;
	        }
        }
        public void UpdateUserCurrentBet()
        {
	        if (CheckUserBetChangeCondition())
	        {
		        if (BaseSlotMachineController.Instance != null)
		        {
			        BaseSlotMachineController.Instance.Upgrade2MaxBet();
		        }
	        }
        }
        private bool CheckUserBetChangeCondition()
        {
	        return needChangeCurrentBet && CheckUserCurrentBetChangeLimitCondition()&&MachineUtility.Instance.CanChangeBet();
        }

        private bool CheckUserCurrentBetChangeLimitCondition()
        {
	        int LevelUpLimit = Plugins.Configuration.GetInstance().GetValueWithPath<int>("ApplicationConfig/AutoLevelUpUserCurrentBetLimit", 6);
	        long currentBet = UserManager.GetInstance().UserProfile().CurrentBet;
	        return MachineBetDatas.Count <= LevelUpLimit && currentBet < GetCurrentLevelMaxBet();
        }
        #endregion

        //根据等级获取最大bet
        public long GetLevelMaxBet(int level)
        {
	        long maxbet = SlotMachineConfig.DEFAULT_MIN_BET;
	        List<long> betList = GetCurrentLevelBetList();
	        int count = betList.Count;
	        if (count>=1)
	        {
		        maxbet = betList[count - 1];
	        }
	        return maxbet;
        }

        private long GetCurrentLevelMaxBet()
        {
	        List<long> list = GetCurrentLevelBetList();
	        return list[list.Count - 1];
        }

        private SlotMachineConfig GetCurrentMachineConfig()
        {
	        if (BaseSlotMachineController.Instance != null)
	        {
		        return BaseSlotMachineController.Instance.slotMachineConfig;
	        }

	        return null;
        }
        /// <summary>
        /// 用于获取当前等级的所有可用betlist 禁止外部调用，外部请使用MachineBetDatas
        /// </summary>
        /// <returns></returns>
        private List<long> GetCurrentLevelBetList(SlotMachineConfig config =null)
        {
	        if (config == null)
	        {
		        config = GetCurrentMachineConfig();
	        }
	        return GetLocalCurrentLevelBetList(config);
        }
        //获取bet列表
        private List<long> GetLocalCurrentLevelBetList(SlotMachineConfig _slotConfig =null)
        {
	        int level = UserManager.GetInstance().UserProfile().Level();
	        long maxbet = 1;
	        if (_slotConfig == null)
	        {
		        _slotConfig = GetCurrentMachineConfig();
	        }
	        List<long> BetList = null;
	        if (_slotConfig==null||_slotConfig.MachineBetLevelDict.Count==0)
	        {
		        MachineBetConfig = BetConfig.ToDictionary(k=>k.Key,k=>k.Value);
		        BetList = BetDatas.ToList();
	        }
	        else
	        {
		        MachineBetConfig = _slotConfig.MachineBetLevelDict.ToDictionary(k=>k.Key,k=>k.Value);
		        BetList = _slotConfig.MachineBetList.ToList();
	        }

	        BetList.Sort ((a,b)=>{return a.CompareTo(b);});//asc sort

	        foreach (KeyValuePair<long, int> K in MachineBetConfig) {
		        if (maxbet < K.Key && level >= K.Value)
			        maxbet = K.Key;
	        }

	        if (1 == maxbet) {
		        Debug.LogError("Cannot found max betting.");
		        return BetList;
	        }

	        return BetList.Where(a => a<=maxbet).ToList();
        }

        public void InitBetData(SlotMachineConfig _slotConfig){
			
			//UnlockAllBetDatas();
			RefreshBetOnEnterSlot(_slotConfig);
		}

		private void SetBetData(List<long> betList)
		{
			if (betList == null || betList.Count == 0) return;
			
			MachineBetDatas = betList;

			SetOpFeatureBet();
		}

		private void SetOpFeatureBet()
		{
			OpFeatureBet = MachineBetDatas[0];
		}
	}
}
