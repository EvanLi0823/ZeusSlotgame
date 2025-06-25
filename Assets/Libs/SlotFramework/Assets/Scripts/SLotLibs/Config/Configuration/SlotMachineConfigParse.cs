using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Core;
using Plugins;
using Utils;

namespace Classic
{
    public class SlotMachineConfigParse
    {
        public const string SlotList_Key = "Module/MachineNameArray";
        public const string SlotInfoDict_Key = "SlotInfoDict";
        public const string SLOT_MACHINES = "SlotMachines";
        public const string NAME = "Name";
        public const string WILD_ACCUMULATION = "WildAccumulation";
        public const string SLOT_MACHINE_REQUIRED_LEVEL_KEY = "RequiredLevel";
        public const string SLOT_MACHINE_Prenotice_Unlock_Level_KEY = "PrenoticeUnlockLevel";
        public const string SLOT_MACHINE_USE_TEST_SPEED_KEY = "UseTestSpeed";
        public const string SLOT_MACHINE_LINE_TABLE_NAME_KEY = "Lines";
        public const string SLOT_MACHINE_REMOTE_BUNDLE_URL_KEY = "RemoteBundleVersion";
        public const string SLOT_MACHINE_REMOTE_CARD_BUNDLE_URL_KEY = "RemoteCardVersion";
        public const string SLOT_MACHINE_Enable_Tournament = "EnableTournament";

        public const string SLOT_MACHINE_IS_FEATURE = "IsFeatureSlot";

//		public const string SLOT_MACHINE_ACTIVATE_3D_EFFECT = "Activate3D";
        public const string JACKPOT_INCREASE = "JackPotIncrease";
        public const string JACKPOT_POOL_PRIZE = "JackPotPoolBase";
        public const string JACKPOT_COMMON_VARS = "JackpotVars";
        public const string JACKPOT_COMMON_ENABLE = "EnableJackpot";
        public const string LOBBY_SHOW_JACKPOT = "LobbyShowJackpot";
        public const string NOWIN_REQUEST_POSSIBILITY_KEY = "NoWinRequestPossibility";
        public const string IS_OPEN_ADFREESPINCOUNTADD = "IsOpenAdFreeCountAdd";

        public const string MACHINE_ID = "ID";

        public const string IS_POWER_MACHINE_KEY = "is_power_machine";
        public const string SLOT_MACHINE_IN_PACKAGE_KEY = "InPackage";
        public const string SLOT_MACHINE_CARD_IN_PACKAGE_KEY = "CardInPackage";

        public const string SLOT_MACHINE_IS_TEMP_UNLOCK = "IsTempUnlock";

//		public const string SLOT_MACHINE_IS_SHAKE_STOP = "IsShakeStop";
        public const string SLOT_MACHINE_ADDED_VERSION = "AppVersion";
        public const string SLOT_MACHINE_ENABLE_ROLLING_UP = "EnableRollingUp";

        public const string SLOT_VIP_LEVEL_KEY = "RequiredVipLevel";

//		public const string SLOT_REEL_LIGHT_KEY = "ReelLight";
//		public const string SLOT_TOP_LIGHT_KEY = "TopLight";
        public const string SLOT_MACHINE_ENABLE_SHOW_JACKPOT_KEY = "ShowJackPotInfo";
        public const string SLOT_COMMING_SOON_KEY = "IsCommingSoon";
        public const string SLOT_UNDER_MAINTENANCE_KEY = "IsUnderMaintenance";
        public const string SLOT_LIST_MACHINE_NAME_KEY = "MachineName";

        public const string IS_PORTRAIT = "IsPortrait";
        /// 是否为竖版机器
#if PLATFORM_GOLDS
        public const string DefaultSlotAddedVersion = "0.0.0";
#else
		public const string DefaultSlotAddedVersion = "1.6.0";
#endif

        public const string SLOT_IS_NEW_KEY = "IsNew";
        public const string USE_OPEN_FEATURE_BET_KEY = "UseFeatureOpenBet";
        public const string CARD_TYPE = "CardType";
        public const string MACHINE_PLIST_NAME = "MachinePlistName";

        public const string HIDDEN_MACHINE_ID_KEY = "HiddenMachineIdKey";

        public const string NEW_USER_SLOT_LIST_DAY = "NewUserSlotOrder/NewSlotListDay";
        public const string NEW_USER_SLOT_LIST_KEY = "NewUserSlotOrder/NewUserSlotList";
        public const string RULE_SWICH = "RuleType";
        private const string Spin_UseNetwork = "SpinUseNetwork";
        private const string Spin_UseFakeStrip = "SpinUseFakeStrip";
        private const string SpinButton_WaitNet = "SpinButtonWaitNet";
        public const string FAST_TIME_SCALE = "SpeedRatio";

        #region ClubSystem
        public const string SLOT_IS_HIGH_Roller_KEY = "HighRoller";
        public const string SLOT_IS_SNEAK_PEAK_KEY = "isSneakPeak";
        public const string SLOT_IS_EXCLUSIVE_KEY = "isExclusive";
        public const string ASSET_NAME_KEY = "AssetName";
        #endregion
        private List<object> SlotNameList, newUserSlotNameList;
        Dictionary<string, object> SlotConfigsDict;
        private Dictionary<string, string> bundleComparePath = new Dictionary<string, string>();

        public SlotMachineConfigParse(Dictionary<string, object> config)
        {
            //todo 需要配置成单个机器，去掉解锁等级，LevelPlan限制
            SlotNameList = InitMachineNameList();
            SlotConfigsDict = CSharpUtil.GetValueWithPath<Dictionary<string, object>>(config, SlotMachineConfigParse.SlotInfoDict_Key, null);
        }

        private List<object> InitMachineNameList()
        {
            List<object> configs = Plugins.Configuration.GetInstance()
                .GetValueWithPath<List<object>>("Module/MachineNameArray", null);
            int MachineNameConfigIndex = Plugins.Configuration.GetInstance()
                .GetValueWithPath<int>("Module/MachineNameIndex", 0);
            if (configs == null || configs.Count <= MachineNameConfigIndex)
            {
                Debug.LogError($"MachineName  Error configs.Count {configs.Count},MachineNameConfigIndex{MachineNameConfigIndex}");
            }

            return configs[MachineNameConfigIndex] as List<object>;
        }


        public List<SlotMachineConfig> ParseSlotConfigs(object context)
        {
            ConfigurationParseResult result = context as ConfigurationParseResult;
            if (result == null) throw new ArgumentNullException("context");
            if (SlotNameList == null || SlotConfigsDict == null) throw new ArgumentNullException("SlotList Or SlotConfigsDict == null");
            List<SlotMachineConfig> slotMachineConfigList = new List<SlotMachineConfig>();

            for (int i = 0; i < SlotNameList.Count; i++)
            {
                string slotName = SlotNameList[i] as string;
                if (string.IsNullOrEmpty(slotName)) continue;
                if (!SlotConfigsDict.ContainsKey(slotName)) continue;

                Dictionary<string, object> slotConfigs = SlotConfigsDict[slotName] as Dictionary<string, object>;
                SlotMachineConfig slotMachineConfig = ParseSlotMachineConfig(slotName, slotConfigs, i);
                if (slotMachineConfig == null) continue;

                slotMachineConfigList.Add(slotMachineConfig);
            }

            return slotMachineConfigList;
        }
        
        protected virtual SlotMachineConfig ParseSlotMachineConfig(string slotName, Dictionary<string, object> slotConfig, int slotIndex = 0)
        {
            if (string.IsNullOrEmpty(slotName)) return null;

            int requireVipLevel = Utilities.GetInt(slotConfig, SLOT_VIP_LEVEL_KEY);
            int requiredLevel = Utilities.GetInt(slotConfig, SLOT_MACHINE_REQUIRED_LEVEL_KEY);
            int prenoticeUnlockLevel = Utilities.GetInt(slotConfig, SLOT_MACHINE_Prenotice_Unlock_Level_KEY);
            //读取noWin的设置概率 默认值为0
            int noWinRequestPossibility = Utilities.GetInt(slotConfig, NOWIN_REQUEST_POSSIBILITY_KEY, 0);

            // 读取机器序号
            int machineId = Utilities.GetInt(slotConfig, MACHINE_ID, 0);

            string remoteCardVersion = Utilities.GetValue<string>(slotConfig, SLOT_MACHINE_REMOTE_CARD_BUNDLE_URL_KEY, "0.0.0");
            string remoteBundleVersion = Utilities.GetValue<string>(slotConfig, SLOT_MACHINE_REMOTE_BUNDLE_URL_KEY, string.Empty);
            string addedVersion = Utilities.GetValue<string>(slotConfig, SLOT_MACHINE_ADDED_VERSION, string.Empty);
            if (string.IsNullOrEmpty(addedVersion)) addedVersion = DefaultSlotAddedVersion;

            bool isNew = Utilities.GetBool(slotConfig, SLOT_IS_NEW_KEY, false);
            bool isFeature = Utilities.GetBool(slotConfig, SLOT_MACHINE_IS_FEATURE, false);
            bool bInPackage = Utilities.GetBool(slotConfig, SLOT_MACHINE_IN_PACKAGE_KEY, false);
            bool IsTempUnlock = Utilities.GetBool(slotConfig, SLOT_MACHINE_IS_TEMP_UNLOCK, false);
            bool enableTournament = Utilities.GetBool(slotConfig, SLOT_MACHINE_Enable_Tournament, false);
            bool isCardInPackage = Utilities.GetBool(slotConfig, SLOT_MACHINE_CARD_IN_PACKAGE_KEY, false);
            bool IsNeedUpdateApp = UnityUtil.CompareVersion(addedVersion, Application.version) > 0;
            bool EnableRollingUp = Utilities.GetBool(slotConfig, SLOT_MACHINE_ENABLE_ROLLING_UP, false);
//			bool activate3DEffect = Utilities.GetBool(slotConfig, SLOT_MACHINE_ACTIVATE_3D_EFFECT, false);
            bool isPortrait = Utilities.GetBool(slotConfig, IS_PORTRAIT, false);
            bool bEnableShowJackPot = Utilities.GetBool(slotConfig, SLOT_MACHINE_ENABLE_SHOW_JACKPOT_KEY, true);
            bool isCommongSoon = Utilities.GetBool(slotConfig, SLOT_COMMING_SOON_KEY, false);
            bool isUnderMaintenance = Utilities.GetBool(slotConfig, SLOT_UNDER_MAINTENANCE_KEY, false);

            int isPowerMachine = Utilities.GetInt(slotConfig, IS_POWER_MACHINE_KEY, 0);

            #region ClubSystem
            bool isHighRoller = Utilities.GetBool(slotConfig, SLOT_IS_HIGH_Roller_KEY, false);
            bool isSneakPeak = Utilities.GetBool(slotConfig, SLOT_IS_SNEAK_PEAK_KEY, false);
            bool isExclusive = Utilities.GetBool(slotConfig, SLOT_IS_EXCLUSIVE_KEY, false);
            string assetName = Utilities.GetValue<string>(slotConfig, ASSET_NAME_KEY, slotName);
            #endregion
            SlotMachineConfig slotMachineConfig = new SlotMachineConfig();
            slotMachineConfig.SetName(slotName);
            slotMachineConfig.SetComparePath(bundleComparePath.ContainsKey(slotName) ? bundleComparePath[slotName] : "");
            slotMachineConfig.SetRequiredLevel(requiredLevel);
            slotMachineConfig.SetPrenoticeUnlockLevel(prenoticeUnlockLevel);
            slotMachineConfig.SlotIndex = slotIndex;
            slotMachineConfig.VipLockLevel = requireVipLevel;
            slotMachineConfig.SetRemoteBundleURL(remoteBundleVersion);
            slotMachineConfig.SetRemoteCardBundleURL(remoteCardVersion);
            slotMachineConfig.EnableTournament = enableTournament;
            slotMachineConfig.IsFeatureSlot = isFeature;
            slotMachineConfig.IsMachineInPackage = bInPackage;
            slotMachineConfig.EnableShowJackPotInfo = bEnableShowJackPot;
            slotMachineConfig.IsCardInPackage = isCardInPackage;
            slotMachineConfig.IsTempUnlock = IsTempUnlock;
//			slotMachineConfig.IsShakeStop = Utilities.GetBool (slotConfig, SLOT_MACHINE_IS_SHAKE_STOP, false);
            slotMachineConfig.IsNeedUpdateApp = IsNeedUpdateApp;
            slotMachineConfig.CardType = Utilities.GetValue<string>(slotConfig, CARD_TYPE, "Cards");
            slotMachineConfig.IsNewSlot = isNew;
            slotMachineConfig.NoWinRequestPossibility = noWinRequestPossibility;
            slotMachineConfig.MachineId = machineId;
            slotMachineConfig.EnableRollingUp = EnableRollingUp;
//			slotMachineConfig.Activate3DEffect = activate3DEffect;
            slotMachineConfig.IsCommingSoon = isCommongSoon;
            slotMachineConfig.IsUnderMaintenance = isUnderMaintenance;
            slotMachineConfig.IsPowerMachine = isPowerMachine;
            #region ClubSystem
            slotMachineConfig.isExclusive = isExclusive;
            slotMachineConfig.isSneakPeak = isSneakPeak;
            slotMachineConfig.isHighRoller = isHighRoller;
            slotMachineConfig.AssetName = assetName;
            #endregion
            slotMachineConfig.IsPortrait = isPortrait;
            slotMachineConfig.PlistName = Utilities.GetValue<string>(slotConfig, MACHINE_PLIST_NAME, assetName);
            slotMachineConfig.InitJackPotPrize(Utils.Utilities.GetValue<List<object>>(slotConfig, JACKPOT_POOL_PRIZE, null));
            slotMachineConfig.InitJackPotIncrease(Utils.Utilities.GetValue<List<object>>(slotConfig, JACKPOT_INCREASE, null));
            
            slotMachineConfig.InitLobbyShowJackInfo(Utils.Utilities.GetValue<Dictionary<string, object>>(slotConfig, LOBBY_SHOW_JACKPOT, null));

            slotMachineConfig.IsOpenAdFreespinCountAdd = Utilities.GetBool(slotConfig, IS_OPEN_ADFREESPINCOUNTADD, false);

            slotMachineConfig.CaculateMachineSpinCount();
            slotMachineConfig.OrderInfo = slotConfig;
            slotMachineConfig.IsUseTestSpeed =
                Utils.Utilities.GetBool(slotConfig, SLOT_MACHINE_USE_TEST_SPEED_KEY, false);
            slotMachineConfig.RuleSwitch.SetSwitchValue(Utilities.GetValue<string>(slotConfig, RULE_SWICH, "11"));
            slotMachineConfig.SpinUseNetwork = Utilities.GetValue<bool>(slotConfig, Spin_UseNetwork, false);
            slotMachineConfig.FastTimeScale = Utilities.GetFloat(slotConfig, FAST_TIME_SCALE, -1f);
            slotMachineConfig.SpinUseFakeStrip = Utilities.GetValue<bool>(slotConfig, Spin_UseFakeStrip, false);
            slotMachineConfig.SpinButtonWaitNet = Utilities.GetValue<bool>(slotConfig, SpinButton_WaitNet, false);
            slotMachineConfig.useFeatureOpenBet = Utilities.GetValue<bool>(slotConfig, USE_OPEN_FEATURE_BET_KEY, false);
            return slotMachineConfig;
        }
    }
}