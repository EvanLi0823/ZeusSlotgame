using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Classic
{
    public class FakeStrip
    {
        public static readonly string ReelData_INFOS = "ReelData";
        private SymbolMap _symbolMap;
        private List<List<int>> BaseStrips = new List<List<int>>();//用于展示的带子
        private Dictionary<string, object> baseGame = new Dictionary<string, object>();//plist中"basegame"节点
        private List<string> Symbol_R = new List<string>();//空Symbol的名字
        private Dictionary<string,List<List<int>>> R_Data = new Dictionary<string, List<List<int>>>();//用来替换空Symbol的内容
        private Dictionary<string,List<int>> R_Weight = new Dictionary<string, List<int>>();//用来替换空Symbol内容的权重序列
        private Dictionary<string,int> TotalWeight = new Dictionary<string, int>();//用来替换空Symbol内容总权重
        private Dictionary<string, Dictionary<int, List<int>>> IndexDictBase;//带子上空Symbol的序列

        #region 解析
        public FakeStrip(Dictionary<string,object> info,SymbolMap symbolMap)
        {
            if (info != null)
            {
                _symbolMap = symbolMap;
                if (info.ContainsKey("BaseGame"))
                {
                    baseGame = info["BaseGame"] as Dictionary<string, object>;
                    ParseBaseStrips(baseGame);
                    ParseExtraInfo(baseGame);
                }
            }
        }
        /// <summary>
        /// 解析basegame的虚拟带子
        /// </summary>
        /// <param name="baseGame"></param>
        private void ParseBaseStrips(Dictionary<string, object> baseGame)
        {
            if (baseGame != null && baseGame.ContainsKey("Strip"))
            {
                List<object> obj = baseGame["Strip"] as List<object>;
                for (int i = 0; i < obj.Count; i++)
                {
                    List<object> symbols = obj[i] as List<object>;
                    List<int> temp = new List<int>();
                    for (int j = 0; j < symbols.Count; j++)
                    {
                        int index = _symbolMap.getSymbolIndex(symbols[j].ToString());
                        temp.Add(index);
                    }
                    BaseStrips.Add(temp);
                }
            }
        }
        /// <summary>
        /// 解析虚拟带子的额外信息
        /// </summary>
        /// <param name="baseGame"></param>
        private void ParseExtraInfo(Dictionary<string, object> baseGame)
        {
            if (baseGame != null && baseGame.ContainsKey("ExtraInfos"))
            {
                Dictionary<string, object> extraInfos = baseGame["ExtraInfos"] as Dictionary<string, object>;
                if (extraInfos.ContainsKey("StackSymbol"))
                {
                    List<object> stackSymbol = extraInfos["StackSymbol"] as List<object>;
                    for (int i = 0; i < stackSymbol.Count; i++)
                    {
                        Symbol_R.Add(stackSymbol[i].ToString());
                    }

                    for (int i = 0; i < Symbol_R.Count; i++)
                    {
                        int totalWeight = 0;
                        if (extraInfos.ContainsKey(Symbol_R[i]+"_StackData"))
                        {
                            List<object> reels = extraInfos[Symbol_R[i] + "_StackData"] as List<object>;
                            List<List<int>> symbolsTemp = new List<List<int>>();
                            List<int> weightsTemp = new List<int>();
                            for (int j = 0; j < reels.Count; j++)
                            {
                                string[] symbols = (reels[j] as Dictionary<string, object>)?["Symbol"].ToString().Split(',');
                                string weights = (reels[j] as Dictionary<string, object>)?["Weights"].ToString();
                                int weight = int.Parse(weights);
                                totalWeight += weight;
                                weightsTemp.Add(weight);
                                List<int> index = new List<int>();
                                for (int k = 0; k < symbols.Length; k++)
                                {
                                    index.Add(_symbolMap.getSymbolIndex(
                                        symbols[k]));
                                }
                                symbolsTemp.Add(index);
                            }
                            R_Data[Symbol_R[i]] = symbolsTemp;
                            R_Weight[Symbol_R[i]] = weightsTemp;
                            TotalWeight[Symbol_R[i]] = totalWeight;
                        }
                    }
                }
            }
        }
        #endregion

        #region 计算
        /// <summary>
        /// 获取带子上空位置的序列
        /// </summary>
        private void GetBlankSymbolIndexDict()
        {
            if (IndexDictBase == null)
            {
                IndexDictBase = new Dictionary<string, Dictionary<int, List<int>>>();
                for (int i = 0; i < Symbol_R.Count; i++)
                {
                    IndexDictBase[Symbol_R[i]] = GetSpecialSymbolIndexInReelData(Symbol_R[i], _symbolMap);
                }
            }
        }
        /// <summary>
        /// 按名字查找虚拟带子上对应空Symbol的位置，并返回这个序列
        /// </summary>
        /// <param name="speicalTag"></param>
        /// <param name="symbolMap"></param>
        /// <returns></returns>
        private Dictionary<int, List<int>> GetSpecialSymbolIndexInReelData (string speicalTag, SymbolMap symbolMap)
        {
            Dictionary<int, List<int>> specialSymbolIndex = new Dictionary<int, List<int>>();
            
            for (int i = 0; i < BaseStrips.Count; i++) {
                int startIndex = 0;
                int endIndex = BaseStrips[i].Count;
                List<int> indexInReel = new List<int> ();
                for (int j = startIndex; j < endIndex; j++) {
                    SymbolMap.SymbolElementInfo info = symbolMap.getSymbolInfo (BaseStrips[i][j]);
                    if (info.name.Equals(speicalTag)) {
                        indexInReel.Add (j);
                    }
                }
                specialSymbolIndex.Add (i, indexInReel);
            }
            return specialSymbolIndex;
        }
        /// <summary>
        /// 获取替换的Symbol
        /// </summary>
        /// <param name="tw"></param>
        /// <param name="WList"></param>
        /// <param name="repeat"></param>
        /// <returns></returns>
        private List<int> ChangeData(int tw, List<int> WList,List<List<int>> repeat)
        {
            int dictTempIndex = UnityEngine.Random.Range(0, tw);
            int dictIndex = GetRandomDictIndex(dictTempIndex, WList);
            List<int> resultForchange = new List<int>();
            for (int i = 0; i < repeat[0].Count; i++)
            {
                resultForchange.Add(repeat[dictIndex][i]);
            }

            return resultForchange;
        }
        /// <summary>
        /// 获取随机权重的下标
        /// </summary>
        /// <param name="random"></param>
        /// <param name="Weights"></param>
        /// <returns></returns>
        private int GetRandomDictIndex(int random, List<int> Weights)
        {
            int temp = 0;
            for (int i = 0; i < Weights.Count; i++)
            {
                if (random >= temp && random < (temp + Weights[i]))
                {
                    return i;
                }
                temp += Weights[i];
            }
            return -1;
        }
        #endregion

        #region 外部访问接口
        /// <summary>
        /// 获取整套带子
        /// </summary>
        /// <param name="isFreespinBonus"></param>
        /// <returns></returns>
        public List<List<int>> GetRunData(bool isFreespinBonus)
        {
            if (!isFreespinBonus)
            {
                GetBlankSymbolIndexDict();
                foreach (string R_name in IndexDictBase.Keys)
                {
                    List<int> resultForchange = ChangeData(TotalWeight[R_name],R_Weight[R_name],R_Data[R_name]);
                    foreach (int reelIndex in IndexDictBase[R_name].Keys)
                    {
                        for (int i = 0; i < IndexDictBase[R_name][reelIndex].Count; i++)
                        {
                            BaseStrips[reelIndex][IndexDictBase[R_name][reelIndex][i]] = resultForchange[reelIndex];
                        }
                    }
                }
                if(BaseStrips.Count>0)
                    return BaseStrips;
            }
            
            return null;
        }

        public List<int> GetRunStrip(int index)
        {
            if (BaseStrips.Count > index) return BaseStrips[index];
            return null;
        }
        #endregion
    }
}