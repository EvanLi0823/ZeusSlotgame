using System;
using System.Collections.Generic;

namespace Libs
{
    [Serializable]
    public class SpinAwardCashTask:BaseTask
    {
        public int cashTargetNum;
        public int cashCollectNum;
        public SpinAwardCashTask(Dictionary<string, object> taskInfoDict, BaseTask parentTask) : base(taskInfoDict, parentTask)
        {
            cashTargetNum = Utils.Utilities.GetInt(taskInfoDict, TaskConstants.TargetCashNum_Key, 0);
            cashCollectNum = Utils.Utilities.GetInt(taskInfoDict, TaskConstants.CollectCashNum_Key, 0);
            //收集现金的任务需要乘以相应的倍数
            cashTargetNum *= OnLineEarningMgr.Instance.GetCashMultiple();
            if (TaskId>0)
            {
                Messenger.AddListener(SlotControllerConstants.OnCashChangeForDisPlay,UpdateCash);
                Messenger.AddListener(SlotControllerConstants.OnSpinEnd,UpdateSpinCount);
            }
        }

        ~SpinAwardCashTask()
        {
            Messenger.RemoveListener(SlotControllerConstants.OnCashChangeForDisPlay,UpdateCash);
            Messenger.RemoveListener(SlotControllerConstants.OnSpinEnd,UpdateSpinCount);
        }

        void UpdateCash()
        {
            if (State == (int)TaskState.CLOSE)
            {
                return;
            }
            cashCollectNum = OnLineEarningMgr.Instance.Cash();
            UpdateTaskStatus();
            Messenger.Broadcast(UpdateTaskDataMsg);
        }
        
        void UpdateSpinCount()
        {
            HasCollectNum++;
        }
        protected override void UpdateTaskStatus()
        {
            IsTaskConditionOK = cashCollectNum >= cashTargetNum;
        }

        public override void Clone(BaseTask task)
        {
            base.Clone(task);
            int savedNum = (task as SpinAwardCashTask).cashCollectNum;
            cashCollectNum = Math.Max(OnLineEarningMgr.Instance.Cash(),savedNum);
            UpdateTaskStatus();
        }
    }
}