using System;
using Classic;
using Libs;
using System.Collections.Generic;
using UnityEngine;

namespace Ads
{
    public enum ADConditionType
    {
        Time = 0, //时间条件
        Count = 1, //次数条件
    }

    public enum ADType
    {
        RewardAD = 0,       //激励视频广告
        InterstitialAD = 1, //插屏广告
    }
    public class ADManager
    {
        public static ADManager Instance => Singleton<ADManager>.Instance;
        private Action<bool> adCB = null;
        public string requestEntranceName = string.Empty;
        public IAcbAdsCallbackHandler adsCallbackHandler = null;

        public int RewardCount = 0;

        public int InterstitialCount = 0;

        public Dictionary<string, BaseAdNode> AdNodes = new Dictionary<string, BaseAdNode>();

        public Dictionary<string, ADCondition> ADConditions = new Dictionary<string, ADCondition>(); //广告节点

        public ADProgressData adProgressData = new ADProgressData(); //广告本地保存数据

        public void Init()
        {
            adsCallbackHandler = new ADCallBack();
            LoadProgressData();
            ParseEntranceConfig();
        }

        void LoadProgressData()
        {
            ADProgressData data = StoreManager.Instance.LoadDataJson<ADProgressData>(adProgressData.fileName);
            if (data != null)
            {
                adProgressData.LoadData(data);
                RewardCount = adProgressData.RewardCount;
                InterstitialCount = adProgressData.InterstitialCount;
                ADConditions = adProgressData.ADConditions;
            }
        }

        public void SaveADProgressData()
        {
            adProgressData.SaveData();
        }
        void LoadConditionFromLocal(string key, ADCondition baseCondition)
        {
            if (ADConditions.ContainsKey(key))
            {
                ADCondition adCondition = ADConditions[key];
                if (adCondition != null)
                {
                    baseCondition.Clone(adCondition);
                }
            }
        }

        void ParseEntranceConfig()
        {
            Dictionary<string, object> ADConfig = Plugins.Configuration.GetInstance()
                .GetValue<Dictionary<string, object>>(ADConstants.ADConfig, null);
            if (ADConfig == null)
            {
                Debug.LogError("ADConfig is null, please check your configuration file.");
                return;
            }

            Dictionary<string, object> rewardConfig =
                CSharpUtil.GetValue<Dictionary<string, object>>(ADConfig, ADConstants.RewardVideo, null);
            if (rewardConfig == null)
            {
                Debug.LogError("rewardConfig is null, please check your configuration file.");
                return;
            }

            foreach (var config in rewardConfig)
            {
                string entranceName = config.Key;
                Dictionary<string, object> data = config.Value as Dictionary<string, object>;
                if (data == null)
                {
                    Debug.LogError($"ADConfig {entranceName} data is null, please check your configuration file.");
                    continue;
                }

                BaseAdNode adNode = CreateADNode(entranceName, data);
                if (adNode == null)
                {
                    continue;
                }

                AdNodes[entranceName] = adNode;
            }

            Dictionary<string, object> interstitialConfig =
                CSharpUtil.GetValue<Dictionary<string, object>>(ADConfig, ADConstants.InterstitialVideo, null);
            if (interstitialConfig == null)
            {
                Debug.LogError("interstitialConfig is null, please check your configuration file.");
                return;
            }

            foreach (var config in interstitialConfig)
            {
                string entranceName = config.Key;
                Dictionary<string, object> data = config.Value as Dictionary<string, object>;
                if (data == null)
                {
                    Debug.LogError($"ADConfig {entranceName} data is null, please check your configuration file.");
                    continue;
                }

                BaseAdNode adNode = CreateADNode(entranceName, data);
                if (adNode == null)
                {
                    continue;
                }

                AdNodes[entranceName] = adNode;
            }
        }

        BaseAdNode CreateADNode(string entranceName, Dictionary<string, object> data)
        {
            List<object> conditions = Utils.Utilities.GetValue<List<object>>(data, ADConstants.Conditions, null);
            if (conditions == null || conditions.Count == 0)
            {
                Debug.LogError(
                    $"ADConfig {entranceName} conditions is null or empty, please check your configuration file.");
                return null;
            }

            Dictionary<string, object> conditionData = conditions[0] as Dictionary<string, object>;
            if (conditionData == null)
            {
                Debug.LogError($"ADConfig {entranceName} conditionData is null, please check your configuration file.");
                return null;
            }

            //创建广告条件
            int adConditionType = Utils.Utilities.GetInt(conditionData, ADConstants.Type, 0);
            ADCondition adCondition = CreateADCondition(adConditionType, conditionData);
            if (adCondition == null)
            {
                Debug.LogError($"ADConfig {entranceName} adCondition is null, please check your configuration file.");
                return null;
            }

            //从本地存储的数据中赋值
            LoadConditionFromLocal(entranceName, adCondition);
            ADConditions[entranceName] = adCondition;
            //创建广告节点
            BaseAdNode adNode = null;
            switch (entranceName)
            {
                case ADEntrances.REWARD_VIDEO_CONTINUE_SPIN:
                    adNode = new ContinueSpinNode(entranceName, data, adCondition);
                    break;
                case ADEntrances.REWARD_VIDEO_ENTRANCE_JACKPOT:
                    adNode = new JackPotNode(entranceName, data, adCondition);
                    break;
                case ADEntrances.REWARD_VIDEO_ENTRANCE_SPINWIN:
                    adNode = new SpinWinAdNode(entranceName, data, adCondition);
                    break;
                case ADEntrances.REWARD_VIDEO_ENTRANCE_LUCKYCASH:
                    adNode = new LuckyCashAdNode(entranceName, data, adCondition);
                    break;
                case ADEntrances.REWARD_VIDEO_ENTRANCE_CARDLOTTERY:
                    adNode = new CardLotteryAdNode(entranceName, data, adCondition);
                    break;
                case ADEntrances.Interstitial_Entrance_CLOSESPINWIN:
                    adNode = new CloseSpinWinAdNode(entranceName, data, adCondition);
                    break;
                case ADEntrances.Interstitial_Entrance_CLOSEFREESPINEND:
                    adNode = new CloseFreeGameAdNode(entranceName, data, adCondition);
                    break;
                case ADEntrances.Interstitial_Entrance_JACKPOTSTART:
                    adNode = new JackpotStartAdNode(entranceName, data, adCondition);
                    break;
                case ADEntrances.Interstitial_Entrance_JACKPOTEND:
                    adNode = new JackpotEndAdNode(entranceName, data, adCondition);
                    break;
                case ADEntrances.Interstitial_Entrance_FREEGAMESTART:
                    adNode = new FreeGameStartAdNode(entranceName, data, adCondition);
                    break;
                case ADEntrances.REWARD_VIDEO_ENTRANCE_FREESPINEND:
                    adNode = new FreeGameEndAdNode(entranceName, data, adCondition);
                    break;
                case ADEntrances.Interstitial_Entrance_CLOSELUCKYCASH:
                    adNode = new BaseAdNode(entranceName, data, adCondition);
                    break;
            }

            return adNode;
        }

        ADCondition CreateADCondition(int type, Dictionary<string, object> conditionData)
        {
            //创建广告条件
            ADCondition condition = null;
            switch (type)
            {
                case (int)ADConditionType.Count:
                    condition = new ADCountCondition(conditionData);
                    break;
                case (int)ADConditionType.Time:
                    condition = new ADTimeCondition(conditionData);
                    break;
            }

            return condition;
        }

        //构造函数
        ADManager()
        {
            Messenger.AddListener<string>(ADConstants.PlayAdByEntrance, PlayADByEntrance);
            
            Messenger.AddListener(ADEntrances.Interstitial_Entrance_CLOSELUCKYCASH, OnCloseLuckyCash);
            Messenger.AddListener(ADEntrances.Interstitial_Entrance_CLOSESPINWIN, OnCloseSpinWin);
            Messenger.AddListener(ADEntrances.Interstitial_Entrance_CLOSEFREESPINEND, OnCloseFreeSpinEnd);
            Messenger.AddListener(ADEntrances.Interstitial_Entrance_CLOSEBONUSGAMEEND, OnCloseBonusGameEnd);
            Messenger.AddListener(ADEntrances.Interstitial_Entrance_BONUSGAMESTART, OnBonusGameStart);
            Messenger.AddListener(ADEntrances.Interstitial_Entrance_CARDLOTTERY, OnCardLotteryWatchAD);
            Messenger.AddListener(ADEntrances.Interstitial_Entrance_CLOSEFREESPINSTART, OnCloseFreeSpinStart);
        }

        //析构函数
        ~ADManager()
        {
            Messenger.RemoveListener<string>(ADConstants.PlayAdByEntrance, PlayADByEntrance);
            
            Messenger.RemoveListener(ADEntrances.Interstitial_Entrance_CLOSELUCKYCASH, OnCloseLuckyCash);
            Messenger.RemoveListener(ADEntrances.Interstitial_Entrance_CLOSESPINWIN, OnCloseSpinWin);
            Messenger.RemoveListener(ADEntrances.Interstitial_Entrance_CLOSEFREESPINEND, OnCloseFreeSpinEnd);
            Messenger.RemoveListener(ADEntrances.Interstitial_Entrance_CLOSEBONUSGAMEEND, OnCloseBonusGameEnd);
            Messenger.RemoveListener(ADEntrances.Interstitial_Entrance_BONUSGAMESTART, OnBonusGameStart);
            Messenger.RemoveListener(ADEntrances.Interstitial_Entrance_CARDLOTTERY, OnCardLotteryWatchAD);
            Messenger.RemoveListener(ADEntrances.Interstitial_Entrance_CLOSEFREESPINSTART, OnCloseFreeSpinStart);
        }

        void PlayADByEntrance(string entranceName)
        {
            if (string.IsNullOrEmpty(entranceName))
            {
                Debug.LogError("PlayADByEntrance entranceName is null or empty.");
                return;
            }
            //根据入口名称来判断是播放激励视频还是全屏广告
            if (AdNodes.ContainsKey(entranceName))
            {
                BaseAdNode adNode = AdNodes[entranceName];
                if (adNode != null && adNode.IsMeetCondition())
                {
                    //播放广告的前置操作
                    adNode.DoAction();
                    adNode.PlayAd();
                }
                else
                {
                    Messenger.Broadcast<string>(ADConstants.NotMeetConditionMsg,entranceName);
                    Debug.LogWarning($"ADNode {entranceName} is not meet condition.");
                }
            }
        }

        public void StartPlayAD(string entranceName,int type)
        {
            this.requestEntranceName = entranceName;
            //播放广告
            if (type == (int)ADType.RewardAD) //激励视频
            {
                PlayRewardVideo(entranceName);
            }
            else if (type == (int)ADType.InterstitialAD) //全屏广告
            {
                PlayInterstitialAd(entranceName);
            }
        }

        //根据广告入口的类型来给出奖励的倍数。目前统一为 2 倍
        public int GetADRewardMultiple(string entranceName)
        {
            if (AdNodes.ContainsKey(entranceName))
            {
                return AdNodes[entranceName].Multiple;
            }
            return 1;        
        }
        
        #region 激励视频

        /// <summary>
        /// 根据入口判定播放广告是否OK
        /// </summary>
        /// <param name="entranceName">
        /// 广告入口名称
        /// </param>
        /// <returns> true 表示有广告，false 无广告
        /// </returns>
        public bool RewardAdIsOk(string entranceName)
        {
            return PlatformManager.Instance.IsADReady(0);
        }

        /// <summary>
        /// 播放激励视频
        /// </summary>
        /// <param name="entranceName"> 广告入口名称 </param>
        public void PlayRewardVideo(string entranceName)
        {
            if (string.IsNullOrEmpty(entranceName))
            {
                return;
            }
            this.requestEntranceName = entranceName;
            
#if UNITY_EDITOR
            ShowLoadingADsUI(2f,() =>
            {
                HandlePlayVideoResult(0);
            },"play ad");
#else
            if (!RewardAdIsOk(entranceName))
            {
                ShowLoadingADsUI(endCallBack:()=>
                {
                    HandlePlayVideoFailedResult(0);
                });
                return;
            }
            PlatformManager.Instance.SendMsgToPlatFormByType(MessageType.ShowVideo,0);
#endif
        }

        #endregion

        #region 全屏广告

        /// <summary>
        /// 根据入口判定播放广告是否OK
        /// </summary>
        /// <param name="entranceName">
        /// 广告入口名称
        /// </param>
        /// <returns> true 表示有广告，false 无广告
        /// </returns>
        public bool InterstitialAdIsOk(string entranceName)
        {
            return PlatformManager.Instance.IsADReady(1);
        }

        /// <summary>
        /// 播放全屏广告
        /// </summary>
        /// <param name="entranceName"> 广告入口名称 </param>
        /// <param name="callBack"> 播放结果回调，参数为true表示成功 </param>
        /// <param name="para"> ES需要的额外参数，默认为空 </param>
        public void PlayInterstitialAd(string entranceName, Action callBack = null)
        {
            if (string.IsNullOrEmpty(entranceName))
            {
                return;
            }
            
            this.requestEntranceName = entranceName;
            
#if UNITY_EDITOR
            ShowLoadingADsUI(2f,() =>
            {
                HandlePlayVideoResult(1);
            },"play ad");
#else
            if (!InterstitialAdIsOk(entranceName))
            {
                ShowLoadingADsUI(endCallBack:()=>
                {
                    HandlePlayVideoFailedResult(1);
                });
                return;
            }
            PlatformManager.Instance.SendMsgToPlatFormByType(MessageType.ShowVideo,1);
#endif
        }

        #endregion

        #region ADCallBack

        public void OnRewardVideoRewarded(string entranceName)
        {
            if (this.requestEntranceName == entranceName) adCB?.Invoke(true);
            adCB = null;
        }

        public void OnRewardVideoFailed(string entranceName)
        {
            if (this.requestEntranceName == entranceName) adCB?.Invoke(false);
            adCB = null;
        }

        #endregion

        /// <summary>
        /// 广告sdk接在Android端，根据类型来处理不同广告的结果，现在结果都是成功
        /// </summary>
        /// <param name="type">暂时无用，通过看广告的入口来判断当前是哪种类型的广告</param>
        public void HandlePlayVideoResult(int type)
        {
            Messenger.Broadcast(ADConstants.OnPlayVideoEnd);
            adsCallbackHandler.OnVideoReward(requestEntranceName);
        }
        
        public void HandlePlayVideoFailedResult(int type)
        {
            adsCallbackHandler.OnVideoRewardError(requestEntranceName);
        }

        #region TriggerEvent

        private void OnCloseLuckyCash()
        {
            bool adIsReady = InterstitialAdIsOk(ADEntrances.Interstitial_Entrance_CLOSELUCKYCASH);
            //广告未加载好
            if (!adIsReady)
            {
                ShowLoadingADsUI();
                return;
            }

            PlayInterstitialAd(ADEntrances.Interstitial_Entrance_CLOSELUCKYCASH);
        }

        private void OnCloseSpinWin()
        {
            bool adIsReady = InterstitialAdIsOk(ADEntrances.Interstitial_Entrance_CLOSESPINWIN);
            //广告未加载好
            if (!adIsReady)
            {
                ShowLoadingADsUI();
                return;
            }

            PlayInterstitialAd(ADEntrances.Interstitial_Entrance_CLOSESPINWIN);
        }

        private void OnCloseFreeSpinEnd()
        {
            bool adIsReady = InterstitialAdIsOk(ADEntrances.Interstitial_Entrance_CLOSEFREESPINEND);
            //广告未加载好
            if (!adIsReady)
            {
                ShowLoadingADsUI();
                return;
            }

            PlayInterstitialAd(ADEntrances.Interstitial_Entrance_CLOSEFREESPINEND);
        }

        private void OnCloseBonusGameEnd()
        {
            bool adIsReady = InterstitialAdIsOk(ADEntrances.Interstitial_Entrance_CLOSEBONUSGAMEEND);
            //广告未加载好
            if (!adIsReady)
            {
                ShowLoadingADsUI();
                return;
            }

            PlayInterstitialAd(ADEntrances.Interstitial_Entrance_CLOSEBONUSGAMEEND);
        }

        private void OnBonusGameStart()
        {
            bool adIsReady = InterstitialAdIsOk(ADEntrances.Interstitial_Entrance_BONUSGAMESTART);
            //广告未加载好
            if (!adIsReady)
            {
                ShowLoadingADsUI();
                return;
            }

            PlayInterstitialAd(ADEntrances.Interstitial_Entrance_BONUSGAMESTART);
        }

        private void OnCardLotteryWatchAD()
        {
            bool adIsReady = InterstitialAdIsOk(ADEntrances.Interstitial_Entrance_CARDLOTTERY);
            //广告未加载好
            if (!adIsReady)
            {
                ShowLoadingADsUI();
                return;
            }

            PlayInterstitialAd(ADEntrances.Interstitial_Entrance_CARDLOTTERY);
        }

        private void OnCloseFreeSpinStart()
        {
            bool adIsReady = InterstitialAdIsOk(ADEntrances.Interstitial_Entrance_CLOSEFREESPINSTART);
            //广告未加载好
            if (!adIsReady)
            {
                ShowLoadingADsUI();
                return;
            }

            PlayInterstitialAd(ADEntrances.Interstitial_Entrance_CLOSEFREESPINSTART);
        }

        public void ShowLoadingADsUI(float duration = 1.5f, Action endCallBack = null,string msg="")
        {
            if (string.IsNullOrEmpty(msg))
            {
                //展示未加载好广告的提示
                msg = "Loading ADs";
            }
            UIManager.ShowLoadingUI(duration, msg, endCB: endCallBack);
        }
        #endregion
    }
}