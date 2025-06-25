using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct ResultProcessData
{
    //结算类型
    public ResultAccountTypeEnum AccountType;
    //结算的金币奖励值
    public double AccountValue;
    //线奖奖励值
    public double LineValue;
    //结算的现金奖励值
    public int CashValue;
    //是否需要结算
    public bool NeedAccount
    {
        get { return AccountType != ResultAccountTypeEnum.LINKING && AccountType != ResultAccountTypeEnum.NOACCOUNT; }
    }

    public bool IsBonusType
    {
        get { return AccountType == ResultAccountTypeEnum.BONUS; }
    }
}

/// <summary>
/// 结算的流程类型
/// </summary>
public enum ResultAccountTypeEnum
{
    NORMAL,
    FS,
    BONUS,
    LINKEND,
    LINKING,
    EXTRA,
    NOACCOUNT
}
