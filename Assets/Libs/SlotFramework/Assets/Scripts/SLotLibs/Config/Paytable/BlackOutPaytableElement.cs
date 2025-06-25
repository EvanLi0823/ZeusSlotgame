using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Utils;

namespace Classic
{
	public class BlackOutPaytableElement
	{
		List<int> symbolList = new List<int> ();
		int awardMap = 0;
		string awardName;

		public BlackOutPaytableElement (SymbolMap symbolMap, Dictionary<string,object> info)
		{
			if (info.ContainsKey (ClassicPaytable.AWARD_MAP)) {
				awardMap = Utilities.CastValueInt (info [ClassicPaytable.AWARD_MAP], 0);
			}
			if (info.ContainsKey (ClassicPaytable.SYMBOL_LIST)) {
				List<object> symbolListString = info [ClassicPaytable.SYMBOL_LIST] as List<object>;
				for (int i=0; i<symbolListString.Count; i++) {
					string symbolString = symbolListString [i] as string;
					symbolList.Add (symbolMap.getSymbolIndex (symbolString));
				}
			}

			if (info.ContainsKey (ClassicPaytable.AwardName)) {
				awardName = info [ClassicPaytable.AwardName] as string;
			}

		}

		public int CheckAward (SymbolMap symbolMap, ResultContent resultContent, out string awardName)
		{
			awardName = this.awardName;

			for (int j=0; j<resultContent.ReelResults.Count; j++) {
				List<int> results = resultContent.ReelResults [j].SymbolResults;
				for (int i=0; i<results.Count; i++) {
					SymbolMap.SymbolElementInfo info = symbolMap.getSymbolInfo (results [i]);
					if (!symbolList.Contains (info.Index)) {
						return 0;
					}
				}
			}

			return awardMap;
		}
	}
}
