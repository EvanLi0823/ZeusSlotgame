using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace Classic{
	public class AtomData  {
		public readonly static string  REEL_STRIP_ATOM_DATA_A_TYPE = "AType";
	
		public AtomData(Dictionary<string,object> info){

		}
		public virtual DataFilterResult GetResultData(object info){
			return null;//目前只有一个元素
		}
	}
	public class DataFilterResult{
		public List<ReelStrip> selectedReelStrips;
		public List<List<int>> selectedFixedResult;
	}
}

