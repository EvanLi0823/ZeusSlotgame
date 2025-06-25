using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IRuleResult
{
    RuleResultMediation GetRuleResult(RuleSwitch ruleSwitch);
    void RuleCreateResult();

    void ClearServerConfig();
}

public struct RuleResultMediation
{
    public bool IsFeature;
    public float AwardValue;
}
