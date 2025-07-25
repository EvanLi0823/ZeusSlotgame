
using System.Collections.Generic;

namespace OnLineEarning
{
    /// <summary>
    /// 奖励节点的功能接口
    /// 1.奖励节点是否满足 2.cash的数量 3.看广告奖励的倍数 4.节点对应弹窗按钮的行为，看激励或者插屏广告 5.奖励次数
    /// </summary>
    public class BaseOnLineEarningTimer:IRewardCondition
    {
        public string name = "";
    
        public string DataSaveKey = "";
        public BaseOnLineEarningTimer(string name,Dictionary<string,object> config = null)
        {
            if (config != null)
            {
                return;
            }
            this.name = name;
            DataSaveKey = OnLineEarningConstants.Prefix;
        }
    
        public virtual int GetReward(int type = 0)
        {
            throw new System.NotImplementedException();
        }
    
        public virtual bool IsConditionMeet()
        {
            throw new System.NotImplementedException();
        }
    
        public virtual int GetAdMultiple()
        {
            throw new System.NotImplementedException();
        }
    
        public virtual int GetCount()
        {
            throw new System.NotImplementedException();
        }
    
        public virtual void DoAction()
        {
            throw new System.NotImplementedException();
        }
    
        public virtual void SaveData()
        {
            throw new System.NotImplementedException();
        }
    
        public virtual void LoadData()
        {
            throw new System.NotImplementedException();
        }
    }
}

