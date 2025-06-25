using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Classic;
public class FirstCoolDownSessionSpinCounIntervaltConditionContentCell:ConditionContentCell {
	private int sessionSpinCountInterval=300;
	public FirstCoolDownSessionSpinCounIntervaltConditionContentCell(object info){
		try {
			sessionSpinCountInterval = Utils.Utilities.CastValueInt(info);
		} catch (System.Exception ex) {
			Utils.Utilities.LogPlistError ("SessionSpinCounIntervalt:"+info.ToString());
		}
	}
	public override bool ConditionIsOK(){
		int currentSpinTimesInTheSession = 0;
		int lastMeetTheConditionSpinCount = 0;
		if (BaseSlotMachineController.Instance!=null) {
			currentSpinTimesInTheSession = BaseSlotMachineController.Instance.statisticsManager.SpinNum;
		} 
		else if(TestController.Instance!=null){
			currentSpinTimesInTheSession = TestController.Instance.statisticsManager.SpinNum;
		}
		string actionName = (baseCondition as EventCondition).eventActionR;
		lastMeetTheConditionSpinCount = StatDataAdapter.Instance.GetSessionSpecialLastSpinCount(actionName);
		if (currentSpinTimesInTheSession >= lastMeetTheConditionSpinCount + sessionSpinCountInterval) {
			return true;
		}
		return false;
	}
}
