using System;
using System.Collections.Generic;
using UnityEngine;
using Utils;

namespace Libs
{
    [Serializable]
    public class AccumulateCashTask:BaseTask
    {
        public AccumulateCashTask(Dictionary<string, object> taskInfoDict, BaseTask parentTask) : base(taskInfoDict, parentTask)
        {
            Debug.Log($"[AccumulateCashTask][AccumulateCashTask]  taskId:{TaskId}");
            if (TaskId>0)
            {
                Messenger.AddListener(SlotControllerConstants.OnCashChangeForDisPlay,UpdateTaskNum);
            }
            TargetNum *= OnLineEarningMgr.Instance.GetCashMultiple();
            HasCollectNum = OnLineEarningMgr.Instance.Cash();
            UpdateTaskStatus();
            //hasCollectNum 因为存储的本来就是扩大的倍数所以无需特殊处理
        }

        ~AccumulateCashTask()
        {
            Messenger.RemoveListener(SlotControllerConstants.OnCashChangeForDisPlay,UpdateTaskNum);
        }
        private void UpdateTaskNum()
        {
            // Debug.Log($"[AccumulateCashTask][UpdateTaskStatus]  taskId:{TaskId}");
            HasCollectNum = OnLineEarningMgr.Instance.Cash();
            base.UpdateTaskStatus();
            //广播刷新任务
            Messenger.Broadcast(UpdateTaskDataMsg);
        }

        public override void Clone(BaseTask task)
        {
            base.Clone(task);
            HasCollectNum = Math.Max(OnLineEarningMgr.Instance.Cash(), task.HasCollectNum);
            UpdateTaskStatus();
        }
    }
}