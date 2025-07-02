using System;
using System.Collections.Generic;
using CardSystem;
namespace Libs
{
    /// <summary>
    /// 收集新卡牌数量的任务，无论类型
    /// </summary>
    [Serializable]
    public class CollectNewCardCountTask:BaseTask
    {
        public CollectNewCardCountTask(Dictionary<string, object> taskInfoDict, BaseTask parentTask) : base(taskInfoDict, parentTask)
        {
            Messenger.AddListener(CardSystemConstants.GetCardNewCountMsg,UpdateCard);
        }

        ~CollectNewCardCountTask()
        {
            Messenger.RemoveListener(CardSystemConstants.GetCardNewCountMsg,UpdateCard);
        }

        void UpdateCard()
        {
            HasCollectNum++;
            UpdateTaskStatus();
            Messenger.Broadcast(UpdateTaskDataMsg);
        }
        public override void Clone(BaseTask task)
        {
            base.Clone(task);
            UpdateTaskStatus();
        }
        
        public override string GetDesc()
        {
            return "DrawMoreCards";
        }
    }
}