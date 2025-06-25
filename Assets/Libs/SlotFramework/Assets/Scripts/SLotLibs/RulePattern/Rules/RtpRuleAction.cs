using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RtpRuleAction : IRuleAction
{
    public float RtpMin,  SkipRate =0f;
    public float RtpMax = float.MaxValue;
    public RuleActionType RAType;

    public bool MinEqual = true;
    public bool MaxEqual = false;

    public RtpRuleAction()
    {
        
    }
    public RtpRuleAction(Dictionary<string, object> dic )
    {
        //rtpmin
        if (dic.ContainsKey("a7c4"))
        {
            RtpMin = RuleUtils.CastValueFloat(dic["a7c4"]);
            MinEqual = true;
        }
        else if(dic.ContainsKey("a7c3"))
        {
            RtpMin = RuleUtils.CastValueFloat(dic["a7c3"]);
            MinEqual = false;
        }
        //rtp max
        if (dic.ContainsKey("a7c2"))
        {
            RtpMax = RuleUtils.CastValueFloat(dic["a7c2"]);
            MaxEqual = true;
        }
        else if (dic.ContainsKey("a7c1"))
        {
            RtpMax = RuleUtils.CastValueFloat(dic["a7c1"]);
            MaxEqual = false;
        }
        //skip rate
        if (dic.ContainsKey("e2b4"))
        {
            this.SkipRate = RuleUtils.CastValueFloat(dic["e2b4"]);
        }

        RAType = RuleActionType.RTP;
    }

    private bool SatisfyRtpMin(float rtp)
    {
        if (MinEqual)
        {
            return rtp >= this.RtpMin;
        }
        else
        {
            return rtp > this.RtpMin;
        }
    }

    private bool SatisfyRtpMax(float rtp)
    {
        if (MaxEqual)
        {
            return rtp <= RtpMax;
        }
        else
        {
            return rtp < RtpMax;
        }
    }
    public bool CheckdResultSkip(SpinResultData data)
    {
//        rtp的rule只在第一次create结果的时候判断下skip的概率。如果需要再次create结果则不再判断概率。之后百分之百跳过
        if (SatisfyRtpMin(data.TotalLineAwardValue) && SatisfyRtpMax(data.TotalLineAwardValue))
        {
            if(data.RuleUseCount == 1)
            {
                if (Math.Abs(this.SkipRate) < 0.000001)
                {
                    return false;
                }

                if (UnityEngine.Random.value <= this.SkipRate)
                {
                    return true;
                }
            }
            else
            {
                return true;
            }
        }
        return false;
    }

    public RuleActionType GetRuleType()
    {
        return RAType ;
    }

    public void MakeTestData()
    {
        RtpMin = 0f;
        RtpMax = 2;
        SkipRate = 1f;
    }
}
