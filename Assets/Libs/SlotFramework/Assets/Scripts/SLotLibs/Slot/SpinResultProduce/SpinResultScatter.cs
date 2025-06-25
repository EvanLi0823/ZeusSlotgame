using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinResultScatter :ISpinResultProvider
{
    public string Name { get; set; }
    private Dictionary<string, object> _dic=new Dictionary<string, object>();

    public SpinResultScatter(Dictionary<string,int> symbols,double win)
    {
        Name = SpinResultType.Scatter.ToString();
        _dic["Symbols"] = symbols;
        _dic["Win"] = win*BaseSlotMachineController.Instance.currentBetting/BaseSlotMachineController.Instance.reelManager.gameConfigs.ScalFactor;
    }
    public object Decode()
    {
        return _dic;
    }
}
