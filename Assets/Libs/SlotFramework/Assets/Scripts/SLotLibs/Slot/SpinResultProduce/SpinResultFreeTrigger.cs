using System.Collections;
using System.Collections.Generic;
using Classic;
using UnityEngine;

public class SpinResultFreeTrigger : ISpinResultProvider
{
    private Dictionary<string, object> _dic = new Dictionary<string, object>();
    public string Name { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="isSuper">MUST 触发的是否为SuperFree
    /// <param name="type">MUST 触发类型 pick/wheel/symbol</param>
    /// <param name="newSpinCount">MUST 触发及Retrigger时的FreeSpin的次数</param>
    /// <param name="symbols">MAY 触发类型为Symbol时的symbol信息 默认用线奖</param>
    /// <param name="isReTrigger">MUST 是否是Retrigger</param>
    public SpinResultFreeTrigger(TriggerFeatureType type,int newSpinCount, Dictionary<string,int> symbols=null,bool isReTrigger=false,bool isSuper=false)
    {
        Name =SpinResultType.FreeTrigger.ToString();
        _dic["IsSuper"] = isSuper;
        _dic["Trigger"] = type.ToString();
        _dic["NewSpinCount"] = newSpinCount;

        if (type == TriggerFeatureType.Symbol)
        {
            if (symbols == null)
            {
                symbols = BaseSlotMachineController.Instance.reelManager.resultContent.FreeSymbolDic;
            }

            _dic["Symbols"] = symbols;
        }
        _dic["ReTrigger"] = isReTrigger;
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
