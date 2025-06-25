using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinResultPick : ISpinResultProvider
{
    private Dictionary<string, object> _dic = new Dictionary<string, object>();
    public string Name { get; set; }

    public SpinResultPick(List<string> list, double totalWin = 0, bool inFreeGame=false)
    {
        Name = SpinResultType.Pick.ToString();
        if (totalWin > 0.0001)//此处判断防止精度问题
        { 
            _dic["TotalWin"] = totalWin;
        }
        _dic["Results"] = list;
        _dic["InFreeGame"] = inFreeGame || BaseSlotMachineController.Instance.reelManager.isFreespinBonus;
    }
    
    public object Decode()
    {
        return _dic;
    }
}
