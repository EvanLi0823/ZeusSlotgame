using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Libs;
using Classic;
using System;

public class NormalCommandItem : BaseCommandItem
{

	public bool IsInboxCommand { get; private set; }//是否可以放在Inbox中
	private bool IsDataValid = false;
	private bool IsNeedPopup;//是否可以弹出

	public string ShowDialogName
	{
		get;
		private set;
	}

	public bool CheckShowType(string dialogName)
	{
		return ShowDialogName.Equals(dialogName);
	}
	public string triggerType{ get; private set; }
	
	public NormalCommandItem(string cmdToken,Dictionary<string,object> dict) : base(cmdToken,dict)
	{
		if (!base.IsValid()) return;

		ShowDialogName = Utils.Utilities.GetValue<string>(ItemDict, GameConstants.ShowDialogName_Key, string.Empty);
		triggerType = Utils.Utilities.GetValue<string>(GetExtraDict(), GameConstants.TRIGGER_TYPE_KEY,string.Empty);
		if(!string.IsNullOrEmpty(ShowDialogName))
		{
			bool showDialogNameEqualsInboxItem = ShowDialogName.Equals(GameConstants.InboxItem_Key);
			IsInboxCommand = showDialogNameEqualsInboxItem || Utils.Utilities.GetBool(ItemDict, GameConstants.IsStoreInbox_Key, false);
			IsNeedPopup = !showDialogNameEqualsInboxItem;
			if(IsNeedPopup)
			{
				if(Priority == 0) Priority = 1;
				CreateDefaultCondition ();
			}
		}
		else
		{
			IsNeedPopup = IsInboxCommand = false;
			Priority = 0;
		}

		eCommandType = ECommandType.CommandType_Normal;

		if (!CheckRewardSpinInboxItem()) return;
		
		IsDataValid = true;
		
	}

	public override bool IsValid() { return IsDataValid && base.IsValid();}

	public override void OnCommandItemCreateEnd()
	{
		ParseCornerConfig();
		base.OnCommandItemCreateEnd();
	}

	protected void ParseCornerConfig()
	{
	}

	protected void CreateDefaultCondition()
	{
		if (conditionList.Count == 0 && IsHasConditionList)
		{
			conditionList = new List<EventCondition>();
			Dictionary<string,object> dict = new Dictionary<string, object>();
			dict.Add(BaseCondition.CONDITION_TYPE_NODE, ConditionFactory.EVENT_CONDITON_TYPE);
			dict.Add(EventCondition.EVENT_ACTION_R_NODE, CommandEvent.EventName);
			Dictionary<string,object> parm = new Dictionary<string, object>();
			string flags = GameConstants.BACK_TO_APP + "," + GameConstants.BACK_TO_LOBBY + "," + GameConstants.ENTER_LOBBY;
			parm.Add(ConditionContentCellFactory.TRIGGER_MOMENT, flags);
			dict.Add(BaseCondition.CONDITION_CONTENT_NODE, parm);
			conditionList.Add(ConditionFactory.CreateCondition(dict[BaseCondition.CONDITION_TYPE_NODE].ToString(), dict)as EventCondition);
		}
  	}

	//现阶段,条件触发至只与 Popup 相关
	public override bool CheckCommandTriggerConditionsIsOk()
	{
		if (!CommandEvent.IsReady()) return false;

        if (IsNeedPopup){
            if (!CheckPopUpCondition()) return false;
			if (!mPeriodFrequencyTrigger.IsCanTrigger()) return false;
			if(CommandManager.Instance.IsCommandDisplay(CommandToken)) return false;
		}

		return base.CheckCommandTriggerConditionsIsOk();//现阶段,条件触发只与 Popup 相关
	}

	//现阶段,条件触发只与 Popup 相关
	public override void DoCommandOperation(Dictionary<string, object> contextInfo = null)
	{
		if (IsNeedPopup){
			mPeriodFrequencyTrigger.OnTrigger();
			if (ShowDialogName == "AssetBundleDialog")
	        {
		        Messenger.Broadcast<NormalCommandItem,string>(ShowDialogName, this, null);
	        }
			else
			{
				Messenger.Broadcast<NormalCommandItem>(ShowDialogName, this);	
			}
			
		}
	}

	public virtual bool IsCanShowInInboxNow()
    {
        if (!IsInboxCommand) return false;
        if (!mPeriodFrequencyTrigger.IsInTimeRange()) return false;

        if (!IsCanDisplay()) return false;
        //if (!CheckBGPicExist()) return false;
        return true;
    }
	
	public bool CheckNeedDiscard()
	{
		if (IsInboxCommand) return false;
        if (mPeriodFrequencyTrigger.EndTimeStamp > 0) return false;
		if (IsNeedPopup) return !mPeriodFrequencyTrigger.IsHaveTriggerCount();
		return false;
	}

	public void ShowPopupDialog(string bundleName = null, string prefab_trigger = null)
	{
		if (!IsNeedPopup) return;
        if (!CheckPopUpCondition()) return;
        
        if (string.IsNullOrEmpty(bundleName))
        {
	        if (ShowDialogName == "AssetBundleDialog")
	        {
		        Messenger.Broadcast<NormalCommandItem,string>(ShowDialogName, this, prefab_trigger);
	        }
	        else
	        {
				Messenger.Broadcast<NormalCommandItem>(ShowDialogName, this);    
	        }
	        
        }
        else
        {
	        Messenger.Broadcast<NormalCommandItem,string>(ShowDialogName, this,bundleName);
        }
	}

	public override void OnDeleteCommand(bool isDelByReload = false)
	{
		Messenger.Broadcast (GameConstants.RemoveCorner_ + CommandToken);

		base.OnDeleteCommand(isDelByReload);
	}

    
    public bool CheckRewardSpinInboxItem()
    {
	    if (CommandEvent!=null&& CommandEvent.EventName==EventFactory.REWARD_SPIN_EVENT_KEY)
	    {
		    string slotName = CommandEvent.GetSlotNameFromEvent();
		    if (string.IsNullOrEmpty(slotName)) { return false; }
		    SlotMachineConfig slotMachineConfig = BaseGameConsole.ActiveGameConsole (true).SlotMachineConfig (slotName);
		    if (slotMachineConfig == null) { return false; }
	    }

	    return true;
    }
}
