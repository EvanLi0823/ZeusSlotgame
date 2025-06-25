using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Utils;

namespace Classic
{
	public class ReelStrip
	{
		public readonly static string SYMBOL = "Symbol";
		public readonly static string WEIGHTS = "Weights";
		public int TotalWeights;
        public int RandomStripElementIndex {
            get;
            private set;
        }

        public List<StripElementInfo> stripElementInfos {
            get;
            private set;
        }

		public ReelStrip (SymbolMap symbolMap, List<object> infos)
		{
			TotalWeights = 0;
			stripElementInfos = new List<StripElementInfo> ();
			//读取顺序倒着来
			for (int i=infos.Count-1; i>=0; i--) {
				Dictionary<string,object> info = infos [i] as Dictionary<string,object>;
				StripElementInfo stripElementInfo = new StripElementInfo (symbolMap, info, TotalWeights);
				TotalWeights += stripElementInfo.weight;
				stripElementInfos.Add (stripElementInfo);
			}
		}

		public int GenerateRandomStripElementValue() {
			GenerateRandomStripElementIndex ();
			return stripElementInfos [RandomStripElementIndex].symboIndex;
		}

        public void GenerateRandomStripElementIndex() {
            RandomStripElementIndex = -1;
            if (TotalWeights >= 0 && stripElementInfos != null && stripElementInfos.Count > 0) {
                int weightAccumulated = 0;
                int randomWeight = Random.Range(0, TotalWeights);
                for (int i = 0; i < stripElementInfos.Count; i++) {
                    StripElementInfo elementInfo = stripElementInfos[i];
                    int lastWeightAccumulated = weightAccumulated;
                    weightAccumulated += elementInfo.weight;
                    if (weightAccumulated > randomWeight && lastWeightAccumulated <= randomWeight) {
                        RandomStripElementIndex = i;
                    }
                }
            }
        }

        /// <summary>
        /// 根据正中间Reel设置正中间位置符号设置带子排序情况
        /// 仅仅限于一条strip没有重复符号的情况
        /// 目前仅仅用于PaidResult（DoubleRose情况）
        /// </summary>
        /// <param name="symbolIndex">Symbol index.</param>
        public void SetRandomStripElementIndex(int randomStripElementIndex)
        {
            RandomStripElementIndex = -1;
            if (stripElementInfos != null && stripElementInfos.Count > 0)
            {
                if(randomStripElementIndex>=0&&randomStripElementIndex<stripElementInfos.Count)
                {
                    RandomStripElementIndex = randomStripElementIndex;
                }
            }
        }
			

        public bool IsRandomStripElementIndexValid() {
            return stripElementInfos != null && RandomStripElementIndex >= 0 && RandomStripElementIndex < stripElementInfos.Count;
        }

        public StripElementInfo RandomStripElement {
            get {
                if (IsRandomStripElementIndexValid()) {
                    return stripElementInfos[RandomStripElementIndex];
                }
                return null;
            }
        }

        public class StripElementInfo
		{
            public int symboIndex {
                get;
                set;
            }
            public int weight {
                get;
                private set;
            }

			public string symbolName
			{
				get;
				private set;
			}

			public StripElementInfo (SymbolMap symbolMap, Dictionary<string,object> info, int lastWeights)
			{
				if (info.ContainsKey (SYMBOL)) {
					symbolName = (string)info [SYMBOL];
					symboIndex = symbolMap.getSymbolIndex (symbolName);
                }else{
                    symboIndex = -1;
                }
				
				if (info.ContainsKey (WEIGHTS)) {
					weight = Utilities.CastValueInt ((info [WEIGHTS]));
                }else{
                    weight = 0;
                }
			}
		}
	}
}
