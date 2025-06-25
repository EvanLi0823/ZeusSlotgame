using System.Collections.Generic;
using Classic;
using Libs;
using System;
using UnityEngine;
using LuaFramework;

public class RemoveRewardInfo
{
	public string Token { get; set; }
	public string Statue { get; set; }

	public RemoveRewardInfo(){}
	public RemoveRewardInfo(string token,string statue){
		Token = token;
		Statue = statue;
	}
}


public class CommandManager
{
	List<NormalCommandItem> m_InboxItemList = new List<NormalCommandItem>();
	List<BaseCommandItem> m_CommandItemList = new List<BaseCommandItem>();
	Dictionary<string, BaseCommandItem> mToken_RewardItemDict = new Dictionary<string, BaseCommandItem>();
	Dictionary<string, List<BaseCommandItem>> mFeature_RewardItemDict = new Dictionary<string, List<BaseCommandItem>>();
	Dictionary<string, RemoveRewardInfo> mAckingCommandDict = new Dictionary<string, RemoveRewardInfo>();

	List<NormalCommandItem> mNormalCommandItemList = new List<NormalCommandItem>();
	List<EventCommandItem>  mEventCommandItemList = new List<EventCommandItem>();
	private List<string> ackRewardTokenList = new List<string>();
	int LocalInboxItemCount
	{
		get { return SharedPlayerPrefs.GetPlayerPrefsIntValue("LocalInboxItemCount", 0); }
		set { SharedPlayerPrefs.SetPlayerPrefsIntValue("LocalInboxItemCount", value); }
	}

	private CommandManager()
	{
		LoadAckingCommand();
		InitAckRewardTokenList();
		Messenger.AddListener<string>(GameConstants.AddDisplayDialogTokenMsg, OnAddDisplayDialogTokenMsg);
        Messenger.AddListener<string>(GameConstants.DelDisplayDialogTokenMsg, OnDelDisplayDialogTokenMsg);
        // Messenger.AddListener(GameConstants.AutopilotDataResponse_Key, OnAutopilotDataResponseMsg);
        Messenger.AddListener<string>(GameConstants.REMOVE_USELESS_REWARD_COMMAND,HandleUselesssRewardCommand);
	}

	~CommandManager()
	{
		Messenger.RemoveListener<string>(GameConstants.AddDisplayDialogTokenMsg, OnAddDisplayDialogTokenMsg);
        Messenger.RemoveListener<string>(GameConstants.DelDisplayDialogTokenMsg, OnDelDisplayDialogTokenMsg);
        // Messenger.RemoveListener(GameConstants.AutopilotDataResponse_Key, OnAutopilotDataResponseMsg);
        Messenger.RemoveListener<string>(GameConstants.REMOVE_USELESS_REWARD_COMMAND,HandleUselesssRewardCommand);
    }
	
	private const string GetRuleByInstanceID_Key = "RuleMgr.GetRuleByInstanceID";

	public void OnInit()
	{
		
	}
	public void OnDispose()
	{
		
	}

	public IRule GetRuleByToken(string token)
	{
		return null;
	}
	
	/// <summary>
	/// 移除Inbox打开卡包等reward的无效reward
	/// </summary>
	/// <param name="cmdToken"></param>
	void HandleUselesssRewardCommand(string cmdToken)
	{
		if (mNormalCommandItemList.Count == 0) return;
		for (int i = 0; i < mNormalCommandItemList.Count; i++)
		{
			if(mNormalCommandItemList[i]==null) continue;
			if (!mNormalCommandItemList[i].IsInboxCommand) continue;
			BaseEvent be = mNormalCommandItemList[i].CommandEvent;
			if(be==null) continue;
			string token = be.ParaDict[GameConstants.Token_Key].ToString();
			int useless = PlayerPrefs.GetInt(token, 1);
			if(useless==1&&cmdToken!=token) continue;
			mNormalCommandItemList[i].OnAccept();
		}
	}
    // private void OnAutopilotDataResponseMsg()
    // {
    //     string outOfCoinsShowDialog = AutopilotManager.GetInstance().GetXValueInTopicByKey<string>(GameConstants.OutOfCoinsTriggerMoment_Key, GameConstants.OutOfCoinsSku_Key, string.Empty);
    //     BaseCommandItem command = GetCommandByToken(outOfCoinsShowDialog);
    //     if (command == null) return;
    //
    //     command.Priority = 90000000;
    //     AutopilotManager.GetInstance().LogTestByKey(GameConstants.OutOfCoinsTriggerMoment_Key);
    // }

	#region DialogDisplayStatus
	List<string> DisplayDialogTokenList = new List<string>();
	void OnAddDisplayDialogTokenMsg(string key)
	{
		if(!DisplayDialogTokenList.Contains(key))
			DisplayDialogTokenList.Add(key);
	}

	void OnDelDisplayDialogTokenMsg(string key)
	{
		if(DisplayDialogTokenList.Contains(key))
			DisplayDialogTokenList.Remove(key);
	}

	public bool IsCommandDisplay(string key)
	{
		return DisplayDialogTokenList.Contains(key);
	}
 
	#endregion

	public void AddNormalCommandItemDict(BaseCommandItem rhs)
	{
		mNormalCommandItemList.Add(rhs as NormalCommandItem);
	}

	public void RemoveNormalCommandItemDict(NormalCommandItem rhs)
	{
		mNormalCommandItemList.Remove(rhs);
	}

	public void AddEventCommandItemDict(BaseCommandItem rhs)
	{
		mEventCommandItemList.Add(rhs as EventCommandItem);
	}

	public void RemoveEventCommandItemDict(EventCommandItem rhs)
	{
		mEventCommandItemList.Remove(rhs);
	}

	public T GetEventCommandByType<T>() where T : EventCommandItem
	{
		return (T)mEventCommandItemList.Find(item =>{
			if (item == null) return false;
			if (!item.IsValid()) return false;
			return (item is T);
		});
	}
	
	public static CommandManager Instance{ get { return Classic.Singleton<CommandManager>.Instance; } }

	public List<NormalCommandItem> GetInboxItemList()
	{
		m_InboxItemList = mNormalCommandItemList.FindAll(item =>{
			if (item == null) return false;
			return item.IsCanShowInInboxNow();
		});
		return m_InboxItemList;
	}

	public List<BaseCommandItem> GetTriggerMomentCommandList()
	{
		return m_CommandItemList.FindAll((item)=>{
			if(item == null) return false;
			return item.Priority > 0 && item.CheckCurrentTriggerMomentOK();
		});
	}

	public List<BaseCommandItem> GetCommandItemList()
	{
		return m_CommandItemList;
	}
	public List<BaseCommandItem> GetExtraDataFromCommandsByKey(string strKey)
	{
		if (string.IsNullOrEmpty(strKey)) return null;

		return m_CommandItemList.FindAll((item)=>{
			if(item == null) return false;
			if(item.IsExpiration()) return false;
			return item.IsContainDataByKey(strKey);
		});
	}
	public void AddImmediatelyCommand(List<object> rewardsList)
	{
		if (rewardsList == null) return;

		CommandTriggerManager.Instance.DealSubType = "RewardFromPromoCode";
		ParseAndCreateImmediatelyCommand(rewardsList);
	}

	public void GetCommandByQuery(List<object> rewardsList)
	{
		if (rewardsList == null) return;
		ParseAndCreateQueryRewards(rewardsList);
	}

	void CheckDelCommandFromFeatureDict(BaseCommandItem delItem)
	{
		if (delItem == null) return;
		if (!delItem.IsContactLocalFeature) return;
		if (!mFeature_RewardItemDict.ContainsKey(delItem.LocalFeature)) return;

		List<BaseCommandItem> featureRewardList = mFeature_RewardItemDict[delItem.LocalFeature];
		if (featureRewardList == null) return;

		featureRewardList.Remove(delItem);

		if (featureRewardList.Count == 0) mFeature_RewardItemDict.Remove(delItem.LocalFeature);
	}

	public void CommandOnDeleteByToken(string strToken)
	{
		BaseCommandItem delItem = GetCommandByToken(strToken);
		if (delItem == null) return;
		delItem.OnDeleteCommand();
	}
	
	public void DelCommandByToken(string strToken,bool isDelByReload = false)
	{
		BaseCommandItem delItem = GetCommandByToken(strToken);
		if (delItem == null) return;

		mToken_RewardItemDict.Remove(strToken);
		m_CommandItemList.Remove(delItem);

		CheckDelCommandFromFeatureDict(delItem);

		delItem.CheckDelLocalTriggerData();

		if (delItem.eCommandType == ECommandType.CommandType_Event)
		{
			EventCommandItem eventCommandItem = delItem as EventCommandItem;
			if (eventCommandItem != null) RemoveEventCommandItemDict(eventCommandItem);
		}
		else if (delItem.eCommandType == ECommandType.CommandType_Normal)
		{
			NormalCommandItem normalCommandItem = delItem as NormalCommandItem;
			if (normalCommandItem != null)
			{
				RemoveNormalCommandItemDict(normalCommandItem);
				if (!isDelByReload)
				{
					//Inbox 红点相关
					if (!normalCommandItem.IsInboxCommand) return;
					LocalInboxItemCount--;
					OnCommandUpdateEnd();
				}
			}
		}
	}

	#region AckingCommand
	void LoadAckingCommand()
	{
		string strInfoList = PlayerPrefs.GetString(GameConstants.AckingCommand_Key, string.Empty);
		if (!string.IsNullOrEmpty(strInfoList))
		{
			List<object> infoList = MiniJSON.Json.Deserialize(strInfoList) as List<object>;
			if (infoList != null)
			{
				for (int i = 0; i < infoList.Count; i++)
				{
					Dictionary<string, object> infoDict = infoList[i] as Dictionary<string, object>;
					RemoveRewardInfo removeRewardInfo = new RemoveRewardInfo();
					removeRewardInfo.Token = Utils.Utilities.GetValue<string>(infoDict, GameConstants.AckingCommand_Token, string.Empty);
					removeRewardInfo.Statue = Utils.Utilities.GetValue<string>(infoDict, GameConstants.AckingCommand_Statue, string.Empty);
					if (!string.IsNullOrEmpty(removeRewardInfo.Token) && !string.IsNullOrEmpty(removeRewardInfo.Statue))
						AddAckingCommand(removeRewardInfo);
				}
			}
		}
	}

	public void SaveAckingCommand()
	{
		List<object> infoList = new List<object>();
		foreach (KeyValuePair<string, RemoveRewardInfo> pair in mAckingCommandDict)
		{
			Dictionary<string, object> infoDict = new Dictionary<string, object>();
			infoDict[GameConstants.AckingCommand_Token] = pair.Value.Token;
			infoDict[GameConstants.AckingCommand_Statue] = pair.Value.Statue;
			infoList.Add(infoDict);
		}

		string str = MiniJSON.Json.Serialize(infoList);
		if(!string.IsNullOrEmpty(str)) PlayerPrefs.SetString(GameConstants.AckingCommand_Key,str);
	}

	public void RemoveAckingCommand(string token)
	{
		if (mAckingCommandDict.ContainsKey(token))
			mAckingCommandDict.Remove(token);
	}

	public void AddAckingCommand(RemoveRewardInfo info)
	{
		mAckingCommandDict[info.Token] = info;
	}
	#endregion

	public void OnCommandUpdateEnd()
	{
		PushTaskDataMgr.Instance.ParseTaskDataByReward();
	}

	public void ResetCommandData()
	{
		List<string> tokenList = new List<string>(mToken_RewardItemDict.Keys);
		for (int i = 0; i < tokenList.Count; i++)
		{
			string token = tokenList[i];

			if (string.IsNullOrEmpty(token)) continue;

			BaseCommandItem commandItem = mToken_RewardItemDict[token];
			if (commandItem == null) continue;

			commandItem.OnDeleteCommand(true);
		}

		LocalInboxItemCount = 0;
		//暂不考虑这个数据上传服务器
		mAckingCommandDict.Clear();
		SaveAckingCommand();
	}

	//取得通过Reward从服务端传过来的数据
	public List<BaseCommandItem> GetExtraDataFromServerCommandsByKey(string strKey)
	{
		if (string.IsNullOrEmpty(strKey)) return null;
		return m_CommandItemList.FindAll((item)=>{
			if(item == null) return false;
			if(item.IsCreateByApp) return false;
			if(item.IsExpiration()) return false;
			return item.IsContainDataByKey(strKey);
		});
	}

	public BaseCommandItem GetCommandByToken(string token)
    {        
        if (string.IsNullOrEmpty(token)) return null;
        if (!mToken_RewardItemDict.ContainsKey(token)) return null;

        return mToken_RewardItemDict[token];
	}

	public BaseCommandItem GetNewestCommandByFeature(string feature)
	{
		List<BaseCommandItem> commandList = GetCommandListByFeature(feature);
		if (commandList == null) return null;
		if (commandList.Count == 0) return null;

		commandList.Sort(delegate(BaseCommandItem kvp1, BaseCommandItem kvp2){
			return (int)(kvp2.CreateTime - kvp1.CreateTime);
		});

		return commandList[0];
	}

	public BaseCommandItem GetCommandByFeatureAndTaskUserID(string feature, int taskUserID)
	{
		List<BaseCommandItem> commandList = GetCommandListByFeature(feature);
		if (commandList == null) return null;
		if (commandList.Count == 0) return null;

		foreach (var cmd in commandList)
		{
			int id = Utils.Utilities.GetInt(cmd.ItemDict, GameConstants.TaskToken_Key, -1);
			if (id == taskUserID)
				return cmd;
		}
		return null;
	}
	
	public List<BaseCommandItem> GetCommandListByFeature(string feature)
	{
		if (string.IsNullOrEmpty(feature)) return null;
        if (!mFeature_RewardItemDict.ContainsKey(feature)) return null;

		List<BaseCommandItem> commandList = mFeature_RewardItemDict[feature];
		if (commandList == null) return null;
		if (commandList.Count < 1) return null;

		return commandList.FindAll((item)=>{
			if(item == null) return false;
			if(!item.IsInTimeRange()) return false;
			if(!item.IsCanDisplay()) return false;
            //if(!item.IsFeatureActivated()) return false;
			return true;
		});
	}

	
	public List<BaseCommandItem> GetCommandListByFeatureWithBundleOK(string feature)
	{
		List<BaseCommandItem> commandList = GetCommandListByFeature(feature);
		if (commandList == null) return null;

		return commandList.FindAll((item)=>{
			if(!item.CheckPopUpCondition()) return false;
			return true;
		});
    }

    public BaseCommandItem GetCommandByFeature(string feature)
    {
        List<BaseCommandItem> featureCommandList = GetCommandListByFeature(feature);
        if (featureCommandList == null) return null;
        if (featureCommandList.Count < 1) return null;
        return featureCommandList[0];
    }

	public BaseCommandItem GetCommandByFeatureWithBundleOK(string feature)
	{
		BaseCommandItem commandItem = GetCommandByFeature(feature);
		if (commandItem == null) return null;
		if (!commandItem.CheckPopUpCondition()) return null;
		return commandItem;
	}

	public void ParseAndCreateImmediatelyCommand(List<object> rewardsList)
	{
		if (rewardsList == null) return;
		List<BaseCommandItem> commondItemList = new List<BaseCommandItem>();
		for (int i = 0; i < rewardsList.Count; i++)
		{
			#region MyRegion
			Dictionary<string, object> rewardDict = rewardsList[i] as Dictionary<string, object>;
			string token = Utils.Utilities.GetValue<string>(rewardDict, GameConstants.Token_Key, string.Empty);
			if (!string.IsNullOrEmpty(token))
			{
				BaseCommandItem rewardItem = AddRewards(token,rewardDict);
				if(rewardItem != null)
					commondItemList.Add(rewardItem);
			}
			#endregion
		}

		List<NormalCommandItem> cmdList = new List<NormalCommandItem>();
		for (int i = 0; i < commondItemList.Count; i++)
		{
			BaseCommandItem commandItem = commondItemList[i];
			if (commandItem == null) continue;

			if(!commandItem.IgnoreTriggerCountLimit()) commandItem.DoCommandOperation();
			if (commandItem.IgnoreTriggerCountLimit())
			{
				cmdList.Add(commandItem as NormalCommandItem);
			}
		}
		
		HandlePopupRewards(cmdList);//CardSystem 相册系统的卡包和各种完成奖励FinishedSet、FinishedAlbum、FinishedPowerCards
		
		OnCommandUpdateEnd();
    }
	
	public void ParsePushReward(Dictionary<string, object> rewardDict)
	{
		if (rewardDict == null || rewardDict.Count == 0) return;
		#region 遍历内存中的Reward,已过期的发送ACK
		List<NormalCommandItem> discardRewardList = mNormalCommandItemList.FindAll(item =>
		{
			if (item == null) return false;
			return item.CheckNeedDiscard();
		});
		for (int i = 0; i < discardRewardList.Count; i++)
		{
			discardRewardList[i].OnDiscard();
		}
		List<BaseCommandItem> expireRewardList = m_CommandItemList.FindAll(item =>{
			if (item == null) return false;
			return item.IsExpiration();
		});
		for (int i = 0; i < expireRewardList.Count; i++)
		{
			BaseCommandItem commandItem = expireRewardList[i];
			commandItem.OnExpiration();
		}
		#endregion
		string token = Utils.Utilities.GetValue(rewardDict, GameConstants.Token_Key, string.Empty);
		if (string.IsNullOrEmpty(token)) return;
		if (mAckingCommandDict.ContainsKey(token))
		{
			RemoveRewardInfo info = mAckingCommandDict[token];
			return;
		}
		if (mToken_RewardItemDict.ContainsKey(token)) return;
		AddRewards(token, rewardDict);
		
		HandlePopupRewards(mNormalCommandItemList);//CardSystem 相册系统的卡包和各种完成奖励FinishedSet、FinishedAlbum、FinishedPowerCards

		OnCommandUpdateEnd();
	}

    public void ParseAndCreateSpinRewards(List<object> rewardsList)
    {
        if (rewardsList == null) return;

        for (int i = 0; i < rewardsList.Count; i++)
        {
            #region MyRegion
            Dictionary<string, object> rewardDict = rewardsList[i] as Dictionary<string, object>;
            string token = Utils.Utilities.GetValue<string>(rewardDict, GameConstants.Token_Key, string.Empty);
            if (string.IsNullOrEmpty(token)) continue;

            if (mAckingCommandDict.ContainsKey(token))
            {
                RemoveRewardInfo info = mAckingCommandDict[token];
                continue;
            }

            if (mToken_RewardItemDict.ContainsKey(token)) continue;

            AddRewards(token,rewardDict);

            #endregion
        }

        List<BaseCommandItem> backGroundCommandlist = m_CommandItemList.FindAll(item =>{
            if (item == null) return false;
            if (item.IsHasConditionList) return false;
            if (item.IsContactLocalFeature) return false;
            return item.eCommandType == ECommandType.CommandType_Event;
        });

        backGroundCommandlist.Sort(delegate(BaseCommandItem kvp1, BaseCommandItem kvp2){
            return (int)(kvp1.CreateTime - kvp2.CreateTime);
        });

        OnCommandUpdateEnd();

        for (int i = 0; i < backGroundCommandlist.Count; i++)
        {
            backGroundCommandlist[i].DoCommandOperation();
            backGroundCommandlist[i].OnAccept();
        }
    }


	public void ParseAndCreateQueryRewards(List<object> rewardsList)
	{
 		if (rewardsList == null) return;
		#region 遍历内存中的Reward,已过期的发送ACK
		List<NormalCommandItem> discardRewardList = mNormalCommandItemList.FindAll(item =>
			{
				if (item == null) return false;
				return item.CheckNeedDiscard();
			});

		for (int i = 0; i < discardRewardList.Count; i++)
		{
			discardRewardList[i].OnDiscard();
		}

		List<BaseCommandItem> expireRewardList = m_CommandItemList.FindAll(item =>{
			if (item == null) return false;
			return item.IsExpiration();
		});

		for (int i = 0; i < expireRewardList.Count; i++)
		{
            BaseCommandItem commandItem = expireRewardList[i];
            commandItem.OnExpiration();
		}

		HandleUselesssRewardCommand(null);
		#endregion

		for (int i = 0; i < rewardsList.Count; i++)
		{
			#region MyRegion
			Dictionary<string, object> rewardDict = rewardsList[i] as Dictionary<string, object>;
			string token = Utils.Utilities.GetValue<string>(rewardDict, GameConstants.Token_Key, string.Empty);
			if (!string.IsNullOrEmpty(token))
			{
				if (mAckingCommandDict.ContainsKey(token))
				{
					RemoveRewardInfo info = mAckingCommandDict[token];
					continue;
				}

				if (mToken_RewardItemDict.ContainsKey(token)) continue;

				AddRewards(token,rewardDict);
			}
			#endregion
		}

		List<BaseCommandItem> backGroundCommandlist = m_CommandItemList.FindAll(item =>{
			if (item == null) return false;
			if (item.IsHasConditionList) return false;
			if (item.IsContactLocalFeature) return false;
			return item.eCommandType == ECommandType.CommandType_Event;
		});

		backGroundCommandlist.Sort(delegate(BaseCommandItem kvp1, BaseCommandItem kvp2){
			return (int)(kvp1.CreateTime - kvp2.CreateTime);
		});

		OnCommandUpdateEnd();

		for (int i = 0; i < backGroundCommandlist.Count; i++)
		{
			backGroundCommandlist[i].DoCommandOperation();
			backGroundCommandlist[i].OnAccept();
		}

		HandlePopupRewards(mNormalCommandItemList);//CardSystem 相册系统的卡包和各种完成奖励FinishedSet、FinishedAlbum、FinishedPowerCards
	}

	#region ClubSystem
	/// <summary>
	/// Do Commands Which With No Condition And LocalFeature
	/// </summary>
	public void DoCommands ()
	{
		List<BaseCommandItem> backGroundCommandlist = m_CommandItemList.FindAll(item =>{
			if (item == null) return false;
			if (item.IsHasConditionList) return false;
			if (item.IsContactLocalFeature) return false;
			return item.eCommandType == ECommandType.CommandType_Event;
		});

		backGroundCommandlist.Sort((kvp1, kvp2) => (int) (kvp1.CreateTime - kvp2.CreateTime));

		OnCommandUpdateEnd();

		for (int i = 0; i < backGroundCommandlist.Count; i++)
		{
			backGroundCommandlist[i].DoCommandOperation();
			backGroundCommandlist[i].OnAccept();
		}
	}


	void HandlePopupRewards(List<NormalCommandItem> rewardsList)
	{
	}
	#endregion
	void CleanPlistCommand()
	{
		List<string> tokenList = new List<string>(mToken_RewardItemDict.Keys);
		for (int i = 0; i < tokenList.Count; i++)
		{
			string token = tokenList[i];
			if(string.IsNullOrEmpty(token)) continue;

			BaseCommandItem commandItem = mToken_RewardItemDict[token];
			if (commandItem == null) continue;
			if(!commandItem.IsCreateByApp) continue;

			commandItem.OnDeleteCommand(true);
		}
	}

	public void ParsePlistCommand()
	{
		CleanPlistCommand();

		ParsePlistTrigmentCommand();
		ParsePlistTournamentAd();
	}

	//Trigment是TriggerMoment的缩写
	public void ParsePlistTrigmentCommand()
	{
		Dictionary<string,object> commandsDict = Plugins.Configuration.GetInstance ().GetValueWithPath<Dictionary<string,object>> ("Module/Commands", null);
		if (commandsDict==null)return;

		long createTime = TimeUtils.ConvertDateMillTimeLong(DateTime.Now);
		Dictionary<string, BaseCommandItem> validActiviesDict = new Dictionary<string, BaseCommandItem>();
		foreach (string key in commandsDict.Keys)
		{
			Dictionary<string,object> cmdDict = commandsDict[key] as Dictionary<string,object>;
			if (cmdDict == null) continue;

			if (!mToken_RewardItemDict.ContainsKey(key))
			{
				cmdDict[GameConstants.Token_Key] = key;
				cmdDict[GameConstants.IsCreateByApp_Key] = true; 
				cmdDict[GameConstants.CreateTime_Key] = createTime;
				BaseCommandItem commandItem = AddRewards(key, cmdDict);
				if (commandItem != null)
				{
					if (commandItem.eCommandType != ECommandType.CommandType_Normal) continue;
					if (!commandItem.IsInTimeRange()) continue;
					if (commandItem.Priority > 0)
					{
						commandItem.ResetTriggerCountForPlistActivity();
						validActiviesDict[key] = commandItem;
					}
				}
			}
		}

		string strTokenList = SharedPlayerPrefs.GetPlayerPrefsStringValue(GameConstants.ActiviesTokens_Key, string.Empty);
		if (!string.IsNullOrEmpty(strTokenList))
		{
			List<object> tokenList = MiniJSON.Json.Deserialize(strTokenList) as List<object>;
			if (tokenList != null)
			{
				for (int i = 0; i < tokenList.Count; i++)
				{
					string strToken = tokenList[i] as string;
					if (!validActiviesDict.ContainsKey(strToken))
					{
						SharedPlayerPrefs.DeleteLocalDataBykey(strToken + GameConstants._LastTriggerTime);
						SharedPlayerPrefs.DeleteLocalDataBykey(strToken + GameConstants._TriggerCount);
					}
				}
			}
		}
		else
		{
			if (SharedPlayerPrefs.HasPlayerPrefsKey(GameConstants.ActiviesNames_Key))
				SharedPlayerPrefs.DeletePlayerPrefsKey(GameConstants.ActiviesNames_Key);
		}

		if (validActiviesDict.Count > 0)
		{
			List<string> tokenList = new List<string>(validActiviesDict.Keys);
			if (tokenList != null)
			{
				strTokenList = MiniJSON.Json.Serialize(tokenList);
				if (!string.IsNullOrEmpty(strTokenList))
					SharedPlayerPrefs.SetPlayerPrefsStringValue(GameConstants.ActiviesTokens_Key, strTokenList);
			}
		}

		OnCommandUpdateEnd();
	}

	public void ParsePlistTournamentAd()
	{
		Dictionary<string,object> commandsDict = Plugins.Configuration.GetInstance ().GetValueWithPath<Dictionary<string,object>> ("TournamentAd", null);
		if (commandsDict==null)return;

		long createTime = TimeUtils.ConvertDateMillTimeLong(DateTime.Now);
		foreach (string key in commandsDict.Keys)
		{
			Dictionary<string,object> cmdDict = commandsDict[key] as Dictionary<string,object>;
			if (cmdDict == null) continue;

			if (!mToken_RewardItemDict.ContainsKey(key))
			{
				cmdDict[GameConstants.Token_Key] = key;
				cmdDict[GameConstants.IsCreateByApp_Key] = true; 
				cmdDict[GameConstants.CreateTime_Key] = createTime;
				AddRewards(key, cmdDict);
			}
		}
	}
	
	BaseCommandItem AddRewards(string cmdToken,Dictionary<string, object> rewardDict)
	{
		string commandType = Utils.Utilities.GetValue<string>(rewardDict,GameConstants.CommandType_Key, GameConstants.NormalCommand_Key);
		if (string.IsNullOrEmpty(commandType)) return null;
		if (mToken_RewardItemDict.ContainsKey (cmdToken)) return null;
		
		BaseCommandItem rewardItem = CommandItemFactory.CreateCommandItem(commandType,cmdToken,rewardDict);
		if (rewardItem == null) return null;

		if (rewardItem.IsValid ()) 
		{
			
			if (CheckRewardByToken(cmdToken))
			{
				rewardItem.OnAccept();
				return null;
			}

			// 金币是0的collectcoins event 直接ack
			if (CheckZeroCoins(rewardItem))
			{
				rewardItem.OnAccept();
				return null;
			}
			//新添加的 CommandItem
			mToken_RewardItemDict [rewardItem.CommandToken] = rewardItem;
			m_CommandItemList.Add (rewardItem);

			if (rewardItem.eCommandType == ECommandType.CommandType_Normal)
				AddNormalCommandItemDict (rewardItem);
			else if (rewardItem.eCommandType == ECommandType.CommandType_Event)
				AddEventCommandItemDict (rewardItem);
			
			if (rewardItem.IsContactLocalFeature)
			{
				List<BaseCommandItem> featureCommandList = null;
				if (mFeature_RewardItemDict.ContainsKey(rewardItem.LocalFeature))
					featureCommandList = mFeature_RewardItemDict[rewardItem.LocalFeature];

				if (featureCommandList == null){
					featureCommandList = new List<BaseCommandItem>();
					mFeature_RewardItemDict[rewardItem.LocalFeature] = featureCommandList;
				}

				featureCommandList.Add(rewardItem);
				//优先级高的排在前面
				featureCommandList.Sort(delegate(BaseCommandItem kvp1, BaseCommandItem kvp2){
					return kvp2.Priority - kvp1.Priority;
				});
			}

			rewardItem.OnCommandItemCreateEnd ();
			return rewardItem;
		}

		return null;
	}
	
	public void AddAckRewardToken(string token)
	{
		if (ackRewardTokenList.Contains(token))
		{
			return;
		}

		ackRewardTokenList.Add(token);

		while (ackRewardTokenList.Count > 100)
		{
			ackRewardTokenList.Remove(ackRewardTokenList[0]);
		}

		SharedPlayerPrefs.SavePlayerPrefsListString(GameConstants.AckRewardTokenSave_Key, ackRewardTokenList);
	}

	private void InitAckRewardTokenList()
	{
		ackRewardTokenList = SharedPlayerPrefs.GetPlayerPrefsListString(GameConstants.AckRewardTokenSave_Key);
	}

	public bool CheckRewardByToken(string token)
	{
		return ackRewardTokenList.Contains(token);
	}
	
	// 金币数量为0的CollectCoinsEvent 直接ACk
	public bool CheckZeroCoins(BaseCommandItem rewardItem)
	{
		if (rewardItem == null)
			return false;
		return false;
	}
}

