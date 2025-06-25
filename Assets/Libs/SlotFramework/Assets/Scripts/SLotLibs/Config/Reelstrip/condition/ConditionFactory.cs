using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Classic{
	public class ConditionFactory {
		public const string REEL_STRIP_CONDITION_TYPE_USER_STATUS_CONDITION ="UserStatusCondition";
		public const string REEL_STRIP_CONDITION_TYPE_CUSTOM_CONDITION ="CustomCondition";
		public const string EVENT_CONDITON_TYPE = "EventCondition";
		public static BaseCondition CreateCondition(string conditionType,Dictionary<string,object>consInfo){
			BaseCondition bc;
			switch (conditionType) {
			case REEL_STRIP_CONDITION_TYPE_CUSTOM_CONDITION:
				bc = new OnCustomReelStripFilterCondition (consInfo);
				break;
			case REEL_STRIP_CONDITION_TYPE_USER_STATUS_CONDITION:
				bc = new OnUserStatusReelStripFilterCondition (consInfo);
				break;
			case EVENT_CONDITON_TYPE:
				bc = new EventCondition (consInfo);
				break;
			default:
				{
					Utils.Utilities.LogPlistError ("ConditionType:"+conditionType+" CreateCondition failed,please check plist or Json");
					bc = new BaseCondition (consInfo);
				}
				break;
			}
			return bc;
		}
	}
}

