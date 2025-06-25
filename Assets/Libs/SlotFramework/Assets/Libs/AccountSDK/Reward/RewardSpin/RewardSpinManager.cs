using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Classic;
using Libs;
using UniRx.Async;
using UnityEngine.SceneManagement;
public class RewardSpinManager  {
	public const string ENABLE_REWARD_SPIN_PANEL = "EnableRewardSpinPanel";
	public const string REFRESH_LEFT_REWARD_SPIN_NUM = "RefreshLeftRewardSpinNum";
	public const string REFRESH_UI_REWARD_SPIN_NUM = "RefreshUIRewardSpinNum";
	private Dictionary<string,NormalCommandItem> rewardSpinDict = new Dictionary<string, NormalCommandItem>();
	private RewardSpinManager(){
		Messenger.AddListener (GameConstants.OnSlotMachineSceneInit,InitMachineData);
		Messenger.AddListener<double>(SlotControllerConstants.OnRefreshRewardSpin,HandleRewardSpinWinCoins);
		Messenger.AddListener (GameConstants.BonusGameIsOver,CheckActivateAnimationCondition);
	}

	~RewardSpinManager(){
		Messenger.RemoveListener (GameConstants.OnSlotMachineSceneInit,InitMachineData);
		Messenger.RemoveListener<double>(SlotControllerConstants.OnRefreshRewardSpin,HandleRewardSpinWinCoins);
		Messenger.RemoveListener (GameConstants.BonusGameIsOver,CheckActivateAnimationCondition);
	}

	public bool hasFreeSpin = false;
	private NormalCommandItem currentCmdItem = null;
	public bool BonusOverRewardIsValid=false;
	private double totalWinCoins=0;
	public double TotalWinCoins {
		get {
			return totalWinCoins;
		}
	}
	void CheckActivateAnimationCondition(){
		if (RewardIsValid()) {
			BonusOverRewardIsValid = true;
		} 
		else {
			BonusOverRewardIsValid = false;
		}
	}
	private int leftSpinNum=0;
	private enum RewardSpinState
	{
		Disabled,
		Enabled,
		Run,
		Finish,
	}
	RewardSpinState currentState=RewardSpinState.Disabled;
	private void InitState(int rewardspinNum){
		totalWinCoins = 0;
		leftSpinNum = rewardspinNum;
	}
//	是否最后一次fs award 结束
	private bool isCloseDialog = false;
	public async Task<bool>  CheckRewardSpinEnd(){
		if (CheckEndCondition()) {
			currentState = RewardSpinState.Finish;
			RewardSpinEvent rse = currentCmdItem.CommandEvent as RewardSpinEvent;
			Messenger.Broadcast<NormalCommandItem> (ENABLE_REWARD_SPIN_PANEL,null);
			isCloseDialog = false;
			Messenger.Broadcast<RewardSpinEvent,System.Action> (GameDialogManager.OpenRewardSpinEndDialog,rse,OnCloseRewardSpinEndDialogCallBack);
			await UniTask.WaitUntil(()=>isCloseDialog);
			return true;
		}

		return false;
	}

	void HandleRewardSpinWinCoins(double winCoins){
		if (RewardIsValid()) {
			totalWinCoins = winCoins;
			if (SpinIsValid()) {
				leftSpinNum--;
				Messenger.Broadcast<int> (REFRESH_UI_REWARD_SPIN_NUM,leftSpinNum);
			}
		}
	}

	private bool SpinIsValid(){
		if (BaseSlotMachineController.Instance!=null) {
			return !BaseSlotMachineController.Instance.reelManager.isFreespinBonus && 
				!BaseSlotMachineController.Instance.onceMore;
		}
		return false;
	}
	public bool RewardIsValid(){
		if (currentState!=RewardSpinState.Disabled && currentState!=RewardSpinState.Finish) {
			return leftSpinNum >= 0;
		}

		return false;
	}

	public bool IsLastRewardSpin()
	{
		return currentState != RewardSpinState.Disabled && currentState == RewardSpinState.Finish && leftSpinNum == 0;
	}

	public bool RewardSpinIsValid(){
		return RewardIsValid () && SpinIsValid ();
	}

	public bool CheckEndCondition(){
		return currentState==RewardSpinState.Run&&leftSpinNum == 0;
	}

	void OnCloseRewardSpinEndDialogCallBack(){
		if (BaseSlotMachineController.Instance!=null&&BaseSlotMachineController.Instance.slotMachineConfig!=null) {
			BaseSlotMachineController.Instance.currentBetting = UserManager.GetInstance ().getBetByBalance (BaseSlotMachineController.Instance.slotMachineConfig);
			Messenger.Broadcast<long> (SlotControllerConstants.OnBetChange,BaseSlotMachineController.Instance.currentBetting);
			SendRewardSpinLogEvent();
			isCloseDialog = true;
		}
	}

	public void SendRewardSpinLogEvent(){
		if (currentState == RewardSpinState.Run||currentState==RewardSpinState.Finish) {
			RewardSpinEvent rse = currentCmdItem.CommandEvent as RewardSpinEvent;
			//todo SendStatisticsEvent  FixedBet TotalWinCoins,
			Dictionary<string,object> dict = new Dictionary<string, object>();
			dict.Add ("FixedBet",rse.FixedBet);
			dict.Add ("WinCoins",totalWinCoins);
			dict.Add (GameConstants.SubType_Key,rse.SubType);
			dict.Add ("leftSpinNum",leftSpinNum);
			dict.Add ("finished",(currentState==RewardSpinState.Finish)?"true":"false");
			dict.Add ("token",currentCmdItem.CommandToken);
			BaseGameConsole.ActiveGameConsole ().LogBaseEvent (Analytics.REWARD_FREE_SPIN, dict);
			if (currentState==RewardSpinState.Finish) {
				currentState = RewardSpinState.Disabled;
			}
		}
	}

	public static RewardSpinManager Instance{
		get{ 
			return Singleton<RewardSpinManager>.Instance;
		}
	}

	public bool CheckRewardSpinItem(object obj){
		NormalCommandItem ci= obj as NormalCommandItem;
		if (ci==null) return false;
		
		if (!rewardSpinDict.ContainsKey(ci.CommandToken)) {
			rewardSpinDict.Add (ci.CommandToken, ci);
			RewardSpinEvent rse = ci.CommandEvent as RewardSpinEvent;
		}
		return true;
	}

	public NormalCommandItem FindRewardSpinItemWhenEnterMachine(string machineName){
		foreach (string token in rewardSpinDict.Keys) {
			RewardSpinEvent rse= rewardSpinDict [token].CommandEvent as RewardSpinEvent;
			if (rse.MachineName.Equals(machineName)) {
				return rewardSpinDict [token];
			}
		}
		return null;
	}

	void InitMachineData()
	{
		hasFreeSpin = false;
		currentCmdItem = FindRewardSpinItemWhenEnterMachine (SwapSceneManager.Instance.GetLogicSceneName());
		if (currentCmdItem!=null)
		{
			hasFreeSpin = true;
			RewardSpinEvent rse = currentCmdItem.CommandEvent as RewardSpinEvent;
			InitRewardSpin (rse);
			currentState = RewardSpinState.Enabled;
			Messenger.Broadcast<NormalCommandItem> (ENABLE_REWARD_SPIN_PANEL,currentCmdItem);
			DelayAction da = new DelayAction (2f,null,()=>{
				currentCmdItem.OnAccept ();
				BaseSlotMachineController.Instance.DoSpin ();
				currentState = RewardSpinState.Run;
			});
			da.Play ();
		} else {
			currentState = RewardSpinState.Disabled;
		}
	}
	void InitRewardSpin(RewardSpinEvent rse){
		if (BaseSlotMachineController.Instance!=null&&BaseSlotMachineController.Instance.reelManager!=null) {
			BaseSlotMachineController.Instance.currentBetting = rse.FixedBet;
			InitState (rse.SpinNum);
			Messenger.Broadcast<long> (SlotControllerConstants.OnBetChange,rse.FixedBet);
		}
	}
}
