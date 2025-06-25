using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinResultReSpinTrigger : ISpinResultProvider
{
    private Dictionary<string, object> _dic=new Dictionary<string, object>();
    public string Name { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="type"></param>
    /// <param name="newSpinCount"></param>
    /// <param name="symbols">触发类型为Symbol时的symbol信息</param>
    public SpinResultReSpinTrigger(TriggerFeatureType type,int newSpinCount,Dictionary<string, int> symbols)
    {
        Name = SpinResultType.ReSpinTrigger.ToString();
        _dic["Trigger"] = type.ToString();
        _dic["NewSpinCount"] = newSpinCount;
        if (symbols !=null)
        {
            _dic["Symbols"] = symbols;
        }
        
    }
    public object Decode()
    {
        return _dic;
    }
    public bool IsValid()
    {
        return true;
    }
}
