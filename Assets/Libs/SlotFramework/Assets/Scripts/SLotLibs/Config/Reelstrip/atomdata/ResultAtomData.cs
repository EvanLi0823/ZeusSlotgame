using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace Classic{
	public class ResultAtomData :AtomData {

		public readonly static string REEL_STRIP_ATOM_DATA_CONSIST_NODE = "WeightedResultData";

		public ResultAtomData(Dictionary<string,object> info):base(info){

		}
	}

	public class ResultAItem{
		public readonly static string RESULT_DATA_ID = "WeightedResultDataID";
		public string fixedResultsDataID;
	
		public ResultAItem(Dictionary<string,object> info){
			if (info.ContainsKey(RESULT_DATA_ID)) {					
				fixedResultsDataID = info[RESULT_DATA_ID].ToString();
			}
		}
	}

	public class RangeData{
		public int min;
		public int max;
		public RangeData(){
		}
		public RangeData(int min,int max){
			this.min = min;
			this.max = max;
		}
	}
}

