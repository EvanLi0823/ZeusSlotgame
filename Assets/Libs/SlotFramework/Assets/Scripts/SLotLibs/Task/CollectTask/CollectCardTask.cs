using System;
using System.Collections.Generic;
using CardSystem;

namespace Libs
{
    [Serializable]
    public class CollectCardTask:BaseTask
    {
        public CollectCardTask(Dictionary<string, object> taskInfoDict, BaseTask parentTask) : base(taskInfoDict, parentTask)
        {
            HasCollectNum = CardSystemManager.Instance.GetHaveCardTypeCount();
            TargetNum = CardSystemManager.Instance.GetTotalCardTypeCount();
            Messenger.AddListener(CardSystemConstants.GetCardCountMsg,UpdateCard);
        }

        ~CollectCardTask()
        {
            Messenger.RemoveListener(CardSystemConstants.GetCardCountMsg,UpdateCard);
        }

        void UpdateCard()
        {
            HasCollectNum = CardSystemManager.Instance.GetHaveCardTypeCount();
            Messenger.Broadcast(UpdateTaskDataMsg);
        }

        public override void Clone(BaseTask task)
        {
            base.Clone(task);
            HasCollectNum = CardSystemManager.Instance.GetHaveCardTypeCount();
            TargetNum = CardSystemManager.Instance.GetTotalCardTypeCount();
        }
    }
}