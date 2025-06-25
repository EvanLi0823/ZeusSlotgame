using UnityEngine;
using System.Text;
using System.Collections;
using System.Collections.Generic;
namespace Classic{
	public class SymbolResult {
		public readonly static string WEIGHT = "Weights";
		public readonly static string RESULT_DATA = "ResultData";
		public List<List<int>> symbolResults;
		public int weight;
		public SymbolResult(SymbolMap symbolMap, Dictionary<string,object> info){
			if (info.ContainsKey(WEIGHT)) {
				weight = Utils.Utilities.CastValueInt(info[WEIGHT]);
			}
			if (info.ContainsKey(RESULT_DATA)) {
				List<object> resultItem =  info[RESULT_DATA] as List<object>;
				symbolResults = new List<List<int>> ();
				for (int column = 0; column < resultItem.Count; column++) {
					string[] row = resultItem[column].ToString ().Split (',');
					List<int> rowSymbolIndex = new List<int> ();
					for (int rowIdex = 0; rowIdex < row.Length; rowIdex++) {
						int symbolIndex = symbolMap.getSymbolIndex(row[rowIdex]);
						rowSymbolIndex.Add (symbolIndex);
					}
					rowSymbolIndex.Reverse ();//适配数据给出的数据 而进行反转排序
					symbolResults.Add (rowSymbolIndex); 
				}
			}
		}

        public SymbolResult(SymbolMap symbolMap, string symobls)
        {
            string[] reelSymbols = symobls.Split(';');
            symbolResults = new List<List<int>>();
            bool valid = true;
            for (int column = 0; column < reelSymbols.Length; column++)
            {
                string[] row = reelSymbols[column].Split(',');
                List<int> rowSymbolIndex = new List<int>();
                for (int rowIdex = 0; rowIdex < row.Length; rowIdex++)
                {
                    int symbolIndex = symbolMap.getSymbolIndex(row[rowIdex]);
                    //当symbol当-1时 判断该符号是否代表BLANK(isExitSymbol方法判断符号是否存在)
                    if (symbolIndex == -1 && !symbolMap.isExitSymbol(row[rowIdex]))
                    {
                        //当符号不存在时 添加该条件 以防BLANK忘记配置的情况
                        //当符号Str不为blank或者SOO时 无效
                        if (!(("blank".Equals(row[rowIdex].ToLower())) || ("S00".Equals(row[rowIdex].ToUpper()))))
                        {
                            valid = false;
                            break;
                        }
                    }

                    rowSymbolIndex.Add(symbolIndex);
                }
                if (!valid)
                {
                    symbolResults = null;
                    break;
                }
                symbolResults.Add(rowSymbolIndex);
            }

        }

	}
}
