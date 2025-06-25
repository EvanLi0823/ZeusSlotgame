using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Core;
using Utils;
using System.Linq;

namespace Classic
{
    public class AwardResult:BaseAward
    {
        public List<AwardPayLine> awardInfos = new List<AwardPayLine> ();
        public BaseAward blackAward = new BaseAward ();

		public static double CurrentResultBet = 1;

        Dictionary<int, Dictionary<int, int>> awardElementIndexOfDisplayCountDict = new Dictionary<int, Dictionary<int, int>> ();

        public void Reset ()
        {
            awardInfos.Clear ();
            this.awardValue = 0;
            blackAward.awardValue = 0;
            awardElementIndexOfDisplayCountDict.Clear ();
        }

		public void CreateAwardData(ReelManager reelManager,bool wildAccumulation){
			double awardTemp = this.awardValue;
            this.Reset ();
            reelManager.awardElements.Clear ();
            reelManager.awardLines.Clear ();
            reelManager.payTable.CreateBlackAward (reelManager.resultContent, this.blackAward);

            for (int i=0; i<reelManager.lineTable.TotalPayLineCount(); i++) {
                PayLine payLine = reelManager.lineTable.GetPayLineAtIndex (i);
                List<int> result = new List<int> ();
                for (int j=0; j<payLine.GetSize(); j++) {
                    result.Add (reelManager.resultContent.ReelResults [j].SymbolResults [payLine.RowNumberAt (j)]);
                }
				AwardResult.AwardPayLine award = reelManager.payTable.CreateAwardResult (i, result,wildAccumulation, payLine);
                if (award.isAwarded) {
					if (award.awardValue < reelManager.gameConfigs.AnimationTypeTag) {
						award.CreateDefaultAnimations((int)BaseElementPanel.AnimationID.AnimationID_NormalAward,payLine.GetSize());
                    } else {
						award.CreateDefaultAnimations((int)BaseElementPanel.AnimationID.AnimationID_BackFromNormalAward,payLine.GetSize());
                    }
					InsertAward (award);
                }
            }
            this.awardValue += this.blackAward.awardValue;

            for (int i=0; i<this.awardInfos.Count; i++) {
                PayLine payLine = reelManager.lineTable.GetPayLineAtIndex (this.awardInfos [i].LineIndex);
                this.awardInfos [i].awardNumberCount = 0;
				this.awardInfos [i].symbolRenderIndexs.Clear ();
                for (int j=0; j<payLine.GetSize(); j++) {
                    if (this.awardInfos [i].awardPosition [j]) {
						int index = payLine.RowNumberAt (j);
						SymbolRenderIndex symbolIndex = new SymbolRenderIndex (j,index);
						AddAwardElementIndexOfDisplay (j, index);
						this.awardInfos [i].symbolRenderIndexs.Add (symbolIndex);
                        this.awardInfos [i].awardNumberCount++ ;
                    }
                }
                if (reelManager.showAwardLineInfo)
                {
	                Log.LogLimeColor("InsertAward lineId: "+i+"  AwardLine: "+awardInfos [i].ToDataString());
                }
            }
            // scatterpay
			foreach (BasePaytableElement payTableElement in reelManager.payTable.scatterPays) {
				List<int> symbolIndexList = payTableElement.symbolList;
				int symbolCount = 0;
				foreach (int symbolIndex in symbolIndexList) {
                    Dictionary<int, List<int>> symbolIndexMap =  reelManager.GetSymbolIndexMapByScatter (symbolIndex);
					symbolCount += Utilities.GetCountInDictList (symbolIndexMap);
				}
				if (payTableElement.awardMap.ContainsKey (symbolCount)) {
					List<BaseElementPanel> symbolElements = new List<BaseElementPanel>();
					foreach (int symbolIndex in symbolIndexList) {
						symbolElements.AddRange(reelManager.GetElementsWithSymbolIndex (symbolIndex));
					}
					AddAwardPayLine (payTableElement.awardMap [symbolCount],payTableElement.awardNames [symbolCount],symbolElements,reelManager.gameConfigs,-3);
				}
			}
			if(!BaseGameConsole.singletonInstance.isForTestScene)
			{
				if (reelManager.GetSingleBoard())
					BaseSlotMachineController.Instance.baseAward += (this.awardValue - awardTemp) *
						BaseSlotMachineController.Instance.currentBettingTemp / reelManager.gameConfigs.ScalFactor;
				else
					BaseSlotMachineController.Instance.baseAward += (this.awardValue - awardTemp);
				if (reelManager.IsNewProcess)
					BaseSlotMachineController.Instance.baseAward = Math.Round(
						Math.Abs(BaseSlotMachineController.Instance.baseAward) *
						BaseSlotMachineController.Instance.currentBettingTemp / reelManager.gameConfigs.ScalFactor);
			}
			// Debug.LogAssertion("min.wang : baseAward : "+BaseSlotMachineController.Instance.baseAward);
        }



		public  static float CreateAwardDataTemp(ReelManager reelManager,bool wildAccumulation){
		try{
			if (reelManager.lineTable == null) {
				return 0f;
			}
			float awardValue = 0;
			for (int i=0; i<reelManager.lineTable.TotalPayLineCount(); i++) {
				PayLine payLine = reelManager.lineTable.GetPayLineAtIndex (i);
				List<int> result = new List<int> ();
				for (int j=0; j<payLine.GetSize(); j++) {
					result.Add (reelManager.resultContent.ReelResults [j].SymbolResults [payLine.RowNumberAt (j)]);
				}
				AwardResult.AwardPayLine award = reelManager.payTable.CreateAwardResult (i, result,wildAccumulation, payLine);
				if (award.isAwarded) {
					awardValue+= (float)award.awardValue;
				}
			}

			foreach (BasePaytableElement payTableElement in reelManager.payTable.scatterPays) {
				List<int> symbolIndexList = payTableElement.symbolList;
				foreach (int symbolIndex in symbolIndexList) {
                    Dictionary<int, List<int>> symbolIndexMap =  reelManager.GetSymbolIndexMapByScatter (symbolIndex);
					int symbolCount = Utilities.GetCountInDictList (symbolIndexMap);
					if (payTableElement.awardMap.ContainsKey (symbolCount)) {
						awardValue += payTableElement.awardMap [symbolCount];
					}
				}
			}
			return awardValue/reelManager.gameConfigs.ScalFactor;
		}catch(System.Exception e){
				return 0;
		}
		}


		public void ChangedAwardValue(float mutilper){
			this.awardValue *= mutilper;
		}

		public void GetAwardAnimationElement (ReelManager reelManager)
		{
			if (reelManager.IsNewSpeedPattern) {
				for (int i = 0; i < this.awardInfos.Count; i++) {
					if (this.awardInfos [i].shouldCreateAnimation) {
						AwardLineElement awardLineElement = new AwardLineElement (this.awardInfos [i]);
						foreach (SymbolRenderIndex symbolIndex in this.awardInfos [i].symbolRenderIndexs) {

							BaseElementPanel render = reelManager.GetSymbolRender (symbolIndex.reelIndex, symbolIndex.symbolIndexInReel);
							if (render == null) {
								Debug.LogError (symbolIndex.reelIndex + ":" + symbolIndex.symbolIndexInReel + "映射map can not found");
								return;
							}

							awardLineElement.awardElements.Add (render);
							if (!reelManager.awardElements.Contains (render)) {
								reelManager.awardElements.Add (render);
							} 
						}

						reelManager.awardLines.Add (awardLineElement);  
					}

				}
			}
		}

        public bool IsAwardElementIndexOfDisplayDictEmpty() {
            if (this.awardElementIndexOfDisplayCountDict == null || 
                this.awardElementIndexOfDisplayCountDict.Count == 0) {
                return true;
            }

            foreach (int reelIndex in this.awardElementIndexOfDisplayCountDict.Keys) {
                Dictionary<int, int> elementIndexCountDict = this.awardElementIndexOfDisplayCountDict [reelIndex];
                if (elementIndexCountDict == null || elementIndexCountDict.Count == 0) {
                    continue;
                }
                return false;
            }
            return true;
        }

        public void AddAwardElementIndexOfDisplay(int reelIndex,int elementIndexInReel ) {
            Dictionary<int, int> elementIndexOfDisplayCountDict = null;
            if (awardElementIndexOfDisplayCountDict.TryGetValue (reelIndex, out elementIndexOfDisplayCountDict) == false || 
                elementIndexOfDisplayCountDict == null) {
                elementIndexOfDisplayCountDict = new Dictionary<int, int>();
                awardElementIndexOfDisplayCountDict [reelIndex] = elementIndexOfDisplayCountDict;
            }

            if (elementIndexOfDisplayCountDict.ContainsKey(elementIndexInReel) == false) {
                elementIndexOfDisplayCountDict[elementIndexInReel] = 1;
            } else {
                elementIndexOfDisplayCountDict[elementIndexInReel] = elementIndexOfDisplayCountDict[elementIndexInReel] + 1;
            }
        }

        public void RemoveAwardElementIndexOfDisplay(int reelIndex, int elementIndexInReel) {
            if (awardElementIndexOfDisplayCountDict.ContainsKey(reelIndex)) {
                Dictionary<int, int> elementIndexOfDisplayCountDict = awardElementIndexOfDisplayCountDict[reelIndex];
                if (elementIndexOfDisplayCountDict.ContainsKey(elementIndexInReel)) {
                    elementIndexOfDisplayCountDict[elementIndexInReel] = elementIndexOfDisplayCountDict[elementIndexInReel] - 1;
                    if (elementIndexOfDisplayCountDict[elementIndexInReel] <= 0) {
                        elementIndexOfDisplayCountDict.Remove(elementIndexInReel);
                        if (elementIndexOfDisplayCountDict.Count <= 0) {
                            awardElementIndexOfDisplayCountDict.Remove(reelIndex);
                        }
                    }
                }
            }
        }

        public bool IsReelAwardForDisplay(int reelIndex) {
            return awardElementIndexOfDisplayCountDict.ContainsKey(reelIndex);
        }

        public List<int> GetAwardElementIndexList(int reelIndex) {
            if (IsReelAwardForDisplay(reelIndex)) {
                return awardElementIndexOfDisplayCountDict [reelIndex].Keys.ToList();
            }
            return null;
        }
        
        

		[Obsolete("将会舍弃")]
		public void AddAwardPayLine(double awardValue, string awardName,List<BaseElementPanel> awardElement,GameConfigs gameConfigs,int lineIndex = -1,bool shouldCreateAnimation = true,PayLine _payLine=null,List<int> lineSymbolIndexs=null) {
			AwardResult.AwardPayLine bonusAwardLine = new AwardResult.AwardPayLine (lineIndex);

			if (lineIndex >= 0) {
				bonusAwardLine.payLine = _payLine;
                if (lineSymbolIndexs!=null)
                {
                    bonusAwardLine.LineSymbolIndexs = lineSymbolIndexs;
                }
            }
				
			bonusAwardLine.shouldCreateAnimation = shouldCreateAnimation;
			bonusAwardLine.awardName = awardName;
			bonusAwardLine.awardValue = awardValue;
			if (bonusAwardLine.awardValue < gameConfigs.AnimationTypeTag) {
				bonusAwardLine.CreateDefaultAnimations ((int)BaseElementPanel.AnimationID.AnimationID_NormalAward, awardElement.Count);
			} else {
				bonusAwardLine.CreateDefaultAnimations ((int)BaseElementPanel.AnimationID.AnimationID_BackFromNormalAward, awardElement.Count);
			}
			foreach (BaseElementPanel e in awardElement) {
				AwardResult.SymbolRenderIndex symbolIndex = new AwardResult.SymbolRenderIndex (e.ReelIndex, e.PositionId);
				bonusAwardLine.symbolRenderIndexs.Add (symbolIndex);
				this.AddAwardElementIndexOfDisplay (e.ReelIndex, e.PositionId);
			}
			InsertAward(bonusAwardLine);
		}
		/// <summary>
		/// 建议都用这种方式
		/// </summary>
		/// <param name="awardValue"></param>
		/// <param name="awardName"></param>
		/// <param name="awardElementPos"></param>
		/// <param name="gameConfigs"></param>
		/// <param name="lineIndex"></param>
		/// <param name="shouldCreateAnimation"></param>
		/// <param name="_payLine"></param>
		/// <param name="lineSymbolIndexs"></param>
		public void AddAwardPayLine(double awardValue, string awardName,List<SymbolRenderIndex> awardElementPos,GameConfigs gameConfigs,int lineIndex = -1,bool shouldCreateAnimation = true,PayLine _payLine=null,List<int> lineSymbolIndexs=null) {
			AwardResult.AwardPayLine bonusAwardLine = new AwardResult.AwardPayLine (lineIndex);

			if (lineIndex >= 0) {
				bonusAwardLine.payLine = _payLine;
				if (lineSymbolIndexs!=null)
				{
					bonusAwardLine.LineSymbolIndexs = lineSymbolIndexs;
				}
			}
				
			bonusAwardLine.shouldCreateAnimation = shouldCreateAnimation;
			bonusAwardLine.awardName = awardName;
			bonusAwardLine.awardValue = awardValue;
			if (bonusAwardLine.awardValue < gameConfigs.AnimationTypeTag) {
				bonusAwardLine.CreateDefaultAnimations ((int)BaseElementPanel.AnimationID.AnimationID_NormalAward, awardElementPos.Count);
			} else {
				bonusAwardLine.CreateDefaultAnimations ((int)BaseElementPanel.AnimationID.AnimationID_BackFromNormalAward, awardElementPos.Count);
			}
			
			bonusAwardLine.symbolRenderIndexs.AddRange(awardElementPos); 
			foreach (SymbolRenderIndex e in awardElementPos) {
				this.AddAwardElementIndexOfDisplay (e.reelIndex, e.symbolIndexInReel);
			}
			InsertAward(bonusAwardLine);
		}

//		public void AddAwardPayLine(AwardPayLine awardPayLine){
//			foreach (SymbolRenderIndex e in awardPayLine.symbolRenderIndexs) {
//				this.AddAwardElementIndexOfDisplay (e.reelIndex, e.symbolIndexInReel);
//			}
//			InsertAward(awardPayLine);
//		}

//		public void AddAwardPayLine(double awardValue, string awardName,List<BaseElementPanel> awardElement, List<int> animationStatus, GameConfigs gameConfigs, int lineIndex = -1, bool shouldCreateAnimation = true) {
//			if (awardElement.Count != animationStatus.Count) {
//				AddAwardPayLine(awardValue, awardName, awardElement, gameConfigs, lineIndex, shouldCreateAnimation);
//				return;
//			}
//			AwardResult.AwardPayLine bounusAwardLine = new AwardResult.AwardPayLine (lineIndex);
//			bounusAwardLine.shouldCreateAnimation = shouldCreateAnimation;
//			bounusAwardLine.awardName = awardName;
//			bounusAwardLine.awardValue = awardValue;
//			for (int i = 0; i < animationStatus.Count; i++) {
//				if (animationStatus[i] == 0) {
//					if (bounusAwardLine.awardValue < gameConfigs.AnimationTypeTag) {
//						animationStatus[i] = 1;
//					} else {
//						animationStatus[i] = 2;
//					}
//				}
//			}
//			bounusAwardLine.CreateDefaultAnimations(animationStatus);
//			foreach (BaseElementPanel e in awardElement) {
//				AwardResult.SymbolRenderIndex symbolIndex = new AwardResult.SymbolRenderIndex (e.ReelIndex, e.PositionId);
//				bounusAwardLine.symbolRenderIndexs.Add (symbolIndex);
//				this.AddAwardElementIndexOfDisplay (e.ReelIndex, e.PositionId);
//			}
//			InsertAward(bounusAwardLine);
//
//		}

//        public void SortAwardLine()
//        {
//            for (int i = 0; i < this.awardInfos.Count - 1; i++)
//            {
//                for (int j = i + 1; j < this.awardInfos.Count; j++)
//                {
//                    if (this.awardInfos[i].awardValue < this.awardInfos[j].awardValue)
//                    {
//                        AwardResult.AwardPayLine tempAwardLine = this.awardInfos[i];
//                        this.awardInfos[i] = this.awardInfos[j];
//                        this.awardInfos[j] = tempAwardLine;
//                    }
//                }
//            }
//        }

	    void InsertAward(AwardResult.AwardPayLine bounusAwardLine)
	    {
		    int index = 0;
		    //Payline显示排序：
		    //AwardValue值高的优先队首，AwardValue相同判断LineSymbolIndexs
		    //LineSymbolIndexs数量多的优先队首，LineSymbolIndexs数量相同判断SymbolIndex
		    //SymbolIndex值小的优先队首
		    //同值按入队先后顺序排序，异常奖励放最后
		    if(!BaseGameConsole.singletonInstance.isForTestScene)
		    {
			    for (int n = 0; n < this.awardInfos.Count; n++)
			    {
				    if (this.awardInfos[n].awardValue < bounusAwardLine.awardValue)
					    break;
				    else if (this.awardInfos[n].awardValue == bounusAwardLine.awardValue)
				    {
					    if (this.awardInfos[n].LineSymbolIndexs.Count < bounusAwardLine.LineSymbolIndexs.Count)
						    break;
					    else if (this.awardInfos[n].LineSymbolIndexs.Count == bounusAwardLine.LineSymbolIndexs.Count)
					    {
						    if (this.awardInfos[n].awardName.Contains("_") && bounusAwardLine.awardName.Contains("_"))
						    {
							    var targetSymbolName = bounusAwardLine.awardName.Split('_')[0];
							    var compareSymbolName = this.awardInfos[n].awardName.Split('_')[0];
							    var targetSymbolIndex =
								    BaseSlotMachineController.Instance.reelManager.symbolMap.getSymbolIndex(
									    targetSymbolName);
							    var compareSymbolIndex =
								    BaseSlotMachineController.Instance.reelManager.symbolMap.getSymbolIndex(
									    compareSymbolName);
							    if (targetSymbolIndex < compareSymbolIndex)
								    break;
							    else
								    index++;
						    }
						    else
							    index++;
					    }
					    else
						    index++;
				    }
				    else
					    index++;
			    }
		    }
		    this.awardInfos.Insert(index, bounusAwardLine);
		    this.awardValue += bounusAwardLine.awardValue;
	    }

        public void RemoveAwardPayLine(AwardPayLine payLine) {
            if (payLine != null && awardInfos.Contains(payLine)) {
                if (payLine.symbolRenderIndexs != null && payLine.symbolRenderIndexs.Count > 0) {
                    foreach(SymbolRenderIndex symbolIndex in payLine.symbolRenderIndexs) {
                        this.RemoveAwardElementIndexOfDisplay(symbolIndex.reelIndex, symbolIndex.symbolIndexInReel);
                    }
                }

                this.awardValue -= payLine.awardValue;
                awardInfos.Remove(payLine);
            }
        }

        public void RemoveAllAwardPayLines() {
            if (awardInfos != null && awardInfos.Count > 0) {
                for (int i = awardInfos.Count - 1; i >= 0; i--) {
                    RemoveAwardPayLine(awardInfos[i]);
                }
            }
        }

        public int MaxKeyForAwardElementIndexOfDisplayDict() {
            int maxKey = 0;
            if (awardElementIndexOfDisplayCountDict != null) {
                foreach (int key in awardElementIndexOfDisplayCountDict.Keys) {                    
                    List<int> awardElementIndexList = GetAwardElementIndexList (key);
                    if (awardElementIndexList != null && awardElementIndexList.Count > 0) {
                        if (key > maxKey) {
                            maxKey = key;
                        }
                    }
                }
            }
            return maxKey;
        }

        public bool IsLastReelForAward(int reelIndex) {
            return reelIndex == MaxKeyForAwardElementIndexOfDisplayDict ();
        }

        public class AwardPayLine:BaseAward
        {
            public enum LineType {
                MultiWay = -1,
                BonusWin = -2,
                ScatterWin = -3
            } 
            public List<bool> awardPosition = new List<bool>();
            public List<int> awardPlayAnimations = new List<int>();
			public List<SymbolRenderIndex> symbolRenderIndexs = new List<SymbolRenderIndex> ();
            public int awardNumberCount =0;
			public bool shouldCreateAnimation = true;
			public List<int> LineSymbolIndexs = new List<int>(); //resultContent
			public PayLine payLine;
            public void CreateDefaultAnimations(int value, int length = 0)
            {
                awardPlayAnimations.Clear ();
                if (length == 0) {
                    length = awardNumberCount;
                }
                for (int i = 0; i < length; i++) {
                    awardPlayAnimations.Add (value);
                }
            }

			public void CreateDefaultAnimations(List<int> values) {
				awardPlayAnimations.Clear();
				for (int i = 0; i < values.Count; i++) {
					awardPlayAnimations.Add(values[i]);
				}
			}

            public int LineIndex {
                get;
                set;
            }

            public AwardPayLine (int LineIndex)
            {
                this.LineIndex = LineIndex;
            }

            public string ToDataString()
            {
	            string renderIdxs = "";
	            foreach (var ele in symbolRenderIndexs)
	            {
		            renderIdxs += "; reelIdx:" + ele.reelIndex + " postionID;" + ele.symbolIndexInReel + ";";
	            }
	            return "LineIndex : " + LineIndex + " LineSymbolIndexs: " + MiniJSON.Json.Serialize(LineSymbolIndexs) +
	                   " symbolRenderIndexs: "+renderIdxs+" awardName:"+awardName+" awardValue:"+awardValue;
            }

        }

		public class SymbolRenderIndex{
			public int reelIndex;
			public int symbolIndexInReel;
			public SymbolRenderIndex(int reelIndex,int symbolIndexInReel){
				this.reelIndex =reelIndex;
				this.symbolIndexInReel = symbolIndexInReel;
			}
		}
    }

    public class BaseAward:IAwardResult
    {
        public string awardName {
            get;
            set;
        }

        public double awardValue {
            get;
            set;
        }
        public int awardCashValue {
	        get;
	        set;
        }
        public bool isAwarded {
            get {
                return awardValue > 0;
            }
        }
    }

    public interface IAwardResult
    {
        string awardName {
            get;
            set;
        }

        double awardValue {
            get;
            set;
        }

        bool isAwarded {
            get ;
        }
    }
}
