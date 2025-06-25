using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Classic;
//开关条件，防止PM删除添加的麻烦
public class SwitchOnConditionContentCell : ConditionContentCell {
	private bool switchOn = false;
	public SwitchOnConditionContentCell(object info){
		try {
			switchOn = Utils.Utilities.CastValueBool(info);
		} catch (System.Exception ex) {
			Utils.Utilities.LogPlistError ("SwitchOn:"+info.ToString());
		}

	}
	public override bool ConditionIsOK(){
		if (switchOn) {
			return true;
		}
		return false;
	}

}
