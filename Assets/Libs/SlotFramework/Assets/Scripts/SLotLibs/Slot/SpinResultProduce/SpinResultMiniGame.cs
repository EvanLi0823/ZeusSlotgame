using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinResultMiniGame : ISpinResultProvider
{
    private Dictionary<string,object> _dic = new Dictionary<string, object>();

    public string Name
    {
        get;
        set;
    }

    public SpinResultMiniGame(double win, bool inFreeGame=false)
    {
        Name = SpinResultType.MiniGame.ToString();
        _dic["Win"] = win;
        _dic["InFreeGame"] = inFreeGame || BaseSlotMachineController.Instance.reelManager.isFreespinBonus;
    }

    public object Decode()
    {
        return _dic;
    }
}
