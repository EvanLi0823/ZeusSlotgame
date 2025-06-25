using System;
using System.Collections.Generic;
using Core;
using Libs;
using UnityEngine;

namespace Classic
{
    public class PopRewardState : ResultState
    {
        private int rewardCash = 0;
        public override void Init()
        {
            Messenger.AddListener(GameConstants.ExitPopRewardState,ExitState);
            action = new BaseActionNormal();
            action.AutoPlayNextAction = false;
            action.PlayCallBack.AddStartMethod(Run);
        }

        public override void Run()
        {
            base.Run();
            Messenger.Broadcast(SlotControllerConstants.AUTO_SPIN_SUSPEND);
            OnLineEarningMgr.Instance.AddGetRewardCount();
            // bool canShowBig = OnLineEarningMgr.Instance.CheckCanShowBig();
            // //满足弹出大弹窗条件时，弹出大弹窗，不弹小弹窗
            // if (canShowBig)
            // {
                //获取大弹窗的奖励金钱
                rewardCash = OnLineEarningMgr.Instance.GetLuckyCashReward();
                if (rewardCash<=0)
                {
                    End();
                    return;
                }
                Messenger.Broadcast<int,Action>(SlotGameDialogManager.OpenPopRewardBigDialog,rewardCash,PopBigRewardQuit);
            // }
        }

        private void PopSmallRewardQuit()
        {
            OnLineEarningMgr.Instance.PopSmallDialogEnd();
            End();
        }
        private void PopBigRewardQuit()
        {
            OnLineEarningMgr.Instance.PopBigDialogEnd();
            ExitState();
            //开启morecash弹窗
            // Messenger.Broadcast<int>(SlotGameDialogManager.OpenGetMoreCashDialog,rewardCash);
        }

        public void ExitState()
        {
            Messenger.RemoveListener(GameConstants.ExitPopRewardState,ExitState);
            End();
        }
        
        public override void End()
        {
            Messenger.Broadcast(SlotControllerConstants.AUTO_SPIN_RESUME);
            base.End();
        }
    }
}