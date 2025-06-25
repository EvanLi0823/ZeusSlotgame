using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Utils;

namespace Classic
{
    public class PaytableElement : BasePaytableElement
	{
		public bool isContinuity = false;
		public bool isFixed = false;
		public bool isScatter = false;
		public bool isInOrder = false;
		public bool IsAnyLineStart = false;
		public bool IsAnyWildFillAndContinuty = false;
		public PaytableElement (SymbolMap symbolMap, Dictionary<string,object> info) 
			: base (symbolMap, info)
		{
			if (info.ContainsKey (ClassicPaytable.IS_CONTINUITY)) {
				isContinuity = (bool)info [ClassicPaytable.IS_CONTINUITY];
			} else {
				isContinuity = false;
			}

			if (info.ContainsKey (ClassicPaytable.IS_FIXED)) {
				isFixed = (bool)info [ClassicPaytable.IS_FIXED];
			} else {
				isFixed = false;
			}

			if (info.ContainsKey (ClassicPaytable.IS_SCATTER)) {
				isScatter = (bool)info [ClassicPaytable.IS_SCATTER];
			} else {
				isScatter = false;
			}

			if (info.ContainsKey (ClassicPaytable.IS_IN_ORDER)) {
				isInOrder = (bool)info [ClassicPaytable.IS_IN_ORDER];
			} else {
				isInOrder = false;
			}

			if (info.ContainsKey (ClassicPaytable.IS_ANY_START)) {
				IsAnyLineStart = (bool)info [ClassicPaytable.IS_ANY_START];
			} else {
				IsAnyLineStart = false;
			}
			if (info.ContainsKey(ClassicPaytable.IS_ANY_WILD_FILL_AND_CONTINUTY)) {
				IsAnyWildFillAndContinuty =(bool)info [ClassicPaytable.IS_ANY_WILD_FILL_AND_CONTINUTY];
			} else {
				IsAnyWildFillAndContinuty = false;
			}
		}
	
        public float CheckAwardSingle (SymbolMap symbolMap, List<int> results, int count, bool allIsWild, out List<bool> awardPosition, out string awardName,bool wildAccumulation)
		{
			awardPosition = new List<bool> ();
			awardName = "";
			if (results == null || results.Count == 0 || symbolList.Count == 0 || awardMap.Count == 0)
				return 0;
		

			SymbolMap.SymbolElementInfo info1 = symbolMap.getSymbolInfo (symbolList [0]);
			if (info1.isWild ) {
				int countWild = 0;
				for (int i=0; i<results.Count; i++) {
					SymbolMap.SymbolElementInfo info = symbolMap.getSymbolInfo (results [i]);
                    if (info.isWild && (info1.Index == info.Index || (info.getIntValue(SymbolMap.SAME_AS_SYMBOL, -2) == info1.Index))) {
						countWild++;
					} else {
						break;
					}
				}
				if (awardMap.ContainsKey (countWild)) {
					for (int i = 0; i < countWild; i++) {
						awardPosition.Add (true);
					}
					for (int i = countWild; i < results.Count; i++) {
						awardPosition.Add (false);
					}
					awardName = awardNames [countWild];
					return awardMap [countWild];
				}
			}
		
			float mutiper = 1;
		
			float CurrentAward = 0;
			int maxInSymbolList = 0;

			bool hasNormalSymbol = false;

			for (int i=0; i<count; i++) {
				SymbolMap.SymbolElementInfo info = symbolMap.getSymbolInfo (results [i]);
				if (info.isWild) {
					awardPosition.Add (true);
					maxInSymbolList += info.Weight; 
					if (awardMap.ContainsKey (maxInSymbolList)) {
						CurrentAward = awardMap [maxInSymbolList];
						awardName = awardNames [maxInSymbolList];
					}
					if (wildAccumulation || mutiper == 1) {
						mutiper *= info.Mutipler;
					}
				} else if (symbolList.Contains (info.Index)) {
					awardPosition.Add (true);
					maxInSymbolList += info.Weight; 
					if (awardMap.ContainsKey (maxInSymbolList)) {
						CurrentAward = awardMap [maxInSymbolList];
						awardName = awardNames [maxInSymbolList];
					}
					hasNormalSymbol = true;
					mutiper *= info.Mutipler == 0 ? 1 :info.Mutipler;
				} else {
					awardPosition.Add (false);
				}
			}

			for (int i=count; i<results.Count; i++) {
				awardPosition.Add (false);
			}

			if (hasNormalSymbol) {
				return	CurrentAward * mutiper;
			} else {
				return 0;
			}
		}
	
        public float CheckAward (SymbolMap symbolMap, List<int> results, bool allIsSame, bool allIsWild, bool hasNormal, out List<bool> awardPosition, out string awardName,bool wildAccumulation)
		{
			awardPosition = new List<bool> ();
			awardName = "";
			if (results == null || results.Count == 0 || symbolList.Count == 0 || awardMap.Count == 0)
				return 0;
			int maxInSymbolList = 0;
			float CurrentAward = 0;

			SymbolMap.SymbolElementInfo info1 = symbolMap.getSymbolInfo (symbolList [0]);
			if (info1.isWild) {
				for (int i=0; i<results.Count; i++) {
					SymbolMap.SymbolElementInfo info = symbolMap.getSymbolInfo (results [i]);
					if (symbolList.Contains (info.Index)) {
						awardPosition.Add (true);
						maxInSymbolList ++; 
						if (awardMap.ContainsKey (maxInSymbolList)) {
							CurrentAward = awardMap [maxInSymbolList];
							awardName = awardNames [maxInSymbolList];
						}
					} else {
						awardPosition.Add (false);
					}
				}
				return CurrentAward;
			} else {
				float mutiper = 1;
				CurrentAward = 0;
				maxInSymbolList = 0;

				bool hasNormalSymbol = false;
		
				for (int i=0; i<results.Count; i++) {
					SymbolMap.SymbolElementInfo info = symbolMap.getSymbolInfo (results [i]);
					if (info.isWild) {
						awardPosition.Add (true);
						maxInSymbolList += info.Weight; 
						if (awardMap.ContainsKey (maxInSymbolList)) {
							CurrentAward = awardMap [maxInSymbolList];
							awardName = awardNames [maxInSymbolList];
						}
                        if (wildAccumulation || mutiper == 1) {
                            mutiper *= info.Mutipler;
                        }
					} else if (symbolList.Contains (info.Index)) {
						awardPosition.Add (true);
						maxInSymbolList += info.Weight; 
						if (awardMap.ContainsKey (maxInSymbolList)) {
							CurrentAward = awardMap [maxInSymbolList];
							awardName = awardNames [maxInSymbolList];
						}
						hasNormalSymbol = true;
						mutiper *= info.Mutipler == 0 ? 1 :info.Mutipler;
					} else {
						awardPosition.Add (false);
					}
				}
		
				if (!hasNormalSymbol)
					return 0;
		
				return CurrentAward * mutiper;
			}
		}

        public float CheckAwardContinue (SymbolMap symbolMap, List<int> results, bool allIsSame, bool allIsWild, bool hasNormal, out List<bool> awardPosition, out string awardName,bool wildAccumulation)
		{
			awardPosition = new List<bool> ();
			awardName = "";
			if (results == null || results.Count == 0 || symbolList.Count == 0 || awardMap.Count == 0)
				return 0;
			if (allIsWild) {
				SymbolMap.SymbolElementInfo info = symbolMap.getSymbolInfo (symbolList [0]);
				if (info.isWild && awardMap.ContainsKey (results.Count)) {
					for (int i=0; i<symbolList.Count; i++) {
						if(symbolList [i] ==  results[i]){
							awardPosition.Add (true);
						}else{
							return 0;
						}
					}
					awardName = awardNames [symbolList.Count];
					return awardMap [symbolList.Count];
				}
			}
			
			if (!hasNormal)
				return 0;


			if (this.IsAnyLineStart) {
				return CheckAnyStart (symbolMap, results,  out  awardPosition, out awardName, wildAccumulation);
			} else {
				int index;
				return  ContinueAwardNoAllWild (symbolMap, results, out awardPosition, out awardName, wildAccumulation,out index);
			}
		}


		public float CheckAwardAnyWildFillAndContinue (SymbolMap symbolMap, List<int> results, bool allIsSame, bool allIsWild, bool hasNormal, out List<bool> awardPosition, out string awardName,bool wildAccumulation)
		{
			awardPosition = new List<bool> ();
			awardName = "";
			if (results == null || results.Count == 0 || symbolList.Count == 0 || awardMap.Count == 0)
				return 0;
			int maxInSymbolList = 0;
			float CurrentAward = 0;
			bool isContinue = true;
			SymbolMap.SymbolElementInfo info1 = symbolMap.getSymbolInfo (symbolList [0]);
			if (info1.isWild) {
				for (int i=0; i<results.Count; i++) {
					SymbolMap.SymbolElementInfo info = symbolMap.getSymbolInfo (results [i]);
					if (isContinue&&symbolList.Contains (info.Index)) {
						awardPosition.Add (true);
						maxInSymbolList ++; 
						if (awardMap.ContainsKey (maxInSymbolList)) {
							CurrentAward = awardMap [maxInSymbolList];
							awardName = awardNames [maxInSymbolList];
						}
					} else {
						awardPosition.Add (false);
						isContinue = false;
					}
				}
				return CurrentAward;
			} 
			else {
				return 0;
			}
		}

		private float CheckAnyStart(SymbolMap symbolMap, List<int> results, out List<bool> awardPosition, out string awardName,bool wildAccumulation)
		{			
			awardPosition = new List<bool> ();
			awardName = "";
			float result = 0f;

			int count = results.Count;
			for (int i = 0; i < count ; ) {
				List<bool> tmpAwardPosition;
				string tmpAwardName;

				if (this.symbolList.Contains (results [i]) || symbolMap.getSymbolInfo (results [i]).isWild) {
					float tmpResult = ContinueAwardNoAllWild (symbolMap, results, out tmpAwardPosition, out tmpAwardName, wildAccumulation, out i,i);
					if (tmpResult > result) {
						result = tmpResult;
						awardPosition = tmpAwardPosition;
						awardName = tmpAwardName;
					}
				} else {
					i++;
				}
			}
			return result;
		}

		private float ContinueAwardNoAllWild(SymbolMap symbolMap, List<int> results, out List<bool> awardPosition, out string awardName,bool wildAccumulation,out int startAwardIndex,int currentAwardIndex=0)
		{
			awardPosition = new List<bool> ();
			awardName = "";

			float mutiper = 1;

			float CurrentAward = 0;

			int maxInSymbolList = 0;

			bool hasNormalSymbol = false;
			int index = currentAwardIndex;

			if (currentAwardIndex != 0) {
				for (int i=0; i<currentAwardIndex; i++) {
					awardPosition.Add (false);
				}
			}

			for (int i=currentAwardIndex; i<results.Count; i++) {
				SymbolMap.SymbolElementInfo info = symbolMap.getSymbolInfo (results [i]);

				if (info.isWild) {
					index++;
					awardPosition.Add (true);
					maxInSymbolList += info.Weight; 
					if (wildAccumulation || mutiper == 1) {
						mutiper *= info.Mutipler;
					}
					if (awardMap.ContainsKey (maxInSymbolList)) {
						CurrentAward = awardMap [maxInSymbolList];
						awardName = awardNames [maxInSymbolList];
					}
				} else if (symbolList.Contains (info.Index)) {
					index ++;
					awardPosition.Add (true);
					maxInSymbolList += info.Weight; 
					hasNormalSymbol = true;
					mutiper *= info.Mutipler == 0 ? 1 :info.Mutipler;
					if (awardMap.ContainsKey (maxInSymbolList)) {
						CurrentAward = awardMap [maxInSymbolList];
						awardName = awardNames [maxInSymbolList];
					}
				} else {
					break;
				}
			}

			for (int i=index; i<results.Count; i++) {
				awardPosition.Add (false);
			}

			startAwardIndex = index;

			if (!hasNormalSymbol)
				return 0;

			return CurrentAward * mutiper;

		}
	
        public float CheckFixed (SymbolMap symbolMap, List<int> results, bool allIsSame, bool allIsWild, bool hasNormal, out List<bool> awardPosition, out string awardName,bool wildAccumulation)
		{
			awardPosition = new List<bool> ();
			awardName = "";
			if (results == null || results.Count == 0 || symbolList.Count == 0 || awardMap.Count == 0)
				return 0;
			int maxInSymbolList = 0;
			float CurrentAward = 0;
			
			SymbolMap.SymbolElementInfo info1 = symbolMap.getSymbolInfo (symbolList [0]);
			if (info1.isWild) {
				List<int> tempSymbolList = new List<int> ();
				
				for (int i=0; i<symbolList.Count; i++) {
					tempSymbolList.Add (symbolList [i]);
				}
				
				for (int i=0; i< results.Count; i++) {
					if (tempSymbolList.Contains (results [i])) {
						awardPosition.Add (true);
						tempSymbolList.Remove (results [i]);
						maxInSymbolList ++;
						if (awardMap.ContainsKey (maxInSymbolList)) {
							CurrentAward = awardMap [maxInSymbolList];
							awardName = awardNames [maxInSymbolList];
						}
					} else {
						awardPosition.Add (false);
					}
				}

				if(CurrentAward>0){
					for(int i= maxInSymbolList;i<results.Count;i++){
						if (symbolList.Contains (results [i])) {
							awardPosition.Add (true);
						}else{
							awardPosition.Add (false);
						}
					}
				}
				return CurrentAward;
			} else {
				float mutiper = 1;
				CurrentAward = 0;
				maxInSymbolList = 0;
				int wildNumber = 0;
				
				bool hasNormalSymbol = false;

				List<int> tempSymbolList = new List<int> ();
				
				for (int i=0; i<symbolList.Count; i++) {
					tempSymbolList.Add (symbolList [i]);
				}

				if (isInOrder) {
					for (int i=0; i<results.Count; i++) {
						SymbolMap.SymbolElementInfo info = symbolMap.getSymbolInfo (results [i]);
						if (info.isWild) {
							awardPosition.Add (true);
							maxInSymbolList += info.Weight; 
							wildNumber ++;
							if (wildAccumulation || mutiper == 1) {
								mutiper *= info.Mutipler;
							}
							if (tempSymbolList.Count > 0) {
								tempSymbolList.RemoveAt (0);
							}
						} else if (tempSymbolList.Count >0 && tempSymbolList[0].Equals (info.Index)) {
							awardPosition.Add (true);
							maxInSymbolList += info.Weight; 
							tempSymbolList.RemoveAt (0);
							hasNormalSymbol = true;
							mutiper *= info.Mutipler == 0 ? 1 :info.Mutipler;
						} else {
							awardPosition.Add (false);
						}
					}
					if (0 == tempSymbolList.Count) {
						if (awardMap.ContainsKey (maxInSymbolList)) {
							CurrentAward = awardMap [maxInSymbolList];
							awardName = awardNames [maxInSymbolList];
						} else {
							return 0;
						}
					} else {
						return 0;
					}
				} else {
					for (int i=0; i<results.Count; i++) {
						SymbolMap.SymbolElementInfo info = symbolMap.getSymbolInfo (results [i]);
						if (info.isWild) {
							awardPosition.Add (true);
							maxInSymbolList += info.Weight; 
							wildNumber ++;
							if (wildAccumulation || mutiper == 1) {
								mutiper *= info.Mutipler;
							}
						} else if (tempSymbolList.Contains (info.Index)) {
							awardPosition.Add (true);
							maxInSymbolList += info.Weight; 
							tempSymbolList.Remove (info.Index);
							hasNormalSymbol = true;
							mutiper *= info.Mutipler == 0 ? 1 :info.Mutipler;
						} else {
							awardPosition.Add (false);
						}
					}
					if (wildNumber == tempSymbolList.Count) {
						if (awardMap.ContainsKey (maxInSymbolList)) {
							CurrentAward = awardMap [maxInSymbolList];
							awardName = awardNames [maxInSymbolList];
						} else {
							return 0;
						}
					} else {
						return 0;
					}
				}
				
				if (!hasNormalSymbol)
					return 0;
				
				return CurrentAward * mutiper;
			}
		}

		public bool SigleElement ()
		{
			return symbolList.Count == 1;
		}
	
		public int GetFirstSymbol ()
		{
			return symbolList [0];
		}

		public bool IsContainsCount(int count)
		{
			return awardMap.ContainsKey (count);
		}
		public float GetAwardMapValue(int count)
		{
			if (awardMap.ContainsKey (count)) {
				return awardMap [count];
			}
			return 0f;
		}
		public string GetAwardName(int count)
		{
			if (awardNames.ContainsKey (count)) {
				return awardNames [count];
			}
			return "";
		}
	}
}