using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Classic;
public class RandomRatioContentCell:RatioContentCell {
	public readonly static string RANDOM_RESTRICTION_UNIT_FLAG_NAME = "ADataID";
	public readonly static string RANDOM_RESTRICTION_UNIT_WEIGHT = "Weight";

	public string flagName;
	public int flagWeight;
	public RandomRatioContentCell(Dictionary<string,object> info){
		if (info.ContainsKey(RANDOM_RESTRICTION_UNIT_FLAG_NAME)) {
			flagName = info [RANDOM_RESTRICTION_UNIT_FLAG_NAME].ToString ();
		}

		if (info.ContainsKey(RANDOM_RESTRICTION_UNIT_WEIGHT)) {
			flagWeight = Utils.Utilities.CastValueInt(info[RANDOM_RESTRICTION_UNIT_WEIGHT]);
		}
	}
}
