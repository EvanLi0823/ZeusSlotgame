using System.Collections;
using System.Collections.Generic;
using Classic;
using UnityEngine;

public class SpinResultReSpinGame : ISpinResultProvider
{
    private Dictionary<string, object> _dic=new Dictionary<string, object>();
    public string Name { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="spinCount"></param>
    /// <param name="totalWin">除了触发 ，respin 最终的奖励</param>
    public SpinResultReSpinGame(int spinCount,double totalWin,Dictionary<string,int> winSymbol=null, bool inFreeGame=false,Dictionary<string,int> symbols=null)
    {
        Name = SpinResultType.ReSpinGame.ToString();
        _dic["TotalWin"] = totalWin;
        _dic["SpinsCount"] = spinCount;
        _dic["Symbols"] = (symbols==null || symbols.Count==0)?SpinResultProduce.InternalGetWinSymbolList():symbols;
        winSymbol = (winSymbol==null || winSymbol.Count==0)?SpinResultProduce.InternalGetWinSymbolList():winSymbol;
        if (winSymbol != null)
        {
            _dic["WinSymbols"] = winSymbol;
        }
        _dic["InFreeGame"] = inFreeGame ? inFreeGame : BaseSlotMachineController.Instance.reelManager.isFreespinBonus;
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
