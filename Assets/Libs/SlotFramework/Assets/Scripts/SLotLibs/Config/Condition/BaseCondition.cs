using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Classic{
	public class BaseCondition {
		public readonly static string CONDITION_TYPE_NODE = "ConditionType";
		public readonly static string CONDITION_CONTENT_NODE = "ConditionContent";
		public readonly static string CONDITION_DESCRIPTION = "Description";

		public BaseCondition(Dictionary<string,object> info=null){
		}

		public virtual bool CheckConditionContent(){
			return false;
		}
		public virtual bool CheckTriggerMomentOK(){
			return false;
		}
	}
}

