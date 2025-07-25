namespace OnLineEarning
{
    public interface IRewardCondition
    {
        int GetReward(int type=0);

        bool IsConditionMeet();
    
        int GetAdMultiple();
    
        int GetCount();
    
        void DoAction();

        void SaveData();

        void LoadData();
    }
}

