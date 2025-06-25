using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinResultBonusTrigger : ISpinResultProvider
{
    private Dictionary<string, object> _dic=new Dictionary<string, object>();
    public string Name { get; set; }

    /// <summary>
    /// <param name="triggerSource">触发来源</param>
    /// <param name="featureType">触发Feature的类型 Pick/Wheel/Collect</param>
    /// <param name="dic">触发类型为Symbol时的symbol信息</param>
    /// </summary>
    public SpinResultBonusTrigger(TriggerFeatureType triggerSource,TriggerFeatureType featureType,Dictionary<string,int> dic)
    {
        Name = SpinResultType.BonusTrigger.ToString();
        _dic["Trigger"] = triggerSource.ToString();
        _dic["FeatureType"] = featureType;
        if (triggerSource==TriggerFeatureType.Symbol && featureType != TriggerFeatureType.Collect && dic != null)
        {
            _dic["Symbols"] = dic;
        }
        
    }
    public object Decode()
    {
        return _dic;
    }
}
