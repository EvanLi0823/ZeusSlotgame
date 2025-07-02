namespace Libs
{
    public class TaskConstants
    {
        public const string Type_Key = "type";
        public const string sub_tasks_Key = "sub_tasks";
        public const string TargetNum_Key = "targetNum";
        public const string TaskId_Key = "taskId";
        
        public const string TaskLocalizeDesc_Key = "localizeDesc";
        public const string TaskState_Key = "taskState";
        public const string TaskMark_Key = "mark";

        public const string ProgressNum_Key = "progressNum";
        public const string RewardList_Key = "rewardList";
        public const string SaveTaskDict_Key = "SaveTaskDict";
        public const string PlistTask_Key = "TaskTables";
        public const string CollectNumber_Key = "sumNumber";
        public const string TargetCashNum_Key = "targetCashNum";
        public const string CollectCashNum_Key = "collectCashNum";
        public const string StartTime_Key = "startTime";
        public const string EndTime_Key = "endTime";
        public const string CanRewardTime_Key = "canRewardTime";

        public const int LeafTask_Key = 10;
        public const int SpinAwardCashTask_Key = 11;    //既要满足spin次数又要满足收集现金
        public const int AccumulateCashTask_Key = 12;   //收集现金
        public const int WatchADTimeTask_Key = 13;      //观看广告次数
        public const int CollectSpinCountTask_Key = 14; //收集Spin次数
        public const int CollectCardTask_Key = 15;      //收集卡片类型数量
        public const int CollectCashFromZeroTask_Key = 16; //从0开始收集现金
        public const int CollectNewCardCountTask_Key = 17; //收集卡片数量的任务，无论类型
        public const int CollectNewCardTypeCountTask_Key = 18; //从0开始收集卡牌类型数量任务
    }
}