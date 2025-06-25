using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinResultWheel : ISpinResultProvider
{
    private Dictionary<string, object> _dic = new Dictionary<string, object>();
    public string Name { get; set; }

    public SpinResultWheel(double totalWin,List<string> list,bool inFreeGame =false)
    {
        Name = SpinResultType.Wheel.ToString();
        _dic["TotalWin"] = totalWin;
        _dic["Results"] = list;
        _dic["InFreeGame"] = inFreeGame || BaseSlotMachineController.Instance.reelManager.isFreespinBonus;
    }
    
    public object Decode()
    {
        return _dic;
    }
}
