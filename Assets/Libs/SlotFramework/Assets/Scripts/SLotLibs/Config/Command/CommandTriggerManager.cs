using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class TriggerMonment
{
	public TriggerMonment(Dictionary<string, object> dict)
	{
		MaxCount = Utils.Utilities.GetInt(dict, GameConstants.MaxCount_Key, 3);
		MinCount = Utils.Utilities.GetInt(dict, GameConstants.MinCount_Key, 0);
		//MinCount_Key
	}

	public int MaxCount{ get; private set; }
	public int MinCount{ get; private set; }
}

public class CommandTriggerManager
{
	public const string SERVER_COMMAND_LIST_NAME = "ServerCommandItem";

	public string DealSubType{ get;set; }
	// 定义一个静态变量来保存类的实例
	private static CommandTriggerManager instance;
	Dictionary<string,TriggerMonment> mTriggerMonmentsDict = new Dictionary<string, TriggerMonment>();
	// 定义私有构造函数，使外界不能创建该类实例
	private CommandTriggerManager()
	{
		mTriggerMonmentsDict.Clear();
		Dictionary<string,object> triggerMonmentsDict = Plugins.Configuration.GetInstance().GetValueWithPath<Dictionary<string,object>>("Module/TriggerMoments", null);
		if (triggerMonmentsDict != null)
		{
			foreach (string key in triggerMonmentsDict.Keys)
			{
				Dictionary<string,object> triggerMonmentDict = triggerMonmentsDict[key] as Dictionary<string,object>;
				if (triggerMonmentDict == null)
					continue;

				TriggerMonment triggerMonment = new TriggerMonment(triggerMonmentDict);
				mTriggerMonmentsDict[key] = triggerMonment;
			}
		}
	}
	
	private const string CheckMomentForRules_Key = "RuleMgr.CheckMoment";
	public static CommandTriggerManager Instance{ get { return Classic.Singleton<CommandTriggerManager>.Instance; } }

	public void CheckMomentConditions(string momentName)
	{
		Messenger.Broadcast(momentName);
		StatDataAdapter.Instance.CurrentMoment = momentName;
		DealSubType = GetSubTypeByMomentName(momentName);

		List<BaseCommandItem> commandItemList = CommandManager.Instance.GetTriggerMomentCommandList();
		if (commandItemList == null) return;

        commandItemList = commandItemList.FindAll((item) =>{
            return item.IsInTimeRange() && item.IsCanDisplay();
        });

		commandItemList.Sort(delegate(BaseCommandItem kvp1, BaseCommandItem kvp2){
			return kvp2.Priority - kvp1.Priority;
		});

		// 相同的LocalFeature只能有一个
		List<string> localFeatrueList = new List<string>();
		for (int i = 0; i < commandItemList.Count;){
			BaseCommandItem command_I = commandItemList[i];
			if (command_I.IsContactLocalFeature)
			{
				if (localFeatrueList.Contains(command_I.LocalFeature))
				{
					commandItemList.Remove(command_I);
				}
				else
				{
					localFeatrueList.Add(command_I.LocalFeature);
					i++;
				}
			}
			else
			{
				i++;
			}
		}
	}

	Dictionary<string, object> getContextInfo(string momentName)
	{
		Dictionary<string, object> contextInfo = new Dictionary<string, object>();
		contextInfo.Add(GameConstants.Trigger_Key, momentName);
		return contextInfo;
	}

	string GetSubTypeByMomentName(string momentName)
	{
		switch (momentName)
		{
			case GameConstants.ENTER_LOBBY:
				return "PopupOffer";
			case GameConstants.BACK_TO_APP:
				return "RewardBacktoApp";
			case GameConstants.BACK_TO_LOBBY:
				return "RewardBacktoLobby";
			case GameConstants.CLOSE_OUTOF_COINS_DIALOG:
				return GameConstants.MachineOutofCoins;
		}
		return momentName;
	}
}
