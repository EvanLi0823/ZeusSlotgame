using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Libs;
namespace Classic
{
    public class BonusGame : BaseExtraAward
    {
		protected readonly string BONUS_GAME_NAME = "BonusGame";
		[HideInInspector]
		public bool IsBonusGame = false;
        //计算奖励和测试用例，经常被是否乘以系数，将来都不用再乘了
        [Obsolete("新的流程也不用了")]
        [Header("用特定的几个关卡")]
        public bool IsNewResultMode = false;

        [HideInInspector] public bool NeedRmBgAni = true; //是否需要删除掉普通中奖线的动画
		public virtual void InitBonusForTest (Dictionary<string,object> slotConfig = null)
		{
			AwardInfo = new BaseAward ();
			AwardInfo.awardValue = 0;
			AwardInfo.awardName = BONUS_GAME_NAME;
		}

        public virtual void InitBonus (Dictionary<string,object> slotConfig = null, GameCallback onGameOver = null)
        {
			onGameOver += delegate() {
				IsBonusGame = false;
				Messenger.Broadcast(GameConstants.BonusGameIsOver);
			};
			IsBonusGame = true;
			
            this.Init (slotConfig, onGameOver);
            AwardInfo.awardValue = 0;
			AwardInfo.awardName = BONUS_GAME_NAME;
        }

        public override void OnEnterGame (ReelManager reelManager)
		{
            base.OnEnterGame (reelManager);
            AudioEntity.Instance.StopMusicAudio(AudioEntity.audioSpin);
		}

		public virtual void ForTest (long currentBet,ReelManager reelManager)
        {
            AwardInfo = new BaseAward ();
			AwardInfo.awardName = BONUS_GAME_NAME;
            AwardInfo.awardValue = 0;
        }

        public virtual void AddAwardAnimation (AwardResult awardResult,ReelManager reelManager)
        {
            List<BaseElementPanel> freespinElements = reelManager.GetBonusGameSymbols ();
            foreach (BaseElementPanel e in freespinElements) {
                if (!reelManager.awardElements.Contains (e)) {
                    reelManager.awardElements.Add (e);
                }
                awardResult.AddAwardElementIndexOfDisplay (e.ReelIndex, e.PositionId);
            }
        }

        public virtual void PlayWinBonusGameAnimation (System.Action callBack)
        {
            AwardResult.AwardPayLine bounusAwardLine = new AwardResult.AwardPayLine (-1);
			AwardLineElement awardLineElement = new AwardLineElement (bounusAwardLine);
			BaseSlotMachineController.Instance.reelManager.resultContent.awardResult.awardInfos.Add (bounusAwardLine);


            awardLineElement.awardElements = (BaseSlotMachineController.Instance.reelManager).GetBonusGameSymbols ();

			bounusAwardLine.CreateDefaultAnimations ((int)BaseElementPanel.AnimationID.AnimationID_BonusTriggered,awardLineElement.awardElements.Count);
            BaseSlotMachineController.Instance.reelManager.awardElements.AddRange (awardLineElement.awardElements);
            BaseSlotMachineController.Instance.reelManager.awardLines.Add (awardLineElement);
            PlayWinBonusGameAnimationEndHandler(callBack);
         
        }

        public virtual void PlayWinBonusGameAnimationEndHandler(System.Action callBack){
            PlayWinBonusGameAnimationEndDelayHandler(BaseSlotMachineController.Instance.reelManager.gameConfigs.TileAnimationDuration,BaseSlotMachineController.Instance.reelManager.gameConfigs.TileAnimationDuration,callBack);
        }

        protected virtual void PlayWinBonusGameAnimationEndDelayHandler(float delayStartTime,float delayEndTime,System.Action callBack){
            DelayAction delayAction = new DelayAction(delayStartTime,
                                      () => {
                                          BaseSlotMachineController.Instance.PlaySymbolAnimation();
                                          BaseSlotMachineController.Instance.PlayExtraWinAnimation();
                                          PlayBonusTriggerAudio();
                                      }, () => {
                DelayAction delayAction2 = new DelayAction(delayEndTime, null,
                                                     () => {
                                                         BaseSlotMachineController.Instance.StopAllAnimation();
                                                         callBack();
                                                     });
                                          delayAction2.Play();

                                      });
            //delayAction.Play();
            new DelayAction(BaseSlotMachineController.Instance.reelManager.gameConfigs.smartSoundSymbolAnimationDuration,null,delegate()  // --暂时解决快停动画消失问题
            {
                delayAction.Play ();
            }).Play();

        }
        //old,需弃用
        public virtual void PlayWinBonusGameAudio(){
            //Libs.SoundEntity.Instance.Celebration ();
            Libs.AudioEntity.Instance.PlayBonusTriggerEffect ();
        }

        //继承单独处理触发音效
        public virtual void PlayBonusTriggerAudio()
        {

        }
    }
}
