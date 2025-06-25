using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Classic;
using Utils;

public class BasePaytableElement {
    
    public List<int> symbolList = new List<int> ();
    public Dictionary<int,float> awardMap = new Dictionary<int, float> ();
    public Dictionary<int,string> awardNames = new Dictionary<int, string> ();
    public ScatterPayInfo ScatterPay;
    
    public BasePaytableElement (SymbolMap symbolMap, Dictionary<string,object> info)
    {
        ScatterPay = new ScatterPayInfo();
        if (info.ContainsKey (ClassicPaytable.SYMBOL_LIST)) {
            List<object> symbolListString = info [ClassicPaytable.SYMBOL_LIST] as List<object>;
            for (int i=0; i<symbolListString.Count; i++) {
                string symbolString = symbolListString [i] as string;
                symbolList.Add (symbolMap.getSymbolIndex (symbolString));
                ScatterPay.ScatterSymbol[symbolMap.getSymbolIndex(symbolString)] = symbolString;
            }
        }

        if (info.ContainsKey (ClassicPaytable.AWARD_MAP)) {
            Dictionary<string,object> awardMapObject = info [ClassicPaytable.AWARD_MAP] as Dictionary<string,object>;

            foreach (string key in awardMapObject.Keys) {
                int number = int.Parse (key);

                awardMap.Add (number, Utilities.CastValueFloat (awardMapObject [key], 0));
            }
        }

        if (info.ContainsKey (ClassicPaytable.AwardName)) {
            Dictionary<string,object> awardMapObject = info [ClassicPaytable.AwardName] as Dictionary<string,object>;

            foreach (string key in awardMapObject.Keys) {
                int number = int.Parse (key);
                awardNames.Add (number, awardMapObject [key] as string);
            }
        }
    }
}

public class ScatterPayInfo
{
    public List<SymbolPos> SymbolPoses = new List<SymbolPos>();
    public string Name
    {
        get;
        set;
    }
    public double AwardValue
    {
        get;
        set;
    }
    public Dictionary<string,int> SymbolDic
    {
        get;
        set;
    }=new Dictionary<string, int>();
    /// <summary>
    /// symbolindex -- symbolName 用于 machintrequest 数据统计
    /// </summary>
    public Dictionary<int,string> ScatterSymbol
    {
        get;
        set;
    }=new Dictionary<int, string>();

    public bool IsValue
    {
        get;
        set;
    }
}

public class SymbolPos
{
    public SymbolPos(int reelIndex,int symbolInReelPos,string name)
    {
        ReelIndex = reelIndex;
        SymbolInReelPos = symbolInReelPos;
    }
    public int ReelIndex;
    public int SymbolInReelPos;
    public string Name;
}
