using System;
using System.Collections;
using System.Collections.Generic;
using Classic;

public class SpinResultBaseGame : ISpinResultProvider
{
   private Dictionary<string, object> _spinDic=new Dictionary<string, object>();
   public string Name { get; set; }
   public Dictionary<string, int> WinSymbols=new Dictionary<string, int>();
   public Dictionary<string, int> Symbols=new Dictionary<string, int>();

   public SpinResultBaseGame()
   {
      Name = SpinResultType.BaseGame.ToString();
   }
   
   
   public object Decode()
   {
      WinSymbols = (WinSymbols==null||WinSymbols.Count==0) ? SpinResultProduce.InternalGetWinSymbolList() : WinSymbols;
      if (WinSymbols != null)
      {
         _spinDic["WinSymbols"] = WinSymbols;
      }

      _spinDic["Symbols"] = (Symbols == null ||Symbols.Count==0) ? SpinResultProduce.InternalGetSymbolsList() : Symbols;
      _spinDic["Win"] = Math.Abs(BaseSlotMachineController.Instance.baseAward);
      return _spinDic;
   }
}
