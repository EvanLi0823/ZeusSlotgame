using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Classic;
using System;
using Libs;
using LuaFramework;
using Object = System.Object;

/// <summary>
/// Base command item.
/// </summary>
public class BaseCommandItem : IRule
{
	private const string CommandAck_Accept_Key = "accept";
	private const string CommandAck_Discard_Key = "discard";
	private const string CommandAck_Expire_Key = "expire";
	protected List<EventCondition> conditionList = new List<EventCondition>();

	public string Name { get; private set; }
	public int Priority { get; set; }
	public string AssetName { get; private set; }
	public long CreateTime { get; protected set; }
	public bool IsCreateByApp { get; private set; }
	public string LocalFeature { get; private set; }
	public string CommandToken { get; protected set; }
	public BaseEvent CommandEvent { get; set; }

	public ECommandType eCommandType { get; protected set; }
	public EventCondition currentOKCondition { get; private set; }
	public Dictionary<string,object> ItemDict { get; private set; }

	public DateTime EndDateTime { get; protected set;}
	public long EndTimeStamp { get { return mPeriodFrequencyTrigger.EndTimeStamp; } }

	private bool IsDataValid = false;
	public bool IsNeedAssetBundle { get; private set; }
	public bool IsContactLocalFeature { get; private set; }
	
	public bool IsHasConditionList { get; set; }
	protected PeriodFrequencyTrigger mPeriodFrequencyTrigger;

    public virtual long RemainSeconds()
    {
        if (EndTimeStamp > 0)
        {
            return (EndTimeStamp - TimeUtils.ConvertDateTimeMillisecondsToLong(DateTime.Now)) / 1000;
        }
        return 0;
	}

	public virtual DateTime GetEndDateTime()
	{
		return EndDateTime;
	}

	public BaseCommandItem(string cmdToken,Dictionary<string,object> info)
	{
		if (info == null) return;

		ItemDict = info;
		CommandToken = cmdToken;

		ParseCondition();
		if (!ParseEvent()) return;

		Priority = Utils.Utilities.GetInt(ItemDict, GameConstants.Priority_Key, 0);
		CreateTime = Utils.Utilities.GetLong(ItemDict, GameConstants.CreateTime_Key, 0);
		IsCreateByApp = Utils.Utilities.GetBool(ItemDict, GameConstants.IsCreateByApp_Key, false);
		
        //CheckDefaultActivatedState(Utils.Utilities.GetValue<string>(ItemDict, GameConstants.featureActivated_Key,string.Empty));

		Name = Utils.Utilities.GetValue<string>(ItemDict, GameConstants.name_Key, string.Empty);
		Name = Utils.Utilities.GetValue<string>(ItemDict, GameConstants.Name_Key, Name);

		LocalFeature = Utils.Utilities.GetValue<string>(ItemDict, GameConstants.LocalFeature_Key, string.Empty);
		IsContactLocalFeature = !string.IsNullOrEmpty(LocalFeature);

		if (!IsUserInAgeDayRange(ItemDict)) return;

		mPeriodFrequencyTrigger = new PeriodFrequencyTrigger(ItemDict,CommandToken);
		if(!mPeriodFrequencyTrigger.IsValid()) return;

		EndDateTime = TimeUtils.ToDateTimeFromMillTimeStamp(mPeriodFrequencyTrigger.EndTimeStamp);

		IsDataValid = true;
	}

	private bool IsUserInAgeDayRange (Dictionary<string, object> rewardDict)
	{
		if (rewardDict == null)
			return false;
		Dictionary<string, Object> ageDayLimit = Utils.Utilities.GetValue<Dictionary<string, Object>> (rewardDict,"AgeDayLimit", null);

		//没有限制
		if (ageDayLimit == null)
			return true;

		int ageDayMin = Utils.Utilities.GetInt(ageDayLimit,"Min", -1);
		int ageDayMax = Utils.Utilities.GetInt(ageDayLimit,"Max", int.MaxValue);
		float userAgeDay = UserManager.GetInstance ().UserProfile ().DaysFromFirstLaunch ();
		return userAgeDay > ageDayMin && userAgeDay < ageDayMax;
	}
	
	private void ParseCondition()
	{
		IsHasConditionList = true;//初始化
		List<object> consList = Utils.Utilities.GetValue<List<object>>(ItemDict, GameConstants.Conditions_Key, null);
		if (consList == null) return;

		IsHasConditionList = consList.Count > 0;//"Conditions":[] 无条件
		for (int i = 0; i < consList.Count; i++)
		{
			Dictionary<string,object> dict = consList[i] as Dictionary<string,object>;
			if (dict == null) continue;
			if(!dict.ContainsKey(BaseCondition.CONDITION_TYPE_NODE)) continue;

			conditionList.Add(ConditionFactory.CreateCondition(dict[BaseCondition.CONDITION_TYPE_NODE].ToString(), dict)as EventCondition);
		}
	}

	protected virtual bool ParseEvent()
	{
        CommandEvent = EventFactory.CreateEvent(Utils.Utilities.GetValue<Dictionary<string, object>>(ItemDict, GameConstants.EventDict_Key, null), this);
		if (CommandEvent == null) return false;
		return CommandEvent.IsValid();
	}

	//没有添加会返回 false
	public virtual bool CheckCommandTriggerConditionsIsOk()
	{
        if(!IsCanDisplay()) return false;
		if (!IsHasConditionList) return true;

		for (int i = 0; i < conditionList.Count; i++)
		{
			if (!conditionList[i].CheckConditionContent()) continue;

			currentOKCondition = conditionList[i];
			return true;
		}

		currentOKCondition = null;
		return false;
	}

	/// <summary>
	/// 目前只针对于卡牌系统，用来过滤能够弹窗的award 数组
	/// </summary>
	/// <returns></returns>
	public virtual bool IgnoreTriggerCountLimit()
	{
		Dictionary<string, object> infoDict = GetExtraDict();
		if (infoDict == null) return false;
		if (infoDict.ContainsKey(GameConstants.IGNORE_TRIGGER_COUNT))
		{
			return Utils.Utilities.CastValueBool(infoDict[GameConstants.IGNORE_TRIGGER_COUNT]);
		}
	
		return false;
	}
	public virtual Dictionary<string, object> GetExtraDict()
	{
		if (ItemDict == null || ItemDict.Count == 0) return null;
		if (ItemDict.ContainsKey(GameConstants.EXTRA_DICT_KEY))//new interface code 
		{
			return ItemDict[GameConstants.EXTRA_DICT_KEY] as Dictionary<string, object>;
		}
		return ItemDict;
	}
	public virtual bool CheckCurrentTriggerMomentOK()
	{
		if (!IsHasConditionList) return false;

		for (int i = 0; i < conditionList.Count; i++)
		{
			if (conditionList[i].CheckTriggerMomentOK()) return true;
		} 

		return false;
	}

	#region 实现IRule接口

	public virtual void DoCommandOperation(Dictionary<string, object> contextInfo = null){}
	int IRule.Priority()
	{
		return Priority;
	}

	string IRule.Token()
	{
		return CommandToken;
	}

	#endregion

	public virtual void CheckDelLocalTriggerData()
	{
		if (!IsCreateByApp) mPeriodFrequencyTrigger.DelLocalData();
	}

	public virtual void OnCommandItemCreateEnd()
	{
	}

	protected bool IsShowInCurVersion()
	{
		Version minVersion = new Version(Utils.Utilities.GetValue<string>(ItemDict, GameConstants.MinVersion_Key, Application.version));
		Version maxVersion = new Version(Utils.Utilities.GetValue<string>(ItemDict, GameConstants.MaxVersion_Key, Application.version));
		Version curVersion = new Version(Application.version);
		return (curVersion >= minVersion && curVersion <= maxVersion);
	}

	public bool CheckPopUpCondition()
	{
		return true;
	}

	public bool IsInTimeRange()
	{
		return mPeriodFrequencyTrigger.IsInTimeRange();
	}


	#region ClubSystem

	public virtual string GetCmdMsgType()
	{
		return string.Empty;
	}
	#endregion
	public void SetStartTimeStamp(long startTimeStamp)
	{
		mPeriodFrequencyTrigger.SetStartTimeStamp(startTimeStamp);
	}

	public void SetEndTimeStamp(long endTimeStamp)
	{
		mPeriodFrequencyTrigger.SetEndTimeStamp(endTimeStamp);
	}

	public virtual bool IsCanDisplay()
	{
		return true;
	}

	public virtual bool IsValid()
	{
		if (!IsDataValid)return false;
		if (IsExpiration()) return false;
		//当前版本不支持
		if (!IsShowInCurVersion())return false;
		return true;
	}

	public virtual bool IsExpiration()
	{
		return mPeriodFrequencyTrigger.IsExpiration();
	}

	public bool IsHaveTriggerCount()
	{
		return mPeriodFrequencyTrigger.IsHaveTriggerCount();
	}

	public virtual void OnAccept(Dictionary<string,object> dict=null)
	{
		bool IsACKByAccept = Utils.Utilities.GetBool(ItemDict, GameConstants.IsSendACKByAccept_Key, true);
        if (!IsACKByAccept) return;

        CommandManager.Instance.AddAckRewardToken(CommandToken);
        SendCommandAck(CommandAck_Accept_Key, Analytics.AcceptReward, dict);
	}

	public virtual void OnDiscard()
	{
		SendCommandAck(CommandAck_Discard_Key, Analytics.DiscardReward);
	}

	public void OnExpiration()
	{
		SendCommandAck(CommandAck_Expire_Key, Analytics.RewardExpiration);
	}

	protected virtual void SendCommandAck(string strStatue, string esMsgName, Dictionary<string,object> dict = null)
	{
		string source = Utils.Utilities.GetValue(ItemDict,"source","");
		if (!string.IsNullOrEmpty(source))
		{
			if (strStatue == CommandAck_Accept_Key)// 使用
			{
				// Buff, Item 或者邮件
				Messenger.Broadcast("SendCommandAck" + source,  Utils.Utilities.GetValue(ItemDict,"token",""));
			}
			else if(strStatue == CommandAck_Discard_Key)// 删除
			{
				// Buff, Item 或者邮件
				Messenger.Broadcast("SendCommandDelete" + source,  Utils.Utilities.GetValue(ItemDict,"token",""));
			}
			// 过期时不需要处理
			
			// 优惠券
			Messenger.Broadcast(GameConstants.AckCommandMsg,this);
			return;
		}
		
		if (!IsCreateByApp)
		{
			//发送DIscard ACK
            CommandManager.Instance.AddAckingCommand(new RemoveRewardInfo(CommandToken,strStatue));

			Dictionary<string,object> parameters = null;
			if (dict != null) parameters = new Dictionary<string, object>(dict);
			else parameters = new Dictionary<string,object>();

			parameters["name"] = Name;
            parameters["token"] = CommandToken;
            parameters["hasPopupCount"] = mPeriodFrequencyTrigger.HasTriggeredCount;
			BaseGameConsole.ActiveGameConsole().LogBaseEvent(esMsgName, parameters);
		}

        Messenger.Broadcast(GameConstants.AckCommandMsg,this);
		OnDeleteCommand();
	}

	public virtual void OnDeleteCommand(bool isDelByReload = false)
	{
        CommandManager.Instance.DelCommandByToken(CommandToken,isDelByReload);
	}

	public void ResetTriggerCountForPlistActivity()
	{
		mPeriodFrequencyTrigger.ResetTriggerCountForPlistActivity();
	}

	public bool IsContainDataByKey(string strKey)
	{
		return ItemDict.ContainsKey(strKey);
	}

	public T GetServerDataFromCommandByKey<T>(string strKey,T defaultValue)
	{
		return Utils.Utilities.GetValue<T>(ItemDict, strKey, defaultValue);
	}
        
    public float GetCommandFloatDataByKey(string strKey,float defaultValue)
    {
        return Utils.Utilities.GetFloat(ItemDict, strKey, defaultValue);
    }

	public int GetCommandIntDataByKey(string strKey,int defaultValue)
	{
		return Utils.Utilities.GetInt(ItemDict, strKey, defaultValue);
	}

    public bool GetCommandBoolDataByKey(string strKey,bool defaultValue)
    {
        return Utils.Utilities.GetBool(ItemDict, strKey, defaultValue);
    }

	public long GetCommandLongDataByKey(string strKey,long defaultValue)
	{
		return Utils.Utilities.GetLong(ItemDict, strKey, defaultValue);
	}

    public virtual void OnAddRewardPush(){}

}

public enum ECommandType
{
	CommandType_Normal,
	CommandType_Event,
}
