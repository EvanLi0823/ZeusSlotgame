using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace Classic{
	public class ReelStripAtomData :AtomData{
		public readonly static string STRIP_DATA = "ClassicReelStripData";
		public List<ReelStrip> reelStrips;
		public ReelStripAtomData(SymbolMap symbolMap,Dictionary<string,object> info):base(info){
			if (info.ContainsKey(STRIP_DATA)) {
				reelStrips = new  List<ReelStrip> ();
				List<object> ls = info [STRIP_DATA] as List<object>;
				for (int i = 0; i < ls.Count; i++) {
					ReelStrip rs = new ReelStrip (symbolMap, ls [i] as List<object>);
					reelStrips.Add (rs);
				}
			}
		}

		public override DataFilterResult  GetResultData(object info){
			DataFilterResult dr = new DataFilterResult ();
			dr.selectedReelStrips = reelStrips;
			return dr;
		}
	}
}

