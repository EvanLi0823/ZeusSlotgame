using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinResultMachineJackpot : ISpinResultProvider
{
    private List<Dictionary<string, object>> _list = new List<Dictionary<string, object>>();
    private Dictionary<string, object> _dic = new Dictionary<string, object>();
    public string Name { get; set; }
    
    public SpinResultMachineJackpot()
    {
        Name = SpinResultType.MachineJackpot.ToString();
    }

    public void Reset()
    {
        _list?.Clear();
    }

    public void AddJackpotItem(JackpotItem jackpotItem)
    {
        _list.Add(jackpotItem.GetJackpotItem());
    }

    public int GetJackpotCount()
    {
        return _list.Count;
    }
    
    public object Decode()
    {
        return _list;
    }
}

public class JackpotItem
{
    private Dictionary<string, object> _dic = new Dictionary<string, object>();
    public JackpotItem(double win,int level,TriggerFeatureType type,Dictionary<string,int> symbols=null,JackPotNameType jackPotNameType=JackPotNameType.Common)
    {
        _dic["Level"] = level.ToString();
        _dic["Win"] = win;
        _dic["JackpotName"] = jackPotNameType.ToString();
        _dic["Trigger"] = type.ToString();
        if (symbols != null)
        {
            _dic["Symbols"] = symbols;
        }
    }

    public Dictionary<string, object> GetJackpotItem()
    {
        return _dic;
    }
}
