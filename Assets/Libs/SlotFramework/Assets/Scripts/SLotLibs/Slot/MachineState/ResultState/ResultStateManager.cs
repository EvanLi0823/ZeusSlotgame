using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Libs;

namespace Classic
{
	public class ResultStateManager
	{

        ActionSequence resultStateAction = new ActionSequence ();

        public static void InitInstane (BaseSlotMachineController slotController)
		{
			Instante = new ResultStateManager (slotController);
		}

		public static ResultStateManager Instante {
			get;
			private set;
		}

        public BaseSlotMachineController slotController;

        private ResultStateManager (BaseSlotMachineController slotController)
		{
			this.slotController = slotController;
		}
/// <summary>
/// 原有的老的流程处理方式
/// </summary>
		public void PlayAwardAfterFeature ()
		{
            ResultStateManager.Instante.CurrentStateOver = true;
            resultStateAction.RemoveAll ();
            AddResultIn ();
            AddBonusGame ();
			AddFreespin (slotController.reelManager.FreespinCount, false, false);
            AddNormal ();
            resultStateAction.Play ();
		}
		/// <summary>
		/// 新的流程处理方式
		/// </summary>
		public void PlayAwardBeforeFeature ()
        {
	        resultStateAction.RemoveAll();
	        //只需要播放动画即可
	        AddNormal();
	        if (slotController.hasPopReward)
	        {
		        ResultStateManager.Instante.AddPopReward(true);
		        slotController.hasPopReward = false;
	        }

	        AddBonusGame();
	        AddFreespin(slotController.reelManager.FreespinCount, false, false);
	        AddResultOut();
	        resultStateAction.Play();

        }

		public void Play()
		{
			resultStateAction.Play();
		}

		public void RemoveAll(){
            resultStateAction.RemoveAll();
        }

        public void PlayNext(){
            resultStateAction.PlayNextChildAction ();
        }

        public void AddState(BaseActionNormal stateAction, bool addAfterCurrentstate = false){
            if (addAfterCurrentstate) {
                resultStateAction.AddAfterCurrent (stateAction);
            } else {
                resultStateAction.AppendAction (stateAction);
            }
        }

		public bool CurrentStateOver = false;

        public void AddNormal (bool addAfterCurrentstate = false)
		{
            AddState(new NormalResultState ().action, addAfterCurrentstate);
		}

		public void AddFreespin (int freespinTime, bool isRestore, bool addAfterCurrentstate,int multiplier = 1)
        {
            if (freespinTime > 0) {
                slotController.reelManager.FreespinCount = freespinTime;
				AddState(new FreespinResultState (freespinTime,multiplier).action, addAfterCurrentstate);
            }
		}

        public void AddBonusGame (bool addAfterCurrentstate = false)
		{
			if (slotController.reelManager.HasBonusGame) {
				AddState (new BonusGameResult ().action, addAfterCurrentstate);
			}
		}

		#region Restore Game
		public void RestoreFreespin(int leftCount,int totalCount,long currentWin,bool addAfterCurrentstate = false,int multiplier = 1,bool actionStateNeedStart = true,bool forceLeftTimeGreaterZero = true, bool retrigger = false,int curWinCash = 0,bool needShowCash = false){
			if (!forceLeftTimeGreaterZero||leftCount > 0) {
				AddState (new FreespinResultState(leftCount,totalCount,currentWin,multiplier,retrigger,curWinCash,needShowCash).action,addAfterCurrentstate);
				if (actionStateNeedStart) {
					resultStateAction.Play ();//从bonus进入时，不需要执行此方法，否则会执行两遍
				}
			}
		}
		public void RestoreFreespinOnBaseGame(int freespinCount,bool addAfterCurrentstate = false,int multiplier = 1){
			if (freespinCount>0) {
				AddState (new FreespinResultState(freespinCount,multiplier).action,addAfterCurrentstate);
				resultStateAction.Play ();
				Messenger.Broadcast<long>(SlotControllerConstants.OnChangeWinTextSilence, BaseSlotMachineController.Instance.winCoinsForDisplay);//奖励的win框恢复
			}
		}
		public void RestoreBonusGame(bool addAfterCurrentstate = false){
			AddState (new BonusGameResult (true).action, addAfterCurrentstate);
			resultStateAction.Play ();
		}
		public void RestoreBonusGameOnBaseGame(bool addAfterCurrentstate = false){
			AddState (new BonusGameResult (true).action, addAfterCurrentstate);
			resultStateAction.Play ();
		}
		public void ExcuteRestoreState(){
			resultStateAction.Play ();
		}

		//考虑到退出时，可能处于extraAward状态，所以需要重新计算baseGame结果值
		public void RestoreNormalGame(bool addAfterCurrentstate = false){
			AddState(new NormalResultState ().action, addAfterCurrentstate);
			resultStateAction.Play ();
		}
		#endregion

        public void AddResultIn (bool addAfterCurrentstate = false)
		{
            AddState(new ResultInState ().action, addAfterCurrentstate);
		}

        public void AddResultOut (bool addAfterCurrentstate = false)
		{
            AddState(new ResultOutState ().action, addAfterCurrentstate);
			ResultStateManager.Instante.CurrentStateOver = false;
		}

        public void AddPopReward (bool addAfterCurrentstate = false)
        {
	        AddState(new PopRewardState().action, addAfterCurrentstate);
        }
	}
}
