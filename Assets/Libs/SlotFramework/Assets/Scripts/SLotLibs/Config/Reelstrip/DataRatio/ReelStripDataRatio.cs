using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Classic;
public class ReelStripDataRatio {
	
	protected DataRatioResult currentResult;
	public ReelStripDataRatio(Dictionary<string,object> info){
		
	}
	public virtual DataRatioResult GetRatioResult(){
		return null;
	}

	public class DataRatioResult {
		public string selectedResultName;
		public DataRatioResult(){
			this.selectedResultName = "";
		}
	}

	public virtual List<List<int>> GetWeightedResult(AtomData atomData,WeightedResults fr){
		return null;
	}
}
