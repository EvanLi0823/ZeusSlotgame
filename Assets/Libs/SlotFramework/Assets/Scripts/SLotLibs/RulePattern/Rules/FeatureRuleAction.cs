using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class FeatureRuleAction :IRuleAction
{
    public float SkipRate;
    public RuleActionType RAType;

    public FeatureRuleAction()
    {
        
    }
    public FeatureRuleAction(Dictionary<string, object> dic)
    {
        if (dic.ContainsKey("e2b4"))
        {
            SkipRate = RuleUtils.CastValueFloat(dic["e2b4"]);
        }

        RAType = RuleActionType.FEATURE;
    }

    public bool CheckdResultSkip(SpinResultData data)
    {
        if (!data.IsFeature)
        {
            //不是feature的时候continue，继续向下走 ,即返回false不跳过
            return false;
        }
        if (Math.Abs(this.SkipRate) < 0.000001)
        {
            return false;
        }
        if (UnityEngine.Random.value <= this.SkipRate)
        {
            return true;
        }
        return false;
    }

    public RuleActionType GetRuleType()
    {
        return RAType;
    }

    public void MakeTestData()
    {
        SkipRate = .5f;
    }
}
