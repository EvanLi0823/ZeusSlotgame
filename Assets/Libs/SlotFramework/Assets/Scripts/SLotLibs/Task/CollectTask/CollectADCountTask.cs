using System.Collections.Generic;

namespace Libs
{
    public class CollectADCountTask:BaseTask
    {
        public CollectADCountTask(Dictionary<string, object> taskInfoDict, BaseTask parentTask) : base(taskInfoDict, parentTask)
        {
            Messenger.AddListener(ADConstants.OnPlayVedioEnd,UpdateAdCount);
        }
        
        ~CollectADCountTask()
        {
            // 这里可以添加清理逻辑，如果有需要的话
            Messenger.RemoveListener(ADConstants.OnPlayVedioEnd,UpdateAdCount);
        }

        void UpdateAdCount()
        {
            if (IsTaskConditionOK)
            {
                return;
            }
            HasCollectNum++;
            UpdateTaskStatus();
            Messenger.Broadcast(UpdateTaskDataMsg);
        }
    }
}