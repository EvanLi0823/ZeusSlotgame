using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
namespace Classic{
	public class ShuffleWeightedResultAtomData:ResultAtomData  {
		public readonly static string  REEL_STRIP_ATOM_DATA_CTEATE_NUM = "CycleNumber";
		public readonly static string REEL_STRIP_ATOM_DATA_LIMIT_TOTAL_NUM = "LimitTotalNum";
		public List<ShuffleWeightedResultAItem> frAList;
		public List<SymbolResult> shuffleSeqList;
		public ShuffleWeightedResultAtomData(Dictionary<string,object>info):base(info){
			if (info.ContainsKey(REEL_STRIP_ATOM_DATA_CONSIST_NODE)) {
				List<object> ls = info [REEL_STRIP_ATOM_DATA_CONSIST_NODE] as List<object>;
				frAList = new List<ShuffleWeightedResultAItem> ();
				for (int i = 0; i < ls.Count; i++) {
					ShuffleWeightedResultAItem fra = new ShuffleWeightedResultAItem (ls[i] as Dictionary<string,object>);
					frAList.Add (fra);
				}
			}

			if (info.ContainsKey(REEL_STRIP_ATOM_DATA_CTEATE_NUM)) {
				totalCount = Utils.Utilities.CastValueInt( info[REEL_STRIP_ATOM_DATA_CTEATE_NUM]);
			}
			if (info.ContainsKey(REEL_STRIP_ATOM_DATA_LIMIT_TOTAL_NUM)) {
				limitCondtionTotalNum =Utils.Utilities.CastValueInt( info[REEL_STRIP_ATOM_DATA_LIMIT_TOTAL_NUM]);
			}
			ResetReelStripShuffleSequence ();


			shuffleSeqList = new List<SymbolResult>();
		}
		public override DataFilterResult GetResultData(object info){
			DataFilterResult dr = new DataFilterResult ();
			if (shuffleSeqList.Count==0) {
				shuffleSeqList = (info as WeightedResults).GetRangeWeightedResults(shuffledSeq);
			}

			//创造返回结果
			List<List<int>> sr = null;
			if (shuffleSeqList.Count>0) {
				sr = new List<List<int>> ();
				for (int i = 0; i <  shuffleSeqList[0].symbolResults.Count; i++) {
					List<int> r = new List<int> ();
					for (int k = 0; k < shuffleSeqList[0].symbolResults[i].Count; k++) {
						r.Add (shuffleSeqList[0].symbolResults[i][k]);
					}
					sr.Add (r);
				}
				//列表移除使用过的值
				shuffleSeqList.RemoveAt (0);
				#if UNITY_EDITOR
				List<Dictionary<string,int>> wr = (info as WeightedResults).resultsTestInfo;
				foreach (string key in wr[0].Keys) {
//					Debug.Log ("Key: "+key+"  Index:"+wr[0][key]);
				}
				wr.RemoveAt (0);
				#endif
			}
			dr.selectedFixedResult = sr;
			return dr;
		}

		public List<string> shuffledSeq;
		public int totalCount;
		public int limitCondtionTotalNum;
		private void ResetReelStripShuffleSequence(){
			int totalNum = limitCondtionTotalNum;
			List<int> getValueNumList = new List<int> ();
			for (int i = 0; i < frAList.Count; i++) {
				int num = Convert.ToInt32 (UnityEngine.Random.Range (frAList [i].range.min, frAList [i].range.max+1));//前闭后开，所以加1
				if (frAList[i].isTotalNumLimit) {
					if (num>=totalNum) {
						num = totalNum;
					} 
					if (frAList[i].isLimitEnd) {
						if (num<totalNum) {
							num = totalNum;
						}
					}
					totalNum -= num;
					totalNum = Mathf.Max (0,totalNum);
				}
				getValueNumList.Add (num);
			}

			ShuffleUnit (getValueNumList);

		}

		private void ShuffleUnit(List<int> numList){
			List<int> orignSequence = new List<int> ();
			shuffledSeq = new List<string> ();
			for (int i = 0; i < totalCount; i++) {
				shuffledSeq.Add ("");
				orignSequence.Add (i);
			}
			for (int i = 0; i < totalCount; i++) {
				int index = UnityEngine.Random.RandomRange (0, orignSequence.Count);
				int startNum = numList [0];
				bool bIncrease = false;
				for (int k = 0; k < numList.Count; k++) {
					if (i < startNum) {
						int value = orignSequence [index];
						string dataID = "";
						if (bIncrease) {
							dataID =frAList[k-1].fixedResultsDataID;
						} else {
							dataID =frAList[k].fixedResultsDataID;
						}
						shuffledSeq [value] = dataID;
						orignSequence.RemoveAt (index);
						break;
					} 
					else {
						startNum += numList [k];
						bIncrease = true;
					}

					if (k == (numList.Count-1)) {
						int value = orignSequence [index];
						shuffledSeq [value] = frAList[k].fixedResultsDataID;
						orignSequence.RemoveAt (index);
					}
				}
			}
		}
		private void FillTheResults(){
			
		}
	}

	public class ShuffleWeightedResultAItem:ResultAItem{
		public readonly static string FIxED_RESULT_DATA_ID = "WeightedResultDataID";
		public readonly static string GET_VALUE_RANGE ="Range";
		public readonly static string IS_LIMIT_TOTAL_NUM = "LimitTotalNum";
		public readonly static string IS_LIMIT_ELEMENT_END = "LimitEnd";

		public RangeData range;
		public bool isTotalNumLimit;
		public bool isLimitEnd;
		public ShuffleWeightedResultAItem(Dictionary<string,object> info):base(info){

			if (info.ContainsKey(GET_VALUE_RANGE)) {
				string[] tmp = info[GET_VALUE_RANGE].ToString().Split(',');
				int min=Utils.Utilities.CastValueInt (tmp[0]);
				int max = Utils.Utilities.CastValueInt (tmp[1]);
				range = new RangeData (min,max);
			}
			if (info.ContainsKey(IS_LIMIT_TOTAL_NUM)) {
				isTotalNumLimit = Utils.Utilities.CastValueBool( info[IS_LIMIT_TOTAL_NUM]);
			}

			if (info.ContainsKey(IS_LIMIT_ELEMENT_END)) {
				isLimitEnd = Utils.Utilities.CastValueBool( info[IS_LIMIT_ELEMENT_END]);

			}
		}
	}
}
	
