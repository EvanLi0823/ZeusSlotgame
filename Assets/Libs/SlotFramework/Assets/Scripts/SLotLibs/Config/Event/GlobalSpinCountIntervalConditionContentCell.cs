using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Classic;
public class GlobalSpinCountIntervalConditionContentCell : ConditionContentCell {
	private int globalSpinCountInterval=300;
	public GlobalSpinCountIntervalConditionContentCell(object info){
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
		//首次不冷却
		if (lastMeetTheConditionSpinCount==0||currentSpinTimes >= lastMeetTheConditionSpinCount + globalSpinCountInterval) {
			return true;
		}
		return false;
	}
}
