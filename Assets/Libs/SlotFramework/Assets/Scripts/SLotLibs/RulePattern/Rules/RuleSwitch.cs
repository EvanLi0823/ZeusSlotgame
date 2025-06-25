using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuleSwitch
{
    private int m_SwitchInt = 0;

    public RuleSwitch(string v)
    {
        SetSwitchValue(v);
    }

    public void SetSwitchValue(string v)
    {
        m_SwitchInt = Convert.ToInt32(v, 2);
    }

    public bool IsOpenRule
    {
        get => (m_SwitchInt & (int)OpenRuleType.TOTAL_OPEN )> 0;
    }
    
    public bool IsOpenRtp
    {
        get => (m_SwitchInt & (int)OpenRuleType.RTP )> 0;
    }

    public bool IsOpenFeature
    {
        get => (m_SwitchInt & (int)OpenRuleType.FEATURE )> 0;
    }
}

public enum OpenRuleType
{
    TOTAL_CLOSE = 0,
    TOTAL_OPEN = 1,
    RTP = 1 << 1,
    FEATURE = 1 << 2
}