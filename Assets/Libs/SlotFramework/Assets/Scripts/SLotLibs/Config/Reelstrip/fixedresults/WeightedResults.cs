using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
namespace Classic{
	public class WeightedResults {
		public readonly static string DATA_INGREDIENT_WEIGHTED_RESULTS = "WeightedResultData";
		public Dictionary<string,List<SymbolResult>> resultsList;
		#if UNITY_EDITOR
		public List<Dictionary<string,int>> resultsTestInfo = new List<Dictionary<string, int>> ();
		#endif
		public WeightedResults(SymbolMap symbolMap,Dictionary<string,object> info)
		{
				resultsList = new Dictionary<string, List<SymbolResult>> ();
				foreach (string key in info.Keys) {
					List<object> kItem = info[key] as List<object>;
					List<SymbolResult> sRList = new List<SymbolResult> ();
					for (int i = 0; i < kItem.Count; i++) {
						SymbolResult sR = new SymbolResult (symbolMap, kItem [i] as Dictionary<string,object>);
						sRList.Add (sR);
					}
					resultsList.Add (key, sRList);
				}
		}

		public List<SymbolResult> GetWeightedResults(string weightResultKey,int genNum ){
			if (resultsList==null) {
				return null;
			}
			List<SymbolResult> results = resultsList [weightResultKey];
			if (results==null) {
				return null;
			}
			List<SymbolResult> temp = new List<SymbolResult> ();

			for (int k = 0; k < genNum; k++) {
				
				//Weight Random
				int totalWeight = 0;
				for (int i = 0; i < results.Count; i++) {
					totalWeight += results [i].weight;
				}

				int randomValue = UnityEngine.Random.Range (0, totalWeight);
				int StartWeight = 0;

				for (int i = 0; i < results.Count; i++) {
					int nextWeightBeginWeight = results [i].weight + StartWeight;
					if (randomValue >= StartWeight && randomValue < nextWeightBeginWeight) {
						temp.Add (results [i]);
						break;
					} 
					else {
						StartWeight = nextWeightBeginWeight;
					}
				}
			}
			return temp;
		}
		public List<SymbolResult> GetRangeWeightedResults(List<string>keyList){
			if (resultsList==null) {
				return null;
			}
			List<SymbolResult> temp = new List<SymbolResult> ();
			Dictionary<string,List<int>> idList = new Dictionary<string, List<int>> ();
			List<int> seq = new List<int> ();
			for (int i = 0; i < keyList.Count; i++) {
				seq.Add (i);
			}
			#if UNITY_EDITOR
			resultsTestInfo.Clear();
			#endif
			for (int k = 0; k < keyList.Count; k++) {
				string key = keyList [k];
				if (idList.ContainsKey(key)&&idList[key].Count==0) {
					idList.Remove (key);
				}
				if (!idList.ContainsKey(key)) {
					List<int> ids = new List<int> ();
					for (int i = 0; i < resultsList [key].Count; i++) {
						ids.Add (i);
					}
					idList.Add (key,ids);
				}
				if (idList[key].Count>0) {
					int id = UnityEngine.Random.Range (0,idList[key].Count);
					temp.Add(resultsList [key][idList[key][id]]);
					#if UNITY_EDITOR
//					Debug.Log (key+" Index:"+idList[key][id]);
					Dictionary<string,int> test = new Dictionary<string, int> ();
					test.Add (key, idList [key] [id]);
					resultsTestInfo.Add (test);
					#endif
					idList[key].RemoveAt (id);
				}
			}

			return temp;
		}
	}
}
