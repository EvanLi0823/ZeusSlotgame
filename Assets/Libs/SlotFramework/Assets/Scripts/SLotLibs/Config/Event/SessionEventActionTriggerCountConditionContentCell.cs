using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Classic;
public class SessionEventActionTriggerCountConditionContentCell : ConditionContentCell {


	public SessionEventActionTriggerCountConditionContentCell(object info):base(1,0){
		try {
			string[] temp = info.ToString().Split (',');
			min = Utils.Utilities.CastValueInt(temp[0]);
			max = Utils.Utilities.CastValueInt (temp[1]);
		} catch (System.Exception ex) {
			Utils.Utilities.LogPlistError ("SessionEventActionTriggerCount:"+info.ToString());
		}
	}
	public override bool ConditionIsOK(){
		string actionName = (baseCondition as EventCondition).eventActionR;
		int sessionEventActionTriggerCount = StatDataAdapter.Instance.GetSessionSpecialEventActionCount(actionName);
		if (sessionEventActionTriggerCount >= min&&sessionEventActionTriggerCount <= max) {
			return true;
		}
		return false;
	}
}
