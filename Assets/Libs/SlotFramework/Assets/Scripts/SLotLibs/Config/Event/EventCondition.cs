using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Classic{
	public class EventCondition:BaseCondition {
		public const string EVENT_ACTION_R_NODE = "R";
		public const string CONDITION_NAME = "ConditionName";
		public List<ConditionContentCell> conditionList;
		public string eventActionR;
		public string conditionName;
		public EventCondition(Dictionary<string,object> info){

			if (info.ContainsKey(CONDITION_CONTENT_NODE)) {
				//由具体子类实现解析
				conditionList = new List<ConditionContentCell>();
				Dictionary<string,object> uSinfo = info [CONDITION_CONTENT_NODE] as Dictionary<string,object>;
				foreach (string key in uSinfo.Keys) {
					ConditionContentCell content = ConditionContentCellFactory.CreateContentCell (uSinfo[key], key);
					content.baseCondition = this;
					if (content!=null) {
						conditionList.Add(content);
					}
				}
			}

			if (info.ContainsKey(EVENT_ACTION_R_NODE)) {
				eventActionR = info[EVENT_ACTION_R_NODE].ToString();
			}
			if (info.ContainsKey(CONDITION_NAME)) {
				conditionName = info [CONDITION_NAME].ToString ();
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

		public override bool CheckTriggerMomentOK(){
			for (int i = 0; i < conditionList.Count; i++) {
				if (conditionList[i].GetType().Equals(typeof(TriggerMomentContentCell)))
					return conditionList[i].ConditionIsOK();
			}
			return false;
        }
	}
}

