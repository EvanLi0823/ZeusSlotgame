using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Libs;
using Core;
using Utils;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace Classic
{
    public class FreespinGame : BaseExtraAward
    {
        public static readonly string FreespinSlotMachine = "FreespinSlotMachine";
        public static readonly string FreespinScatterPay = "ScatterPay";
        public const string AdFreespinCountAdd = "AdFreespincountAdd";

		public readonly static string IN_FREESPIN_AUDIO ="In-FreeSpin";
		public readonly static string OUT_FREESPIN_AUDIO ="Out-FreeSpin";

		public float InFreeSpinMusicDuration = 5f;
		public float OutFreeSpinMusicDuration = 5f;
		public bool enableInFreespinAudioEffect = true;
		public bool enableOutFreespinAudioEffect = true;

        public GameConfigs freeSpinGameConfigs;
        public GameConfigs normalGameConfigs;
        protected Dictionary<string,object> savedFreeSpinInfos;

        protected SymbolMap savedSymbolMap;
        protected ReelStripManager savedReelStrips;
        protected ClassicPaytable savedPaytable;
        protected LineTable savedLineTable;
        protected SmartSoundReelStripsController savedSmartSoundReelController;
		protected float scatterPay = 0;
		
		public bool isForceRemoveSpinResult;

		//用来区分未看广告的 freegame和看过广告的 freegame
		//为true时表明需要乘以看广告倍数计算奖励
		public bool NeedShowCash
		{
			get;
			set;
		}
		
		
		public static bool isAdFreeSpinCountAdd
		{
			set
			{
				if (BaseGameConsole.ActiveGameConsole ().isForTestScene)
				{
					SharedPlayerPrefs.SetPlayerPrefsBoolValue(TestController.Instance.SlotDropdown.captionText.text+ "IsAddFreespinCountByAds", value);
					return;
				}
				SharedPlayerPrefs.SetPlayerPrefsBoolValue(BaseGameConsole.ActiveGameConsole ().SlotMachineController.slotMachineConfig.Name ()+ "IsAddFreespinCountByAds", value);
			}
			get
			{
				if (BaseGameConsole.ActiveGameConsole ().isForTestScene) 
				{
					return SharedPlayerPrefs.GetPlayerBoolValue(TestController.Instance.SlotDropdown.captionText.text+"IsAddFreespinCountByAds",false);
				}
				return SharedPlayerPrefs.GetPlayerBoolValue(BaseGameConsole.ActiveGameConsole ().SlotMachineController.slotMachineConfig.Name ()+"IsAddFreespinCountByAds",false);
			}
		}

		[Header("在进入fs之前就计算奖励")]
		[HideInInspector]
		public bool AwardBeforeEnterFS = false; 
		[FormerlySerializedAs("AwardBAdded")]
		[Header("保存恢复时，是否需要重新加线奖")]
		[HideInInspector]
		public bool AwardAdded = false;
		public bool HaveAddedTimes {
			get;
			set;
		}
		public bool HaveInitedFreeSpinCount{
			get;
			private set;
		}
        public int TotalTime {
            get;
            set;
        }

        public int LeftTime {
            get;
            set;
        }
        
        /// <summary>
        /// 用于 machinequest 数据退出freespin 时计算
        /// </summary>
        public double FirstMoreAwardValue
        {
	        get;
	        set;
        }
        public double NextMoreAwardValue
        {
	        get;
	        set;
        }
        private bool spinFinished = true;

//        public bool SpinFinished {
//	        get { return spinFinished; }
//	        protected set { spinFinished = value; }
//        }
        public int multiplier {
			get{ return  _multiplier;}
			set{_multiplier = value; }
        }

		private int _multiplier =1;

        protected bool isGameConfigDataiIsDirty = false;

		public virtual void InitFreespin (int totalTime, Dictionary<string,object> infos = null, GameCallback onGameOver = null,int multiplier = 1)
        {
	        if (BaseSlotMachineController.Instance.reelManager.IsNewProcess)
	        {
		        this.OnGameOver = null;
		        //加这个操作是避免初始化，线奖没有加进入，只把父类的部分加进去
		        this.OnGameOver += onGameOver;
	        }
	        else
	        {
		        this.Init(infos, onGameOver);
	        }
			InitData (totalTime,infos,multiplier);
			OnFreespinTimesChanged();
        }

		public void OnFreespinTimesChanged(){
			Messenger.Broadcast<int,int,int> (SlotControllerConstants.OnFreespinTimesChanged, this.LeftTime, this.TotalTime,this.multiplier);
		}

		#region Restore FreeSpin
		[HideInInspector]
		public bool currentSpinFinished = false;
		[HideInInspector]
		public bool isRestoreFreespin = false;

		[HideInInspector]
		protected ReelManager reelManager= null;
		public virtual void RestoreFreespin(int leftCount,int totalCount,long currentWinCoins,Dictionary<string,object> infos = null,GameCallback onGameOver = null,int multiplier = 1,int currentWinCash = 0,bool needShowCash = false){
			this.Init (infos, onGameOver);
			RestoreData (leftCount, totalCount,currentWinCoins, infos, multiplier,currentWinCash,needShowCash);
			Messenger.Broadcast<int,int,int> (SlotControllerConstants.OnFreespinTimesChanged, this.LeftTime, this.TotalTime,multiplier);
			isRestoreFreespin = true;
		}

		public virtual void RestoreData(int leftCount,int totalCount,long currentWinCoins,Dictionary<string,object> infos = null,int multiplier = 1,int currentWinCash = 0,bool needShowCash = false){
			this.TotalTime = totalCount;
			this.LeftTime = leftCount;
			AwardInfo.awardValue = (double)currentWinCoins;
			AwardInfo.awardCashValue = currentWinCash;
			this.multiplier = multiplier;
			savedFreeSpinInfos = infos;
			isGameConfigDataiIsDirty = false;

			if (savedFreeSpinInfos != null && savedFreeSpinInfos.ContainsKey(FreespinScatterPay)) {
				scatterPay = float.Parse(savedFreeSpinInfos[FreespinScatterPay].ToString());
			}
		}
		#endregion


		public virtual void InitData(int totalTime,Dictionary<string,object> infos = null,int multiplier = 1){
			Messenger.AddListener(AdFreespinCountAdd,AddFreeCount);
			this.TotalTime = totalTime;
			this.LeftTime = totalTime;
			//2020 0327,老流程的奖励
			if (BaseGameConsole.singletonInstance.isForTestScene)
			{
				AwardInfo.awardValue = 0;
				AwardInfo.awardCashValue = 0;
			}
			else if (!BaseSlotMachineController.Instance.reelManager.IsNewProcess)
			{
				AwardInfo.awardValue = 0;
				AwardInfo.awardCashValue = 0;
			}

			this.multiplier = multiplier;
			savedFreeSpinInfos = infos;
			isGameConfigDataiIsDirty = false;
			HaveAddedTimes = false; 
			HaveInitedFreeSpinCount=true;
			if (savedFreeSpinInfos != null && savedFreeSpinInfos.ContainsKey(FreespinScatterPay)) {
				scatterPay = float.Parse(savedFreeSpinInfos[FreespinScatterPay].ToString());
			}
		}

		//看广告增加freespincount
	    protected  void AddFreeCount()
		{
			isAdFreeSpinCountAdd = true;
			this.TotalTime++;
			if (reelManager != null) reelManager.FreespinCount = TotalTime;
			this.LeftTime = TotalTime;
			OnFreespinTimesChanged();
			if (reelManager != null) reelManager.SavePlayerCurrentSlotMachineStateData();//强制用户保存，防止看广告后数据丢失
		}

        public override void OnEnterGame (ReelManager reelManager)
        {
            base.OnEnterGame (reelManager);

			reelManager.IsTriggeredFreespinAndNotReelStartRun = true;
			if (savedFreeSpinInfos != null && savedFreeSpinInfos.ContainsKey(FreespinScatterPay))
			{
				scatterPay = float.Parse(savedFreeSpinInfos[FreespinScatterPay].ToString());
			}
			//关闭常规背景音乐
			AudioEntity.Instance.StopSpinAudio();
			//播放 freespin背景音乐
            AudioEntity.Instance.PlayFreeSpinBackgroundMusic();
			Messenger.Broadcast(global::SpinButtonStyle.CHANGEBUTTONIMAGEONENTERFREE);
        }
		public virtual void OnStartRun(System.Action action){
			if(action!=null) action();
		}
		public virtual void OnResumeRun(System.Action action){
			if(action!=null) action();
		}
		public virtual BaseAward ChangerDateOnEnterGame(ReelManager reelManager){
			SpinClickedMsgMgr.Instance.SaveBaseGameOutBeforeEnterFreespin();
            BaseAward moreAward = new BaseAward ();
			if (!Mathf.Approximately(scatterPay, 0))
			{
				moreAward.awardName = "InFreespinScatter";
				moreAward.awardValue = (scatterPay);
			}

            savedSymbolMap = reelManager.symbolMap;
            savedReelStrips = reelManager.reelStrips;
            savedPaytable = reelManager.payTable;
            savedLineTable = reelManager.lineTable;

            SymbolMap symbolMap = CreateSymbolMap (savedFreeSpinInfos);
            ReelStripManager reelStrips = CreateReelStrips (savedFreeSpinInfos, symbolMap);
            ClassicPaytable paytable = CreatePaytable (savedFreeSpinInfos, symbolMap);
            LineTable lineTable = CreateLineTable (savedFreeSpinInfos);

            BecomeWildStripsProbabilityTable becomeWildStripsProbabilityTable = CreateBecomeWildStripsProbabilityTable (savedFreeSpinInfos);
            reelManager.isFreespinBonus = true;
            //reelManager.FreespinCount = 0;
            AwardAdded = false;
//            reelManager.AutoRun = true;
            if (freeSpinGameConfigs != null) {
                isGameConfigDataiIsDirty = true;
            }

            if (isGameConfigDataiIsDirty) {
                reelManager.ChangeGameConfigs (symbolMap, reelStrips, freeSpinGameConfigs, paytable,lineTable);
            }
			this.reelManager = reelManager;	

			#region Restore FreeSpin
			if (isRestoreFreespin&&currentSpinFinished) {
				isRestoreFreespin = false;
				this.reelManager.resultContent.ChangeResult (reelManager.freeSpinResult);
				this.reelManager.ChangeSymbols (reelManager.freeSpinResult);
			}
			#endregion

			return moreAward;
        }

        public override void OnQuitGame (ReelManager reelManager)
        {
            base.OnQuitGame (reelManager);
            ChangeDataOnQuitGame (reelManager);
            isAdFreeSpinCountAdd = false;
	        //新流程处理需要重置为0
	            AwardInfo.awardValue = 0;
            //Libs.SoundManager.Instance.Pause ();
			if (!reelManager.EnableInitBackgroundAudio) {
				Libs.AudioEntity.Instance.StopAllAudio();

				if (BaseSlotMachineController.Instance.slotMachineConfig.hasSpecialSlotSound) {
					//Libs.SoundManager.Instance.Play (Application.loadedLevelName);
					Libs.AudioEntity.Instance.PlayMuisc (SceneManager.GetActiveScene ().name);
				}
			}
			Messenger.RemoveListener(AdFreespinCountAdd,AddFreeCount);
			if (!reelManager.AutoRun)
			{
				Messenger.Broadcast(global::SpinButtonStyle.CHANGEBUTTONIMAGEONQUITFREE);
			}

        }

        public virtual void ChangeDataOnQuitGame(ReelManager reelManager){
            reelManager.isFreespinBonus = false;
            reelManager.freespinFinished = true;
//            reelManager.AutoRun = false;
            if (isGameConfigDataiIsDirty) {
                reelManager.ChangeGameConfigs (savedSymbolMap, savedReelStrips, normalGameConfigs, savedPaytable,savedLineTable);
            }

			SpinClickedMsgMgr.Instance.RestroeBaseGameOutAfterExitFreespin();
        }

		public virtual BaseAward AddMore (int time)
        {
			BaseAward moreAward = new BaseAward ();
            this.TotalTime += time;
            this.LeftTime += time;
			HaveAddedTimes = true;
			if (!Mathf.Approximately(scatterPay, 0))
			{
				moreAward.awardName = "InFreespinScatter";
				moreAward.awardValue = scatterPay;
			}

			reelManager.SetIsAddFreeSpinCount(true);
			OnFreespinTimesChanged();
            return moreAward;
        }

        public override void OnSpin ()
        {
	        
            if (!BaseSlotMachineController.Instance.onceMore) {
	            
	            LeftTime--;
	            if (LeftTime<0)
	            {
		            LeftTime = 0;
	            }
	            HaveAddedTimes = false;
				HaveInitedFreeSpinCount = false;
//				SpinFinished = false;
				#region Restore FreeSpin
				currentSpinFinished = false;
				#endregion
				OnFreespinTimesChanged();
            }
        }

        /// <summary>
        /// 从服务器接收PaidResult的FreeSpinMultiplier
        /// </summary>
        /// <returns>The free spin multiplier.</returns>
        public virtual int GetFreeSpinMultiplier(){
            return SequencePaidResultsManager.FreeSpinMultiplierFromForceOutcome();
        }

        public virtual void OnSpinForTest ()
        {
        }

        public virtual void PlayWinFreespinAnimation(System.Action callBack){
            reelManager = BaseSlotMachineController.Instance.reelManager;
            AwardResult.AwardPayLine bounusAwardLine = new AwardResult.AwardPayLine (-1);
			AwardLineElement awardLineElement = new AwardLineElement (bounusAwardLine);
            awardLineElement.awardElements = reelManager.GetFreespinSymbols ();
            if (awardLineElement.awardElements.Count > 0) {
				bounusAwardLine.CreateDefaultAnimations ((int)BaseElementPanel.AnimationID.AnimationID_BonusTriggered,awardLineElement.awardElements.Count);
				PlayBonusTriggerExtraAnimation (awardLineElement.awardElements);
                reelManager.awardElements.AddRange (awardLineElement.awardElements);
				reelManager.resultContent.awardResult.awardInfos.Add (bounusAwardLine);
                reelManager.awardLines.Add (awardLineElement);
            }
            DelayAction delayAction = new DelayAction (GetTileAnimationDurationTime(),
                () =>
                {
	                PlayFreeSpinTipAnimation(reelManager);
                }, 
				()=>{
					StopBonusTriggerExtraAnimation();
					callBack?.Invoke();
				});
            // delayAction.Play ();
            new DelayAction(BaseSlotMachineController.Instance.reelManager.gameConfigs.smartSoundSymbolAnimationDuration,null,delegate()  // --暂时解决快停动画消失问题
            {
                delayAction.Play ();
            }).Play();
        }

        public virtual float GetTileAnimationDurationTime()
        {
	        return reelManager.gameConfigs.TileAnimationDuration;
        }

        protected virtual void PlayFreeSpinTipAnimation(ReelManager reelManager)
        {
	        BaseSlotMachineController.Instance.PlaySymbolAnimation ();
	        PlayWinFreeSpinAudio();
	        AudioEntity.Instance.PauseBackGroundAudio(AudioEntity.audioSpin, 3f);
        }

		public void PlayBonusTriggerExtraAnimation(List<BaseElementPanel> awardElements){
			if (BaseSlotMachineController.Instance!=null) {
				reelManager = BaseSlotMachineController.Instance.reelManager;
				if (reelManager.bonusTriggerAnimations!=null&&awardElements!=null) {
					for (int i = 0; i < awardElements.Count; i++) {
						int reelIndex = awardElements [i].ReelIndex;
						if (reelIndex < reelManager.bonusTriggerAnimations.Count) {
							if (reelManager.bonusTriggerAnimations[reelIndex]!=null) {
								reelManager.bonusTriggerAnimations [reelIndex].SetActive (true);
							}
						}
					}
				}
			}

		}

		public void StopBonusTriggerExtraAnimation(){
			if (BaseSlotMachineController.Instance!=null) {
				reelManager = BaseSlotMachineController.Instance.reelManager;
				if (reelManager.bonusTriggerAnimations!=null&&reelManager.bonusTriggerAnimations.Count > 0) {
					for (int i = 0; i < reelManager.bonusTriggerAnimations.Count; i++) {
						if (reelManager.bonusTriggerAnimations[i]!=null) {
							reelManager.bonusTriggerAnimations [i].SetActive (false);
						}
					}
				}
			}
		}
        public virtual void PlayWinFreeSpinAudio(){

            Libs.AudioEntity.Instance.PlayBonusTriggerEffect ();
        }

        public override BaseAward GetAwardResult ()
        {
            return AwardInfo;
        }

        public override void PlayAnimation ()
        {
            base.PlayAnimation ();
        }

        public override void OnSpinEnd ()
        {
//	        SpinFinished = true;
        }

        public virtual void AddAwardAnimation (AwardResult awardResult,ReelManager reelManager)
        {
            List<BaseElementPanel> freespinElements = reelManager.GetFreespinSymbols ();
            foreach (BaseElementPanel e in freespinElements) {
                if (!reelManager.awardElements.Contains (e)) {
                    reelManager.awardElements.Add (e);
                }
                awardResult.AddAwardElementIndexOfDisplay (e.ReelIndex, e.PositionId);
            }
        }
        public virtual bool IsSuperFreeSpin()
        {
	        return false;
        }

        public virtual bool HaveReTrigger()
        {
	        return true;
        }

        public virtual void showFirstWinFreespin (int times, System.Action callback)
        {
			PlayEnterAudio ();
            DelayAction delayAction = new DelayAction (0.3f, null, BaseSlotMachineController.Instance.StopAllAnimation);
            delayAction.Play ();
			Messenger.Broadcast<int,System.Action,bool,bool> (GameDialogManager.OpenAutoSpinWinDialog, times, () => {
                callback ();
			}, true,false);
        }

		public virtual void PlayEnterAudio()
        {
			// if (BaseSlotMachineController.Instance.reelManager.EnableInitBackgroundAudio) {
			// 	Libs.DelayAction da = new DelayAction (InFreeSpinMusicDuration,()=>{
			// 		BaseSlotMachineController.Instance.reelManager.StopBaseGameMusic();
			// 		if (enableInFreespinAudioEffect) {
			// 			Libs.AudioEntity.Instance.PlayMuisc(IN_FREESPIN_AUDIO,false);
			// 		}
			// 	},
			// 		()=>{
			// 			Libs.AudioEntity.Instance.StopMusicAudio(IN_FREESPIN_AUDIO);
			// 			Libs.AudioEntity.Instance.PlayFreeSpinBackgroundMusic();
			// 		});
			// 	da.Play ();
			// }
		}
		public void PlayExitAudio(){
			if (BaseSlotMachineController.Instance.reelManager.EnableInitBackgroundAudio) {
				Libs.DelayAction da = new DelayAction (OutFreeSpinMusicDuration,()=>{
					Libs.AudioEntity.Instance.StopFreeSpinBackgroundMusic();
					if (enableOutFreespinAudioEffect){
						Libs.AudioEntity.Instance.PlayMuisc(OUT_FREESPIN_AUDIO,false);
					}
				},
					()=>{
						Libs.AudioEntity.Instance.StopMusicAudio(OUT_FREESPIN_AUDIO);
					});
				da.Play ();
			}
		}
	

        public virtual void showNextWinFreespin (int times, System.Action callback)
        {
            DelayAction delayAction = new DelayAction (1f, null, BaseSlotMachineController.Instance.StopAllAnimation);
            delayAction.Play ();
			AudioEntity.Instance.PlayFreeSpinAddEffect ();
			Messenger.Broadcast<int,System.Action,bool> (GameDialogManager.OpenAutoSpinAdditionalDialog, times, () => {
                callback ();
			},false);
        }

        public virtual void KeepLastGameStateResult(List<List<int>> lastReuslt){
            
        }
		
        public virtual void showWinDialog (System.Action callback)
        {
	        long awardShow = (long)AwardInfo.awardValue;
	        int awardCash = AwardInfo.awardCashValue;
	        if (BaseSlotMachineController.Instance.reelManager.IsNewProcess)
	        {
		        awardShow = BaseSlotMachineController.Instance.winCoinsForDisplay;
		        awardCash = BaseSlotMachineController.Instance.winCashForDisplay;
	        }
			ShowWinDialogHandler(BaseSlotMachineController.Instance.reelManager.gameConfigs.TileAnimationDuration,awardShow,callback,awardCash);
        }

        /// <summary>
        /// Shows the window dialog handler.
        /// </summary>
        /// <param name="delayTime">Delay time.</param>
        /// <param name="winCoins"> FreeSpin Win coins.</param>
        /// <param name="callback">Callback.</param>
		protected virtual void ShowWinDialogHandler(float delayTime,long winCoins,System.Action callback,int cash){
            DelayAction delayAction = new DelayAction(delayTime, null,
               () => {
                   PlayExitAudio();
                   Dictionary<string, object> data = new Dictionary<string, object>();
                   data[GameConstants.WinCoins_key] = winCoins;
                   data[GameConstants.WinCash_key] = cash;
                   data[GameConstants.FreeSpinCount_key] = TotalTime;
	               Messenger.Broadcast<Dictionary<string,object>, System.Action,bool>(GameDialogManager.OpenFreeSpinEndDialog, data, () => {
                       callback();
                   },false);
               });

            delayAction.Play();
        }

        protected SymbolMap CreateSymbolMap (Dictionary<string,object> dict)
        {
            if (dict!=null && dict.ContainsKey (SymbolMap.SYMBOL_MAP)) {
                isGameConfigDataiIsDirty = true;
                return new SymbolMap (dict [SymbolMap.SYMBOL_MAP] as Dictionary<string,object>);
            } else {
                return null;
            }
        }

        protected virtual ReelStripManager CreateReelStrips (Dictionary<string,object> dict, SymbolMap symbolMap)
        {
			if (dict!=null && symbolMap != null && dict.ContainsKey (ReelStripManager.MACHINE_MATH_NODE)) {
                isGameConfigDataiIsDirty = true;
				return new ReelStripManager (symbolMap, dict [ReelStripManager.MACHINE_MATH_NODE] as Dictionary<string,object>);
            } else {
                return null;
            }
        }

        protected ClassicPaytable CreatePaytable (Dictionary<string,object> dict, SymbolMap symbolMap)
        {
            if (dict!=null && symbolMap != null && dict.ContainsKey (ClassicPaytable.PAY_TABLE)) {
                isGameConfigDataiIsDirty = true;
                return new ClassicPaytable (symbolMap, dict [ClassicPaytable.PAY_TABLE] as List<object>, null);
            } else {
                return null;
            }
        }


        protected SmartSoundReelStripsController CreateSmartSoundReelStripsController (Dictionary<string,object> dict, SymbolMap symbolMap)
        {
            if (dict!=null && symbolMap != null && dict.ContainsKey (SmartSoundReelStripsController.SMART_SOUND_REEL_STRIPS_CONFIG)) {
                isGameConfigDataiIsDirty = true;
                return new SmartSoundReelStripsController (symbolMap, dict [SmartSoundReelStripsController.SMART_SOUND_REEL_STRIPS_CONFIG] as List<object>);
            } else {
                return null;
            }
        }
        protected BecomeWildStripsProbabilityTable CreateBecomeWildStripsProbabilityTable (Dictionary<string,object> dict)
        {
            if (dict!=null  && dict.ContainsKey (BecomeWildStripsProbabilityTable.BECOME_WILD_STRIPS_PROBABILITY_TABLE)) {
                isGameConfigDataiIsDirty = true;
                return new BecomeWildStripsProbabilityTable ( dict [BecomeWildStripsProbabilityTable.BECOME_WILD_STRIPS_PROBABILITY_TABLE] as List<object>);
            } else {
                return null;
            }
        }

        protected LineTable CreateLineTable (Dictionary<string,object> dict)
        {
	        LineTable lineTable = null;
			if (dict!=null && dict.ContainsKey (SlotMachineConfigParse.SLOT_MACHINE_LINE_TABLE_NAME_KEY)) 
			{
				string lineTableName = Utilities.GetValueOrDefault<string> (dict, SlotMachineConfigParse.SLOT_MACHINE_LINE_TABLE_NAME_KEY);
				//先检索小plist
				if (BaseSlotMachineController.Instance != null && BaseSlotMachineController.Instance.slotMachineConfig != null)
				{
					lineTable = BaseSlotMachineController.Instance.slotMachineConfig.GetLineTable(lineTableName);
				}

                //测试数据需要切换lineTable
                if (TestController.Instance != null && TestController.Instance.classicMachineConfig != null)
				{
					lineTable = TestController.Instance.classicMachineConfig.GetLineTable(lineTableName);
				}

				//小plist没有的话，再去大plist里找
				if (lineTable == null)
				{
					Plugins.ConfigurationParseResult result = Plugins.Configuration.GetInstance ().ConfigurationParseResult ();
					lineTable = result.GetLineTable (lineTableName);
				}
            }

            if (lineTable != null) {
                isGameConfigDataiIsDirty = true;
            }
            return lineTable;
        }
        
        public virtual bool GetForceRemmoveSpinResult()
        {
	        return isForceRemoveSpinResult;
        }
        public virtual void SetForceRemoveSpinResult(bool isRemove)
        {
	        isForceRemoveSpinResult = isRemove;
        }

        public virtual TriggerFeatureType GetTriggerFeatureType()
        {
	        return TriggerFeatureType.Symbol;
        }
    }
}
