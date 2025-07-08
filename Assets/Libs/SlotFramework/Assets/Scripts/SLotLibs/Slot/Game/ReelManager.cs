using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Core;
using Utils;
using Libs;
using System.Text;
using UnityEngine.Profiling;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace Classic
{
    public class ReelManager : MonoBehaviour, IRunControllerAdapter, IRuleResult
    {
        public bool showAwardLineInfo = false;
        public AnimationCanvas animationCanvas;
        public GameObject runningEffect;

        public IResultChangeController resultChangeContronller;

        public List<BaseElementPanel> awardElements = new List<BaseElementPanel>();
        public List<AwardLineElement> awardLines = new List<AwardLineElement>();

        public List<GameObject> reelWinCoinAnimations;

        public List<GameObject> MiddleWinAnimations;
        public List<GameObject> bonusTriggerAnimations;
        protected List<List<int>> preSymbolResultsList = new List<List<int>>();
        public static readonly string InitiationAudioName = "Initiation";

        public bool EnableInitBackgroundAudio = false; 

        //此变量为解决EpicWin弹窗后Bet按钮必定变为有效的问题而设置
        public bool enableBetChangeAfterEpicWin = true;

        [HideInInspector] public long MaxBalanceInTheSession = 0; //EventCondition条件检测使用 
        protected WaitForSeconds m_AnimationWaitForScecond = null;

        //有link的game需要设置此值,否则link的rule将不起作用.默认为0的关卡不开启
        protected int LinkNeedNum = 0;
        public double JackPotIncreaseValue = 0;
        public ResultProcessData ResultData;
        [HideInInspector]
        public bool EnterFreeIsWait = true;

        public FakeStrip fakeStrip;
        private bool isSingleBoard;

        [HideInInspector]
        protected bool IsAlreadyAddFreeSpinCount = false;

        public void SetIsAddFreeSpinCount( bool isAdd)
        {
            IsAlreadyAddFreeSpinCount = isAdd;
        }

        public void StopBaseGameMusic()
        {
            if (EnableInitBackgroundAudio)
            {
                Libs.AudioEntity.Instance.StopMusicAudio(InitiationAudioName);
            }
        }
        
        public virtual bool IsStartSpin()
        {
            return true;
        }

        public virtual bool IsShowBigWin()
        {
            return true;
        }

        public void OnRollEndHandler(System.Action callback)
        {
            if (!(isFreespinBonus || NextTimeOnceMore || RewardSpinManager.Instance.RewardIsValid()))
            {
                Messenger.Broadcast(SlotControllerConstants.OnRechargeableSpinWillEnd);
            }

            HandleRollEndEvent(callback);
        }


        protected virtual void HandleRollEndEvent(System.Action callback)
        {
            if (callback != null) callback();
        }

        public virtual void InitReels(SlotMachineConfig slotConfig, GameCallback onStop = null,
            GameCallback onStart = null)
        {
            //NewSpeed
            boardController = this.GetComponent<ReelController>();
            smartEffectController = this.GetComponent<SmartAntiEffectController>();
            SetSingleBoard(false);
            long currentBalance = UserManager.GetInstance().UserProfile().Balance();
            long MaxBalance = UserManager.GetInstance().MaxBalance;
            UserManager.GetInstance().MaxBalance = currentBalance > MaxBalance ? currentBalance : MaxBalance;
            //Debug.Log ("MaxBalanceInSession:"+MaxBalanceInTheSession+" currentBalance:"+currentBalance);
            this.slotConfig = slotConfig;
            this.reelManagerDataConfig = slotConfig.reelManagerDataConfig;

// #if DEBUG || UNITY_EDITOR
            this.machineTestDataFromPlist = slotConfig.machineTestDataFromPlistConfig;
// #endif

            if (slotConfig.MainBoardStripConfig.LineTable() == null)
                throw new System.Exception("lineTable is null,please check slotmachine plist");

            InitReels(slotConfig.MainBoardStripConfig.SymbolMap, slotConfig.MainBoardStripConfig.BoardStrips,
                slotConfig.MainBoardStripConfig.PayTable, slotConfig.MainBoardStripConfig.LineTable(), onStop, onStart,
                slotConfig.Name());

            if (BaseSlotMachineController.Instance != null)
            {
                m_AnimationWaitForScecond =
                    new WaitForSeconds(BaseSlotMachineController.Instance.averageFrame.averageTime);
            }


            //用于GoldMine和SuperWheel5Reels的Bonus中服务器结果接收 不重置Manager中的结果
            if (!IsInBonusGame)
            {
                SequencePaidResultsManager.Reset();
            }

            if (BonusGame == null)
            {
                BonusGame = GetComponent<BonusGame>();
            }
        }

        public virtual void PlayBGAudio()
        {
            if (EnableInitBackgroundAudio && !isFreespinBonus && !BaseSlotMachineController.Instance.onceMore)
            {
                Libs.AudioEntity.Instance.PlayMuisc(InitiationAudioName, false);
            }
        }

        protected virtual void Awake()
        {
        }

        protected virtual void OnDestroy()
        {
        }
        
        public virtual bool IsMultiBoard()
        {
            return false;
        }

        protected virtual void SetSingleBoard(bool _isSingleBoard)
        {
            isSingleBoard = _isSingleBoard;
        }
        

        public bool GetSingleBoard()
        {
            return isSingleBoard;
        }

        public virtual void InitReels(SymbolMap symbolMap, ReelStripManager reelStrips, ClassicPaytable payTable,
            LineTable lineTable, GameCallback onStop = null, GameCallback onStart = null,
            string name = "GameController")
        {
            if (animationCanvas == null)
            {
                animationCanvas = GetComponentInChildren<AnimationCanvas>();
            }

            this.name = name;
            this.OnStop = onStop;
            this.OnStart = onStart;
            State = GameState.READY;
            resultChangeContronller = GetComponent<BaseResultChange>();
            extraAward = GetComponent<ExtraAwardGame>();
            FreespinGame = GetComponent<FreespinGame>();
            try
            {
                ChangeGameConfigs(symbolMap, reelStrips, GetComponent<GameConfigs>(), payTable, lineTable);
            }
            catch (System.Exception ex)
            {
                string errorMsg = (" reelStrips:" + (reelStrips == null ? true : false) +
                                   " R:" + (reelStrips != null ? reelStrips.GetCurrentUseRName() : "none") +
                                   " A:" + (reelStrips != null ? reelStrips.GetCurrentUseAName() : "none") +
                                   " reels.count:" + ((resultContent != null && resultContent.currentReelStrips != null)
                                       ? resultContent.currentReelStrips.Count
                                       : 0));
                throw new System.Exception(ex.Message + " StackTrace:" + ex.StackTrace + " data:" + errorMsg);
            }

            AutoRun = false;
            StartAnticipationIndex = -1;
            NeedReCreatResult = true;
            GameCallback startRunEffect = () =>
            {
                if (RenderLevelSwitchMgr.Instance.CheckRenderLevelIsOK(GameConstants.SpinAnimationLevel_Key))
                {
                    if (runningEffect != null)
                    {
                        runningEffect.SetActive(true);
                    }
                }
            };

            GameCallback stopRunEffect = () =>
            {
                if (runningEffect != null)
                {
                    runningEffect.SetActive(false);
                }
            };

            this.OnStart = this.OnStart == null ? startRunEffect : this.OnStart + startRunEffect;
            this.OnStop = this.OnStop == null ? stopRunEffect : this.OnStop + stopRunEffect;

            InitAtStart();
        }

        public void ChangeGameConfigs(SlotMachineConfig slotConfig, GameConfigs gameConfigs)
        {
            ChangeGameConfigs(slotConfig.MainBoardStripConfig.SymbolMap, slotConfig.MainBoardStripConfig.BoardStrips,
                gameConfigs, slotConfig.MainBoardStripConfig.PayTable, slotConfig.MainBoardStripConfig.LineTable());
        }

        public virtual void ChangeGameConfigs(SymbolMap symbolMap,
            ReelStripManager reelStrips,
            GameConfigs gameConfigsTemp,
            ClassicPaytable payTable = null,
            LineTable lineTable = null)
        {
            if (symbolMap != null)
            {
                this.symbolMap = symbolMap;
            }

            if (reelStrips != null)
            {
                this.reelStrips = reelStrips;
            }

            if (gameConfigsTemp == null)
            {
                gameConfigsTemp = this.gameConfigs;
            }


            if (lineTable != null)
            {
                this.lineTable = lineTable;
            }

            if (payTable != null)
            {
                this.payTable = payTable;
            }

            if (gameConfigsTemp != null)
            {
                if (!gameConfigsTemp.Equals(this.gameConfigs))
                {
                    this.gameConfigs = gameConfigsTemp;
                }

                if (reelStrips != null) //非后端化机器的使用方式
                {
                    ChangeResultContentOnInit(reelStrips, gameConfigsTemp);
                }

                if (this.SpinUseNetwork)
                {
                    this.resultContent = new ResultContent(gameConfigsTemp,GetBoardConfigs());
                }

                this.gameConfigs.ReplaceReelManagerData(reelManagerDataConfig);

                BonusGame = gameConfigs.bonusGame;

                //新的转速模型
                LayoutBoardOnNewSpeedPattern();
            }

            AfterInitReelAnimationHandler();
            Messenger.Broadcast<ReelManager>(GameConstants.ChangeGameConfigsMsg, this);
        }

        public virtual void LayoutBoardOnNewSpeedPattern()
        {
            //新的转速模型
            if (this.IsNewSpeedPattern)
            {
                LayoutBoard();
            }
        }

        public virtual void ChangeResultContentOnInit(ReelStripManager reelStrips, GameConfigs gameConfigsTemp)
        {
            this.resultContent = new ResultContent(reelStrips, gameConfigsTemp, GetBoardConfigs());
            ChangeResultContentDataOnChangeGameConfigs(gameConfigsTemp);
        }

        public virtual void ChangeResultContentOnSpin(ReelStripManager reelStrips, GameConfigs gameConfigsTemp)
        {
            if (!reelStrips.CanMeetTheCondition(resultContent.currentReelStrips))
            {
                resultContent.ChangeStripsResultContent(reelStrips, gameConfigsTemp, GetBoardConfigs());
            }
        }

        public virtual float PreBigWinEvent()
        {
            return 0f;
        }

        protected void BackupPreSymbolResults()
        {
            //备份结果值，以供下次使用
            preSymbolResultsList.Clear();
            for (int i = 0; i < this.GetReelCount(); i++)
            {
                List<int> SR = new List<int>();
                for (int j = 0; j < resultContent.ReelResults[i].SymbolResults.Count; j++)
                {
                    SR.Add(resultContent.ReelResults[i].SymbolResults[j]);
                }

                preSymbolResultsList.Add(SR);
            }
        }

        protected void RestorePreExcludeSymbolResults(List<int> excludereelIndexList)
        {
            //还原结果值 因为现在每次spin会自动将resultContent重新实例化，导致原有数据消失
            if (preSymbolResultsList.Count > 0)
            {
                for (int i = 0; i < excludereelIndexList.Count; i++)
                {
                    int reelIndex = excludereelIndexList[i];
                    List<int> sR = resultContent.ReelResults[reelIndex].SymbolResults;
                    sR.Clear();
                    sR.AddRange(preSymbolResultsList[reelIndex]);
                }
            }
        }

        //指定本次Spin是否花钱,在Spin开始时被设置为false,花钱后被设置为true;
        public bool IsSpinCostCoins { get; set; }

        public virtual bool IsCostBetSpin()
        {
            bool onceMore = false;
            if (BaseSlotMachineController.Instance != null)
            {
                //确保是主的,多棋盘的时候算免费的
                if (this != BaseSlotMachineController.Instance.reelManager) return false; 
                onceMore = BaseSlotMachineController.Instance.onceMore;
            }
            else
            {
                onceMore = TestController.Instance.isOneMore;
            }

            return !isFreespinBonus && !onceMore && !(BonusGame != null && BonusGame.IsBonusGame);
        }

        public virtual void ChangeResultContentDataOnChangeGameConfigs(GameConfigs gameConfigsTemp)
        {
        }

        public virtual bool CheckClearWinTextCondition()
        {
            return true;
        }
        
        #region Restore ExtraGameData

        public List<List<int>> baseGameResult;
        public List<List<int>> freeSpinResult;

//        public string baseGameRName;
//        public string baseGameAName;
        [HideInInspector] public bool freespinFinished = false;

        //具体机器返回大厅后，在进来后进行恢复
        public virtual void RecoveryPlayerPreviousSlotMachineStateData()
        {
            ExtraGameStateResultDataManager dataRecoveryManaer = new ExtraGameStateResultDataManager();
            if (!dataRecoveryManaer.GetNeedRecoveryDataOnInit())
            {
                return;
            }

            switch (dataRecoveryManaer.GetExtraGameTypeOnInit())
            {
                case ExtraGameStateResultDataManager.ExtraGameType.ExtraGameType_BonusGame:
                {
                    RestoreExtraGameDataToBonusGame();
                }
                    break;
                case ExtraGameStateResultDataManager.ExtraGameType.ExtraGameType_FreespinGame:
                {
                    RestoreExtraGameDataToFreeSpin();
                }
                    break;
                case ExtraGameStateResultDataManager.ExtraGameType.ExtraGameType_BaseGameHitBonus:
                {
                    RestoreExtraGameDataFromBaseGameToBonusGame();
                }
                    break;
                case ExtraGameStateResultDataManager.ExtraGameType.ExtraGameType_BaseGameHitFreeSpin:
                {
                    RestoreExtraGameDataFromBaseGameToFreeSpin();
                }
                    break;
                case ExtraGameStateResultDataManager.ExtraGameType.ExtraGameType_ExtraAward:
                {
                    RestoreExtraGameDataToExtraAwardGame();
                }
                    break;
                default:
                    break;
            }
        }

        public virtual void RestoreExtraGameDataToBonusGame()
        {
            BonusGameStateResultDataManager bonusDataRecovery = new BonusGameStateResultDataManager();
            bonusDataRecovery.RecoverySlotMachinePlayerStateData(this);
            ResultStateManager.Instante.RestoreBonusGame();
        }

        public virtual void RestoreExtraGameDataToFreeSpin()
        {
            FreeSpinStateResulteDataManager freespinRecovery = new FreeSpinStateResulteDataManager();
            freespinRecovery.RecoverySlotMachinePlayerStateData(this);
            //先还原freespin的Gameconfig配置，再还原结果
            ResultStateManager.Instante.RestoreFreespin(freespinRecovery.leftFreeSpinCount,
                freespinRecovery.totalFreeSpinCount,
                freespinRecovery.currentWinCoins, true, freespinRecovery.awardMultipler,curWinCash:freespinRecovery.currentWinCash,needShowCash:freespinRecovery.needShowCash);

            resultContent.ChangeResult(freespinRecovery.freespinResult);
            ChangeSymbols(freespinRecovery.freespinResult);
        }

        public virtual void RestoreExtraGameDataFromBaseGameToBonusGame()
        {
        }

        public virtual void RestoreExtraGameDataFromBaseGameToFreeSpin()
        {
            FreeSpinStateResulteDataManager freespinGameDataManager = new FreeSpinStateResulteDataManager();
            freespinGameDataManager.RecoverySlotMachinePlayerStateDataOnBaseGame(this);
            resultContent.ChangeResult(freespinGameDataManager.baseResult);
            ChangeSymbols(freespinGameDataManager.baseResult);
            ResultStateManager.Instante.RestoreFreespinOnBaseGame(freespinGameDataManager.freespinCount, true,
                freespinGameDataManager.awardMultipler);
        }

        public virtual void RestoreExtraGameDataToExtraAwardGame()
        {
        }

        public virtual void SavePlayerCurrentSlotMachineStateData()
        {
            if (!NeedSavePlayerSlotMachineStateData || freespinFinished)
            {
                return;
            }

            if (BonusGame != null && BonusGame.IsBonusGame)
            {
                currentExtraType = ExtraGameStateResultDataManager.ExtraGameType.ExtraGameType_BonusGame;
            }
            else if (isFreespinBonus && !IsTriggeredFreespinAndNotReelStartRun)
            {
                currentExtraType = ExtraGameStateResultDataManager.ExtraGameType.ExtraGameType_FreespinGame;
            }
            else if (HasBonusGame)
            {
                currentExtraType = ExtraGameStateResultDataManager.ExtraGameType.ExtraGameType_BaseGameHitBonus;
            }
            else if (IsTriggeredFreespinAndNotReelStartRun)
            {
                currentExtraType = ExtraGameStateResultDataManager.ExtraGameType.ExtraGameType_BaseGameHitFreeSpin;
            }
            else if (NeedSaveExtraAwardGameData())
            {
                currentExtraType = ExtraGameStateResultDataManager.ExtraGameType.ExtraGameType_ExtraAward;
            }
            else
            {
                currentExtraType = ExtraGameStateResultDataManager.ExtraGameType.ExtraGameType_None;
            }

            switch (currentExtraType)
            {
                case ExtraGameStateResultDataManager.ExtraGameType.ExtraGameType_BonusGame:
                {
                    SaveExtraGameDataOnBonusGame();
                }
                    break;
                case ExtraGameStateResultDataManager.ExtraGameType.ExtraGameType_FreespinGame:
                {
                    SaveExtraGameDataOnFreeSpin();
                }
                    break;
                case ExtraGameStateResultDataManager.ExtraGameType.ExtraGameType_BaseGameHitBonus:
                {
                    SaveExtraGameDataWhenHitBonusOnBaseGame();
                }
                    break;
                case ExtraGameStateResultDataManager.ExtraGameType.ExtraGameType_BaseGameHitFreeSpin:
                {
                    SaveExtraGameDataWhenHitFreeSpinOnBaseGame();
                }
                    break;
                case ExtraGameStateResultDataManager.ExtraGameType.ExtraGameType_ExtraAward:
                {
                    SaveExtraGameDataOnExtraAwardGame();
                }
                    break;
                default:
                    break;
            }
        }

        public virtual void SaveExtraGameDataOnBonusGame()
        {
            BonusGameStateResultDataManager bonusGameDataManager = new BonusGameStateResultDataManager();
            bonusGameDataManager.SaveSlotMachinePlayerStateData(this);
        }

        public virtual void SaveExtraGameDataOnFreeSpin()
        {
            FreeSpinStateResulteDataManager freespinGameDataManager = new FreeSpinStateResulteDataManager();
            freespinGameDataManager.SaveSlotMachinePlayerStateData(this);
        }

        public virtual void SaveExtraGameDataWhenHitBonusOnBaseGame()
        {
        }

        public virtual void SaveExtraGameDataWhenHitFreeSpinOnBaseGame()
        {
            FreeSpinStateResulteDataManager freespinGameDataManager = new FreeSpinStateResulteDataManager();
            freespinGameDataManager.SaveSlotMachinePlayerStateDataOnBaseGame(this);
        }

        public virtual void SaveExtraGameDataOnExtraAwardGame()
        {
        }

        public virtual bool NeedSaveExtraAwardGameData()
        {
            return false;
        }

        #endregion

        public bool IsTriggeredFreespinAndNotReelStartRun = false;

        /// 用于SuperWheel5Reels GoldMine  以及埃及关Bonsu信息向服务器汇报使用
        /// 特殊关卡使用的时候 用new操作符重载
        public bool IsInBonusGame = false;

        //将处理结果放到一个方法里进行处理，未来可能会不断的优化

        public virtual bool StartRun()
        {
            if (State == GameState.READY)
            {
                
                IsSpinCostCoins = false; //开始时初始化为false;
                State = GameState.PRERUNNING;

                PreStartState();

                try
                {
                    BaseSlotMachineController.Instance.baseAward = 0;
                    SpinClickedMsgMgr.Instance.ResetData(BaseSlotMachineController.Instance.isFreeRun || IsInBonusGame);
                    bool isPatternOutComeSucceed = false;
                    CreateRawResult();
                    if(!HasUseRuleFeature)
                    {
                        //用rule的时候已经check过了,　不用再执行了
                        CheckFeature();
                    }

                    if (this.OnStart != null)
                    {
                        this.OnStart();
                    }

                    ParseSpinResult();
                    
                    #region OutCome

                    
                    //使用了OutCome 而且没有轮子被固定住 则使用AddOutCome(OutCome outcome)方法上传
                    //                    if(isUsePattern && ReelLocked ==false)
                    if (isUseForcePattern)
                    {
                        if (currentServerOutCome != null)
                        {
                            //使用服务器
                            SpinClickedMsgMgr.Instance.AddOutCome(currentServerOutCome);
                            isPatternOutComeSucceed = true;
                        }
                    }

                    if (isPatternOutComeSucceed == false)
                    {
                        SpinClickedMsgMgr.Instance.AddOutCome(this);
                    }

                    #endregion
                }
                  catch (Exception e)
                {
                    Debug.LogError(e.Message);
                    State = GameState.READY;
                    throw;
                    return false;
                }

                InitStartState();
                ReelsStartRun();

                return true;
            }

            return false;
        }

        public virtual void PreStartState()
        {
            ResetDataBeforeSpin();
            if (reelStrips.selectedResult != null && reelStrips.selectedResult.selectedFixedResult != null)
            {
                reelStrips.selectedResult.selectedFixedResult = null;
            }

            if (NeedReCreatResult)
            {
                ChangeResultContentOnSpin(this.reelStrips, this.gameConfigs);
            }

        }
        
        protected void ResetDataBeforeSpin()
        {
            if (BaseSlotMachineController.Instance!=null)
            {
                if(IsNewProcess)
                    ResultStateManager.Instante.RemoveAll();

                SetIsAddFreeSpinCount(false);
                this.FreespinCount = 0;
                
                HitFs = false;
            }
           
            this.HitLinkGame = false; //每次都要初始化
            HasUseRuleFeature = false;
            if (this.IsCostBetSpin())
            {
                this.ResetPatternEsType();
                IsCollectGame = false;
            }
        }
        
        //jackpot 的优先级最高
        public virtual bool CheckCustomCondition(Dictionary<string, object> info)
        {
            if (info == null)
            {
                return false;
            }

            if (!info.ContainsKey(SlotControllerConstants.CUSTOM_JACKPOT_FLAG))
            {
                return CheckCustomConditionExtra(info);
            }

            if ((bool) (info[SlotControllerConstants.CUSTOM_JACKPOT_FLAG]) == false)
            {
                return false;
            }

            //生成JackFlag的前置判断条件：非Freespin、轮子非锁定、非OnceMore
//			Log.Trace("ExistReelLocked:"+ExistReelLocked +"----isFreespinBonus:"+isFreespinBonus+"---NextTimeOnceMore:"+NextTimeOnceMore);
            if (this.ExistReelLocked || this.isFreespinBonus || this.NextTimeOnceMore)
            {
                return false;
            }
            //         //未付费用户不能中jackpot 暂时注释
            //if (UserManager.GetInstance ().UserProfile ().AllConsumedUSDs <= 0) {
            //	return false;
            //}
//			return true;  //for test

            return false;
        }

        /// <summary>
        /// 有限
        /// Checks the custom condition extra.
        /// </summary>
        /// <returns><c>true</c>, if custom condition extra was checked, <c>false</c> otherwise.</returns>
        /// <param name="info">Info.</param>
        public virtual bool CheckCustomConditionExtra(Dictionary<string, object> info)
        {
            return false;
        }

        protected virtual void InitStartState()
        {
            fastStop = false;
//            if (!IsUnifiedParseResult)
//            {
//                CheckAnticipationAnimation();
//            }
            StopSmallWinAnimation();
            StopMiddleWinAnimation();
            StopBigAnimation();
            StopSound();
//            PlaySpinSound(); // 20210601，只有8个classic移植关卡有用到
        }

        protected void StopSound()
        {
            Libs.AudioEntity.Instance.StopAllEffect();
            Messenger.Broadcast(SlotControllerConstants.OnInterruptRollingEffect);
        }

//        protected virtual void PlaySpinSound()
//        {
//            PlayWheelSpinSound();
//        }

        public virtual void PlayRollEndSound(int RollIndex)
        {
            Libs.AudioEntity.Instance.PlayReelStopEffect(RollIndex);
        }

        private static readonly string audioWhellSpin = "Wheel-spin";
        private int spinCountForAudio = 0;

        protected void PlayWheelSpinSound()
        {
            spinCountForAudio++;
            spinCountForAudio = spinCountForAudio % 3;
            if (spinCountForAudio == 0)
            {
                spinCountForAudio = 3;
            }

            Libs.AudioEntity.Instance.PlayEffect(audioWhellSpin + spinCountForAudio);
        }


        protected virtual void RestoreFreeSpinOnReelsStartRun()
        {
            #region Restore FreeSpin

            if (isFreespinBonus && FreespinGame.isRestoreFreespin && !FreespinGame.currentSpinFinished)
            {
                FreespinGame.isRestoreFreespin = false;
                resultContent.ChangeResult(freeSpinResult);
                ChangeSymbols(freeSpinResult);
            }

            #endregion
        }

        protected virtual void ReelsStartRun()
        {
            // PlayStartRunAudio();

            RestoreFreeSpinOnReelsStartRun();

            // Messenger.Broadcast(SlotControllerConstants.InCommonJackPotPanel);//关闭通用jaccpot

            //NewSpeed
            if (this.IsNewSpeedPattern)
            {
                //NeedAntiReels = this.spinEffectController.GenerateSpinEffectResult (GetCurrentResults(),symbolMap);
                //TODO: result处理
                if (smartEffectController != null)
                {
                    this.NeedAntiReels =
                        smartEffectController.CheckSmartPositionAndAnticipation(this.resultContent.ReelResults,
                            this.symbolMap, this);
                }

                this.NeedPlaySmartSound = CheckPlaySmartSound();

                this.boardController.SetTestSpeedPattern(this.slotConfig.IsUseTestSpeed);
              
                this.boardController.DoReelsSpin(GetResultData(),GetShowData(), this.NeedAntiReels,
                    GetReelCurveIndexs(), NeedPlaySmartSound);
            }

            if (isFreespinBonus)
            {
                IsTriggeredFreespinAndNotReelStartRun = false;
            }
            else
            {
                if (FreespinCount > 0)
                {
                    IsTriggeredFreespinAndNotReelStartRun = true;
                }
            }

            SaveBoardProgressResult();
        }

        protected virtual List<List<int>> GetResultData()
        {
            List<List<int>> data = new List<List<int>>();
            
            for (int i = 0; i < resultContent.ReelResults.Count; i++)
            {
                data.Add(resultContent.ReelResults[i].SymbolResults);
            }

            return data;
        }
        
        protected virtual List<List<int>> GetShowData()
        {
            List<List<int>> data = slotConfig.FakeStrip?.GetRunData(isFreespinBonus);
            if (data != null) return data;
            
            data = new List<List<int>>();
            for (int i = 0; i < resultContent.ReelResults.Count; i++)
            {
                data.Add(resultContent.ReelResults[i].ShowIndexs);
            }
            return data;
        }

        protected virtual void SaveBoardProgressResult()
        {
            #region Restore ReelsResultData

            if (isFreespinBonus)
            {
                //保存每次旋转产生的初始结果数据
                freeSpinResult = new List<List<int>>();
                for (int i = 0; i < resultContent.ReelResults.Count; i++)
                {
                    freeSpinResult.Add(resultContent.ReelResults[i].SymbolResults);
                }
            }
            else
            {
                baseGameResult = new List<List<int>>();
                for (int i = 0; i < resultContent.ReelResults.Count; i++)
                {
                    baseGameResult.Add(resultContent.ReelResults[i].SymbolResults);
                }
            }

            #endregion
        }

        /// <summary>
        /// 此处必须做深拷贝
        /// </summary>
        /// <returns></returns>
        public List<List<int>> GetCurrentResults()
        {
            List<List<int>> gameResult = new List<List<int>>();
            for (int i = 0; i < resultContent.ReelResults.Count; i++)
            {
                int count = resultContent.ReelResults[i].SymbolResults.Count;
                List<int> reelResult = new List<int>();
                for (int j = 0; j < count; j++)
                {
                    reelResult.Add(resultContent.ReelResults[i].SymbolResults[j]);
                }

                gameResult.Add(reelResult);
            }

            return gameResult;
        }

        public virtual void PlayStartRunAudio()
        {
            AudioEntity.Instance.StopAwardSymbolAudio();

            if (this.isFreespinBonus)
            {
                AudioEntity.Instance.UnPauseBackGroundAudio(AudioEntity.audioSpecial);
            }
            else if (!(BonusGame != null && BonusGame.IsBonusGame))
            {
                AudioEntity.Instance.UnPauseBackGroundAudio(AudioEntity.audioSpin);
            }
        }

        //暂停音效至0
        public virtual void PauseBackGroundAudio(bool reduce = false)
        {
            if (!this.isFreespinBonus && !reduce)
            {
                AudioEntity.Instance.PauseBackGroundAudio(AudioEntity.audioSpin, 6f);
            }

            if (!reduce) return;

            if (this.isFreespinBonus)
            {
                AudioEntity.Instance.PauseBackGroundAudio(AudioEntity.audioSpecial, 10f,
                    ReduceVolume(AudioEntity.audioSpecial));
            }
            else if (this.AutoRun)
            {
                AudioEntity.Instance.PauseBackGroundAudio(AudioEntity.audioSpin, 6f,
                    ReduceVolume(AudioEntity.audioSpin));
            }
            else
            {
                AudioEntity.Instance.PauseBackGroundAudio(AudioEntity.audioSpin, 6f,
                    ReduceVolume(AudioEntity.audioSpin));
            }
        }

        public float ReduceVolume(string name)
        {
            if (AudioSlotsConfig.Instance == null) return 0.3f;
            return AudioSlotsConfig.Instance.ReduceVolume(name);
        }
        
        //设置背景音乐的音量,在 time 内音量 设置为 value
        public virtual void SetBackGroundAudio(float time= 0,float value = 0.8f)
        {
            if (this.isFreespinBonus)
            {
                AudioEntity.Instance.SetBackGroundAudioVolume(AudioEntity.audioSpecial, time,value);
            }
            else if (this.AutoRun)
            {
                AudioEntity.Instance.SetBackGroundAudioVolume(AudioEntity.audioSpin, time,value);
            }
            else
            {
                AudioEntity.Instance.SetBackGroundAudioVolume(AudioEntity.audioSpin, time,value);
            }
        }
        
        public virtual void StopRun() //the function is only excuting once
        {
            boardController.DoReelsStop();
        }


        public virtual void ChangeCurrentFirstRunningReelIndex(int stopReelIndex)
        {
        }

        public virtual bool ShouldStopAtOnceIgnoreAnticipation()
        {
            return gameConfigs.StopAtOnceIgnoreAnticipation ? fastStop : false;
        }

        public virtual void PlayShineAnimation()
        {
        }

        public virtual void PlayBaseWinAnimation()
        {
        }

        public virtual void StopShineAnimation()
        {
        }

        public virtual void StopBaseWinAnimation()
        {
        }

        public virtual void PlaySmallWinAnimation()
        {
            if (gameConfigs.ShineAnimator != null)
            {
                gameConfigs.ShineAnimator.SetInteger("state", 1);
            }

            if (reelWinCoinAnimations != null && reelWinCoinAnimations.Count > 0)
            {
                for (int i = 0; i < gameConfigs.GetReelNumber() && i < reelWinCoinAnimations.Count; i++)
                {
                    bool isReelAward = resultContent.awardResult.IsReelAwardForDisplay(i);
                    if (isReelAward)
                    {
                        reelWinCoinAnimations[i].SetActive(true);
                    }
                }
            }
        }

        public virtual void StopSmallWinAnimation()
        {
            if (gameConfigs.ShineAnimator != null)
            {
                gameConfigs.ShineAnimator.SetInteger("state", 0);
            }

            if (reelWinCoinAnimations != null && reelWinCoinAnimations.Count > 0)
            {
                for (int i = 0; i < reelWinCoinAnimations.Count; i++)
                {
                    reelWinCoinAnimations[i].SetActive(false);
                }
            }
        }


        public virtual void PlayMiddleWinAnimation()
        {
            if (gameConfigs.ShineAnimator != null)
            {
                gameConfigs.ShineAnimator.SetInteger("state", 1);
            }

            //if (!BaseSlotMachineController.Instance.isSmallWin && !BaseSlotMachineController.Instance.isBigWin)
            //if (!BaseSlotMachineController.Instance.isBigWin)
            //{
            if (MiddleWinAnimations != null && MiddleWinAnimations.Count > 0)
            {
                for (int i = 0; i < gameConfigs.GetReelNumber() && i < MiddleWinAnimations.Count; i++)
                {
                    bool isReelAward = resultContent.awardResult.IsReelAwardForDisplay(i);
                    if (isReelAward)
                    {
                        MiddleWinAnimations[i].SetActive(true);
                    }
                }
            }

            //}
        }

        public virtual void StopMiddleWinAnimation()
        {
            if (gameConfigs.ShineAnimator != null)
            {
                gameConfigs.ShineAnimator.SetInteger("state", 0);
            }

            if (MiddleWinAnimations != null && MiddleWinAnimations.Count > 0)
            {
                for (int i = 0; i < MiddleWinAnimations.Count; i++)
                {
                    MiddleWinAnimations[i].SetActive(false);
                }
            }
        }

        public virtual void PlayBigAnimation()
        {
            //解决移植机器播放边框动画
            Messenger.Broadcast<bool>(SlotControllerConstants.PLAY_OUTER_BORDER_BIG_WIN_ANIMATION, true);
        }

        protected virtual void StopBigAnimation()
        {
            Messenger.Broadcast<bool>(SlotControllerConstants.PLAY_OUTER_BORDER_BIG_WIN_ANIMATION, false);
        }

        public virtual void CheckFeature()
        {
            CheckHitFS();
            CheckHitBonus();
            // CheckFreespinAndAnticipationAnimation();
            // CheckBonusGameAnticipationAnimation();
            CheckCircleAnticipationAnimation();
            CheckHitLink();
            CheckHitActBonus();
        }

        public virtual void CheckHitActBonus()
        {
            HitActBonus = false;
        }
        
        public virtual int GetFreeSpinTimes(int symbolCount)
        {
            if (symbolCount < gameConfigs.FreeSpinNeedNum)
            {
                return 0;
            }

            return gameConfigs.FreeSpinTimes;
        }

        public virtual void CheckHitFS()
        {
            this.FreespinCount = 0;
            HitFs = false;
            if (this.gameConfigs.HasFreespinGame)
            {
                Dictionary<int, List<int>> freeSpinSymbolIndex = GetSpecialSymbolIndex(SymbolMap.IS_FREESPIN);
                this.FreespinCount = GetFreeSpinTimes(Utilities.GetCountInDictList(freeSpinSymbolIndex));
                HitFs = this.FreespinCount > 0;
            }
        }

        public virtual void CheckHitBonus()
        {
            if (this.gameConfigs.BonusNeedToSatisfyLineTable)
            {
                CheckBonusGameNumByPayLine();
            }
            else
            {
                CheckBonusGameNumNoPayLine();
            }
            if (HasBonusGame)
            {
                AddBonusTriggerData();
            }
        }

        public virtual void CheckBonusGameNumNoPayLine()
        {
            this.HasBonusGame = false;
            if (this.gameConfigs.HasBonusGame)
            {
                Dictionary<int, List<int>> bonusSymbolIndex = GetSpecialSymbolIndex(SymbolMap.IS_BONUS);
                this.BonusNum = Utilities.GetCountInDictList(bonusSymbolIndex);
                this.HasBonusGame = (this.BonusNum >= gameConfigs.BonusGameNeedNum);
            }
        }
        
        public virtual void CheckBonusGameNumByPayLine()
        {
            this.HasBonusGame = false;
            if (this.gameConfigs.HasBonusGame)
            {
                for (int k = 0; k < lineTable.TotalPayLineCount(); k++)
                {
                    PayLine payLine = lineTable.GetPayLineAtIndex(k);
                    this.BonusNum = 0;
                    int lastSymbolPos = 0;
                    for (int i = 0; i < payLine.GetSize(); i++)
                    {
                        int symbolIndex = resultContent.ReelResults[i].SymbolResults[payLine.RowNumberAt(i)];
                        SymbolMap.SymbolElementInfo info = symbolMap.getSymbolInfo(symbolIndex);
                        if (info.isBonus)
                        {
                            if (this.gameConfigs.NeedBonusSymbolContinue)
                            {
                                if (i == lastSymbolPos) lastSymbolPos++;
                                else break;
                            }

                            this.BonusNum++;
                        }
                    }

                    if (this.BonusNum >= gameConfigs.BonusGameNeedNum)
                    {
                        this.HasBonusGame = true;
                        break;
                    }
                }
            }
        }
        
        private Dictionary<string,int> _bonusWheelDic=new Dictionary<string, int>();
        /// <summary>
        /// 添加bonusTrigger,默认设置为 wheel 
        /// </summary>
        public virtual void AddBonusTriggerData(TriggerFeatureType triggerSource=TriggerFeatureType.Symbol,TriggerFeatureType featureType=TriggerFeatureType.OtherBonus,Dictionary<string,int> dic=null)
        {
            SpinResultProduce.AddProvider(new SpinResultBonusTrigger(triggerSource,featureType,dic==null?_bonusWheelDic:dic));
        }
        
        private void AddBonusDic(string symbolName)
        {
            if (_bonusWheelDic.ContainsKey(symbolName))
            {
                _bonusWheelDic[symbolName] += 1;
            }
            else
            {
                _bonusWheelDic[symbolName] = 1;
            }
        }

        public virtual void CheckCircleAnticipationAnimation()
        {
            this.HasCircleGame = false;
            if (this.gameConfigs.HasCircleSymbol)
            {
                Dictionary<int, List<int>> circleSymbolIndex = GetSpecialSymbolIndex(SymbolMap.IS_CIRCLE);
                this.HasCircleGame = (Utilities.GetCountInDictList(circleSymbolIndex) >= gameConfigs.WinCirCleNum);
            }
        }

        public virtual void CheckHitLink()
        {
            this.HitLinkGame = false;

            if (this.LinkNeedNum > 0)
            {
                bool onceMore = false;
                if (BaseSlotMachineController.Instance != null)
                {
                    onceMore = BaseSlotMachineController.Instance.onceMore;
                }
                else
                {
                    onceMore = TestController.Instance.isOneMore;
                }

                if (onceMore)
                {
                    HitLinkGame = false;
                }
                else
                {
                    Dictionary<int, List<int>> linkSymbolIndex = GetSpecialSymbolIndex(SymbolMap.IS_RESPIN);
                    this.HitLinkGame = (Utilities.GetCountInDictList(linkSymbolIndex) >= LinkNeedNum);
                }
            }
        }

        //用于检测是否使用Pattern
        bool isUseForcePattern = false;

        [Obsolete]
        public bool GetUsePattern()
        {
            return isUseForcePattern;
        }

        private bool HasUseRuleFeature { set; get; } = false;
        //备份Pattern 用于判断轮子是不是由于本地产生的结果锁住！
//        bool isUsePatternBackUp = false;

        OutCome currentServerOutCome = null;

        //用于控制NoWin请求，如果受到无效的消息，则关闭请求
        [HideInInspector] public bool NoWinPatternSwitch = true;
        //pattern的使用判断无需reel锁定的判断
        //用于判断目前机器是否处于锁住状态 并且是因为本地生成结果所导致的
//        public bool ReelLockedByNormal = false;
        //轮子是否锁住装盘  用来判断调用AddOutCome的哪个接口
//        public bool ReelLocked = false;

        //设置reelManager的锁住状态
//        public void  SetNatureLockReelFlag(List<int> excludeResultReelIndexes)
//        {
//            bool isReelLocked = false;
//
//            if(excludeResultReelIndexes!=null && excludeResultReelIndexes.Count>0)
//            {
//                isReelLocked = true;
//            }
//
////            ReelLockedByNormal = (isReelLocked) && (!isUsePatternBackUp);
//        }

//        [Obsolete("请勿扩展或继承,用ParseSpinResult替代")]
        public virtual void CreateRawResult()
        {
            currentServerOutCome = null;
            isUseForcePattern = false;
            //去掉了NeedReCreatResult，原则上都需要create

            //20200120 限制只有付费的情况才用pattern，将来再考虑其他
            //首先检查服务器Pattern结果是否存在 如果使用了Force或者bbPattern 则不再使用NoWinPattern
            //首先更新目前正在使用的服务器OutCome
            if (this.IsCostBetSpin())
            {
                //重置是否使用标识变量及OutCome
                SequencePaidResultsManager.ChangeOutComeFromSequencePaidResults();
                List<List<int>> patternSymbolResult = SequencePaidResultsManager.SymbolResultsFromForceOutcome();
                if (this.ValidResultInfoFromServer(patternSymbolResult))
                {
                    this.resultContent.ChangePaidResult(patternSymbolResult);
                    isUseForcePattern = true;
                    currentServerOutCome = SequencePaidResultsManager.currentForceOutCome;
#if UNITY_EDITOR
                    Log.LogLimeColor(this.resultContent.DebugReelContent(this.resultContent,
                        BaseSlotMachineController.Instance.reelManager.symbolMap, 0));
#endif
                }

                //如果启用了NoWin和并且不是在测试场景
                if (!isUseForcePattern && this.NoWinPatternSwitch && !BaseGameConsole.singletonInstance.isForTestScene)
                {
                    //计算此次结果的中奖金币数
                    float awardTempValue = AwardResult.CreateAwardDataTemp(this,
                        BaseSlotMachineController.Instance.slotMachineConfig.wildAccumulation);
                    List<List<int>> noWinResultFromServer = null;
                    //当用户此次不中奖而且也不触发Respin等Feature 调用NO_WIN PATTERN(切记：对于GoldMine这类使用slot作为bonus的机器，记得将Bonus使用的ReelManager的WhetherRespin方法重写使其返回true)
                    if (awardTempValue < 0.1f)
                    {
                        //当slotConfig进行了NoWinRequestPossibility配置的时候
                        if (this.slotConfig.NoWinRequestPossibility > 0)
                        {
                            //判断是否需要进行NoWinPattern的请求 调用该方法会改变currentNoWinOutCome 如果需要进行请求 则返回true
                            if (!SequencePaidResultsManager.NeedRequestNoWinPattern())
                            {
                                //若随机到了 则可从服务器获取NoWinPattern信息
                                if (UnityEngine.Random.Range(0, 1000) < this.slotConfig.NoWinRequestPossibility)
                                {
                                    noWinResultFromServer = SequencePaidResultsManager.SymbolResultsFromNoWinOutcome();
                                    if (this.ValidResultInfoFromServer(noWinResultFromServer))
                                    {
                                        this.resultContent.ChangePaidResult(noWinResultFromServer);
                                        isUseForcePattern = true;
                                        currentServerOutCome = SequencePaidResultsManager.currentNoWinOutCome;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            //没有用force pattern
            if (isUseForcePattern == false)
            {
                if (this.reelStrips.selectedResult.selectedFixedResult != null)
                {
                    //Plist配置设定改变结果值
                    this.resultContent.ChangePaidResult(this.reelStrips.selectedResult.selectedFixedResult);
#if UNITY_EDITOR
                    Log.Trace(this.resultContent.DebugReelContent(this.reelStrips.selectedResult.selectedFixedResult,
                        this.symbolMap, 3));
#endif
                }
                else
                {
                    if (this.SpinData == null)
                    {
                        this.SpinData = CreateSpinResult();
                    }

                    //普通的数据处理，需要考虑rule，付费的情况才考虑使用。并且plist开启
                    if (this.slotConfig.RuleSwitch.IsOpenRule && this.IsCostBetSpin() && 
                        this.SpinData.NeedRuleCheck(UserManager.GetInstance().UserProfile().Balance()))
                    {
                        ConfigRuleAction configRule =  RulePatternManager.GetInstance().GetConfigRuleAction();
                        if (configRule != null)
                        {
                            //做config的替换操作，判断是否在使用中且使用的是对应的snippet name，是的话跳过；
                            //否则：正在使用中的snippet 是别的则切换成正常的config且 如果有设置server数据；
                            //队列里有snippet name的config却没有使用的话才使用一次；没有snippet name则需添加标记；
                            //特例： 需要判断是否已经正在使用configrule，没使用的时候需要清除config，放在clearRule方法里处理
                            
                            BaseSlotMachineController.Instance.RuleConfig.CreateSpinRet(configRule.Snippet);
                            CreateRealAwardResult();
                        }
                        else
                        {
                            if(this.slotConfig.RuleSwitch.IsOpenFeature) //开启功能，标示开启用了Feature的功能，就不用再check了
                            {
                                HasUseRuleFeature = true;
                            }
                            this.SpinData.RuleCreateResult(this.slotConfig.RuleSwitch);
                        }
                    }
                    else
                    {
                        CreateRealAwardResult();
//                        this.resultContent.CreateRawResult();
                    }
                }
            }

            SimulationDebugResult();
        }

        
        private void SimulationDebugResult()
        {
            // #if DEBUG
                if (TestController.Instance != null) return;
                //场景模拟测试
                if (this.resultChangeContronller != null)
                {
                    this.resultChangeContronller.ChangeResult(this);
                }
                #if DEBUG
                    //只在baseGame中使用
                    if (BaseGameConsole.singletonInstance.isForTestScene)
                        return;
                    TestBoardDataPanel testBoardDataPanel = BaseSlotMachineController.Instance.TestBoardData;
                    if (testBoardDataPanel != null)
                    {
                        if (testBoardDataPanel.IsEnable)
                        {
                            resultContent.ChangeResult( testBoardDataPanel.SymbolResult);
                            testBoardDataPanel.IsEnable =false;
                        }
                    }
                #endif
                
            // #endif
        }

        /// <summary>
        /// 判断从服务器接收到的结果是否有效 主要看轮子数是否相等(列数目前不做判断) 符号有效是在OutCome类中的SymbolResult进行判断
        /// </summary>
        public bool ValidResultInfoFromServer(List<List<int>> resultFromServer)
        {
            //如果等于null 返回false
            if (resultFromServer == null)
            {
                return false;
            }

            //如果轮子数不对 或者轮子数<1
            if (resultFromServer.Count != resultContent.ReelResults.Count || resultFromServer.Count < 1)
            {
                return false;
            }

            return true;
        }

        public string ConvertResultToString()
        {
            List<List<string>> showInfo = new List<List<string>>();
            List<List<int>> list = new List<List<int>>();
            for (int i = 0; i < resultContent.ReelResults.Count; i++)
            {
                list.Add(resultContent.ReelResults[i].SymbolResults);
            }

            for (int i = 0; i < list.Count; i++)
            {
                List<string> ls = new List<string>();
                for (int j = 0; j < list[i].Count; j++)
                {
                    SymbolMap.SymbolElementInfo info = symbolMap.getSymbolInfo(list[i][j]);
                    ls.Add(info.name);
                }

                showInfo.Add(ls);
            }

            string textContent = "";
            for (int i = 0; i < showInfo.Count; i++)
            {
                for (int j = 0; j < showInfo[i].Count; j++)
                {
                    textContent += showInfo[i][j] + "\t";
                }

                textContent += "\n";
            }

            return textContent;
        }

        public bool CheckAnticipation()
        {
            if (this.IsNewSpeedPattern)
            {
                return this.NeedAntiReels.Count > 0;
            }

            return false;
        }
        
        public virtual void ChangeSymbols(List<List<int>> symbolIndex)
        {
            if (symbolIndex == null)
                return;
            for (int i = 0; i < symbolIndex.Count; i++)
            {
                if (i >= GetReelCount())
                {
                    continue;
                }

                for (int j = 0; j < symbolIndex[i].Count; j++)
                {
                    if (j >= GetReelSymbolRenderCount(i))
                    {
                        continue;
                    }

                    BaseElementPanel render = GetSymbolRender(i, j);
                    if (render != null)
                    {
                        render.ChangeSymbol(symbolIndex[i][j]);
                    }
                }
            }
        }

        public virtual void ChangeSymbol(int reelIndex, int elementIndex, int tgtSymbolIndex)
        {
            BaseElementPanel render = GetSymbolRender(reelIndex, elementIndex);
            if (render != null)
            {
                render.ChangeSymbol(tgtSymbolIndex);
            }
        }

        ///LQ 双向计算连续带子出现相同Symbol个数乘积的中奖奖励。34543机器用的最多
        protected virtual float CalculateProductPayTableAwardInBidirection(Dictionary<string, object> infos,
            bool onlyComputeLineValue = false)
        {
            float ret = 0;
            Dictionary<int, Dictionary<int, int>>
                checkForwardAwardSymbols = new Dictionary<int, Dictionary<int, int>>(); //LQ 从左到右
            Dictionary<int, Dictionary<int, int>>
                checkReverseAwardSymbols =
                    new Dictionary<int, Dictionary<int, int>>(); //LQ 从右到左 之所以另建字典，是为了方便计算，而且排除正向存在，反向亦存在同一Symbol的情况
            Dictionary<int, BasePaytableElement> normalPayTable = null;
            Dictionary<int, BasePaytableElement> freespinPayTable = null;
            //LQ 优化使用较多的引用调用
            int reelStripsCount = resultContent.ReelResults.Count; //最终结果带子数
            List<ResultContent.ReelResult> reelStrips = resultContent.ReelResults; //每个带子最终采用显示的Symbol数

            #region 从左到右查找中奖元素

            //获取可能中奖Symbol
            for (int i = 0; i < reelStripsCount; i++)
            {
                bool hasWild = false;
                for (int j = 0; j < reelStrips[i].SymbolResults.Count; j++)
                {
                    int symbolValue = reelStrips[i].SymbolResults[j];
                    SymbolMap.SymbolElementInfo info = symbolMap.getSymbolInfo(symbolValue);

                    if (info.isWild)
                    {
                        hasWild = true;
                    }
                    else
                    {
                        if (!checkForwardAwardSymbols.ContainsKey(symbolValue))
                        {
                            Dictionary<int, int> symbolCountInReel = new Dictionary<int, int>();
                            for (int m = 0; m < reelStripsCount; m++)
                            {
                                symbolCountInReel.Add(m, 0);
                            }

                            checkForwardAwardSymbols.Add(symbolValue, symbolCountInReel);
                        }
                    }
                }

                if (!hasWild)
                {
                    break;
                }
            }

            //计数可能中奖Symbol每列的数量
            for (int i = 0; i < reelStripsCount; i++)
            {
                bool interruptAtOnce = true; //LQ 因为中奖要求相邻带子必须包含中奖Symbol，所以如果一带子循环下来，没有发现中奖元素，则终止循环累加，以方便反向从右到左中奖判定
                for (int j = 0; j < reelStrips[i].SymbolResults.Count; j++)
                {
                    int symbolValue = reelStrips[i].SymbolResults[j];
                    SymbolMap.SymbolElementInfo info = symbolMap.getSymbolInfo(symbolValue);
                    if (info.isWild)
                    {
                        foreach (int key in checkForwardAwardSymbols.Keys)
                        {
                            checkForwardAwardSymbols[key][i] += info.Mutipler; //LQ 指定Symbol指定轮子的数量累加
                        }

                        interruptAtOnce = false;
                    }
                    else
                    {
                        if (checkForwardAwardSymbols.ContainsKey(symbolValue))
                        {
                            checkForwardAwardSymbols[symbolValue][i] += info.Mutipler; //LQ 指定Symbol指定轮子的数量累加
                            interruptAtOnce = false;
                        }
                    }
                }

                if (interruptAtOnce)
                {
                    break;
                }
            }

            #endregion

            #region 正向数据处理不连续计数的symbol数量，此数量为无效数据

            foreach (int key in checkForwardAwardSymbols.Keys)
            {
                bool clearNextElements = false;
                for (int j = 0; j < checkForwardAwardSymbols[key].Count; j++)
                {
                    if (checkForwardAwardSymbols[key][j] == 0)
                    {
                        clearNextElements = true; // LQ 将此轮后面的所有计数元素值清空
                    }

                    if (clearNextElements)
                    {
                        checkForwardAwardSymbols[key][j] = 0;
                    }
                }
            }

            #endregion

            #region 从右到左查找中奖元素

            ///获取可能中奖Symbol 不能排除已包含的情况，如1带子出现x，3，4，5出现x，但是你不能因为第一带子已存在该元素而剔除从右到左的可能性
            for (int i = reelStripsCount - 1; i > 0; i--)
            {
                //最终结果带子数
                bool hasWild = false;
                for (int j = 0; j < reelStrips[i].SymbolResults.Count; j++)
                {
                    //每个带子最终采用显示的Symbol数
                    int symbolValue = reelStrips[i].SymbolResults[j];
                    SymbolMap.SymbolElementInfo info = symbolMap.getSymbolInfo(symbolValue);
                    if (info.isWild)
                    {
                        hasWild = true;
                    }
                    else
                    {
                        if (checkForwardAwardSymbols.ContainsKey(symbolValue) &&
                            checkForwardAwardSymbols[symbolValue][i] > 0)
                        {
                            continue; //当前只要正向带子Symbol累加器>0,说明正向已经计算过，故需要跳过排除此Symbol
                        }

                        if (!checkReverseAwardSymbols.ContainsKey(symbolValue))
                        {
                            Dictionary<int, int> symbolCountInReel = new Dictionary<int, int>();
                            for (int m = 0; m < reelStripsCount; m++)
                            {
                                symbolCountInReel.Add(m, 0);
                            }

                            checkReverseAwardSymbols.Add(symbolValue, symbolCountInReel);
                        }
                    }
                }

                if (!hasWild)
                {
                    break;
                }
            }

            //计数可能中奖Symbol每列的数量
            for (int i = reelStripsCount - 1; i > 0; i--)
            {
                bool interruptAtOnce = true; //LQ 因为中奖要求连续且相邻带子必须包含相同中奖Symbol，所以如果一带子循环下来，没有发现中奖元素，则终止循环累加，不要影响正向遍历累加值
                for (int j = 0; j < reelStrips[i].SymbolResults.Count; j++)
                {
                    int symbolValue = reelStrips[i].SymbolResults[j];
                    SymbolMap.SymbolElementInfo info = symbolMap.getSymbolInfo(symbolValue);
                    if (info.isWild)
                    {
                        foreach (int key in checkReverseAwardSymbols.Keys)
                        {
                            checkReverseAwardSymbols[key][i] += info.Mutipler; //LQ 指定Symbol指定轮子的数量累加
                        }

                        interruptAtOnce = false;
                    }
                    else
                    {
                        if (checkReverseAwardSymbols.ContainsKey(symbolValue))
                        {
                            checkReverseAwardSymbols[symbolValue][i] += info.Mutipler; //LQ 指定Symbol指定轮子的数量累加
                            interruptAtOnce = false;
                        }
                    }
                }

                if (interruptAtOnce)
                {
                    break;
                }
            }

            #endregion

            #region 根据游戏类型初始化payTable

            //初始化payTable 如果Plist中有数据则用plist数据，没有的话初始化一下相应的paytable，但是不会中奖。因为里面没有对应元素中奖值
            Dictionary<int, BasePaytableElement> payTable;
            if (isFreespinBonus)
            {
                if (freespinPayTable == null)
                {
                    freespinPayTable = new Dictionary<int, BasePaytableElement>();
                    if (this.payTable.singleSymbolPaysCon.Count > 0)
                    {
                        foreach (int key in this.payTable.singleSymbolPaysCon.Keys)
                        {
                            if (!this.payTable.singleSymbolPaysCon[key].isScatter)
                            {
                                freespinPayTable.Add(key,
                                    this.payTable.singleSymbolPaysCon[key] as BasePaytableElement);
                            }
                        }
                    }

                    if (this.payTable.singleSymbolPaysNoCon.Count > 0)
                    {
                        foreach (int key in this.payTable.singleSymbolPaysNoCon.Keys)
                        {
                            if (!this.payTable.singleSymbolPaysNoCon[key].isScatter)
                            {
                                freespinPayTable.Add(key,
                                    this.payTable.singleSymbolPaysNoCon[key] as BasePaytableElement);
                            }
                        }
                    }
                }

                payTable = freespinPayTable;
            }
            else
            {
                if (normalPayTable == null)
                {
                    if (this.payTable != null)
                    {
                        normalPayTable = new Dictionary<int, BasePaytableElement>();
                        if (this.payTable.singleSymbolPaysCon.Count > 0)
                        {
                            foreach (int key in this.payTable.singleSymbolPaysCon.Keys)
                            {
                                if (!this.payTable.singleSymbolPaysCon[key].isScatter)
                                {
                                    normalPayTable.Add(key,
                                        this.payTable.singleSymbolPaysCon[key] as BasePaytableElement);
                                }
                            }
                        }

                        if (this.payTable.singleSymbolPaysNoCon.Count > 0)
                        {
                            foreach (int key in this.payTable.singleSymbolPaysNoCon.Keys)
                            {
                                if (!this.payTable.singleSymbolPaysNoCon[key].isScatter)
                                {
                                    normalPayTable.Add(key,
                                        this.payTable.singleSymbolPaysNoCon[key] as BasePaytableElement);
                                }
                            }
                        }
                    }
                    else
                    {
                        normalPayTable = new Dictionary<int, BasePaytableElement>();
                    }
                }

                payTable = normalPayTable;
            }

            #endregion

            #region 计算正向奖励

            //计算正向奖励
            foreach (int key in checkForwardAwardSymbols.Keys)
            {
                int multipler = checkForwardAwardSymbols[key][0];
                int lastMultiper = multipler;
                int awardIndex = 1;
                for (int i = 1; i < checkForwardAwardSymbols[key].Count; i++)
                {
                    multipler *= checkForwardAwardSymbols[key][i];
                    if (multipler == 0)
                    {
                        break;
                    }
                    else
                    {
                        awardIndex = i + 1;
                        lastMultiper = multipler;
                    }
                }

                if (payTable.ContainsKey(key) && payTable[key].awardMap.ContainsKey(awardIndex))
                {
                    float rewardValue = payTable[key].awardMap[awardIndex] * lastMultiper;
                    ret += rewardValue;
                    if (onlyComputeLineValue)
                        continue;
//                    List<BaseElementPanel> awardElement = new List<BaseElementPanel>();
                    List<AwardResult.SymbolRenderIndex> awardPos = new List<AwardResult.SymbolRenderIndex>();  
                    List<int> LineResultIndexs = new List<int>();
                    for (int i = 0; i < awardIndex; i++)
                    {
                        int renderNum = this.boardController.GetReelRenderCount(i);
                        for (int j = 0; j < renderNum; j++)
                        {
                            int symbolIndex = resultContent.ReelResults[i].SymbolResults[j];
                            SymbolMap.SymbolElementInfo info = symbolMap.getSymbolInfo(symbolIndex);
                            if (info.isWild || symbolIndex == key)
                            {
                                awardPos.Add(new AwardResult.SymbolRenderIndex(i,j));
                            }
                        }
                    }
                    for (int i = 0; i < awardIndex; i++)
                    {
                        int renderNum = this.boardController.GetReelRenderCount(i);
                        for (int j = 0; j < renderNum; j++)
                        {
                            int symbolIndex = resultContent.ReelResults[i].SymbolResults[j];
                            SymbolMap.SymbolElementInfo info = symbolMap.getSymbolInfo(symbolIndex);
                            if (info.isWild || symbolIndex == key)
                            {
                                awardPos.Add(new AwardResult.SymbolRenderIndex(i,j));
                                if ((symbolIndex == key || info.isWild) && LineResultIndexs.Count == i &&
                                    awardIndex >= 5)
                                {
                                    LineResultIndexs.Add(symbolIndex);
                                }
                            }
                        }
                    }

                    resultContent.awardResult.AddAwardPayLine(rewardValue, payTable[key].awardNames[awardIndex],
                        awardPos, gameConfigs, 1000,true,null,LineResultIndexs);
                }
            }

            #endregion

            #region 计算反向奖励

            //计算反向奖励
            foreach (int key in checkReverseAwardSymbols.Keys)
            {
                int multipler = checkReverseAwardSymbols[key][reelStripsCount - 1];
                int lastMultiper = multipler;
                int awardIndex = 1;
                for (int i = reelStripsCount - 2; i > 0; i--)
                {
                    multipler *= checkReverseAwardSymbols[key][i];
                    if (multipler == 0)
                    {
                        break;
                    }
                    else
                    {
                        awardIndex = reelStripsCount - i;
                        lastMultiper = multipler;
                    }
                }

                if (payTable.ContainsKey(key) && payTable[key].awardMap.ContainsKey(awardIndex))
                {
                    float rewardValue = payTable[key].awardMap[awardIndex] * lastMultiper;
                    ret += rewardValue;
                    if (onlyComputeLineValue)
                        continue;
                    
                    List<AwardResult.SymbolRenderIndex> awardPos = new List<AwardResult.SymbolRenderIndex>();  
                    for (int i = GetReelCount() - 1; i >= GetReelCount() - awardIndex; i--)
                    {
                        int renderNum = this.boardController.GetReelRenderCount(i);
                        for (int j = 0; j < renderNum; j++)
                        {
                            int symbolIndex = resultContent.ReelResults[i].SymbolResults[j];
                            SymbolMap.SymbolElementInfo info = symbolMap.getSymbolInfo(symbolIndex);
                            if (info.isWild || symbolIndex == key)
                            {
                                awardPos.Add(new AwardResult.SymbolRenderIndex(i,j));
                            }
                        }
                    }

                    resultContent.awardResult.AddAwardPayLine(rewardValue, payTable[key].awardNames[awardIndex],
                        awardPos, gameConfigs, 1000);
                }
            }

            #endregion

            BaseSlotMachineController.Instance.baseAward += ret;
            return ret / gameConfigs.ScalFactor;
        }

        ///LQ 单向计算连续带子出现相同Symbol个数乘积的中奖奖励
        /// onlyComputeLineValue 返回计算线奖的rtp
        protected virtual float CalculateProductPayTableAwardInUnidirectional(Dictionary<string, object> infos,
            int totalmultiplier = 1, bool onlyComputeLineValue = false)
        {
            float ret = 0;
            Dictionary<int, Dictionary<int, int>>
                checkForwardAwardSymbols = new Dictionary<int, Dictionary<int, int>>(); //LQ 从左到右
            Dictionary<int, BasePaytableElement> normalPayTable = null;
            Dictionary<int, BasePaytableElement> freespinPayTable = null;
            //LQ 优化使用较多的引用调用
            int reelStripsCount = resultContent.ReelResults.Count; //最终结果带子数
            List<ResultContent.ReelResult> reelStrips = resultContent.ReelResults; //每个带子最终采用显示的Symbol数

            #region 从左到右查找中奖元素

            //获取可能中奖Symbol
            for (int i = 0; i < reelStripsCount; i++)
            {
                bool hasWild = false;
                for (int j = 0; j < reelStrips[i].SymbolResults.Count; j++)
                {
                    int symbolValue = reelStrips[i].SymbolResults[j];
                    SymbolMap.SymbolElementInfo info = symbolMap.getSymbolInfo(symbolValue);

                    if (info.isWild)
                    {
                        hasWild = true;
                    }
                    else
                    {
                        if (!checkForwardAwardSymbols.ContainsKey(symbolValue))
                        {
                            Dictionary<int, int> symbolCountInReel = new Dictionary<int, int>();
                            for (int m = 0; m < reelStripsCount; m++)
                            {
                                symbolCountInReel.Add(m, 0);
                            }

                            checkForwardAwardSymbols.Add(symbolValue, symbolCountInReel);
                        }
                    }
                }

                if (!hasWild)
                {
                    break;
                }
            }

            //计数可能中奖Symbol每列的数量
            for (int i = 0; i < reelStripsCount; i++)
            {
                bool interruptAtOnce = true; //LQ 因为中奖要求相邻带子必须包含中奖Symbol，所以如果一带子循环下来，没有发现中奖元素，则终止循环累加，以方便反向从右到左中奖判定
                for (int j = 0; j < reelStrips[i].SymbolResults.Count; j++)
                {
                    int symbolValue = reelStrips[i].SymbolResults[j];
                    SymbolMap.SymbolElementInfo info = symbolMap?.getSymbolInfo(symbolValue);
                    if(info==null) continue;
                    if (info.isWild)
                    {
                        foreach (int key in checkForwardAwardSymbols.Keys)
                        {
                            checkForwardAwardSymbols[key][i] += info.Mutipler; //LQ 指定Symbol指定轮子的数量累加
                        }

                        interruptAtOnce = false;
                    }
                    else
                    {
                        if (checkForwardAwardSymbols.ContainsKey(symbolValue))
                        {
                            checkForwardAwardSymbols[symbolValue][i] += info.Mutipler; //LQ 指定Symbol指定轮子的数量累加
                            interruptAtOnce = false;
                        }
                    }
                }

                if (interruptAtOnce)
                {
                    break;
                }
            }

            #endregion

            #region 正向数据处理不连续计数的symbol数量，此数量为无效数据

            foreach (int key in checkForwardAwardSymbols.Keys)
            {
                bool clearNextElements = false;
                for (int j = 0; j < checkForwardAwardSymbols[key].Count; j++)
                {
                    if (checkForwardAwardSymbols[key][j] == 0)
                    {
                        clearNextElements = true; // LQ 将此轮后面的所有计数元素值清空
                    }

                    if (clearNextElements)
                    {
                        checkForwardAwardSymbols[key][j] = 0;
                    }
                }
            }

            #endregion

            #region 根据游戏类型初始化payTable

            //初始化payTable 如果Plist中有数据则用plist数据，没有的话初始化一下相应的paytable，但是不会中奖。因为里面没有对应元素中奖值
            Dictionary<int, BasePaytableElement> payTable;
            if (isFreespinBonus)
            {
                if (freespinPayTable == null)
                {
                    freespinPayTable = new Dictionary<int, BasePaytableElement>();
                    if (this.payTable.singleSymbolPaysCon.Count > 0)
                    {
                        foreach (int key in this.payTable.singleSymbolPaysCon.Keys)
                        {
                            if (!this.payTable.singleSymbolPaysCon[key].isScatter)
                            {
                                freespinPayTable.Add(key,
                                    this.payTable.singleSymbolPaysCon[key] as BasePaytableElement);
                            }
                        }
                    }

                    if (this.payTable.singleSymbolPaysNoCon.Count > 0)
                    {
                        foreach (int key in this.payTable.singleSymbolPaysNoCon.Keys)
                        {
                            if (!this.payTable.singleSymbolPaysNoCon[key].isScatter)
                            {
                                freespinPayTable.Add(key,
                                    this.payTable.singleSymbolPaysNoCon[key] as BasePaytableElement);
                            }
                        }
                    }
                }

                payTable = freespinPayTable;
            }
            else
            {
                if (normalPayTable == null)
                {
                    if (this.payTable != null)
                    {
                        normalPayTable = new Dictionary<int, BasePaytableElement>();
                        if (this.payTable.singleSymbolPaysCon.Count > 0)
                        {
                            foreach (int key in this.payTable.singleSymbolPaysCon.Keys)
                            {
                                if (!this.payTable.singleSymbolPaysCon[key].isScatter)
                                {
                                    normalPayTable.Add(key,
                                        this.payTable.singleSymbolPaysCon[key] as BasePaytableElement);
                                }
                            }
                        }

                        if (this.payTable.singleSymbolPaysNoCon.Count > 0)
                        {
                            foreach (int key in this.payTable.singleSymbolPaysNoCon.Keys)
                            {
                                if (!this.payTable.singleSymbolPaysNoCon[key].isScatter)
                                {
                                    normalPayTable.Add(key,
                                        this.payTable.singleSymbolPaysNoCon[key] as BasePaytableElement);
                                }
                            }
                        }
                    }
                    else
                    {
                        normalPayTable = new Dictionary<int, BasePaytableElement>();
                    }
                }

                payTable = normalPayTable;
            }

            #endregion

            #region 计算正向奖励

            //计算正向奖励
            foreach (int key in checkForwardAwardSymbols.Keys)
            {
                int multipler = checkForwardAwardSymbols[key][0];
                int lastMultiper = multipler;
                int awardIndex = 1;
                for (int i = 1; i < checkForwardAwardSymbols[key].Count; i++)
                {
                    multipler *= checkForwardAwardSymbols[key][i];
                    if (multipler == 0)
                    {
                        break;
                    }
                    else
                    {
                        awardIndex = i + 1;
                        lastMultiper = multipler;
                    }
                }

                if (payTable.ContainsKey(key) && payTable[key].awardMap.ContainsKey(awardIndex))
                {
                    float rewardValue = payTable[key].awardMap[awardIndex] * lastMultiper * totalmultiplier;
                    ret += rewardValue;
                    if (onlyComputeLineValue)
                    {
                        continue;
                    }
                    else
                    {
                        List<AwardResult.SymbolRenderIndex> awardPos = new List<AwardResult.SymbolRenderIndex>();  
                        List<int> LineResultIndexs = new List<int>();
                        if (!BaseGameConsole.singletonInstance.isForTestScene)
                        {
                            for (int i = 0; i < awardIndex; i++)
                            {
                                int renderNum = this.boardController.GetReelRenderCount(i);
                                for (int j = 0; j < renderNum; j++)
                                {
                                    int symbolIndex = resultContent.ReelResults[i].SymbolResults[j];
                                    SymbolMap.SymbolElementInfo info = symbolMap.getSymbolInfo(symbolIndex);
                                    if (info.isWild || symbolIndex == key)
                                    {
                                        awardPos.Add(new AwardResult.SymbolRenderIndex(i,j));
                                    }
                                }
                            }

                            for (int i = 0; i < awardIndex; i++)
                            {
                                int renderNum = this.boardController.GetReelRenderCount(i);
                                for (int j = 0; j < renderNum; j++)
                                {
                                    int symbolIndex = resultContent.ReelResults[i].SymbolResults[j];
                                    SymbolMap.SymbolElementInfo info = symbolMap.getSymbolInfo(symbolIndex);
                                    if (info.isWild || symbolIndex == key)
                                    {
                                        awardPos.Add(new AwardResult.SymbolRenderIndex(i,j));
                                        if ((symbolIndex == key || info.isWild) && LineResultIndexs.Count == i &&
                                            awardIndex >= 5)
                                        {
                                            LineResultIndexs.Add(symbolIndex);
                                        }
                                    }
                                }
                            }
                        }

                        if (awardIndex != LineResultIndexs.Count) LineResultIndexs.Clear();

                        resultContent.awardResult.AddAwardPayLine(rewardValue, payTable[key].awardNames[awardIndex],
                            awardPos, gameConfigs, 1000 + key, true, null, LineResultIndexs);
                        BaseSlotMachineController.Instance.baseAward += (rewardValue*BaseSlotMachineController.Instance.currentBettingTemp / gameConfigs.ScalFactor);
                        // Debug.LogAssertion("全线 : baseAward : "+BaseSlotMachineController.Instance.baseAward);
                    }
                }
            }

            #endregion

            return ret / gameConfigs.ScalFactor;
        }

        //TODO: modify say tow
        public virtual List<BaseElementPanel> GetFreespinSymbols()
        {
            if(!this.IsNewProcess)
            {
                //无需再去判断fs的hit
                CheckHitFS();
            }
            if (FreespinCount > 0)
            {
                return GetElementsWithSymbolTag(SymbolMap.IS_FREESPIN);
            }
            else
            {
                return new List<BaseElementPanel>();
            }
        }

        public virtual List<BaseElementPanel> GetBonusGameSymbols()
        {
            // if(!this.IsNewProcess)
            // {
            //     CheckHitBonus();
            // }
            if (HasBonusGame)
            {
                return GetElementsWithSymbolTag(SymbolMap.IS_BONUS);
            }
            else
            {
                return new List<BaseElementPanel>();
            }
        }

        public virtual List<BaseElementPanel> GetCircleGameSymbols()
        {
            CheckCircleAnticipationAnimation();
            if (HasCircleGame)
            {
                return GetElementsWithSymbolTag(SymbolMap.IS_CIRCLE);
            }
            else
            {
                return new List<BaseElementPanel>();
            }
        }

        public List<BaseElementPanel> GetElementsWithSymbolTag(string symbolTag)
        {
            return GetSymbolByIndexDict(GetSpecialSymbolIndex(symbolTag));
        }

        //如果是单条线判断special则需重写此方法
        public virtual Dictionary<int, List<int>> GetSpecialSymbolIndex(string symbolTag)
        {
            if (resultContent == null)
            {
                return new Dictionary<int, List<int>>();
            }

            if (not_singleLine)
            {
                return resultContent.GetSpecialSymbolIndex(symbolTag, symbolMap);
            }
            else
            {
                return resultContent.GetMiddleLineSpecialSymbolIndex(symbolTag, symbolMap);
            }
        }

        public virtual Dictionary<int, List<int>> GetSpecialSymbolIndexIgnorePayLine(string symbolTag)
        {
            return resultContent.GetSpecialSymbolIndex(symbolTag, symbolMap);
        }

        public List<BaseElementPanel> GetElementsWithSymbolTagIgnorePayLine(string symbolTag)
        {
            return GetSymbolByIndexDict(GetSpecialSymbolIndexIgnorePayLine(symbolTag));
        }

        public virtual int GetSpecialCount(string symbolTag)
        {
            Dictionary<int, List<int>> specialSymbolIndexs = GetSpecialSymbolIndex(symbolTag);
            return Utils.Utilities.GetCountInDictList(specialSymbolIndexs);
        }

        public List<BaseElementPanel> GetElementsWithSymbolName(string symbolName)
        {
            return GetSymbolByIndexDict(GetSymbolIndexMap(symbolName));
        }

        public List<BaseElementPanel> GetElementsWithSymbolIndex(int symbolIndex)
        {
            return GetSymbolByIndexDict(GetSymbolIndexMapByScatter(symbolIndex));
        }

        public Dictionary<int, List<int>> GetSymbolIndexMap(string symbolName)
        {
            if (not_singleLine)
            {
                return resultContent.GetSymbolIndexMap(symbolName, symbolMap);
            }
            else
            {
                return resultContent.GetMiddleLineSymbolIndexMap(symbolName, symbolMap);
            }
        }

        public Dictionary<int, List<int>> GetSymbolIndexMap(int symbolIndex)
        {
            if (not_singleLine)
            {
                return resultContent.GetSymbolIndexMap(symbolIndex, symbolMap);
            }
            else
            {
                return resultContent.GetMiddleLineSymbolIndexMap(symbolIndex, symbolMap);
            }
        }

        public virtual Dictionary<int, List<int>> GetSymbolIndexMapByScatter(int symbolIndex,
            bool IsCheckExcludeResultReels = true)
        {
            if (not_singleLine)
            {
                return resultContent.GetSymbolIndexMap(symbolIndex, symbolMap);
            }
            else
            {
                return resultContent.GetMiddleLineSymbolIndexMap(symbolIndex, symbolMap);
            }
        }

        public List<BaseElementPanel> GetElementsInReelsBySymbolIndex(int symbolIndex,
            bool IsCheckExcludeResultReels = true)
        {
            return GetSymbolByIndexDict(resultContent.GetSymbolIndexMap(symbolIndex, symbolMap));
        }

        public List<BaseElementPanel> GetSymbolByIndexDict(Dictionary<int, List<int>> symbolIndex)
        {
            List<BaseElementPanel> symbols = new List<BaseElementPanel>();
            if (symbolIndex != null)
            {
                if (this.IsNewSpeedPattern)
                {
                    foreach (int key in symbolIndex.Keys)
                    {
                        if (symbolIndex[key] != null)
                        {
                            for (int i = 0; i < symbolIndex[key].Count; i++)
                            {
                                BaseElementPanel render = this.GetSymbolRender(key, symbolIndex[key][i]);
                                if (render != null)
                                {
                                    symbols.Add(render);
                                }
                            }
                        }
                    }
                }
            }

            return symbols;
        }

        #region  Animation

        protected bool hasPlayAllSymbolHighAwardAnimation = false;

        public void ClearAnimationInfos()
        {
            awardElements.Clear();
            awardLines.Clear();
            hasPlayAllSymbolHighAwardAnimation = false;
        }

        public virtual void AddAwardElements()
        {
            resultContent.awardResult.GetAwardAnimationElement(this);
        }

        public virtual void PlayAwardSymbolAnimation()
        {
            if (gameConfigs.ShowAllLinesAnimation)
            {
                StartCoroutine(PlayAllAwardSymbolAnimaiton());
            }
            else
            {
                StartCoroutine(PlaySymbolAnimationByLine(gameConfigs.TileAnimationDuration));
            }
        }

        public virtual void PlayAwardSymbolAudio(string name)
        {
            AudioEntity.Instance.PlayAwardSymbolAudio(name);
        }


        public virtual IEnumerator PlaySymbolAnimationByLine(float delayTime)
        {
            if (hasPlayAllSymbolHighAwardAnimation)
            {
                PauseAllSymbolAnimation();
            }

            float startTime = Time.realtimeSinceStartup;
            if (awardLines.Count <= 0)
            {
                yield break;
            }
            else
            {
                int i = 0;
                this.PauseAllSymbolAnimation();
                Messenger.Broadcast<ReelManager>(SlotControllerConstants.PAYLINE_ANIMATION_HIDE, this);
                awardLines[i].PlayAwardAnimation(not_singleLine, this, false);
                //PlayAwardLineAudio (awardLines[i]);
                awardLines[i].NeedReBuildPayLine = (gameConfigs.SingleLineReBuild || awardLines.Count > 1);
                int Next = (i + 1) % awardLines.Count;
                while (true)
                {
                    yield return m_AnimationWaitForScecond;
                    if (startTime + delayTime <= Time.realtimeSinceStartup)
                    {
                        awardLines[i].PauseAwardAnimation(not_singleLine, this);
                        yield return new WaitForEndOfFrame();
                        awardLines[Next].PlayAwardAnimation(not_singleLine, this);
                        //PlayAwardLineAudio (awardLines[Next]);
                        i = Next;
                        Next = (Next + 1) % awardLines.Count;
                        startTime = Time.realtimeSinceStartup;
                    }
                }
            }
        }


        public virtual void PlayAllSymbolHighAwardAnimation()
        {
            for (int i = 0; i < awardElements.Count; i++)
            {
                awardElements[i].PlayAnimation(2);
            }

            hasPlayAllSymbolHighAwardAnimation = true;
        }

        protected void PauseAllSymbolAnimation()
        {
            for (int i = 0; i < awardElements.Count; i++)
            {
                awardElements[i].PauseAnimation();
            }
        }

        public virtual IEnumerator PlayAllAwardSymbolAnimaiton()
        {
            if (hasPlayAllSymbolHighAwardAnimation)
            {
                PauseAllSymbolAnimation();
            }

            yield return new WaitForEndOfFrame();
            if (resultContent.awardResult.awardInfos.Count <= 0)
            {
                yield break;
            }
            else
            {
                for (int i = 0; i < resultContent.awardResult.awardInfos.Count; i++)
                {
                    awardLines[i].PauseAwardAnimation(false, this);
                    yield return new WaitForEndOfFrame();
                    awardLines[i].PlayAwardAnimation(false, this);
                }
            }
        }

        public virtual void PlayWholePaylineAnimation()
        {
            for (int i = awardLines.Count - 1; i >= 0; i--)
            {
                awardLines[i].PlayAwardAnimation(false, this, true);
            }
        }

        public virtual void DisableAllUnAwardSymbol()
        {
            for (int i = 0; i < GetReelCount(); i++)
            {
                //DisableUnAwardSymbol(i);
            }
        }

        public virtual void DisableUnAwardSymbol(int reelIndex)
        {
            for (int i = 0; i < GetReelSymbolRenderCount(reelIndex); i++)
            {
                BaseElementPanel element = GetSymbolRender(reelIndex, i);
                if (!awardElements.Contains(element))
                {
                    element.ChangeColor(0.588F, 0.588F, 0.588F);
                }
                else
                {
                    element.ChangeColor();
                }
            }
        }

        public virtual void StopAllAnimation()
        {
            StopAllCoroutines();
            WebmSymbolAssets.Instance.ClearKey();
            StopAllSmartAndAntiEffectAnimation();
            for (int i = 0; i < this.awardElements.Count; i++)
            {
                if (this.awardElements[i] != null)
                {
                    this.awardElements[i].StopAnimation();
                }
            }

            if (gameConfigs.EnableAnimationPayLine)
            {
                Messenger.Broadcast<ReelManager>(SlotControllerConstants.PAYLINE_ANIMATION_HIDE, this);
            }
        }

        public void StopAllSmartAndAntiEffectAnimation()
        {
            if (smartEffectController != null) smartEffectController.StopAllSmartAndAntiEffectAnimation(this);
        }

        #endregion

        #region Dialog

        public virtual void OpenRewinDialog()
        {
            DelayAction delayAction = new DelayAction(gameConfigs.TileAnimationDuration, null,
                () => { Messenger.Broadcast<System.Action>(GameDialogManager.OpenRewin, () => { StartRun(); }); });
            delayAction.Play();
        }

        // BaseSlotMachineController判定满足Blackout条件后调用
        // 通过重载，可以做除了弹窗以外的事情
        // onDialogClose：弹窗结束后的回调，不弹窗的话可以无视
        public virtual void OpenBlackoutDialog(System.Action onDialogClose)
        {
            Messenger.Broadcast<System.Action>(GameDialogManager.OpenBlackOutPanel, onDialogClose);
        }

        // 在Epicwin之类弹窗结束后恢复音乐的播放
        // 某些特殊关卡需要重载以避免同时播放多个音乐
        public virtual void RestoreMusicAfterAnimation()
        {
            if (isFreespinBonus)
            {
                Libs.AudioEntity.Instance.UnPauseFreeGameBgMusic();
            }
        }

        public virtual void StopMusicBeforeAniamtion()
        {
            if (isFreespinBonus)
            {
                Libs.AudioEntity.Instance.PauseFreeGameBgMusic();
            }
            else
            {
                StopBaseGameMusic();
            }
        }

        #endregion


        public virtual void RecoveryBaseGameData(string RName, string AName, List<List<int>> results)
        {
            resultContent.ChangeResult(results);
            reelStrips.ResetRAData(RName, AName, results);
            ChangeSymbols(results);
        }

        public virtual void CheckAward(bool wildAccumulation, Dictionary<string, object> infos)
        {
            resultContent.awardResult.CreateAwardData(this, wildAccumulation);
            ReCheckAward(infos);
        }

        protected virtual void ReCheckAward(Dictionary<string, object> infos)
        {
        }

        //目前专门为test使用
        public virtual void PostCheckAwardEvent()
        {
        }

        public virtual void InitAtStart()
        {
        }

        public virtual void AfterInitReelAnimationHandler()
        {
        }

        public virtual bool extraAwardIsAccumulatedAlone()
        {
            return false;
        }

        public void ReelStripSimulationTest()
        {
            if (TestController.Instance != null)
            {
                BackupPreSymbolResults();
            }
        }

        public virtual void ChangTestData()
        {
        }

        public virtual bool HasAwardSymbol()
        {
            return !resultContent.awardResult.IsAwardElementIndexOfDisplayDictEmpty();
        }

        public virtual bool EnableOnceMore()
        {
            return (gameConfigs.HasOnceMore && HasAwardSymbol() &&
                    (UnityEngine.Random.Range(0f, 1f) < gameConfigs.GenRate));
        }

        /// <summary>
        /// 此方法用于判断不中奖时 是否触发Respin
        /// </summary>
        public virtual bool WhetherRespin()
        {
            return false;
        }

        protected List<List<bool>> smartSoundMap = new List<List<bool>>();

        public virtual bool CheckPlaySmartSoundCondition(int reelIndex, int elementIndex)
        {
            if (smartSoundMap.Count <= reelIndex || smartSoundMap[reelIndex].Count <= elementIndex)
                return false;
            return smartSoundMap[reelIndex][elementIndex];
        }

        public virtual void ChangeMiddleMessage(string contextMsg)
        {
            Messenger.Broadcast<string>(SlotControllerConstants.ChangeMiddleMessage, contextMsg);
        }

        #region Spin Effect

        protected SmartAntiEffectController smartEffectController;

        #endregion

        #region 转速新版

        [HideInInspector] public ReelController boardController;

        public bool IsNewSpeedPattern
        {
            get { return true; }
        }

        //需要anti的reel数组，在转动之前传入进去
        protected List<int> NeedAntiReels = new List<int>();

        protected bool NeedPlaySmartSound = true;

        //是否已经设置过棋盘了
        protected bool HasLayoutBoard = false;

        protected virtual void LayoutBoard()
        {
            if (BaseGameConsole.singletonInstance.isForTestScene)
            {
                BoardConfigs boardConfigNew = GetBoardConfigs();
                boardController.TestSetBoardConfig(boardConfigNew);
                return; // 测试模式下无需layout，跟ui不相关
            }

            //如果不需要重新设置棋盘则退出,需要根据goldconfig中的变量NeedResetBoard来判断
            if (HasLayoutBoard || this.isFreespinBonus)
            {
                GoldGameConfig goldConfig = this.GetComponent<GoldGameConfig>();
                if (goldConfig != null && goldConfig.NeedResetBoard)
                {
                    BoardConfigs boardConfigNew = GetBoardConfigs();
                    if (boardConfigNew != null)
                    {
                        boardController.LayOut(boardConfigNew, GetLayOutReelShowData(), this);
                    }
                }

                HasLayoutBoard = true;
            }
            else
            {
                BoardConfigs boardConfigs = GetComponent<BoardConfigs>();
                if (boardController != null)
                {
                    boardController.LayOut(boardConfigs, GetLayOutReelShowData(), this);
                }

                HasLayoutBoard = true;
            }
        }

        /// <summary>
        /// 初始化牌面的数据
        /// </summary>
        /// <returns></returns>
        protected virtual List<List<int>> GetLayOutReelShowData()
        {
            List<List<int>> data = slotConfig.FakeStrip?.GetRunData(isFreespinBonus);
            if (data != null&&data.Count>0) return data;
            return this.resultContent.ReelSpinShowData;
        }

        //获取boardConfig，子类进行赋值，比如freespin、 respin中的config进行单独的配置。即默认的boardconfig跟gameConfig进行绑定
        protected virtual BoardConfigs GetBoardConfigs()
        {
            return this.gameConfigs.GetComponent<BoardConfigs>();
        }

        //接口
        public virtual void DoRunStopHandler(bool isFastStop)
        {
            State = GameState.READY;
            if (isFastStop)
            {
                if (smartEffectController != null)
                {
                    smartEffectController.StopAllBlinkAnimation(this);
                }
            }

            OnStop();
        }

        public virtual void DoEachReelStopHandler(int reelIndex)
        {
            if (EachReelStopHandler != null)
            {
                EachReelStopHandler(reelIndex);
            }

            if (this.boardController != null)
            {
                this.boardController.ReSetAnimationRender(reelIndex);
            }
        }
        
        //轴开始减速的时间节点回调
        public virtual void DoEachReelSlowDownHandler(int reelIndex)
        {
            
        }

        //回弹时的回调
        public virtual void ReelBounceBackHandler(int reelIndex)
        {
        }

        public virtual void LastReelDistanceHandler(int reelIndex, SymbolRender render)
        {
        }

        /// <summary>
        /// Checks the play smart sound.每次spin的时候进行检测是否播放smart的声音
        /// </summary>
        /// <returns><c>true</c>, if play smart sound was checked, <c>false</c> otherwise.</returns>
        public virtual bool CheckPlaySmartSound()
        {
            return true;
        }

        //接口，symbolRender用
        public SymbolMap.SymbolElementInfo GetSymbolInfoByIndex(int symbolIndex)
        {
            return symbolMap.getSymbolInfo(symbolIndex);
        }

        public GameConfigs GetGameConfig()
        {
            return this.gameConfigs;
        }

        //从映射中获取静止状态的render
        public BaseElementPanel GetSymbolRender(int reelIndex, int positionIndex)
        {
            return this.boardController.GetSymbolRender(reelIndex, positionIndex);
        }

        public int GetReelCount()
        {
            return this.boardController.GetReelCount();
        }

        //当前reel显示的个数
        public int GetReelSymbolRenderCount(int index)
        {
            return this.boardController.GetReelRenderCount(index);
        }

        /// <summary>
        /// Gets the current reel curves.
        /// 给转速面板提供转速曲线ID数组，标记每个状态下使用的曲线ID，只提供真实转速轴的ID，不考虑不转动的轴的ID
        /// </summary>
        /// <returns>The current reel curves.</returns>
        protected virtual List<int> GetReelCurveIndexs()
        {
            List<int> curReelCurves = new List<int>();
            for (int i = 0; i < GetReelCount(); i++)
            {
                curReelCurves.Add(i);
            }

            return curReelCurves;
        }

        public virtual void ChangeSmartAnimationPosition(int reelId, float offsetY)
        {
            if (smartEffectController != null)
            {
                foreach (var positionId in smartEffectController.GetReelSmartPositionId(reelId))
                {
                    Transform animationRender = this.boardController.GetAnimationRender(reelId, positionId);
                    if (animationRender != null)
                        animationRender.localPosition = new Vector2(0, animationRender.localPosition.y + offsetY);
                }
            }
        }

        #endregion

        #region fast speed mode

        public void SetFastModeOnStart()
        {
            int fastmode = PlayerPrefs.GetInt(SlotControllerConstants.MachineSpeedFastMode);
            this.boardController.SetFastMode(fastmode!=0);
        }
        
        public void SetFastMode(bool isFast)
        {
            this.boardController.SetFastMode(isFast);
            PlayerPrefs.SetInt(SlotControllerConstants.MachineSpeedFastMode,isFast?1:0);
        }

        public void SetPlistFastTimeScale(float allMachine,float thisMachine)
        {
            float mFastTimeScalePlist = 1;
            if (allMachine > 0.001f)
            {
                mFastTimeScalePlist = allMachine;
            }
				
            if (thisMachine>0.001f)
            {
                mFastTimeScalePlist = thisMachine;
            }
				
            if (allMachine < -0.001f)
            {
                mFastTimeScalePlist = 1;
            }
            this.boardController.SetPlistFastTimeScale(mFastTimeScalePlist);
        }

        public bool GetFastMode()
        {
            return this.boardController.GetFastMode();
        }

        #endregion

        #region 20191223 流程新扩展方法

        /// <summary>
        ///分析结果数据
        ///加奖励
        /// </summary>
        public virtual void ParseSpinResult()
        {
            if (!IsNewProcess)
            {
                return;
            }
            
            if(!BaseGameConsole.singletonInstance.isForTestScene)
            {
                //onceMore的状态需要提前设置下
                if (BaseSlotMachineController.Is_Quit)
                    return;
                BaseSlotMachineController.Instance.SetNextLinkState();
                BaseSlotMachineController.Instance.ResetTotalAward();
                //获取结果奖励值
                ResultData = SetSpinEndProcessData();
                if (ResultData.NeedAccount)
                {
                    //加奖励，并处理onceMore
                    BaseSlotMachineController.Instance.GetResultAward(ResultData);
                    //发送es事件
                    // SpinEndSendEs();
                }
            }
        }

        /// <summary>
        /// 所有中奖动画播放完毕，下次spin之前的流程逻辑，有些关卡需要动画完之后做操作
        /// 已使用在MoreFu关卡中，切换fs的棋盘
        /// </summary>
        public virtual void PlayAnimationFinishHandler(Action callback)
        {
            Messenger.Broadcast(global::SpinButtonStyle.ENABLESPIN);
            callback();
        }

        /// <summary>
        /// 所有动画流程走完之后最终的回调，
        /// 小心在FS之后需要有正常的normalResult的时候才会调用，已使用在MoreFu关卡中，做super fs的逻辑
        /// </summary>
        /// <param name="callback"></param>
        public virtual void BeforeNextSpinHandler(Action callback)
        {
            callback();
        }

        #region  20200324 流程扩展，默认跟之前的一样,IsNewProcess为true代表先结算后进入
        ///标识是否是用新流程的开关
        public bool IsNewProcess { set; get; } = false;
        /// <summary>
        /// 轮子停下的逻辑，跟数值无关
        /// 可以自己关卡扩充处理
        /// </summary>
        /// 
        public virtual void HandleGameProcedureState()
        {
            if (this.IsNewProcess)
            {
                ResultStateManager.Instante.PlayAwardBeforeFeature();
            }
            else
            {
                ResultStateManager.Instante.PlayAwardAfterFeature();
            }
        }
         /// <summary>
        /// spin 结果出来后link、fs、normal的判断
        /// </summary>
        /// <returns></returns>
        public ResultProcessData SetSpinEndProcessData()
        {
            ResultProcessData data = new ResultProcessData();
            if (IsLinking())
            {
                data.AccountType = ResultAccountTypeEnum.LINKING;
            }
            else if (this.IsLinkEnd())
            {
                data.AccountType = ResultAccountTypeEnum.LINKEND;
                data.AccountValue = LinkAward();
            }
            else if (this.isFreespinBonus)
            {
                data.AccountType = ResultAccountTypeEnum.FS;
                data.LineValue = data.AccountValue = GetLineAwardValue();
                 
                if (IsHitOrRetriggerFs())
                {
                    double scatterPayAwardValue = this.FsRetriggerScatterAward();
                    data.AccountValue += scatterPayAwardValue;
                    BaseSlotMachineController.Instance.baseAward += scatterPayAwardValue;
                    // Debug.LogAssertion("ScatterPay : "+BaseSlotMachineController.Instance.baseAward);
                }
                
                SlotControllerConstants.JACKPOT_TYPE type;
                double jpCoins = this.JpAwardCoin(out type);
                if (jpCoins > 0)
                {
                    data.AccountValue += jpCoins;
                }

                // if (this.FreespinGame.NeedShowCash)
                // {
                //     //在freeSpin结算奖励中添加现金奖励值
                //     data.CashValue = OnLineEarningMgr.Instance.GetFreeSpinReward();
                // }
            }
            else
            {
                data.AccountType = ResultAccountTypeEnum.NORMAL;
                //line award和scatter奖励，可能还有别的奖励，到时候再扩展个方法
                data.LineValue = data.AccountValue = GetLineAwardValue() ;
                
                if (IsHitOrRetriggerFs())
                {
                    double scatterPayAwardValue = this.FsEnterScatterAward();
                    data.AccountValue += scatterPayAwardValue;
                    BaseSlotMachineController.Instance.baseAward += scatterPayAwardValue;
                    // Debug.LogAssertion("ScatterPay : "+BaseSlotMachineController.Instance.baseAward);
                }

                SlotControllerConstants.JACKPOT_TYPE type;
                double jpCoins = this.JpAwardCoin(out type);
                if (jpCoins > 0)
                {
                    data.AccountValue += jpCoins;
                }
            }

            if (!PlatformManager.Instance.IsWhiteBao())
            {
                //计算现金奖励
                if (isFreespinBonus)
                {
                    data.CashValue = OnLineEarningMgr.Instance.GetFreeSpinReward(FreespinGame.NeedShowCash);
                }
                else
                {
                    if (data.AccountValue > GetCurrentBet() && data.AccountValue< gameConfigs.BigWinTag * GetCurrentBet())
                    {
                        data.CashValue = OnLineEarningMgr.Instance.GetNormalSpinReward();
                    }
                }
                Debug.Log($"[ReelManager] [SetSpinEndProcessData] data.CashValue:{data.CashValue}");
            }
            
            return data;
        }
         
         
        /// <summary>
        /// 是否在link中，
        /// </summary>
        /// <returns></returns>
        protected virtual bool IsLinking()
        {
            return BaseSlotMachineController.Instance.lastOnceMore && BaseSlotMachineController.Instance.onceMore;
        }
        /// <summary>
        /// 是否link结束，如MonsterParty可以扩展是否需要在最终处理
        /// </summary>
        /// <returns></returns>
        protected virtual bool IsLinkEnd()
        {
            return BaseSlotMachineController.Instance.lastOnceMore && !BaseSlotMachineController.Instance.onceMore;
        }
        /// <summary>
        /// line奖励
        /// </summary>
        /// <returns></returns>
        protected virtual double GetLineAwardValue()
        {
            AwardResult.CurrentResultBet = GetBetWeight ();
            CheckAward (this.slotConfig.wildAccumulation,slotConfig.extroInfos.infos);

            resultContent.awardResult.awardValue = Math.Round(resultContent.awardResult.awardValue * GetCurrentBet() / gameConfigs.ScalFactor);
            double totalAward = resultContent.awardResult.awardValue;
            
            if (this.isFreespinBonus) {
                totalAward *= this.FreespinGame.multiplier;
                resultContent.awardResult.ChangedAwardValue (FreespinGame.multiplier);
                SpinClickedMsgMgr.Instance.SetMultiplier(FreespinGame.multiplier);
            }
            // BaseSlotMachineController.Instance.baseAward += totalAward;
            // Debug.LogAssertion("min.wang : baseAward : "+BaseSlotMachineController.Instance.baseAward);
            return totalAward;
        }

        protected float GetBetWeight()
        {
            if (BaseGameConsole.ActiveGameConsole().isForTestScene)
            {
                return TestController.Instance.GetBetWeight();
            }
            return  Utilities.CastValueFloat(BaseSlotMachineController.Instance.GetBetWeight ());
        }

        protected long GetCurrentBet()
        {
            if (BaseGameConsole.ActiveGameConsole().isForTestScene)
            {
                return TestController.Instance.currentBetting;
            }
            return  BaseSlotMachineController.Instance.currentBettingTemp;
        }
    
        /// <summary>
        /// link过程中的奖励
        /// </summary>
        /// <returns></returns>
        public virtual double LinkAward()
        {
            if (extraAward != null) {
                extraAward.GetAwardResult ();
                return extraAward.AwardInfo.awardValue;
            }
            
            return 0;
        }

        public double LinkAwardForEs()
        {
            if (extraAward != null) {
                return extraAward.AwardInfo.awardValue;
            }
            
            return 0;
        }
        /// <summary>
        /// 进入fs的scatter奖励,需要乘以bet
        /// </summary>
        /// <returns></returns>
        public virtual double FsEnterScatterAward()
        {
            return 0;
        }
        
        /// <summary>
        /// retriger fs的奖励，需要乘以bet，默认跟进入fs的奖励相同，后续可以扩展
        /// </summary>
        /// <returns></returns>
        public virtual double FsRetriggerScatterAward()
        {
            return FsEnterScatterAward();
        }
        /// <summary>
        /// 触发或者retrigger fs
        /// </summary>
        /// <returns></returns>
        protected virtual bool IsHitOrRetriggerFs()
        {
            return FreespinCount > 0;
        }

        /// <summary>
        /// 关卡自己的jp,参考关卡wildgenie
        /// </summary>
        /// <returns></returns>
        public virtual double JpAwardCoin(out SlotControllerConstants.JACKPOT_TYPE type)
        {
            type = SlotControllerConstants.JACKPOT_TYPE.NONE;
            return 0;
        }

        /// <summary>
        /// bonus 的award,newprocess可以继承此类,也可用老的BonusGame.AwardInfo
        /// </summary>
        /// <returns></returns>
        public virtual double GetBonusAward()
        {
            double totalAward= 0;
            if (HasBonusGame &&BonusGame!=null &&BonusGame.AwardInfo!=null ) {
                double bonusAward = 0;
                if (BonusGame.IsNewResultMode)
                {
                    bonusAward = BonusGame.AwardInfo.awardValue;
                }
                else
                {
                    bonusAward= BonusGame.AwardInfo.awardValue * SlotsBet.GetCurrentBet();
                }
                totalAward += bonusAward;
                
            }

            return totalAward;
        }
        /// <summary>
        /// 添加bonus的奖励值
        /// </summary>
        public virtual void IncreaseBonusAward()
        {
            if (!this.IsNewProcess)
                return;
            IsSpinCostCoins = false;
            double bonusAward = GetBonusAward();
            if (bonusAward > 0)
            {
                ResultProcessData data = new ResultProcessData();
                data.AccountType = ResultAccountTypeEnum.BONUS;
                data.AccountValue = bonusAward;
                BaseSlotMachineController.Instance.GetResultAward(data);
                
                Messenger.Broadcast<float, long, long>(SlotControllerConstants.OnChangeWinText, 1f, (long)(BaseSlotMachineController.Instance.winCoinsForDisplay-bonusAward), (long)bonusAward);
                Messenger.Broadcast (SlotControllerConstants.OnBlanceChangeForDisPlay);
                SpinClickedMsgMgr.Instance.AddWinDictData(SpinClickedMsgMgr.BonusWin_Key, bonusAward );
            }
            
        }
        
        /// <summary>
        /// link和fs触发及过程中,以及在bonus中都不能发送，link end、fs end、bonus end才有可能发送 。morefu等的superfs单独处理
        /// </summary>
        /// <returns></returns>
        protected virtual bool IsSendEsCurrentSpin()
        {
            if (ResultStateManager.Instante.slotController.onceMore == true)
            {
                return false;
               
            }
            if (this.FreespinCount > 0)
            {
                return false;
               
            }
            if (ResultStateManager.Instante.slotController.reelManager.isFreespinBonus &&
                ResultStateManager.Instante.slotController.freespinGame.LeftTime > 0)
            {
                return false;
            }

            if (RewardSpinManager.Instance.RewardIsValid())
                return true;

            return true;
        }

        public bool IsChangeBet()
        {
            return IsSendEsCurrentSpin();
        }

        /// <summary>
        /// 0代表正常，1为super fs，2为 op发送的fs.方便数值统计
        /// </summary>
        /// <returns></returns>
        protected int EsSpinClickSourceType(bool isFsAward)
        {
            int ret = 0;
            if (isFsAward)
            {
                ret = 2;
            }else if (this.IsCollectGame)
            {
                ret = 1;
            }

            return ret;
        }
        public void SpinEndSendEs(bool isFsAward)
        {
            if (IsSendEsCurrentSpin())
            {
                Log.LogLimeColor($"send es log : winCoins:{BaseSlotMachineController.Instance.winCoinsForDisplay}, currentBet:{BaseSlotMachineController.Instance.currentBettingTemp}");
                int sourceType = EsSpinClickSourceType(isFsAward);
                ResultStateManager.Instante.slotController.SendSpinEvent(sourceType);
            }
        }
        #endregion
        #endregion

        #region rule result

        [HideInInspector] [NonSerialized] public SpinResultData SpinData = null;
        

        protected virtual Type SpinDataType()
        {
            return typeof(SpinResultData);
        }

        public virtual SpinResultData CreateSpinResult()
        {
            Type t = SpinDataType();
            if (t == null) t = typeof(SpinResultData);
            SpinResultData spinResult = (SpinResultData) Activator.CreateInstance(t);
            spinResult.Init(this);
            return spinResult;
        }

        public RuleResultMediation GetRuleResult(RuleSwitch ruleSwitch)
        {
            RuleResultMediation mediation;
            mediation.IsFeature = ruleSwitch.IsOpenFeature ? IsResultFeature() : false ;
            mediation.AwardValue = ruleSwitch.IsOpenRtp? GetRuleLineAwardValue(): 0;
            return mediation;
        }

        //目前先用在baseGame中，仅仅生成结果
        public void RuleCreateResult()
        {
            CreateRealAwardResult();
        }

        /// <summary>
        /// 用于填充替换数据，用于基础及rule中
        /// </summary>
        public virtual void CreateRealAwardResult()
        {
            resultContent.CreateRawResult();
        }

        public virtual bool IsResultFeature()
        {
            this.CheckFeature();
            return HasBonusGame || this.FreespinCount > 0 || HitLinkGame || HitFs;
        }

        /// <summary>
        /// 修复AutoSpin时，进入Feature前点击Spin后可以更改Bet
        /// </summary>
        /// <returns></returns>
        public virtual bool IsExistFeature()
        {
            return HasBonusGame || this.FreespinCount > 0 || HitLinkGame || HitFs;
        }

        //极个别关卡可以重载此方法
        public virtual float GetRuleLineAwardValue()
        {
            float ret = 0;
            //简单的跟据payline长度来计算
            if (this.lineTable.TotalPayLineCount() > 0)
            {
                ret = AwardResult.CreateAwardDataTemp(this, this.slotConfig.wildAccumulation);
            }
            else
            {
                //看BuffaloTerritory和MightyGollia baseGame是都不需要乘以系数的
                ret = CalculateProductPayTableAwardInUnidirectional(null, 1, true);
            }

//            Log.LogWhiteColor($"line rtp:{ret}");
            return ret;
        }
        
        /// <summary>
        /// 20211116   ruleConfig，子类中需要添加继承的，处理数据
        /// </summary>
        public virtual void ReplaceConfigData()
        {
            Log.LogLimeColor("ReplaceConfigData");
        }

        /// <summary>
        /// 在rule失效的时候能够调用到清除配置，且只在创建结果的时候去清除，初始化不做处理
        /// </summary>
        public void ClearServerConfig()
        {
            BaseSlotMachineController.Instance.RuleConfig.ClearServerConfigData();
        }

        #endregion

        private List<ISpinResultProvider> _spinResultProviders=new List<ISpinResultProvider>();
       
        public void AddSpinResultProvider(ISpinResultProvider provider)
        {
            SpinResultProduce.AddProvider(provider);
        }
        public void ClearSpinResultProvider()
        {
            _spinResultProviders?.Clear();
        }
        
        public bool IsReSpinIng()
        {
            //trigger 并未进入到respin
            if (!BaseSlotMachineController.Instance.lastOnceMore && NextTimeOnceMore)
            {
                return false;
            }
            //本次仍然处理spin状态 可能 NextTimeOnceMore = false 但是此处算还在respin 中 是否在repsin 中
            if (BaseSlotMachineController.Instance.lastOnceMore)
            {
                return true;
            }
            
            return false;
        }
        
        public bool NeedCheckFreeSpinAward;
        public virtual void CheckSpinResult()
        {
            //respin 中都需要单独处理
            if (IsReSpinIng())
            {
                return;
            }

            if (isFreespinBonus)
            {
                CheckFreeResult();
            }
            else
            {
                SpinResultBaseGame resultBaseGame = new SpinResultBaseGame();

                //只适用用 触发 freespin 的 symbol 不参与其他奖励  添加symboL
                FreespinGame freeSpinGame = BaseSlotMachineController.Instance.freespinGame;
                if (NeedCheckFreeSpinAward && BaseSlotMachineController.Instance.isReturnFromFreespin &&
                    freeSpinGame.FirstMoreAwardValue > 0.00001)
                {
                    BaseSlotMachineController.Instance.baseAward += freeSpinGame.FirstMoreAwardValue*BaseSlotMachineController.Instance.currentBetting;
                    Dictionary<string, int> winSymbols = SpinResultProduce.InternalGetWinSymbolList();
                    // Debug.LogAssertion("min.wang baseaward  : "+BaseSlotMachineController.Instance.baseAward+" moreaward : "+freeSpinGame.FirstMoreAwardValue);
                    if (resultContent.FreeSymbolDic.Count>0)
                    {
                        if (winSymbols == null)
                        {
                            winSymbols=new Dictionary<string, int>();
                        }
                        foreach (KeyValuePair<string,int> pair in resultContent.FreeSymbolDic)
                        {
                            winSymbols[pair.Key] = pair.Value;
                        }
                    }
                }
                SpinResultProduce.AddProvider(resultBaseGame);
            }
        }

        public void CheckFreeResult()
        {
            //只适用用 触发 freespin 的 symbol 不参与其他奖励  添加symboL
            Dictionary<string, int> winSymbols = null;
            FreespinGame freeSpinGame = BaseSlotMachineController.Instance.freespinGame;
            if (NeedCheckFreeSpinAward && freeSpinGame.NextMoreAwardValue > 0.00001)
            {
                if (resultContent.FreeSymbolDic.Count>0)
                {
                    if (winSymbols == null)
                    {
                        winSymbols=new Dictionary<string, int>();
                    }
                    foreach (KeyValuePair<string,int> pair in resultContent.FreeSymbolDic )
                    {
                        winSymbols[pair.Key] = pair.Value;
                    }
                }
            }
            SpinResultFreeGame resultFreeGame = new SpinResultFreeGame(FreespinGame.TotalTime,
                FreespinGame.LeftTime == 0, 0,
                ResultStateManager.Instante.slotController.freespinGame.IsSuperFreeSpin(),null,winSymbols);
            SpinResultProduce.AddProvider(resultFreeGame);
        }

        public List<ISpinResultProvider> GetSpinResult()
        {
            CheckSpinResult();
            if (FreespinGame!=null&&FreespinGame.GetForceRemmoveSpinResult())
            {
                _spinResultProviders.RemoveAt(_spinResultProviders.Count - 1);
                FreespinGame.SetForceRemoveSpinResult(false);
            }

            return _spinResultProviders;
        }
        #region TestLog

        public virtual void ClearTestLog()
        {
        }

        public virtual string GetTestLog()
        {
            return "";
        }

        #endregion

        public bool isFreespinBonus { get; set; }

        public virtual bool isSpining()
        {
            return State != GameState.READY;
        }

        public GameState State { get; set; }

        public List<BaseReel> Reels { get; set; }

        public GameCallback OnStart { get; set; }
        
        public int AutoSpinSegment { get; set; }
        public GameCallback OnStop { get; set; }

        public bool AutoRun { get; set; }
        public void SetAutoRun(bool autoRun)
        {
            AutoRun = autoRun;
        }

        public ResultContent resultContent { get; set; }

        public GameConfigs gameConfigs { get; set; }


        public IExtraGame extraAward { get; set; }

        public BonusGame BonusGame { get; set; }

        public FreespinGame FreespinGame { get; set; }

        public int StartAnticipationIndex { get; set; }

        public int FreespinCount { get; set; }

        public bool HitFs { get; set; } = false;

        /// <summary>
        /// 处理op活动任务仅统计触发
        /// </summary>
        public bool HitActBonus { get; set; }
        
        public bool HasBonusGame { get; set; }

        public bool HitLinkGame { get; set; }

        public int BonusNum { set; get; }

        public bool NeedReCreatResult { get; set; }

        public bool HasCircleGame { get; set; }

        public bool NeedSavePlayerSlotMachineStateData = false; //LQ 防止影响classic 故默认关闭保存项


        public SymbolMap symbolMap { get; set; }

        public ReelStripManager reelStrips { get; set; }

        public bool not_singleLine
        {
            get { return (lineTable != null && lineTable.PayLines().Count != 1); }
        }

        public LineTable lineTable { get; set; }

        public ClassicPaytable payTable { get; set; }

        public ReelManagerDataConfig reelManagerDataConfig { get; set; }

        public SlotMachineConfig slotConfig { get; set; }

        private float clientV = 0f;

        public float ClientV
        {
            get { return clientV; }
        }

        public MachineTestDataFromPlistConfig machineTestDataFromPlist { get; set; }

        [HideInInspector] public bool enableWinType;

        public ExtraGameStateResultDataManager.ExtraGameType currentExtraType { get; set; }

        public ReelStopCallback EachReelStopHandler { set; get; }

        [HideInInspector] public bool ExistReelLocked = false;

        [HideInInspector] public bool NextTimeOnceMore = false;


        #region pattern es data

        public int PatternEsType { set; get; } = -1;
        public string PatternEsId { set; get; } = null;

        public void ResetPatternEsType()
        {
            PatternEsType = -1;
            PatternEsId = null;
            RulePatternManager.GetInstance().Round = 0;
        }

        #endregion
        // 收集触发的game需要设置此值，在es上rtp统计有用
        public bool IsCollectGame { get; set; } = false;

        public bool SpinUseNetwork { set; get; } = false;
        #region obsote弃用的部分

        public bool fastStop { get; set; }

        public bool fastStopDisabled { get; set; }

        #endregion


        public Vector3 GetMiddlePosition()
        {
            if (this==null)
            {
                return Vector3.zero;
            }
            return transform.position;
        }
    }
}