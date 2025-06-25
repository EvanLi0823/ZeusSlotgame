using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Classic;
public class GlobalEventTriggerCountConditionContentCell : ConditionContentCell {

	public GlobalEventTriggerCountConditionContentCell(object info):base(1,0){
		try {
			string[] temp = info.ToString().Split (',');
			min = Utils.Utilities.CastValueInt(temp[0]);
			max = Utils.Utilities.CastValueInt (temp[1]);
		} catch (System.Exception ex) {
			Utils.Utilities.LogPlistError ("GlobalEventTriggerCount:"+info.ToString());
		}
	}
	public override bool ConditionIsOK(){
		string actionName = (baseCondition as EventCondition).eventActionR;
		int globalEventActionTriggerCount = StatDataAdapter.Instance.GetGlobalSpecialEventActionCount(actionName);
		if (globalEventActionTriggerCount >= min&&globalEventActionTriggerCount <= max) {
			return true;
		}
		return false;
	}
}