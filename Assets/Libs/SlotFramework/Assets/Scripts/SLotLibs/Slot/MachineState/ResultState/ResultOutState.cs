using UnityEngine;
using System.Collections;
using RealYou.Core.UI;
using Libs;

namespace Classic
{
	public class ResultOutState : ResultState
	{
        public override void Init()
        {
            action = new BaseActionNormal();
            action.AutoPlayNextAction = false;
            action.PlayCallBack.AddStartMethod(Run2);
        }
        public async void Run2()
		{
			base.Run ();
			ReelManager reelManager = ResultStateManager.Instante.slotController.reelManager;
			if(reelManager == null)
				return;
            if (reelManager.isFreespinBonus && ResultStateManager.Instante.slotController.freespinGame != null) {
				ResultStateManager.Instante.slotController.freespinGame.OnSpinEnd ();
			}

            if (reelManager.isFreespinBonus && 
                ResultStateManager.Instante.slotController.freespinGame.LeftTime == 0 && 
                ResultStateManager.Instante.slotController.onceMore == false) {
	           
				ResultStateManager.Instante.slotController.freespinGame.OnGameEnd ();
            } else {
                if (ResultStateManager.Instante.slotController.onceMore) {
	                reelManager.NeedReCreatResult = false;
                    ResultStateManager.Instante.slotController.addLastWinCoinsToDisplay = true;
                    reelManager.OpenRewinDialog();
				} else {
//	                20191223 尝试新增加最终控制的流程接口
	                Messenger.Broadcast<double>(SlotControllerConstants.OnRefreshRewardSpin,
		                BaseSlotMachineController.Instance.winCoinsForDisplay);
	                //为了新流程统计准确，老流程不发送
	                if(reelManager.IsNewProcess)
	                {
		                bool isFsAward = RewardSpinManager.Instance.CheckEndCondition(); // 是否是由发送fs导致的
		                if(!reelManager.SpinUseNetwork)
		                {
			                reelManager.SpinEndSendEs(isFsAward);
		                }
		                await RewardSpinManager.Instance.CheckRewardSpinEnd();
	                }

	                reelManager.BeforeNextSpinHandler(NormalSpinEnd);
				}
			}
            //ggggg
            if (!SpinResultProduce.Instance.NeedDelaySend)
            {
                SpinResultProduce.InternalSend();
            }
		}

		async void NormalSpinEnd ()
		{
			ReelManager reelManager = ResultStateManager.Instante.slotController.reelManager;
			if (reelManager.IsChangeBet())
			{
				await MachineUtility.Instance.InvokeSpinEndEvent();
			}

			
			ResultStateManager.Instante.slotController.NormaEndToNextSpin();
			
//			if (ResultStateManager.Instante.slotController.reelManager.AutoRun || ResultStateManager.Instante.slotController.isFreeRun) {
//				DelayAction delayAction = new DelayAction (1f, null, ResultStateManager.Instante.slotController.ForAutoSpin);
//				delayAction.Play ();
//			}
//			if (!(ResultStateManager.Instante.slotController.reelManager.AutoRun|| ResultStateManager.Instante.slotController.isFreeRun)) {
//				//此判断为解决EpicWin弹窗后Bet按钮必定变为有效的问题而设置
//				if (ResultStateManager.Instante.slotController.reelManager.enableBetChangeAfterEpicWin) {
//					Messenger.Broadcast<bool> (SlotControllerConstants.ActiveButtons,false);
//				}
//			}	
//			BaseSlotMachineController slotController = ResultStateManager.Instante.slotController;
//			slotController.spinning = false;
//			slotController.addLastWinCoinsToDisplay = false;
//			
//			if (!(slotController.isFreeRun || slotController.GetIsSpining() || (slotController.reelManager != null && (!slotController.reelManager.IsStartSpin()))))
//			{
//				Messenger.Broadcast(GameConstants.NEXT_REWARD_HANDLER);//弹下一个中奖框
//			}

		}
		
		public override void End ()
		{
			base.End ();
		}
	}
}
