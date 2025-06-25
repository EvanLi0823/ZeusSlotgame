using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Classic;

public class ReelStripFilterCondition :BaseCondition {



	public readonly static string REEL_STRIP_CONDITION_R_NODE ="R";

	public string ratioName;
	public ReelStripFilterCondition(Dictionary<string,object> info){
		
		if (info.ContainsKey(CONDITION_CONTENT_NODE)) {
			//由具体子类实现解析
		}

		if (info.ContainsKey(REEL_STRIP_CONDITION_R_NODE)) {
			ratioName = info[REEL_STRIP_CONDITION_R_NODE].ToString();
		}
	}
	public virtual ConditionResult GetQualifiedConditionResult(){
		ConditionResult conditionResult = new ConditionResult ();
		return conditionResult;
	}
	public class ConditionResult {
		public string dependencyRName;
		public bool meetTheCondition;
		public ConditionResult(bool mc= false,string dR=""){
			this.dependencyRName = dR;
			this.meetTheCondition = mc;
		}
	}
}
