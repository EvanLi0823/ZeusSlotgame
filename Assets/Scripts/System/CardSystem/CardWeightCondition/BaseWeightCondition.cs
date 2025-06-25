using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CardSystem
{
    public enum ConditionType
    {
        Free = 0,
        CostCoin =1,
        AD = 2
    }
    
    public class BaseWeightCondition
    {
        public int percent;
        public ConditionType type;
        public int weightIndex = -1;
        public CardsReel _cardsReel;
        public void Init(Dictionary<string,object> data)
        {
            try
            {
                percent = Utils.Utilities.GetInt(data, CardSystemConstants.Percent, 0);
                Dictionary<string,object> conditions = Utils.Utilities.GetValue<Dictionary<string,object>>(data, CardSystemConstants.Condition, null);
                type = (ConditionType)Utils.Utilities.GetInt(conditions, CardSystemConstants.type, 0);
                ParseConditionConfig(conditions);
                weightIndex = Utils.Utilities.GetInt(data, CardSystemConstants.WeightIndex, -1);
                _cardsReel = CardSystemManager.Instance.GetCardsGroupByIndex(weightIndex);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public virtual void ParseConditionConfig(Dictionary<string,object> data)
        {
        }

        //
        public virtual bool CheckCondition()
        {
            return true;
        }

        public int GetWeightPercent()
        {
            return percent;
        }
        
        //获取当前转轮结果
        public virtual int GetResultByWeight()
        {
            if (_cardsReel!=null)
            {
                return _cardsReel.GetResultRandomByWeight();
            }
            return -1;
        }

        
        //当前转轮执行的操作
        public virtual void Execute()
        {
            
        }
    }
}