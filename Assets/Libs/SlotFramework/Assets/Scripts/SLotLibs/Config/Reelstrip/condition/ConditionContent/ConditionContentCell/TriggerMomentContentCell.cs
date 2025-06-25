using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Classic;
public class TriggerMomentContentCell : ConditionContentCell {
	List<string> flags = new List<string> ();
	public TriggerMomentContentCell(object info){
		try {
			string[] temp = info.ToString().Split(',');
			if (temp.Length>0) {
				for (int i = 0; i < temp.Length; i++) {
					if (!flags.Contains(temp[i])) {
						flags.Add(temp[i]);
					}
				}
			}
		} catch (System.Exception ex) {
			Utils.Utilities.LogPlistError ("TriggerMomentContentCell:value format is not correct:"+info.ToString());
		}
	}
	public override bool ConditionIsOK(){
		if (flags!=null&&!string.IsNullOrEmpty(StatDataAdapter.Instance.CurrentMoment)) {
			return flags.Contains (StatDataAdapter.Instance.CurrentMoment);
		}
		return false;
	}
}
