using System;
using System.Collections.Generic;
using Classic;
using UnityEngine;

public class SpinResultFreeGame : ISpinResultProvider
{
    private Dictionary<string, object> _dic = new Dictionary<string, object>();
    private int _totalCount;
    private double _totalWin;
    private bool _isEnd;
    private bool _isSuper;
    private Dictionary<string, int> _symbols;
    private Dictionary<string, int> _winSymbols;
    
    public string Name { get; set; }

    public SpinResultFreeGame(int totalCount,bool isEnd=false,double totalWin =0,bool isSuper =false,Dictionary<string,int> symbols=null,Dictionary<string,int> winSymbols=null)
    {
        Name = SpinResultType.FreeGame.ToString();
        _totalCount = totalCount;
        _totalWin = totalWin;
        _isEnd = isEnd;
        _isSuper = isSuper;
        _symbols = symbols;
        _winSymbols = winSymbols;
    }

    public object Decode()
    {
        double win = BaseSlotMachineController.Instance.isReturnFromFreespin
            ? 0
            : Math.Abs(BaseSlotMachineController.Instance.baseAward);
        _dic["Win"] = win ;
        _dic["TotalWin"] = _totalWin > 0 ? _totalWin : BaseSlotMachineController.Instance.FreeSpinTotalAward;
        _dic["SpinsCount"] = _totalCount;
        _dic["IsEnd"] = _isEnd;
        _dic["IsSuper"] = _isSuper;
        _dic["RTP"] = win/BaseSlotMachineController.Instance.currentBetting;
        _dic["Symbols"] = (_symbols==null||_symbols.Count==0)?SpinResultProduce.InternalGetSymbolsList():_symbols;
        Dictionary<string, int> winSymbol = (_winSymbols==null || _winSymbols.Count==0)? SpinResultProduce.InternalGetWinSymbolList():_winSymbols;
        if (winSymbol != null)
        {
            _dic["WinSymbols"] = winSymbol;
        }
        return _dic;
    }
    public bool IsValid()
    {
        return true;
    }
    
}