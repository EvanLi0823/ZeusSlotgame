using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Libs;
using Classic;
using System;

public class EventCommandItem : BaseCommandItem
{
	public EventCommandItem(string commandToken,Dictionary<string,object> dict) : base(commandToken,dict)
	{
		if (!base.IsValid()) return;
		eCommandType = ECommandType.CommandType_Event;
	}

	#region public function

	public override bool CheckCommandTriggerConditionsIsOk(){
		if (!mPeriodFrequencyTrigger.IsCanTrigger()) return false;
		if (IsNeedAssetBundle)
		{
			if (!CheckPopUpCondition()) return false;
		}

		return base.CheckCommandTriggerConditionsIsOk();
	}

	public override void DoCommandOperation(Dictionary<string, object> contextInfo = null){
		Dictionary<string,object> dict = new Dictionary<string, object>();
		if (IsHasConditionList)
		{
			if (currentOKCondition == null || CommandEvent == null)
			{
				Utils.Utilities.LogPlistError("Please Check Plist Condition or Event not null!");
				return;
			}
			dict.Add(GameConstants.SubType_Key, currentOKCondition.conditionName);
		}

		dict.Add(GameConstants.EVENT_ID, CommandToken);
		Dictionary<string,object> ParameterDict = new Dictionary<string, object>();
		ParameterDict.Add(GameConstants.ESMsgDict_Key, dict);
		string strEvent = Utils.Utilities.GetValue<string>(CommandEvent.EventDict,GameConstants.Event_Key,null);
		CommandEvent.ExcuteEventAction(ParameterDict);
		mPeriodFrequencyTrigger.OnTrigger();
	}
	#endregion
}