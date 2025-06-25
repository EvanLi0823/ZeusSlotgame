using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Classic;
public class FirstCoolDownGlobalSpinCountIntervalConditionContentCell: ConditionContentCell {
	private int globalSpinCountInterval=300;
	public FirstCoolDownGlobalSpinCountIntervalConditionContentCell(object info){
		try {
			globalSpinCountInterval = Utils.Utilities.CastValueInt(info);

		} catch (System.Exception ex) {
			Utils.Utilities.LogPlistError ("GlobalSpinCountInterval:"+info.ToString());
		}
	}
	public override bool ConditionIsOK(){
		int currentSpinTimes = 0;
		int lastMeetTheConditionSpinCount = 0;
		if (BaseSlotMachineController.Instance!=null) {
			currentSpinTimes = StatDataAdapter.Instance.GlobalSpinNum;
		} 
		else if(TestController.Instance!=null){
			currentSpinTimes = TestController.Instance.statisticsManager.SpinNum;// 临时性处理
		}
		string actionName = (baseCondition as EventCondition).eventActionR;
		lastMeetTheConditionSpinCount = StatDataAdapter.Instance.GetGlobalSpecialLastSpinCount(actionName);

		if (currentSpinTimes >= lastMeetTheConditionSpinCount + globalSpinCountInterval) {
			return true;
		}
		return false;
	}
}
