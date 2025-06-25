using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Utils;


public class SymbolMap
{
    public readonly static string SYMBOL_MAP = "SymbolMap";
    public readonly static string MUTIPLER = "Mutipler";
    public readonly static string WEIGHT = "Weights"; //一个可以当多个symbol
    public readonly static string IS_WILD = "IsWild";
    public readonly static string IS_BONUS = "IsBonus";
    public readonly static string IS_FREESPIN = "IsFreespin";
    public readonly static string IS_SMART_SOUND = "IsSmartSound";
    public readonly static string INDEX = "Index";

    public readonly static string IS_UP = "IsUp";
    public readonly static string IS_DOWN = "IsDown";
    public readonly static string IS_CIRCLE = "IsCircle";
    public readonly static string IS_CHANGABLE = "IsChangable";
    public readonly static string IS_JackPot = "IsJackPot";
    public readonly static string IS_SCATTER = "IsScatter";
    public readonly static string IS_RED = "IsRed";
    public readonly static string IS_Green = "IsGreen";
    public readonly static string IS_Yellow = "IsYellow";
    public readonly static string IS_RESPIN = "IsReSpin";

    public readonly static string IS_REPLACE_BY_REEL_ANI = "IsReplaceByReelAni";
    public readonly static int BLANK_SYMBOL_INDEX = -1;
    public readonly static string SAME_AS_SYMBOL = "SameAsSymbol";
    public readonly static string IS_BLANK = "IsBlank";

    public readonly static string SCORE_LEVEL = "ScoreLevel"; //显示的层次，高分值，低分值等

    private Dictionary<int, SymbolElementInfo> elementInfos;
    private Dictionary<string, int> indexMap;

    public Dictionary<string, int> ElementsData
    {
        get => indexMap;
    }

    public int getSymbolIndexByUniqueBoolPropertyName(string propertyName)
    {
        foreach (int idx in elementInfos.Keys)
        {
            SymbolElementInfo info = elementInfos[idx];
            if (info.getBoolValue(propertyName))
            {
                return idx;
            }
        }
        return -1;
    }

    public SymbolMap(Dictionary<string, object> infos)
    {
        indexMap = new Dictionary<string, int>();
        elementInfos = new Dictionary<int, SymbolElementInfo>();
        if (infos == null)
        {
            return;
        }

        foreach (string key in infos.Keys)
        {
            Dictionary<string, object> info = infos[key] as Dictionary<string, object>;
            SymbolElementInfo symbolInfo = new SymbolElementInfo(info, key);
            elementInfos.Add(symbolInfo.Index, symbolInfo);
            indexMap.Add(key, symbolInfo.Index);
        }
    }

    public int getSymbolIndex(string name)
    {
        if (indexMap.ContainsKey(name))
        {
            return Utilities.CastValueInt((indexMap[name]));
        }
        else
        {
            Debug.LogWarning(name + " dont exists");
        }
        return -1;
    }

    /// <summary>
    /// 判断某个符号是否存在
    /// </summary>
    /// <returns><c>true</c>, if exit symbol was ised, <c>false</c> otherwise.</returns>
    /// <param name="symbolName">Symbol name.</param>
    public bool isExitSymbol(string symbolName)
    {
        if (indexMap.ContainsKey(symbolName))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public SymbolElementInfo getSymbolInfo(int index)
    {
        if (elementInfos.ContainsKey(index))
        {
            return elementInfos[index];
        }
        else
        {
            Debug.LogWarning(index + " dont exists");
            return null;
        }
    }

    public class SymbolElementInfo
    {
        private int _index;
        private bool _isWild;
        private int _multipler;
        private int _weight;
        private bool _isBonus;
        private bool _isFreeSpin;
        private bool _isCircle;
        private bool _isChangable;
        private bool _isRespin;
        private int _ScoreLevel;
        private bool _isJackpot;
        private bool _isScatter;

        public SymbolElementInfo(Dictionary<string, object> info, string name)
        {
            this.name = name;
            this.info = info;
            _index =  getIntValue(INDEX, -1); 
            _isWild = getBoolValue(IS_WILD);
            _multipler= getIntValue(MUTIPLER, 1);
            _weight =getIntValue(WEIGHT, 1);
            _isBonus =getBoolValue(IS_BONUS);
            _isFreeSpin = getBoolValue(IS_FREESPIN); 
            _isCircle = getBoolValue(IS_CIRCLE);
            _isChangable = getBoolValue(IS_CHANGABLE);
            _isRespin = getBoolValue(IS_RESPIN);
            _ScoreLevel = getIntValue(SCORE_LEVEL, 0);
            _isJackpot = getBoolValue(IS_JackPot);
            _isScatter = getBoolValue(IS_SCATTER);
        }
        public int Index
        {
            get { return _index; }
        }
        public bool isWild
        {
            get { return _isWild; }
        }
        public int Mutipler
        {
            get { return _multipler; }
        }
        public int Weight
        {
            get { return _weight; }
        }
        public bool isBonus
        {
            get { return _isBonus; }
        }
        public bool isFreespin
        {
            get { return _isFreeSpin; }
        }

        public bool IsJackpot
        {
            get { return _isJackpot; }
        }

        public bool IsScatter
        {
            get { return _isScatter; }
        }

        public bool isCircle
        {
            get { return _isCircle; }
        }
        public bool isChangable
        {
            get { return _isChangable; }
        }
        public bool isRespin
        {
            get { return _isRespin; }
        }
        public int ScoreLevel
        {
            get { return _ScoreLevel; }
        }
        
//        public bool isSmartSound
//        {
//            get { return getBoolValue(IS_SMART_SOUND); }
//        }
       
        public string name;

        private Dictionary<string, object> info;

        /// <summary>
        /// 尽量少用
        /// </summary>
        /// <param name="infoName"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public bool getBoolValue(string infoName, bool defaultValue = false)
        {
            if (info != null && info.ContainsKey(infoName))
            {
                return Utilities.CastValueBool(info[infoName]);
            }
            return defaultValue;
        }

        public void SetMultiplier(int multiplier)
        {
            _multipler = multiplier;
        }
/// <summary>
/// 尽量少用
/// </summary>
/// <param name="infoName"></param>
/// <param name="defaultValue"></param>
/// <returns></returns>
        public int getIntValue(string infoName, int defaultValue = 0)
        {
            if (info != null && info.ContainsKey(infoName))
            {
                return Utilities.CastValueInt((info[infoName]));
            }
            return defaultValue;
        }

        public bool IsBlank()
        {
            return this.Index == BLANK_SYMBOL_INDEX;
        }
    }
}

