using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace Classic{
	public class WeightedResultAtomData:ResultAtomData  {
		public List<WeightedResultAItem> wrAList;
		public WeightedResultAtomData(Dictionary<string,object>info):base(info){
			List<object> ls = info [REEL_STRIP_ATOM_DATA_CONSIST_NODE] as List<object>;
			wrAList = new List<WeightedResultAItem> ();
			for (int i = 0; i < ls.Count; i++) {
				WeightedResultAItem fra = new WeightedResultAItem (ls[i] as Dictionary<string,object>);
				wrAList.Add (fra);
			}
		}
		public override DataFilterResult GetResultData(object info){
			DataFilterResult dr = new DataFilterResult ();
			dr.selectedFixedResult =  (info as WeightedResults).GetWeightedResults (wrAList[0].fixedResultsDataID,1)[0].symbolResults;//目前只有一个元素
			return dr;
		}
	}
	public class WeightedResultAItem:ResultAItem{
		public readonly static string FIxED_RESULT_DATA_ID = "WeightedResultDataID";
		public readonly static string WEIGHT ="Weight";
		public int weight;
		public WeightedResultAItem(Dictionary<string,object> info):base(info){
			if (info.ContainsKey(FIxED_RESULT_DATA_ID)) {
				weight = Utils.Utilities.CastValueInt(info[FIxED_RESULT_DATA_ID]);
			}
		}
	}
}
