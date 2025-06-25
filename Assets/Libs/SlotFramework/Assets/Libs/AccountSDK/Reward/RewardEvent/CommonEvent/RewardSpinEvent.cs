using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Classic;
using Libs;
using System;
/// <summary>
/// Reward spin event.
/// 代码中提供了支持远端下载更新界面，虽然暂时性不用
/// 目前需求文案为固定，不需要下载，需要改文案，且不下载的话，请调整代码默认值
/// </summary>
public class RewardSpinEvent : BaseEvent {
	public const string MACHINE_NAME ="machineName";
	public const string SPIN_TIMES= "spinNum";
	public const string FIXED_BET ="fixedBet";

	public const string NAME_KEY = "name";
	public const string SLOTURL_KEY = "slot_url";

	public string MachineName { get;private set;}
	public int SpinNum{ 
		get{
			return Utils.Utilities.GetInt(ParaDict,SPIN_TIMES,GameConstants.DEFAULT_SPIN_NUM);
		}
	}
	public long FixedBet {
		get{
			return Utils.Utilities.GetLong(ParaDict,FIXED_BET,100);
		}
	}

	#region Event State Flag
	private string subType;
	public string SubType {
		get {
			return !string.IsNullOrEmpty(subType)?subType:GameConstants.InboxOffer;
		}
		set {
			subType = value;
		}
	}

	#endregion



	public RewardSpinEvent(Dictionary<string, object> dict, object owner):base(dict,owner){
		MachineName = Utils.Utilities.GetValue<string>(ParaDict,MACHINE_NAME,string.Empty);
	}

	public override bool IsValid(){
		return !string.IsNullOrEmpty(MachineName);
	}

	public override string GetSlotNameFromEvent()
	{
		return MachineName;
	}

	public override void ExcuteEventAction(Dictionary<string,object> ParameterDict)
	{
		Dictionary<string,object> esMsgDict = ParameterDict[GameConstants.ESMsgDict_Key] as Dictionary<string,object>;
		ICommandEvent acceptEvent = ParameterDict[GameConstants.ICommandEvent_Key] as ICommandEvent;
		if (RewardSpinManager.Instance.CheckRewardSpinItem (mOwner))
		{
			SwapSceneManager.Instance.IsFsAwardEnterMachine = true;
			SwapSceneManager.Instance.EnterSlotType = Utils.Utilities.GetValue<string>(ParameterDict,GameConstants.EnterSlotSource_Key,GameConstants.EnterSlotType_Unknow);
            Action startCB = () => {
                if (BaseSlotMachineController.Instance != null)
                {
                    BaseSlotMachineController.Instance.SaveDataBeforeSwitchScene();
                }
            };
            SwapSceneManager.Instance.SwapGameScene (MachineName, true,startCB,null,true);
		} else {
			#if DEBUG||UNITY_EDITOR
			string strError = "RewardSpinEvent ExcuteEventAction() function CheckRewardSpinItem false,please check server json or plist config";
			Utils.Utilities.LogError(strError);
			#endif
		}
		esMsgDict["RewardMachine"] = MachineName;
		esMsgDict["FixedBet"]= FixedBet;
		esMsgDict["RewardSpinNum"] = SpinNum;
		BaseGameConsole.ActiveGameConsole().LogBaseEvent(Analytics.RewardSpinClicked, esMsgDict);
		Messenger.Broadcast (GameConstants.CloseInboxDialog);
	}


	public override void OnShow(Dictionary<string,object> dict){
		dict["RewardMachine"] = MachineName;
		dict["FixedBet"]= FixedBet;
		dict["RewardSpinNum"] = SpinNum;
		BaseGameConsole.ActiveGameConsole ().LogBaseEvent (Analytics.ShowRewardSpin, dict);
	}
}
