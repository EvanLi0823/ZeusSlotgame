using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Utils;
using Core;
namespace Classic
{
	public class ClassicPaytable
	{
		public readonly static string PAY_TABLE = "PayTable";
		public readonly static string BLACK_OUT_PAYTABLE = "BlackOutPayTable";
		public readonly static string SYMBOL_LIST = "SymbolList";
		public readonly static string AWARD_MAP = "AwardMap";
		public readonly static string IS_CONTINUITY = "IsContinuity";
		public readonly static string IS_SCATTER = "IsScatter";
		public readonly static string IS_IN_ORDER = "IsInOrder";
		public readonly static string AwardName = "AwardName";
		public readonly static string IS_FIXED = "IsFixed";
		public readonly static string WAY_PAY_TABLE = "WayPayTable";
		public readonly static string IS_ANY_START = "IsAnyStart";
		public readonly static string IS_ANY_WILD_FILL_AND_CONTINUTY="IsAnyWildFillAndContinuty";
		List<BlackOutPaytableElement> blackOutPaytable = new List<BlackOutPaytableElement> ();
		List<PaytableElement> paysWithSymbols = new List<PaytableElement> ();
		public List<PaytableElement> scatterPays = new List<PaytableElement> ();
		public Dictionary<int,PaytableElement> singleSymbolPaysCon = new Dictionary<int, PaytableElement> ();
		public Dictionary<int,PaytableElement> singleSymbolPaysNoCon = new Dictionary<int, PaytableElement> ();
		public Dictionary<int,PaytableElement> singleSymbolPaysConAnyStart = new Dictionary<int, PaytableElement> ();
		private SymbolMap symbolMap;
		bool hasContinuity = false;

		public ClassicPaytable (SymbolMap symbolMap, List<object> infos, List<object> blackOutInfos)
		{
			this.symbolMap = symbolMap;
			if (infos == null)
				return;

			for (int i=0; i<infos.Count; i++) {
				PaytableElement paytableElement = new PaytableElement (this.symbolMap, infos [i] as Dictionary<string,object>);
				if (paytableElement.isScatter) {
					scatterPays.Add (paytableElement);
				} else {
					if (paytableElement.SigleElement ()) {
						if (paytableElement.isContinuity) {
							hasContinuity = true;
							singleSymbolPaysCon.Add (paytableElement.GetFirstSymbol (), paytableElement);
							if (paytableElement.IsAnyLineStart) {
								this.singleSymbolPaysConAnyStart.Add(paytableElement.GetFirstSymbol (),paytableElement);
							}
						} else {
							singleSymbolPaysNoCon.Add (paytableElement.GetFirstSymbol (), paytableElement);
						}
					} else {
						paysWithSymbols.Add (paytableElement);
					}
				}
			}

			if (blackOutInfos != null) {
				for (int i=0; i<blackOutInfos.Count; i++) {
					BlackOutPaytableElement paytableElement = new BlackOutPaytableElement (this.symbolMap, blackOutInfos [i] as Dictionary<string,object>);
					blackOutPaytable.Add (paytableElement);
				}
			}
		}

		public void CreateBlackAward (ResultContent resultContent, BaseAward blackAward)
		{
			blackAward.awardValue = 0;
			int temp = 0;

			for (int i=0; i<blackOutPaytable.Count; i++) {
				string awardName;
				temp = blackOutPaytable [i].CheckAward (symbolMap, resultContent, out awardName);
				if (temp > blackAward.awardValue) {
					blackAward.awardValue = temp;
					blackAward.awardName = awardName;
				}
			}
		}

		public AwardResult.AwardPayLine CreateAwardResult (int lineNumber, List<int> results,bool wildAccumulation, PayLine _payLine )
		{
            for(int i = 0; i < results.Count;i++)
            {
                SymbolMap.SymbolElementInfo info1 = symbolMap.getSymbolInfo(results[i]);
                int sameAsIndex = -100;
                if (info1 != null)
                {
	                sameAsIndex = info1.getIntValue(SymbolMap.SAME_AS_SYMBOL, -100);
                }
                

                if(sameAsIndex >=0)
                {
                    results[i] = sameAsIndex;
                }
            }

            float lastAward = 0;
			float CurrentAward = 0;
			AwardResult.AwardPayLine awardResultElement = new AwardResult.AwardPayLine (lineNumber);
			awardResultElement.payLine = _payLine;
			awardResultElement.LineSymbolIndexs = results;

			if (results != null && results.Count > 0) {
				int symbolIndex = -1;
				bool allIsSame = AllIsSame (results, out symbolIndex);
				int blankNumber = getBlankNumber (results);
				int wildNumer = getWildNumber (results);
				int normalNumber = results.Count - wildNumer - blankNumber;
				bool allIsWild = (wildNumer == results.Count);
				bool hasNormal = (normalNumber > 0);
				if (hasContinuity) {
					if (singleSymbolPaysCon.Count > 0 ) {
						int continiutySymbol = -1;
						int countNumber = ContiniutySame (results, out continiutySymbol);

						// 相同的symbol起点在别的地方
						if (this.singleSymbolPaysConAnyStart.Count > 0) {
							foreach (PaytableElement paytableElement in singleSymbolPaysConAnyStart.Values) {
								if (results.Contains(paytableElement.GetFirstSymbol ()) ){
									List<bool> tempList;
									string awardName;
									CurrentAward = paytableElement.CheckAwardContinue (symbolMap, results, allIsSame, allIsWild, hasNormal, out tempList, out awardName, wildAccumulation);

									if (CurrentAward > lastAward) {
										lastAward = CurrentAward;
										awardResultElement.awardPosition = tempList;
										awardResultElement.awardName = awardName;
									}
								}
							}
						}

						if (countNumber > 0) {
							SymbolMap.SymbolElementInfo info1 = symbolMap.getSymbolInfo (results [0]);
                            int sameAsIndex = info1.getIntValue(SymbolMap.SAME_AS_SYMBOL, -100);
							if (info1.isWild) {
                                if (singleSymbolPaysCon.ContainsKey (results [0])||singleSymbolPaysCon.ContainsKey(sameAsIndex)) {
                                    int awardIndex = results[0];
                                    if(singleSymbolPaysCon.ContainsKey(sameAsIndex) && !singleSymbolPaysCon.ContainsKey(results[0])){
                                        awardIndex = sameAsIndex;
                                    }
                                    PaytableElement paytableElementWild = singleSymbolPaysCon [awardIndex];
									List<bool> tempList1;
									string awardName2;
									CurrentAward = paytableElementWild.CheckAwardSingle (this.symbolMap, results, countNumber
										, allIsWild, out tempList1, out awardName2, wildAccumulation);
									if (CurrentAward > lastAward) {
										lastAward = CurrentAward;
										awardResultElement.awardPosition = tempList1;
										awardResultElement.awardValue = lastAward;
										awardResultElement.awardName = awardName2;
									}
								}
							}

							if (singleSymbolPaysCon.ContainsKey (continiutySymbol)) {
								List<bool> tempList;
								string awardName;
								PaytableElement paytableElement = singleSymbolPaysCon [continiutySymbol];
								CurrentAward = paytableElement.CheckAwardSingle (this.symbolMap, results, countNumber
                                , allIsWild, out tempList, out awardName, wildAccumulation);
								if (CurrentAward > lastAward) {
									lastAward = CurrentAward;
									awardResultElement.awardPosition = tempList;
									awardResultElement.awardValue = lastAward;
									awardResultElement.awardName = awardName;
									//return awardResultElement;
								}
							}

						}

					}
				}

				if (allIsSame) {
					if (singleSymbolPaysNoCon.ContainsKey (symbolIndex)) {
						List<bool> tempList;
						string awardName;
						PaytableElement paytableElement = singleSymbolPaysNoCon [symbolIndex];
						CurrentAward = paytableElement.CheckAwardSingle (this.symbolMap, results, results.Count
                            , allIsWild, out tempList, out awardName,wildAccumulation);
						if (CurrentAward > lastAward) {
							lastAward = CurrentAward;
							awardResultElement.awardPosition = tempList;
							awardResultElement.awardValue = lastAward;
							awardResultElement.awardName = awardName;
							//return awardResultElement;
						}
					}
				}


				foreach (PaytableElement paytableElement in singleSymbolPaysNoCon.Values) {
					List<bool> tempList;
					string awardName;
                    CurrentAward = paytableElement.CheckAward (symbolMap, results, allIsSame, allIsWild, hasNormal, out tempList, out awardName,wildAccumulation);
				
					if (CurrentAward > lastAward) {
						lastAward = CurrentAward;
						awardResultElement.awardPosition = tempList;
						awardResultElement.awardName = awardName;
					}
				}

				for (int i=0; i<paysWithSymbols.Count; i++) {
					List<bool> tempList;
					string awardName;
					if (paysWithSymbols [i].isContinuity) {
                        CurrentAward = paysWithSymbols [i].CheckAwardContinue (symbolMap, results, allIsSame, allIsWild, hasNormal, out tempList, out awardName,wildAccumulation);
					} else {
						if (paysWithSymbols [i].isFixed) {
                            CurrentAward = paysWithSymbols [i].CheckFixed(symbolMap, results, allIsSame, allIsWild, hasNormal, out tempList, out awardName,wildAccumulation);
						} 
						else if (paysWithSymbols[i].IsAnyWildFillAndContinuty) {
							CurrentAward= paysWithSymbols [i].CheckAwardAnyWildFillAndContinue (symbolMap, results, allIsSame, allIsWild, hasNormal, out tempList, out awardName,wildAccumulation);
						}
						else {
                            CurrentAward = paysWithSymbols [i].CheckAward (symbolMap, results, allIsSame, allIsWild, hasNormal, out tempList, out awardName,wildAccumulation);
						}
					}

					if (CurrentAward > lastAward) {
						lastAward = CurrentAward;
						awardResultElement.awardPosition = tempList;
						awardResultElement.awardName = awardName;
					}
				}
			}
			awardResultElement.awardValue = lastAward;
			return awardResultElement;
		}

		public int ContiniutySame (List<int> results, out int symbol)
		{
			symbol = -1;
			if (results == null || results.Count == 0)
				return 0;
			
			bool isHasNorml = false;
			int lastIndex = -1;
			int symbolIndex = -1;
			symbol = lastIndex;
			int count = 0;
			for (int i=0; i<results.Count; i++) {
				symbolIndex = results [i];
				if (symbolIndex < 0) {
					return count;
				}
				SymbolMap.SymbolElementInfo info = symbolMap.getSymbolInfo (symbolIndex);
				
				if (info!=null && info.isWild) {
					if (!isHasNorml) {
						symbol = symbolIndex;
					}
					count ++;
					continue;
				}
				
				if (isHasNorml) {
					if (lastIndex != symbolIndex) {
						return count;
					} else {
						count ++;
					}
				} else {
					lastIndex = symbolIndex;
					symbol = lastIndex;
					isHasNorml = true;
					count ++;
				}
			}
			return count;
		}

		public bool AllIsSame (List<int> results, out int symbol)
		{
			symbol = -1;
			if (results == null || results.Count == 0)
				return false;

			bool isHasNorml = false;
			int lastIndex = -1;
			int symbolIndex = -1;
			symbol = lastIndex;
			for (int i=0; i<results.Count; i++) {
				symbolIndex = results [i];
				if (symbolIndex < 0) {
					return false;
				}

				if (symbolMap == null) return false;
				SymbolMap.SymbolElementInfo info = symbolMap.getSymbolInfo (symbolIndex);
				if (info == null) return false;
				if (info.isWild) {
					if (!isHasNorml) {
						lastIndex = symbolIndex;
					}
					continue;
				}

				if (isHasNorml) {
					if (lastIndex != symbolIndex) {
						symbol = lastIndex;
						return false;
					}
				} else {
					lastIndex = symbolIndex;
					symbol = lastIndex;
					isHasNorml = true;
				}
			}
			return true;
		}

		public bool AllIsWild (List<int> results)
		{
			if (results == null || results.Count == 0)
				return false;
			int symbolIndex = -1;
			for (int i=0; i<results.Count; i++) {
				symbolIndex = results [i];
				if (symbolIndex < 0) {
					return false;
				}
				SymbolMap.SymbolElementInfo info = symbolMap.getSymbolInfo (symbolIndex);
				
				if (info.isWild) {
					continue;
				} else {
					return false;
				}
			}
			return true;
		}

		private int getWildNumber (List<int> results)
		{

			if (results == null || results.Count == 0)
				return 0;

			int count = 0;

			int symbolIndex = -1;
			for (int i=0; i<results.Count; i++) {
				symbolIndex = results [i];
				if (symbolIndex < 0) {
					continue;
				}
				SymbolMap.SymbolElementInfo info = symbolMap?.getSymbolInfo (symbolIndex);
				
				if (info!=null && info.isWild) {
					count ++;
				} 
			}
			return count;
		}

		private int getBlankNumber (List<int> results)
		{
			if (results == null || results.Count == 0)
				return 0;
			
			int count = 0;
			
			int symbolIndex = -1;
			for (int i=0; i<results.Count; i++) {
				symbolIndex = results [i];
				if (symbolIndex < 0) {
					count ++;
				}
			}
			return count;
		}
			
		public float GetAwardValueBySymbolIndex(int symbolIndex,int symbolAllCount,out string awardName)
		{
			PaytableElement paytableElement = singleSymbolPaysNoCon [symbolIndex];
			if (paytableElement.IsContainsCount (symbolAllCount)) {
				awardName = paytableElement.GetAwardName (symbolAllCount);
				return paytableElement.GetAwardMapValue (symbolAllCount);
			}
			awardName = "";
			return 0f;
		}

	}	
}
