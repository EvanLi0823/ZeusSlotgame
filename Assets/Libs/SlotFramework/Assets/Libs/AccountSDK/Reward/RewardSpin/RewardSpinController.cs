using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Classic;
using TMPro;
/// <summary>
/// Reward spin controller.
/// 机制为控制器自检，发现条件满足，进入场景自动触发,
/// 全部改为被动式UI刷新，功能性逻辑移植到RewardSpinManager
/// </summary>
public class RewardSpinController : MonoBehaviour {
	
	public GameObject RewardSpinNode;
	//[Header("当机器处于freespin时，会调整freespin按钮到rewardspin按钮的上层,调整层级时注意")]
	public GameObject topPanel;
	public GameObject bottomPanel;
	//public TextMeshProUGUI contentTxt;
	public TextMeshProUGUI spinTimesTxt;
	public Animator spinNumPanel;
	protected NormalCommandItem rewardSpinItemData;
	private int TotalRewardSpinNum;
	private string FromSource;
	private string InitRewardPanelContext;

	void Awake(){
		EnableRewardSpinNode (false);
		EnableMask(false);
		//Messenger.AddListener<int> (START_REWARD_SPIN,StartRewardSpin);
		Messenger.AddListener<int>(RewardSpinManager.REFRESH_UI_REWARD_SPIN_NUM,RefreshRewardSpinPanel);
		Messenger.AddListener<NormalCommandItem> (RewardSpinManager.ENABLE_REWARD_SPIN_PANEL,EnableRewardSpinPanel);
		//当根对象先激活，后广播bonus执行完毕事件
		Messenger.AddListener (GameConstants.BonusGameIsOver,CheckAnimation);
	}

	//当bonus先广播，后激活根对象时，执行此方法
	void OnEnable(){
		if (RewardSpinManager.Instance.BonusOverRewardIsValid) {
			CheckAnimation ();
			RewardSpinManager.Instance.BonusOverRewardIsValid = false;
		}
	}
	void OnDestroy(){
		//Messenger.RemoveListener<int> (START_REWARD_SPIN,StartRewardSpin);
		Messenger.RemoveListener<int>(RewardSpinManager.REFRESH_UI_REWARD_SPIN_NUM,RefreshRewardSpinPanel);
		Messenger.RemoveListener<NormalCommandItem> (RewardSpinManager.ENABLE_REWARD_SPIN_PANEL,EnableRewardSpinPanel);
		Messenger.RemoveListener (GameConstants.BonusGameIsOver,CheckAnimation);
	}
	public const string ENTER_MACHINE_INIT_CONTENT="enterMachineInitContent";
	void EnableRewardSpinPanel(NormalCommandItem ci){
		if (ci!=null)
		{
			EnableMask(true);
			rewardSpinItemData = ci;
			RewardSpinEvent rse = rewardSpinItemData.CommandEvent as RewardSpinEvent;
			SequencePaidResultsManager.ChangeSequncePaidResults (Utils.Utilities.GetValue<string>(rse.EventDict,"p",string.Empty));
			TotalRewardSpinNum =rse.SpinNum;
			InitRewardPanelContext = Utils.Utilities.GetValue<string>(rse.ParaDict,ENTER_MACHINE_INIT_CONTENT,"FREE SPINS");

			Libs.DelayAction da = new Libs.DelayAction (1.5f,null,()=>{
				InitUI ();
			});
			da.Play ();
		} 
		else {
			//播放Hide动画
			if (spinNumPanel!=null) {
				spinNumPanel.SetTrigger ("Hide");
			}
			Libs.DelayAction da = new Libs.DelayAction (0.5f,null,()=>{
				EnableRewardSpinNode(false);
				EnableMask(false);
			});
			da.Play ();

		}
	}


	string totalSpinText="";
	void InitUI(){
		EnableRewardSpinNode(true);
		//播放show动画
		if (spinNumPanel!=null) {
			spinNumPanel.SetTrigger ("Show");
		}

		if (spinTimesTxt!=null) {
			totalSpinText = Utils.Utilities.ThousandSeparatorNumber (TotalRewardSpinNum, false);
			spinTimesTxt.text = Utils.Utilities.ThousandSeparatorNumber(TotalRewardSpinNum,false)+"/"+totalSpinText;
		}
	}
	void RefreshRewardSpinPanel(int leftNum){
		if (spinTimesTxt!=null) {
			spinTimesTxt.text = Utils.Utilities.ThousandSeparatorNumber(leftNum,false)+"/"+totalSpinText;
		}

	}
	void EnableRewardSpinNode(bool enable){
		if (RewardSpinNode!=null) {
			RewardSpinNode.SetActive (enable);
		}
	}

	void EnableMask(bool enable)
	{
		if (bottomPanel!=null) {
			bottomPanel.SetActive (enable);
		}
		if (topPanel!=null) {
			topPanel.SetActive (enable);
		}
	}
	void CheckAnimation(){
		//播放show动画
		if (spinNumPanel!=null&&RewardSpinManager.Instance.RewardIsValid()) {
			spinNumPanel.SetTrigger ("Show");
		}
	}
}
