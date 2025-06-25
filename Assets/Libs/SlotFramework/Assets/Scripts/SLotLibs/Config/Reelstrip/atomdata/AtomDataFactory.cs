using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Classic{
	public class AtomDataFactory {
		public const string ATYPE_RANGE_WEIGHTED_RESULTS= "RangeWeightedResultA";
		public const string ATYPE_WEIGHTED_RESULTS= "WeightedResultA";
		public const string ATYPE_REEL_STRIPS= "ClassicReelStrip";
		public static AtomData CreateAtomData(string adType,SymbolMap symbolMap,Dictionary<string,object> aDInfo){
			AtomData ad = null;
			switch (adType) {
			case ATYPE_RANGE_WEIGHTED_RESULTS:
				{
					ad = new ShuffleWeightedResultAtomData (aDInfo);
				}
				break;
			case ATYPE_WEIGHTED_RESULTS:
				{
					ad = new WeightedResultAtomData (aDInfo);
				}
				break;
			case ATYPE_REEL_STRIPS:
				{
					ad = new ReelStripAtomData (symbolMap,aDInfo);
				}
				break;
			default:
				{
					ad = new ReelStripAtomData (symbolMap,aDInfo);
				}
				break;
			}

			return ad;
		}
	}
}

