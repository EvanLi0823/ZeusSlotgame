using System.Collections.Generic;

namespace Libs
{
    public class CollectSpinCountTask:BaseTask
    {
        public CollectSpinCountTask(Dictionary<string, object> taskInfoDict, BaseTask parentTask) : base(taskInfoDict, parentTask)
        {
            Messenger.AddListener(SlotControllerConstants.OnSpinEnd,UpdateSpinCount);
        }

        ~CollectSpinCountTask()
        {
            Messenger.RemoveListener(SlotControllerConstants.OnSpinEnd,UpdateSpinCount);
        }

        void UpdateSpinCount()
        {
            if (IsTaskConditionOK)
            {
                return;
            }
            HasCollectNum++;
            UpdateTaskStatus();
            Messenger.Broadcast(UpdateTaskDataMsg);
        }

        public override string GetDesc()
        {
            return "SpinNumTimes";
        }
    }
}