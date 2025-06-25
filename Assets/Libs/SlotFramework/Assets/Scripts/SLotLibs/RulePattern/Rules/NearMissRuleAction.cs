using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class NearMissRuleAction : IRuleAction
{
    public float ReplaceRate;
    public RuleActionType RAType;
    public NearMissRuleAction()
    {
        
    }
    public NearMissRuleAction(Dictionary<string, object> dic )
    {
        if (dic.ContainsKey("e1b4"))
        {
            this.ReplaceRate = RuleUtils.CastValueFloat(dic["e1b4"]);
        }
        RAType = RuleActionType.NEAR_MISS;
    }
    public bool CheckdResultSkip(SpinResultData data)
    {
        if (data.TotalLineAwardValue >= ReplaceRate && data.TotalLineAwardValue < ReplaceRate)
        {
            if (Math.Abs(this.ReplaceRate) < 0.000001)
            {
                return false;
            }
            if (UnityEngine.Random.value <= this.ReplaceRate)
            {
                return true;
            }
        }
        return false;
    }

    public RuleActionType GetRuleType()
    {
        return RAType;
    }
    
}
