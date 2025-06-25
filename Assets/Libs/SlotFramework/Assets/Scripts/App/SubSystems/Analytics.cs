using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using UnityEngine;
using MiniJSON;
using Core;
using App;
using Plugins;
using System;
using Beebyte.Obfuscator;

namespace Classic
{
	[Skip]
    public class Analytics
    {
		/**
		 * Application Event
		 */
		public const string Launch = "Launch";
       
        public const string RewardVideoRewarded = "RewardVideoRewarded";
        public const string RewardVideoClosed = "ReWardVideoClosed";
        public const string CATCH_EXCEPTION = "catch_exception";
        public const string CATCH_ERROR = "catch_error";
        #region GoodSKU常量

        public const string GoodPurchase = "GoodPurchase";
        public const string GoodPurchaseFailed = "GoodPurchaseFailed";
        public const string GoodPurchaseSucceeded = "GoodPurchaseSucceeded";
        public const string GoodPurchaseResumed = "GoodPurchaseResumed";
        public const string GoodPurchaseFromShop = "GoodPurchaseFromShop";
        public const string GoodPurchaseCoins = "GoodPurchaseCoins";
        #endregion

        public const string CoinPurchase = "CoinPurchase";
		public const string CoinPurchaseFailed = "CoinPurchaseFailed";
		public const string CoinPurchaseSucceeded = "CoinPurchaseSucceeded";
		public const string CoinPurchaseFreeFailed = "CoinPurchaseFreeFailed";
		public const string CoinPurchaseFreeSucceeded = "CoinPurchaseFreeSucceeded";
		public const string CoinPurchaseResumed = "CoinPurchaseResumed";
		public const string CoinPurchaseFromShop = "CoinPurchaseFromShop";
		public const string CoinPurchaseCoins = "CoinPurchaseCoins";
		public const string CoinPurchaseClicked = "CoinPurchaseClicked";
		public const string OnPurchaseBegin = "OnPurchaseBegin";

		public const string CoinsQuestAward = "CoinsQuestAward";
		
		public const string SlotsPurchase = "SlotsPurchase";
		public const string SlotsPurchaseFailed = "SlotsPurchaseFailed";
		public const string SlotsPurchaseSucceeded = "SlotsPurchaseSucceeded";
		public const string SlotPurchaseResumed = "SlotPurchaseResumed";

		public const string RemoteNotification = "RemoteNotification";

		public const string Error = "Error";
		public const string Exception = "Exception";
		public const string TOURNAMENT_RANK = "TournamentRank";
		
		public const string SCENE_LOAD_SUCCEED = "SceneLoadSucceed";

		/**
		 * Settings Events
		 * 
		 */
		public const string SoundsEnabledChanged = "SoundsEnabledChanged";
		public const string ScreenLockDuringAutoSpinChanged = "ScreenLockDuringAutoSpinChanged";
		public const string NotificationEnabledChanged = "NotificationEnabledChanged";
		public const string JackpotAnnouncementsEnabledChanged = "JackpotAnnouncementsEnabledChanged";
		public const string GDPREnabledChanged = "GDPREnabledChanged";
		public const string RateUsClicked = "RateUsClicked";
		public const string ContactUsClicked = "ContactUs";
		public const string RatingUs = "RatingUs";
		public const string PrivacyStatus = "PrivacyStatus";
		/**
		 * Lobby Related Events
		 */
		public const string InviteFriend = "InviteFriend";
		public const string CollectHourlyBonus = "CollectHourlyBonus";
		public const string FacebookClicked = "FacebookClicked";
		public const string AppleIDClicked = "AppleIDClicked";
		public const string ChooseSlots = "ChooseSlots";
		public const string ShowMachineCards = "ShowMachineCards";
        public const string TaskSlotDetailPanelShow = "TaskSlotDetailPanelShow";
        public const string TaskIntroduceDialogShow = "TaskLobbyDisplayDialogShow";
        public const string TaskStartedShow = "TaskStartedShow";
		public const string TaskCollectClicked = "TaskCollectClicked";
        public const string TaskCompleted = "TaskCompleted";
        public const string MapTaskPanelShow = "MapTaskPanelShow";
        /**
		 * Slot Related Events 
		 */
		public const string AutoSpinCounts = "AutoSpinCounts";
		public const string SpinClicked = "SpinClicked";
		public const string BackLobbyClicked = "BackLobbyClicked";
		public const string AutoSpinClicked = "AutoSpinClicked";
		public const string BuyCoinsClicked = "BuyCoinsClicked";
		public const string BuyCoinsByCount = "buyCoinsbyCount";
		public const string MinusBetClicked = "MinusBetClicked";
		public const string MinusLinesClicked = "MinusLinesClicked";
		public const string PlusBetClicked = "PlusBetClicked";
		public const string PlusLinesClicked = "PlusLinesClicked";
		public const string MaxLinesClicked = "MaxLinesClicked";
		public const string SettingMenuClicked = "SettingMenuClicked";
		public const string LevelUpEvent = "LevelUpEvent";
		public const string NoCoinEvent = "NoCoinEvent";
		public const string BigWinEvent = "BigWinEvent";
		public const string FreeSpinEvent = "FreeSpinEvent";
		public const string BonusGameEvent = "BonusGameEvent";
		public const string FiveOfAKindEvent = "FiveOfAKindEvent";
		public const string WinJackpotEvent = "WinJackpot";
		public const string ExperienceClicked = "ExperienceClicked";
		public const string PayTableClicked = "PayTableClicked";
		public const string FreeSpinReward = "FreeSpinReward";
		public const string RespinClicked = "RespinClicked";
		public const string OpenLevelBoostTipDialog = "Levelup_Tips";
		/*
		 * Pattern Events
		 */
		public const string PatternSaverCounter1 = "SaverCounter1";
		public const string PatternSaverCounter2 = "SaverCounter2";
		public const string PatternTop1 = "Top1";
		public const string PatternTop2 = "Top2";

		public const string DailyBonusCollectBonus = "CollectDailyBonus";
		public const string SevenDayCollectBonus = "CollectDailyBonus";
		public const string FourHourCollectBonus = "CollectBonusHours4";
        public const string HourlyBonusCollectWheelBonus = "CollectWheelBonus";
		public const string AdvertisementClicked = "AdvertisementClicked";
		public const string OpenShop = "OpenShop";
		public const string AppOut = "AppOut";
		public const string AppClose = "App_Close";

		public const string SevenDayShow_Key = "SevenDayShow";
		public const string WelcomeShow_Key = "WelcomeShow";
		public const string WelcomeBackShow_Key = "WelcomeBackShow";
		public const string CollectWelcomeBackCoins = "CollectWelcomeBackCoins";
		public const string SpecialOffer = "SpecialOffer";
		public const string SpecialOfferShow = "SpecialOfferShow";
		public const string SpecialOfferBuySucceed = "SpecialOfferBuySucceed";
		public const string RookieOfferShowed = "RookieOfferShowed";
        public const string RookieOfferBuy = "RookieOfferBuy";
        public const string ShopBuySucceed = "ShopBuySucceed";
		public const string RookieOfferShowed2 = "RookieOffer2Showed";
		public const string RookieOfferBuy2 = "RookieOffer2Buy";
		public const string PiggyShowed = "PiggyShowed";
		public const string PiggyBuy = "PiggyBuy";
		public const string SecondChange = "SecondChance";

		public const string CurrentBalanceKey = "CurrentBalance";
		public const string CurrentLevelKey = "CurrentLevel";
		public const string CurrentBetNumKey="CurrentBetNum";
		public const string CurrentCoinsGetKey = "WinCoins";
		public const string CurrentThemeKey = "CurrentMachine";
		public const string ContinuousLoginDaysKey = "LoginDays";
		public const string TotalLoginDays = "TotalLoginDays";
		public const string LastLoginTime = "LastLoginTime";
		public const string LastCollectUpTime = "LastCollectUpTime";
		public const string UserID = "UserID";

		public const string RestoreLocalProgress = "RestoreLocalProgress";

		public const string SpecialOfferClick = "SpecialOfferClick";

		public const string MissionComplete = "MissionComplete";
		public const string MissionID = "MissionID";
		public const string MissionFnishedDateTime = "FnishedDateTime";

		public const string RateMachine="RateMachine";
		public const string RateLevel="RateLevel";
		public const string RateMachineName = "RateMachineName";

        public const string FirstPurchaseShowed = "FirstPurchaseShowed";
        public const string FirstPurchaseGoToStoreClicked = "GoToStoreClicked";

		public const string SendAllSpinTimeNoMoney = "NoMoneyAllSpinTime";
		public const string SendAllSpinTimeCurrentMachine = "CurrentMachineSpinTime";

		public const string LobbyShowMachineNames = "LobbyShowMachineNames";

		public const string FirstSessionSeconds = "FirstSessionSeconds";

		public const string EnterExitMachine = "EnterExitMachine";
		public const string FirstNoMoneyGameSession = "FirstNoMoneyGameSession";
		public const string OutOfCoins = "OutOfCoins";
		public const string NewUserReward = "NewUserReward";
		public const string DeadCare = "DeadCare";
		public const string FACEBOOK_ID = "FacebookID";
		public const string VipLevelUpEventName = "VipLevelUp";
		public const string VIPSHOW = "VipShow";
		public const string VIP_UNLOCK = "VipUnlock";
		public const string ASSETBUNDLE_BEGIN = "assetbundle_begin";
		public const string ASSETBUNDLE_END = "assetbundle_end";
		public const string FB_LOGIN = "Fb_Login";
		public const string APPLE_LOGIN = "Apple_Login";
		public const string SPIN_AWARD = "Spin_Award";
		public const string VIP_ROOM_CLICK = "VipRoomClick";
		public const string JACKPOT_COMMON_WIN = "JackpotWin";
		public const string JACKPOT_COMMON_WIN_COLLECT = "JackpotWinCollect";

		public const string CommonJackpotWin_CheckAward = "CommonJackpotWin_CheckAward";
		public const string CommonJackpotWin_CashAward = "CommonJackpotWin_CashAward";
		public const string CommonJackpotWin_Miss = "CommonJackpotWin_Miss";
		public const string JACKPOT_COMMON_NOTIFY = "JackpotNotify";
		public const string UNLOCK_CURRENT_MACHINE = "UnlockCurrentMachine";
		public const string REWARD_FREE_SPIN = "RewardFreeSpins";
		public const string OPEN_REWARD_SPIN_DIALOG = "OpenRewardSpinDialog";
		public const string ClassName_KEY = "class_name";
		public const string MethodName_KEY = "method_name";
		public const string Deserialize_NULL = "Deserialize_NULL";
		public const string AccountSDK_Migration_Successed = "Migration_OK";
		public const string AccountSDK_Bind_Facebook_Successed = "Bind_Facebook__OK";
		public const string AccountSDK_Bind_Apple_Successed = "Bind_Apple_OK";
		public const string AccountSDK_SignUp_Successed = "SignUp_OK";


		public const string AccountSDK_RewardAck_Failed = "RewardAck_Failed";
		public const string AccountSDK_Migration_Failed = "Migration_Failed";
		public const string AccountSDK_Update_Failed = "Update_Failed";
		public const string AccountSDK_Bind_FaceBook_Failed = "Bind_Facebook_Failed";
		public const string AccountSDK_Bind_Apple_Failed = "Bind_Apple_Failed";
		public const string AccountSDK_Query_Failed = "Query_Failed";
		
		public const string AccountSDK_RewardTask_Create_Failed = "RewardTask_Create_Failed";

		public const string AccountSDK_HOURLY_BONUS_WHEEL_QUERY_FAILED = "HourlyBonusWheelQueryFailed";
        public const string UpdateQuestPointMessage_Failed = "UpdateQuestPointMessage_Failed_C";
		public const string AccountSDK_TaskSpin_Failed = "TaskSpin_Failed_C";
		public const string AccountSDK_TaskQuery_Failed = "TaskQuery_Failed_C";
		public const string AccountSDK_TaskAward_Failed = "TaskAward_Failed_C";
        public const string AccountSDK_TaskTrigger_Failed = "TaskTrigger_Failed_C";
        public const string AccountSDK_TaskReset_Failed = "ResetTask_Failed_C";
        public const string AccountSDK_TaskCreate_Failed = "TaskCreate_Failed_C";
        public const string AccountSDK_TaskMultiCreate_Failed = "TaskMultiCreate_Failed_C";
        public const string AccountSDK_TaskComplete_Failed = "TaskComplete_Failed_C";
        public const string AccountSDK_TaskMultiComplete_Failed = "TaskMultiComplete_Failed_C";
        public const string AccountSDK_TaskCheckParams_Failed = "TaskCheckParams_Failed_C";

        public const string AccountSDK_NewTaskQuery_Failed = "NewTaskQuery_Failed_C";
        public const string AccountSDK_NewTaskTrigger_Failed = "NewTaskTrigger_Failed_C";
        public const string AccountSDK_TaskUpdate_Failed = "TaskUpdate_Failed_C";


        public const string ActivitySession_Failed = "ActivitySession_Failed_C";
        public const string RequestAward_Failed = "RequestAward_Failed_C";


		public const string AccountSDK_GiftBox_Failed = "GiftBox_Failed_C";

        public const string AccountSDK_Albums_Failed = "Albums_Failed";
        public const string AccountSDK_Crafting_Query_Failed = "Crafting_Query_Failed";
        public const string AccountSDK_Crafting_Result_Failed = "Crafting_Result_Failed";
        public const string AccountSDK_PowerMachine_Query_Game_List_Failed = "PowerMachine_Query_Game_List_Failed";
        public const string AccountSDK_PowerMachine_Query_Jackpot_Config_Failed = "PowerMachine_Query_Jackpot_Config_Failed";
        public const string AccountSDK_PowerMachine_Result_Failed = "PowerMachine_Result_Failed";
        public const string AccountSDK_Update_Card_State_Failed = "Update_Card_State_Failed";
        
		public const string AccountSDK_SignUp_Failed = "SignUp_Failed";
		public const string AccountSDK_AccountUpdate_Failed = "AccountUpdate_Failed";
		public const string AccountSDK_UnbindMessage_Failed = "UnbindMessage_Failed";
		public const string AccountSDK_Validate_Failed = "Validate_Failed";
		public const string AccountSDK_TourAward_Failed = "TourAward_Failed";
		public const string AccountSDK_TourEnroll_Failed = "TourEnroll_Failed";
		public const string AccountSDK_TourSpin_Failed = "TourSpin_Failed";
		public const string AccountSDK_CodeRedeemed_Failed = "CodeRedeemed_Failed";
		public const string AccountSDK_RA_EVENT_Failed = "RA_Event_Failed";
		public const string AccountSDK_SessionExpired= "SessionExpired";
		public const string AccountSDK_SwitchAccount = "SwitchAccount";

		public const string AccountSDK_EventTrigger_Failed = "EventTrigger_Failed";
        public const string EnterSlotTrigger_Failed = "EnterSlotTrigger_Failed";
		public const string ExitSlotTrigger_Failed = "ExitSlotTrigger_Failed";
		public const string AcceptGiftBoxTrigger_Failed = "AcceptGiftBoxTrigger_Failed";
	    public const string AcceptDailyBonus_Failed = "AcceptDailyBonus_Failed";
		public const string RequestGiftBoxTrigger_Failed = "RequestGiftBoxTrigger_Failed";
        public const string AccountSDK_PurchaseTrigger_Failed = "PurchaseTrigger_Failed";
		public const string AccountSDK_LevelUpTrigger_Failed = "LevelUpTrigger_Failed";
		public const string RequestRelativetimeRewardTrigger_Failed = "RequestRelativetimeRewardTrigger_Failed";
		public const string RequestLeaderBoardMessage_Failed = "RequestLeaderBoardMessageTrigger_Failed";
		public const string RequestUserRankMessage_Failed = "RequestUserRankMessageTrigger_Failed";

        public const string IdleRewardAd_Failed = "IdleRewardAd_Failed";


		public const string OpenInboxDialog = "OpenInboxDialog";
		public const string OpenSettingDialog = "OpenSettingDialog";
		public const string OpenSupportDialog = "OpenSupportDialog";
		public const string OpenKeepInTouchDialog = "OpenKeepInTouchDialog";
		public const string DiscardReward = "DiscardReward";
		public const string AcceptReward = "AcceptReward";
		public const string RewardExpiration = "RewardExpiration";
		public const string RequestGetFreeCardTrigger_Failed = "RequestFreeCardTrigger_Failed";
        public const string CollectPointEvent = "CollectPointEvent";
		public const string OutOfCoinsTriggerMessage_Failed = "OutOfCoinsTriggerMessage_Failed";

		public const string AddVipPointEvent = "AddVipPointEvent";
		public const string CollectCoinsRewardEvent = "CollectCoinsRewardEvent";
		public const string ReceiveCardEvent = "ReceiveCard";
		public const string CloseInboxAndOpenRewardDialogEvent = "CloseInboxAndOpenRewardDialogEvent";
		public const string ChangeLevelPlanEvent = "ChangeLevelPlanEvent";
		public const string Deal_1RewardEvent = "Deal_1RewardEvent";
		public const string SALES_ITEM_SHOW = "SalesItemShow";
		public const string SALES_ITEM_BUY = "SalesItemBuy";
		public const string CollectCoinsActiviesEvent = "CollectCoinsActiviesEvent";
		public const string CollectCoinsGiftItem = "CollectCoinsGiftItem";
		public const string Deal_1ActiviesEvent = "Deal_1ActiviesEvent";
		public const string BetUnlockEvent = "BetUnlockEvent";
		public const string RewardSpinClicked = "RewardSpinClicked";
		public const string ShowRewardSpin = "ShowRewardSpin";
		public const string BusinessLogic_EmailRegister = "EmailRegister";
		public const string BusinessLogic_EmailRegister_Failed = "EmailRegister_Failed";

		public const string BusinessLogic_UserPromoCode_OK = "CodeRedeemPage_Submit_Clicked_OK";
		public const string BusinessLogic_UserPromoCode_Failed = "CodeRedeemPage_Submit_Clicked_Failed";
		public const string ChangeSegmentEvent = "ChangeSegmentEvent";
	    public const string UpdateConversionValue = "UpdateConversionvalue";
		#region GM 命令

		public const string ExecuteGM_Key = "ExecuteGM";

		#endregion
		
		#region Lua系统

		public const string LuaActivityMgrDownload_Key = "LuaActivityMgrAssets";
		public const string LuaBundleDownLoad_Key = "LuaAssets";
		public const string LuaShortActDownload_Key = "LuaShortActAssets";

		#endregion

		#region 通用资源校验
		public const string AssetCheckFileownLoad_Key = "CommonAssetsCheckFile";
		#endregion

		#region 活动系统
		public const string CreateActivity_Key = "CreateActivity";
		public const string Sys_State_Key = "Sys_State";
		public const string Sys_Bundle_Start_Key = "Sys_Bundle_Start";
		public const string Sys_Bundle_Succeed_Key = "Sys_Bundle_Succeed";
		public const string Sys_Bundle_Failed_Key = "Sys_Bundle_Failed";
		public const string ParseActivityDataFail_Key = "ParseActivityDataFail";
		#endregion

		#region ProgressQuest活动
		public const string ProgressQuestOfferMsg_Failed_C = "ProgressQuestOfferMsg_Failed_C";
		public const string PQIntroDlgShow_Key = "PQIntroShow";
		public const string PQMainDlgShow_Key = "PQMainShow";
		public const string PQSubRulesDlgShow_Key = "PQSubRulesDlgShow";
		public const string PQDlgShowErr_Key = "PQDlgShowErr";
		public const string PQGetItem_Key = "PQGetItem";
		public const string PQGetAward_Key = "PQGetAward";
		public const string PQOfferShow_Key = "PQOfferShow";
		#endregion

		#region PrimeDeal
		public const string PRIME_DEAL_SKU_MSG_FAILDED = "PrimeDealSKUMsg_Failed";
		#endregion
		#region AdTask
		public const string AdTaskMainDlgShow_Key = "AdTaskMainDlgShow";
		public const string AdTaskEntranceShow_Key = "AdTaskEntranceShow";
		public const string AdTaskCompleted_Key = "AdTaskCompleted";
		#endregion

		#region Coupon活动
		public const string CouponOfferMsg_Failed_C = "CouponOfferMsg_Failed_C";
		public const string CouponMainDlgShow_Key = "CouponMainShow";
		public const string CouponDlgShowErr_Key = "CouponDlgShowErr";
		public const string CouponBuySucceed_Key = "CouponBuySucceed";
		#endregion
		public const string Lobby_LastPlayEntrance_Viewed = "Lobby_LastPlayEntrance_Viewed";
		public const string Lobby_LastPlay_Clicked = "Lobby_LastPlay_Clicked";
		public const string Lobby_FB_Clicked = "Lobby_FB_Clicked";
		public const string SCENE_EXCHANGE_EXCEPTION = "SceneExchangeException";
        public const string DATA_HANDLER_EXCEPTION = "DataHandlerException";
		#region Ads EventLog
		public const string RewardVideo_Clicked = "RewardVideo_Clicked";
		public const string RewardVideo_Start = "RewardVideo_Start";
		public const string RewardVideoEntrance_Viewed = "RewardVideoEntrance_Viewed";
        public const string RewardVideoRequest = "RewardVideoRequest";
        public const string RewardVideoLoadSucceeded = "RewardVideoLoadSucceeded";
		public const string InterstitialAd_Viewed = "InterstitialAd_Viewed";
		//https://developers.google.com/admob/android/interstitial 目前没有提供相应的API，支持点击方法的回调支持
		public const string InterstitialAd_Clicked = "InterstitialAd_Clicked";
		public const string ADS_LOAD = "AdsLoad";
        public const string INTERSTITIAL_SHOW_FAILED = "InterstitialNotShown";
        public const string InterstitialAd = "InterstitialAd";
        public const string ImpressionTrackedEvent = "ImpressionTrackedEvent";

        #endregion

        public const string OPEN_PUSH = "OpenPush";
		public const string LOACALPUSH_SUCCESSED = "LocalPushScheduledSucceed";
		public const string CHOOSE_SETTING_NOTIFICATION = "ChooseSettingNotification";
		public const string CHOOSE_SETTING_JACKPOT_ANNOUNCEMENTS = "ChooseSettingAnnouncements";


		public const string LOG_REPORTER_OPEN = "LogReporterOpen";

		#region NetErrorEvent
		public const string SERVER_SPIN_ERROR = "ServerSpinError";
	    #endregion
        public static Analytics GetInstance ()
        {
            if (instance == null) {
                lock (syncRoot) {
                    if (instance == null) {
                        instance = new Analytics ();
                        instance.StartAnalytics ();
                    }
                }
            }
            return instance;
        }

		public static Dictionary<string,object> AppInstallTimeParams
		{
			get {
				if (appInstallTimeParams == null) {
					appInstallTimeParams = new Dictionary<string, object> ();
					double appInstallTime = NativeAPIs._AppInstallTime ();
					DateTime dateTime = new DateTime (1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds (appInstallTime);
					applicationInstallTime = dateTime.ToString ("yyyy-MM-dd-HH-mm");
					appInstallTimeParams.Add ("appInstallTime", applicationInstallTime);
				}
				return appInstallTimeParams;
			}
		}

		public static string ApplicationInstallTime{
			get  {
				return applicationInstallTime;
			}
		}

        public void StartAnalytics ()
        {
           //_StartAnalytics("","","","","");
        }

        public void EndAnalytics ()
        {
            //_EndAnalytics ();
        }

        public void LogEvent (string info)
        {
			LogVerboseEvent (info, (Dictionary<string,object>)null);
        }

        public void LogEvent (string info, string key, object value)
        {
            Dictionary<string,object> parameters = new Dictionary<string,object> ();
            parameters.Add (key, value);
			LogVerboseEvent (info, parameters);
        }

		public void LogEvent (string info, Dictionary<string,object> parameters)
		{
			LogVerboseEvent (info, parameters);
		}

		public void LogVerboseEvent(string info, string key, string value)
		{
			Dictionary<string,object> parms = new Dictionary<string, object> ();
			parms.Add (key, value);
			LogVerboseEvent (info, parms);
		}

		public  void LogVerboseEvent (string info, Dictionary<string,object> parameters)
		{
			if (parameters == null) {
				parameters = new Dictionary<string, object> ();
			}

			UserProfile profile = UserManager.GetInstance ().UserProfile ();
			long balance = profile.Balance ();
			parameters [CurrentBalanceKey] = balance;

			parameters [CurrentLevelKey] = profile.Level();

			if (BaseGameConsole.ActiveGameConsole ().IsInSlotMachine ()) {
				BaseSlotMachineController slot = BaseGameConsole.ActiveGameConsole ().SlotMachineController;
				parameters [CurrentBetNumKey] = slot.currentBetting;
//				parameters[CurrentCoinsGetKey] =slot.totalAward;
			} else {
				parameters [CurrentBetNumKey] = 0;
//				parameters[CurrentCoinsGetKey] = 0;
			}
			parameters [CurrentThemeKey] = UnityEngine.SceneManagement.SceneManager.GetActiveScene ().name; //Application.loadedLevelName;

			parameters [ContinuousLoginDaysKey] = 0;
			
			parameters[UserID] = SystemInfo.deviceUniqueIdentifier ;//AppInstallTimeParams["appInstallTime"];
		
			int maxParamCount = 7;
			if (parameters.Count > maxParamCount) {
				string[] keys = new string[parameters.Count];
				parameters.Keys.CopyTo (keys, 0);

				if (parameters.Count > maxParamCount && parameters.ContainsKey (CurrentCoinsGetKey)) {
					parameters.Remove (CurrentCoinsGetKey);
				}
				if (parameters.Count > maxParamCount && parameters.ContainsKey (CurrentBetNumKey)) {
					parameters.Remove (CurrentBetNumKey);
				} 
				if (parameters.Count > maxParamCount &&parameters.ContainsKey (CurrentLevelKey)) {
					parameters.Remove (CurrentLevelKey);
				} 
				if (parameters.Count > maxParamCount && parameters.ContainsKey (ContinuousLoginDaysKey)) {
					parameters.Remove (ContinuousLoginDaysKey);
				} 

			}
			_LogEvent (info, parameters);

		}

		public static readonly string BalanceKey = "1";
		public static readonly string LevelKey = "2";
		public static readonly string BetKey = "3";
		public static readonly string LinesKey = "4";
		public static readonly string ThemeKey = "5";

        //private static bool _StartAnalytics (string appId, 
        //                                                 string flurryKey, string tapjoyID,
        //                                                 string tapjoySecretKey, string mPointID)
        //{
        //    return Plugins.NativeAPIs._StartAnalytics (appId, flurryKey, tapjoyID, tapjoySecretKey, mPointID);
        //}

        //private static bool _EndAnalytics ()
        //{
        //    //return Plugins.NativeAPIs._EndAnalytics ();
        //}

		protected static bool _LogEvent (string info, Dictionary<string, object> parameters)
		{
			if (parameters == null || (parameters!=null && parameters.Count <= 7)) 
			{
                //string keyvalueJSONStr = "";
                //if (parameters != null) {
                //	keyvalueJSONStr = Json.Serialize (parameters);
                //} 

                Dictionary<string, string> flurryDict = new Dictionary<string, string>();
				if(parameters != null)
				{
					foreach (var item in parameters.Keys)
					{
						if(!string.IsNullOrEmpty(item))
						{
							if(parameters[item] != null)
							{
								flurryDict.Add(item, parameters[item].ToString());
							}
						}
					}
				}
				return true;

            } else {
				return false;
			}
		}

        protected static Analytics instance;
        private static object syncRoot = new System.Object ();

		private static Dictionary<string,object> appInstallTimeParams;
		private static string applicationInstallTime;
		
		#region MiniGame
		public const string MINI_GAME_MSG_FAILDED = "MiniGameMsg_Failed";
		#endregion
    }
}
