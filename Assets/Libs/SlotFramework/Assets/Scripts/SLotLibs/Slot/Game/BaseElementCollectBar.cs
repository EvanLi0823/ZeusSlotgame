using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Classic;
using Libs;
using System;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class BaseElementCollectBar : MonoBehaviour {

	public UIProgressBar progressBar;
	public UIText collectText;
	public string CustomPrefix = "";
	public BaseProgressBarEffect barEffect;
	public double totalCollect {get; private set;}
	public  string collectElementKey{
		get{ 
			if (!string.IsNullOrEmpty(CustomPrefix)) {
				return  CustomPrefix+ "collect";
			} else {
				return SceneManager.GetActiveScene ().name + "collect";
			}
		}
	}
	public  string IncreaseCollectKey{
		get{ 
			if (!string.IsNullOrEmpty(CustomPrefix)) {
				return CustomPrefix+ "gold";
			} 
			else {
				return SceneManager.GetActiveScene ().name + "gold";
			}
		}
	}
	public string StartCollectKey{
		get{ 
			if (!string.IsNullOrEmpty(CustomPrefix)) {
				return CustomPrefix+ "start";
			} 
			else {
				return SceneManager.GetActiveScene ().name + "start";
			}
		}
	}
	public string UpdateCollectUIKey{ 
		get{ 
			if (!string.IsNullOrEmpty(CustomPrefix)) {
				return CustomPrefix+ "UpdataUI";
			} 
			else {
				return SceneManager.GetActiveScene ().name + "UpdataUI";
			}
		}
	}

	private string saveIncreaseNumberkey{
		get{ 
			if (!string.IsNullOrEmpty(CustomPrefix)) {
				return CustomPrefix+ "CollectIncreaseNumber";
			} 
			else {
				return SceneManager.GetActiveScene ().name + "CollectIncreaseNumber";
			}
		}
	}
	private string saveCollectImageFillAmountkey {
		get{ 
			if (!string.IsNullOrEmpty(CustomPrefix)) {
				return CustomPrefix+ "CoinsCollectImageFillAmount";
			} 
			else {
				return SceneManager.GetActiveScene ().name + "CoinsCollectImageFillAmount";
			}
		}
	}
	private string saveCollectFillAmountkey {
		get{ 
			if (!string.IsNullOrEmpty(CustomPrefix)) {
				return CustomPrefix+ "CoinsCollectFillAmount";
			} 
			else {
				return SceneManager.GetActiveScene ().name + "CoinsCollectFillAmount";
			}
		}
	}
	private string CollectFeatureAward{
		get{ 
			if (!string.IsNullOrEmpty(CustomPrefix)) {
				return CustomPrefix+ "CollectFeatureAward";
			} 
			else {
				return SceneManager.GetActiveScene ().name + "CollectFeatureAward";
			}
		}
	}

	private string IsCollectFulledKey{
		get{ 
			if (!string.IsNullOrEmpty(CustomPrefix)) {
				return CustomPrefix+ "IsCollectFulled";
			} 
			else {
				return SceneManager.GetActiveScene ().name + "IsCollectFulled";
			}
		}
	}

	public bool IsCollectFulled{
		get{
			return SharedPlayerPrefs.GetPlayerBoolValue (IsCollectFulledKey, false);
		}
		set{
			SharedPlayerPrefs.SetPlayerPrefsBoolValue (IsCollectFulledKey, value);
		}
	}
	//public Transform target;

	private double initCollectIncreaseNum = 0;
	private float initCollectLightIncreaseNum = 0;
	public float collectIncreaseTime = 0.5f;

	double collect = 0;
	double collectImageFillAmount = 0;

	private double initCollectIncreaseValue = 0;
	double collectValue = 0;
	double collectFillAmount = 0;
	protected virtual void Awake()
	{
		Messenger.AddListener<double,double,bool>(IncreaseCollectKey, IncreaseCollect);
		Messenger.AddListener<BaseElementPanel>(collectElementKey, OnPlayCollectPathMsg);
		Messenger.AddListener(StartCollectKey, ResetCollectProgressBar);
		Messenger.AddListener<double>(UpdateCollectUIKey, UpdateUI); 
		Messenger.AddListener (GameConstants.OnSlotMachineSceneInit, InitData);
		if (barEffect==null) {
			barEffect = GetComponent<BaseProgressBarEffect> ();
		}
	}

	protected virtual void OnDestroy()
	{
		Messenger.RemoveListener<double,double,bool>(IncreaseCollectKey, IncreaseCollect);
		Messenger.RemoveListener<BaseElementPanel>(collectElementKey, OnPlayCollectPathMsg);
		Messenger.RemoveListener(StartCollectKey, ResetCollectProgressBar);
		Messenger.RemoveListener<double>(UpdateCollectUIKey, UpdateUI);
		Messenger.RemoveListener (GameConstants.OnSlotMachineSceneInit, InitData);
	}

	protected virtual void InitData()
	{
		double goldFillAmount = Utils.Utilities.GetValueDoubleFromFloat(saveCollectImageFillAmountkey);
		double fillAmount = Utils.Utilities.GetValueDoubleFromFloat(saveCollectFillAmountkey,0);

		IncreaseCollect(goldFillAmount,fillAmount , false);
	}

	public float delayCollectTime;//选出最大延迟时间
	public CurveCollectPathController mCurvePathController;
	protected virtual void OnPlayCollectPathMsg(BaseElementPanel elementPanel)
	{
		if (mCurvePathController == null) return;
		float aniDuration = mCurvePathController.DoCurvePath(elementPanel);
		if (aniDuration < 0) return;
		if (delayCollectTime > float.Epsilon && aniDuration > delayCollectTime) return;

		delayCollectTime = aniDuration;
	}

	void UpdateUI(double increaseNumber)
	{
		Utils.Utilities.SetValueDoubleFromFloat(saveIncreaseNumberkey, increaseNumber);
		collectText.SetText("$"+Utils.Utilities.ThousandSeparatorNumber(increaseNumber));
	}

	void IncreaseCollect(double collect_progress,double collectElementValue, bool isPlayEffect = true)
	{
		this.collect = collect_progress;
		this.collectValue = collectElementValue;
		SetCurrentCollectProgess(collect_progress,collectElementValue);
	}

	public void OnClickButton()
	{
		ResetCollectProgressBar();
		UpdateUI(0);
	}
		

	private void SetCurrentCollectProgess(double progress,double collectValue)
	{
		Utils.Utilities.AnimationTo(initCollectIncreaseNum,progress,collectIncreaseTime,UpdateCollectProgressBar,null,() => {
			//更新数据状态
			collectImageFillAmount += this.initCollectIncreaseNum;
			this.initCollectIncreaseNum = 0;
			collect = 0;
			Utils.Utilities.SetValueDoubleFromFloat(saveCollectImageFillAmountkey, collectImageFillAmount);
		});
		Utils.Utilities.AnimationTo (initCollectIncreaseValue,collectValue, collectIncreaseTime, UpdateCollectElementAmount, null, () => {
			collectFillAmount+=this.initCollectIncreaseValue;
			this.initCollectIncreaseValue = 0;
			collectValue = 0;
			Utils.Utilities.SetValueDoubleFromFloat(saveCollectFillAmountkey,collectFillAmount);
		});
	}


	//更新收集金币进度条
	protected virtual void UpdateCollectProgressBar(double num)
	{
		initCollectIncreaseNum = num;
		float current = Utils.Utilities.CastValueFloat (collectImageFillAmount + this.initCollectIncreaseNum);
		if (current >= 1)
		{
			current = 1;
			totalCollect = collectFillAmount + this.collectValue;
			IsCollectFulled = true;
		}
		else if (current>0) {
			if (barEffect != null) barEffect.SetProgress (current);
		}

		progressBar.SetProgress(current);
	}

	protected virtual void UpdateCollectElementAmount(double amount){
		initCollectIncreaseValue = amount;
		float current = Utils.Utilities.CastValueFloat (collectFillAmount + this.initCollectIncreaseValue);
		//Debug.Log ("UpdateCollectElementAmount:"+Utils.Utilities.ThousandSeparatorNumber(current));
		collectText.SetText("$"+Utils.Utilities.ThousandSeparatorNumber(current));
	}
	//重置收集金币进度条
	public virtual void ResetCollectProgressBar()
	{
		progressBar.SetProgress(0);
		collectImageFillAmount = 0;
		collectFillAmount = 0;
		UpdateUI(0);
		Utils.Utilities.SetValueDoubleFromFloat(saveCollectImageFillAmountkey, 0);
		Utils.Utilities.SetValueDoubleFromFloat(saveIncreaseNumberkey, 0);
		Utils.Utilities.SetValueDoubleFromFloat(saveCollectFillAmountkey,0);
		IsCollectFulled = false;
	}
}
