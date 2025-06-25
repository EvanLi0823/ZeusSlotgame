using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Classic;
public class OnCustomReelStripFilterCondition:ReelStripFilterCondition {
	public CustomContentCell customConditionContentCell;
	public string selectedRName;
	public OnCustomReelStripFilterCondition(Dictionary<string,object> info) :base(info){
		if (info.ContainsKey(CONDITION_CONTENT_NODE)) {
			Dictionary<string,object> csInfo = info [CONDITION_CONTENT_NODE] as Dictionary<string,object>;
			if (csInfo==null) {
				return;
			}
			customConditionContentCell = new CustomContentCell (csInfo);
		}
	}

	public override ConditionResult GetQualifiedConditionResult(){
		ConditionResult conditionResult = new ConditionResult ();
		if (customConditionContentCell!=null) {
			if (CheckConditionContent()) {
				conditionResult.dependencyRName = this.ratioName;
				conditionResult.meetTheCondition = true;
			}
		}
		return conditionResult;
	}
		
	public override bool CheckConditionContent(){
		return customConditionContentCell.ConditionIsOK ();
	}
}
