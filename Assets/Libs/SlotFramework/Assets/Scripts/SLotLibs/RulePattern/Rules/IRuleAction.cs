using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IRuleAction
{
//    void Init(object obj);
    bool CheckdResultSkip(SpinResultData data);//是否跳过这个结果
    RuleActionType GetRuleType();
}

public enum RuleActionType
{
    NEAR_MISS,
    RTP,
    FEATURE,
    REPACE_CONFIG,
    NULL
}
