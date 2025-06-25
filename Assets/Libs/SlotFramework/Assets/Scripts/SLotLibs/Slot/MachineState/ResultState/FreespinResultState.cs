using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Libs;
using SevenZip.Compression.LZMA;

namespace Classic
{
    public class FreespinResultState : ResultState
    {
        int times = 0;
		public List<List<int>> lastReuslt = new List<List<int>> ();
		string lastRName = "";
		string lastAName ="";
		int multiplier =1;

		public FreespinResultState (int times,int multiplier)
        {
            this.times = times;
			this.multiplier = multiplier;
        }

        #region Restore Freespin

        public void AddRestoreMethod()
        {
            action = new BaseActionNormal();//将默认的run方法剔除
            action.AutoPlayNextAction = false;
            action.PlayCallBack.AddStartMethod(Restore);
        }

        int leftCount=0;
		int totalCount=0;
		long currentWinCoins = 0;
		//当前获得的现金奖励
		int  currentWinCash = 0;
		bool retrigger = false;
		//当前是否需要展示 cash
		bool showCash = false;
		public FreespinResultState(int leftCount,int totalCount,long currentWin,int multiplier, bool retrigger = false,int currentCash = 0,bool showCash = false){
			this.leftCount = leftCount;
			this.totalCount = totalCount;
			this.multiplier = multiplier;
			this.currentWinCoins = currentWin;
			this.retrigger = retrigger;
			this.currentWinCash = currentCash;
			this.showCash = showCash;
			AddRestoreMethod ();
		}
	    
		public void Restore(){
			base.Init ();// 对action进行绑定事件还原，使其执行run方法
			if (ResultStateManager.Instante.slotController.reelManager.isFreespinBonus) {
				//还原直接存储到reelManager里，正常运行会记录状态
				lastReuslt = ResultStateManager.Instante.slotController.reelManager.baseGameResult;

				ResultStateManager.Instante.slotController.freespinGame = ResultStateManager.Instante.slotController.reelManager.FreespinGame;
				ResultStateManager.Instante.slotController.freespinGame.RestoreFreespin (this.leftCount,this.totalCount,this.currentWinCoins,ResultStateManager.Instante.slotController.slotMachineConfig.extroInfos.GetSubInfos(FreespinGame.FreespinSlotMachine), OnFreespinEnd,this.multiplier);

				ResultStateManager.Instante.slotController.freespinGame.OnEnterGame (ResultStateManager.Instante.slotController.reelManager);
				//ResultStateManager.Instante.slotController.freespinGame.KeepLastGameStateResult (lastReuslt);
				ResultStateManager.Instante.slotController.freespinGame.ChangerDateOnEnterGame(ResultStateManager.Instante.slotController.reelManager);
				if (showCash)
				{
					//显示金钱的文本
					Messenger.Broadcast<bool>(SlotControllerConstants.RefreshCashState,showCash);
					//刷新底部金钱文本显示
					Messenger.Broadcast<float, int, int>(SlotControllerConstants.OnChangeCashText, 1f, 0, BaseSlotMachineController.Instance.winCashForDisplay);
				}
				Messenger.Broadcast<float, long, long>(SlotControllerConstants.OnChangeWinText, 1f, 0, BaseSlotMachineController.Instance.winCoinsForDisplay);
				Messenger.Broadcast (SlotControllerConstants.OnBlanceChangeForDisPlay);
				Messenger.Broadcast (SlotControllerConstants.OnEnterFreespin);
				ResultStateManager.Instante.AddResultOut(true);

				if(retrigger && ResultStateManager.Instante.slotController.reelManager.FreespinCount > 0)
				{
					this.times = ResultStateManager.Instante.slotController.reelManager.FreespinCount;
					PlayWinFreespinAnimation (showNextWinFreespin);
				}else
				{
					DelayAction dalayRoll = new DelayAction (0.5f,null,()=>{
						End ();
					});
					dalayRoll.Play ();//当玩家进入到游戏后，再旋转，给其一个延时处理
				}
			}
		}

        #endregion
        public override void Init()
        {
            action = new BaseActionNormal();
            action.AutoPlayNextAction = false;
            action.PlayCallBack.AddStartMethod(Run2);
        }

        //freespin 开始时进行的操作，打开 freespinstart/freespinretrigger弹窗，
        public  void Run2 ()
        {
            base.Run ();
			bool isSuper;
            if (ResultStateManager.Instante.slotController.reelManager.isFreespinBonus) {
	            //在 free中又触发了free
				if (ResultStateManager.Instante.slotController.reelManager.IsNewProcess)
	            {
		            float time = 0;
		            ReelManager rm = ResultStateManager.Instante.slotController.reelManager;
		            if (rm.ResultData.LineValue > 0) time = rm.gameConfigs.TileAnimationDuration;
		            if (rm.SpinUseNetwork && rm.awardLines.Count > 0)
		            {
			            time = rm.gameConfigs.TileAnimationDuration;
		            }
		            new DelayAction(time, null, delegate
		            {
			            BaseSlotMachineController.Instance.StopAllAnimation();
			            PlayWinFreespinAnimation (showNextWinFreespin);
			            ResultStateManager.Instante.slotController.StopAwardSymbolAnimation();
		            }).Play();	
	            }else
				{
					PlayWinFreespinAnimation (showNextWinFreespin);
				}
				isSuper = ResultStateManager.Instante.slotController.freespinGame.IsSuperFreeSpin();
				if(ResultStateManager.Instante.slotController.freespinGame.HaveReTrigger())
					SpinResultProduce.AddProvider(new SpinResultFreeTrigger(ResultStateManager.Instante.slotController.freespinGame.GetTriggerFeatureType(),times,null,true,isSuper));
            } else {
				lastReuslt.Clear ();
				if (!BaseSlotMachineController.Instance.reelManager.SpinUseNetwork)
				{
					lastRName = ResultStateManager.Instante.slotController.reelManager.reelStrips.GetCurrentUseRName ();
					lastAName = ResultStateManager.Instante.slotController.reelManager.reelStrips.GetCurrentUseAName ();
				}
				for (int i=0; i< ResultStateManager.Instante.slotController.reelManager.resultContent.ReelResults.Count; i++) 
				{
					List<int> temp = new List<int> ();
					temp.AddRange (ResultStateManager.Instante.slotController.reelManager.resultContent.ReelResults [i].SymbolResults);
					lastReuslt.Add (temp);
				}
				ResultStateManager.Instante.slotController.reelManager.baseGameResult = lastReuslt;//用于恢复数据
//				ResultStateManager.Instante.slotController.reelManager.baseGameRName = lastRName;
//				ResultStateManager.Instante.slotController.reelManager.baseGameAName = lastAName;
				ResultStateManager.Instante.slotController.freespinGame = ResultStateManager.Instante.slotController.reelManager.FreespinGame;
				ResultStateManager.Instante.slotController.freespinGame.InitFreespin (times, ResultStateManager.Instante.slotController.slotMachineConfig.extroInfos.GetSubInfos(FreespinGame.FreespinSlotMachine), OnFreespinEnd,multiplier);
				
//				2020 0105 enter fs 结算
	            if (ResultStateManager.Instante.slotController.reelManager.IsNewProcess)
	            {
		           if(ResultStateManager.Instante.slotController.reelManager.EnterFreeIsWait)
					{
						float time = 0;
						ReelManager rm = ResultStateManager.Instante.slotController.reelManager;
						if (rm.ResultData.LineValue > 0) time = rm.gameConfigs.TileAnimationDuration;
						if (rm.SpinUseNetwork && rm.awardLines.Count > 0)
						{
							time = rm.gameConfigs.TileAnimationDuration;
						}
						Debug.Log("Enter Free Spin=============>>"+time);
						new DelayAction(time, null, delegate
						{
							BaseSlotMachineController.Instance.StopAllAnimation();
							PlayWinFreespinAnimation (showFirstWinFreespin);
						}).Play();
					}else
					{
						ResultStateManager.Instante.slotController.reelManager.EnterFreeIsWait = true;
						PlayWinFreespinAnimation (showFirstWinFreespin);
					}	
	            }
				else if (ResultStateManager.Instante.slotController.freespinGame.AwardBeforeEnterFS)
				{
					if (ResultStateManager.Instante.slotController.freespinGame.AwardAdded)
					{
						new DelayAction(2f,null, delegate
						{
							PlayWinFreespinAnimation (showFirstWinFreespin);
						}).Play();	
					}
					else
					{
						ResultStateManager.Instante.slotController.CheckResult();
						ResultStateManager.Instante.slotController.freespinGame.AwardAdded = true;
						if (ResultStateManager.Instante.slotController.totalAward> 0)
						{
							ResultStateManager.Instante.slotController.reelManager.AddAwardElements();
							ResultStateManager.Instante.slotController.reelManager.PlayAwardSymbolAnimation ();
						
							//ui显示相关，top和winDisplay的显示
							Messenger.Broadcast (SlotControllerConstants.OnBlanceChangeForDisPlay);
							Messenger.Broadcast<float, long, long>(SlotControllerConstants.OnChangeWinText, 1f, 0, BaseSlotMachineController.Instance.winCoinsForDisplay);
					
							new DelayAction(2f,null, delegate
							{
								ResultStateManager.Instante.slotController.reelManager.StopAllAnimation ();
								ResultStateManager.Instante.slotController.reelManager.ClearAnimationInfos ();
								if (ResultStateManager.Instante.slotController.reelManager.CheckClearWinTextCondition())
								{
									BaseSlotMachineController.Instance.winCoinsForDisplay = 0;
									BaseSlotMachineController.Instance.winCashForDisplay = 0;
									Messenger.Broadcast<long>(SlotControllerConstants.OnChangeWinTextSilence, 0);
									Messenger.Broadcast<int>(SlotControllerConstants.OnChangeCashTextSilence, 0);
								}
							
								PlayWinFreespinAnimation (showFirstWinFreespin);
							}).Play();	
						}
						else
						{
							PlayWinFreespinAnimation(showFirstWinFreespin);
						}
					}
				}
				else
				{
					 PlayWinFreespinAnimation (showFirstWinFreespin);
				}
	            isSuper = ResultStateManager.Instante.slotController.freespinGame.IsSuperFreeSpin();
	            SpinResultProduce.AddProvider(new SpinResultFreeTrigger(ResultStateManager.Instante.slotController.freespinGame.GetTriggerFeatureType(),times,null,false,isSuper));
            }
        }
		
        //freespinstart
        private void showFirstWinFreespin ()
        {
			if(BaseSlotMachineController.Instance != null)
			{
				BaseSlotMachineController.Instance.ReSetSpinAwardState();
			}
            ResultStateManager.Instante.slotController.freespinGame.showFirstWinFreespin (times, () => 
			{
				SpinResultProduce.InternalSend();
                ResultStateManager.Instante.slotController.freespinGame.OnEnterGame (ResultStateManager.Instante.slotController.reelManager);
                ResultStateManager.Instante.slotController.freespinGame.KeepLastGameStateResult (lastReuslt);
				BaseAward moreAward = ResultStateManager.Instante.slotController.freespinGame.ChangerDateOnEnterGame(ResultStateManager.Instante.slotController.reelManager);
				//20200330,新流程不再处理scatter奖励
				if(!ResultStateManager.Instante.slotController.reelManager.IsNewProcess)
				{
					if (moreAward.isAwarded)
					{
						long AwardConis =
							(long) (moreAward.awardValue * BaseSlotMachineController.Instance.currentBetting);
						if (RewardSpinManager.Instance.RewardIsValid() ||
						    !BaseSlotMachineController.Instance.reelManager.CheckClearWinTextCondition())
						{
							BaseSlotMachineController.Instance.winCoinsForDisplay =
								AwardConis + BaseSlotMachineController.Instance.winCoinsForDisplay;
						}
						else
						{
							BaseSlotMachineController.Instance.winCoinsForDisplay = AwardConis;
						}

						Messenger.Broadcast<float, long, long>(SlotControllerConstants.OnChangeWinText, 1f, 0,
							BaseSlotMachineController.Instance.winCoinsForDisplay);
						UserManager.GetInstance().IncreaseBalanceAndSendMessage(AwardConis);
						Dictionary<string, object> dict = new Dictionary<string, object>();
						dict.Add("increaseCoins", AwardConis);
						BaseGameConsole.singletonInstance.LogBaseEvent("TriggerFreespinAward", dict);

						#region Statistics RTP

						if (BaseGameConsole.ActiveGameConsole().IsInSlotMachine() && AwardConis > 0)
						{
							BaseSlotMachineController slot = BaseGameConsole.ActiveGameConsole().SlotMachineController;
							slot.statisticsManager.WinCoinsNum += AwardConis;
						}

						#endregion

//					ResultStateManager.Instante.slotController.freespinGame.AwardInfo.awardValue += AwardConis;
					}
				}
                Messenger.Broadcast (SlotControllerConstants.OnEnterFreespin);
				ResultStateManager.Instante.slotController.freespinGame.OnStartRun(()=>{
//					ResultStateManager.Instante.AddResultOut (true);
					//20191230 尝试新增加最终控制的流程接口,不再放入到队列里面了
					ResultStateManager.Instante.slotController.NormaEndToNextSpin();
//					End ();
				});
            });
        }
        
		//freespinretrigger
        void showNextWinFreespin ()
        {
			if(BaseSlotMachineController.Instance != null)
			{
				BaseSlotMachineController.Instance.ReSetSpinAwardState();
			}
			//通过控制状态，控制在freespin弹框时，reelmanager暂停
			//GameState preState = ResultStateManager.Instante.slotController.reelManager.State;
			ResultStateManager.Instante.slotController.reelManager.State = GameState.PAUSE;
            ResultStateManager.Instante.slotController.freespinGame.showNextWinFreespin (times, () => {
				BaseAward moreAward = ResultStateManager.Instante.slotController.freespinGame.AddMore (times);
				//20200330,新流程不再处理scatter奖励
				if (!ResultStateManager.Instante.slotController.reelManager.IsNewProcess)
				{
					if (moreAward.isAwarded)
					{
						long AwardConis =
							(long)(moreAward.awardValue * BaseSlotMachineController.Instance.currentBetting);
						BaseSlotMachineController.Instance.baseAward += moreAward.awardValue;
						BaseSlotMachineController.Instance.FreeSpinTotalAward += AwardConis;
						BaseSlotMachineController.Instance.winCoinsForDisplay += AwardConis;
						Messenger.Broadcast<float, long, long>(SlotControllerConstants.OnChangeWinText, 1f, 0,
							BaseSlotMachineController.Instance.winCoinsForDisplay);
						UserManager.GetInstance().IncreaseBalanceAndSendMessage(AwardConis);
						Dictionary<string, object> dict = new Dictionary<string, object>();
						dict.Add("increaseCoins", AwardConis);
						BaseGameConsole.singletonInstance.LogBaseEvent("TriggerReFreespinAward", dict);

						#region Statistics RTP

						if (BaseGameConsole.ActiveGameConsole().IsInSlotMachine() && AwardConis > 0)
						{
							BaseSlotMachineController slot = BaseGameConsole.ActiveGameConsole().SlotMachineController;
							slot.statisticsManager.WinCoinsNum += AwardConis;
						}

						#endregion

						ResultStateManager.Instante.slotController.freespinGame.AwardInfo.awardValue += AwardConis;
					}
				}

				ResultStateManager.Instante.slotController.freespinGame.OnResumeRun(()=>{
					ResultStateManager.Instante.slotController.reelManager.State = GameState.READY;
					End ();
				});
			
            });
        }

        public  override void End ()
        {
	        // Messenger.Broadcast<bool>(BaseSlotMachineController.CALCULATE_CASH,false);
            base.End ();
        }

        public void OnFreespinEnd ()
        {	
            showWinDialog ();
        }

        void showWinDialog ()
        {
            ResultStateManager.Instante.slotController.freespinGame.showWinDialog ( () => {
	            ResultStateManager.Instante.slotController.StopAllAnimation (); 
				ResultStateManager.Instante.slotController.reelManager.StopAllSmartAndAntiEffectAnimation();   
                ResultStateManager.Instante.slotController.freespinGame.OnQuitGame (ResultStateManager.Instante.slotController.reelManager);
                ResultStateManager.Instante.slotController.reelManager.RecoveryBaseGameData(lastRName,lastAName,lastReuslt);
	            ResultStateManager.Instante.slotController.addLastWinCoinsToDisplay = true;
				ResultStateManager.Instante.slotController.isReturnFromFreespin = true;
                Messenger.Broadcast (SlotControllerConstants.OnQuitFreespin);
                //退出 freespin，广播关闭计算翻倍cash的状态
				Messenger.Broadcast<bool>(BaseSlotMachineController.CALCULATE_CASH,false);
				//恢复背景音乐
				Libs.AudioEntity.Instance.UnPauseBackGroundAudio(Libs.AudioEntity.audioSpin);
                if (ResultStateManager.Instante.slotController.reelManager.IsNewProcess)
                {
					//ResultOut -> fs end -> ResultOut
	                ResultStateManager.Instante.AddResultOut(true);
                }
                else if(ResultStateManager.Instante.slotController.freespinGame.AwardBeforeEnterFS)
				{
					BaseSlotMachineController.Instance.SendSpinEvent();
					ResultStateManager.Instante.AddResultOut(true);
				}
                else
                {
	                ResultStateManager.Instante.AddNormal (true);
                }

                if(ResultStateManager.Instante.slotController.hasLevelUp){
	                if (!ResultStateManager.Instante.slotController.reelManager.IsNewProcess)
	                {
		                ResultStateManager.Instante.slotController.hasLevelUp = false;
	                }
                }
                End ();
            });
        }

        void PlayWinFreespinAnimation(System.Action callBack){
	        
            BaseSlotMachineController.Instance.reelManager.ChangeMiddleMessage ("BONUS TRIGGERED");
            ResultStateManager.Instante.slotController.freespinGame.PlayWinFreespinAnimation (callBack);
        }

    }
}
