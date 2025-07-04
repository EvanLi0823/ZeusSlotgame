using System.Collections.Generic;
using Activity;

namespace Libs
{
    public class TaskFactory
    {
        public static BaseTask CreateTask(Dictionary<string,object> taskInfoDict,BaseTask parentTask = null,int type = GameConstants.LeafTask_Key)
        {
            if (taskInfoDict == null) return null;
            if (taskInfoDict.ContainsKey(TaskConstants.sub_tasks_Key)) type = GameConstants.InternalTask_Key;
            type = Utils.Utilities.GetInt(taskInfoDict, TaskConstants.Type_Key,type);
            BaseTask task = null;
            switch (type)
            {
                case TaskConstants.LeafTask_Key:
                    task = new BaseTask(taskInfoDict, parentTask);
                    break;
                case TaskConstants.SpinAwardCashTask_Key:
                    task = new SpinAwardCashTask(taskInfoDict, parentTask);
                    break;
                case TaskConstants.AccumulateCashTask_Key:
                    task = new AccumulateTotalCashTask(taskInfoDict, parentTask);
                    break;
                case TaskConstants.CollectSpinCountTask_Key:
                    task = new CollectSpinCountTask(taskInfoDict, parentTask);
                    break;
                case TaskConstants.WatchADTimeTask_Key:
                    task = new CollectADCountTask(taskInfoDict, parentTask);
                    break;
                case TaskConstants.CollectCardTask_Key:
                    task = new CollectCardTypeCountTask(taskInfoDict, parentTask);
                    break;
                case TaskConstants.CollectCashFromZeroTask_Key:
                    task = new CollectCashFromZeroTask(taskInfoDict, parentTask);
                    break;
                case TaskConstants.CollectNewCardCountTask_Key:
                    task = new CollectNewCardCountTask(taskInfoDict, parentTask);
                    break;
                case TaskConstants.CollectNewCardTypeCountTask_Key:
                    task = new CollectNewCardTypeCountTask(taskInfoDict, parentTask);
                    break;
            }
            return task;
        }
    }
}