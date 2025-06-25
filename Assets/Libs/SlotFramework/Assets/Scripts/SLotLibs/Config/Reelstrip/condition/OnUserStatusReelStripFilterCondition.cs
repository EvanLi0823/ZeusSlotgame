using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Classic;
public class OnUserStatusReelStripFilterCondition:ReelStripFilterCondition  {

	private List<ConditionContentCell> conditionList;

	public OnUserStatusReelStripFilterCondition(Dictionary<string,object>info):base(info){
		if (info.ContainsKey(CONDITION_CONTENT_NODE)) {
			Dictionary<string,object> uSinfo = info [CONDITION_CONTENT_NODE] as Dictionary<string,object>;
			conditionList = new List<ConditionContentCell> ();
			foreach (string key in uSinfo.Keys) {
				ConditionContentCell content = ConditionContentCellFactory.CreateContentCell (uSinfo[key], key);
				if (content!=null) {
					conditionList.Add(content);
				}
			}
		}
	}

	public override bool CheckConditionContent(){
		if (conditionList == null)
			return false;

		for (int i = 0; i < conditionList.Count; i++) {
			if (!conditionList[i].ConditionIsOK()) {
				return false;
			}
		}
		return true;
	}

	public override ConditionResult GetQualifiedConditionResult(){
		ConditionResult conditionResult = new ConditionResult ();
		if (conditionList!=null) {
			if (CheckConditionContent()) {
				conditionResult.dependencyRName= this.ratioName;
				conditionResult.meetTheCondition = true;
			}
		} 
		return conditionResult;
	}
}
