using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Classic;

public class RandomReelStripDataRatio : ReelStripDataRatio {
	public readonly static string REEL_STRIP_DATA_RESTRICTION_WEIGHT_DEPENDENCY = "AGroup";

	List<RandomRatioContentCell> randomCells;

	public RandomReelStripDataRatio(Dictionary<string,object> info):base(info){
		if (info.ContainsKey(REEL_STRIP_DATA_RESTRICTION_WEIGHT_DEPENDENCY)) {
			List<object> cdInfo = info [REEL_STRIP_DATA_RESTRICTION_WEIGHT_DEPENDENCY] as List<object>;
			randomCells = new List<RandomRatioContentCell> ();
			for (int i = 0; i < cdInfo.Count; i++) {
				RandomRatioContentCell cell = new RandomRatioContentCell (cdInfo[i] as Dictionary<string,object>);
				randomCells.Add (cell);
			}
		}
	}

	public override DataRatioResult GetRatioResult(){
		currentResult = GetQualifiedRatioResultByRandom ();
		return currentResult;
	}
	private RandomReelStripDataRatio.DataRatioResult GetQualifiedRatioResultByRandom(){
		if (randomCells==null) {
			return null;
		}
		int totalWeight = 0;
		for (int i = 0; i < randomCells.Count; i++) {
			totalWeight += randomCells [i].flagWeight;
		}
		int randomValue = UnityEngine.Random.Range (0,totalWeight);
		int StartWeight = 0;
		RandomReelStripDataRatio.DataRatioResult resResult = new ReelStripDataRatio.DataRatioResult ();

		for (int i = 0; i < randomCells.Count; i++) {
			int nextWeightBeginWeight =randomCells[i].flagWeight +StartWeight;
			if (randomValue >= StartWeight && randomValue < nextWeightBeginWeight) {
				resResult.selectedResultName = randomCells [i].flagName;
				break;
			} else {
				StartWeight = nextWeightBeginWeight;
			}
		}
		return resResult;
	}

	public override List<List<int>> GetWeightedResult(AtomData atomData,WeightedResults fr){
		List<List<int>> results = null;
		if (currentResult==null) {
			return results;
		}

		if ((atomData as ResultAtomData)!=null) {
			results = (atomData as ResultAtomData).GetResultData(fr).selectedFixedResult;
		}

		return results;
	}
}
