using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Libs;
using DG.Tweening;

namespace Classic
{
public class ProgressiveAward : ProgressiveAward01Base 
{
		public  enum FruitType{
			TYPE1 = 0,
			TYPE2 =1,
			TYPE3 =2
		}
		private FruitType type;
		private long showNum;
		private Animator awardAnimator;
		private long curShowNum = 0;

		public static void ShowAward(long money,int fruitType,System.Action dialogCloseCallBack =null)
		{
//			ProgressiveAward awardUI = null;
			if (BaseGameConsole.ActiveGameConsole ().IsInSlotMachine ()) {
				if (fruitType == (int)FruitType.TYPE1)
				{
					UIManager.Instance.OpenSystemDialog(new OpenConfigParam<ProgressiveAward01>(
						openType: OpenType.FrontOfHead, uiPopupStrategy: new SystemUIPopupStrategy(),
						dialogInitCallBack: (awardUI) =>
						{
							awardUI.SetMoney(money);
						}, dialogCloseCallBack: dialogCloseCallBack, animationIn: UIAnimation.NOAnimation,
						animationOut: UIAnimation.NOAnimation));
				} else if (fruitType == (int)FruitType.TYPE2) {
					UIManager.Instance.OpenSystemDialog(new OpenConfigParam<ProgressiveAward02>(
						openType: OpenType.FrontOfHead, uiPopupStrategy: new SystemUIPopupStrategy(),
						dialogInitCallBack: (awardUI) =>
						{
							awardUI.SetMoney(money);
						}, dialogCloseCallBack: dialogCloseCallBack, animationIn: UIAnimation.NOAnimation,
						animationOut: UIAnimation.NOAnimation));
				} else if (fruitType == (int)FruitType.TYPE3) {
					UIManager.Instance.OpenSystemDialog(new OpenConfigParam<ProgressiveAward03>(
						openType: OpenType.FrontOfHead, uiPopupStrategy: new SystemUIPopupStrategy(),
						dialogInitCallBack: (awardUI) =>
						{
							awardUI.SetMoney(money);
						}, dialogCloseCallBack: dialogCloseCallBack, animationIn: UIAnimation.NOAnimation,
						animationOut: UIAnimation.NOAnimation));
				}
			}
		}

     	protected override void Awake()
    	{
//			Debug.LogError("ProgressiveAward.awake");
       	 	base.Awake();
			awardAnimator = this.Animation.GetComponent<Animator> ();
			UGUIEventListener.Get (this.gameObject).onClick = OnButtonClickHandler;
			UGUIEventListener.Get (this.BtnBg.gameObject).onClick = OnButtonClickHandler;
//			this.TxtSkip.gameObject.SetActive (false);
    	}

		public void SetMoney(long money)
		{
//			Awake();
			this.WinText.text = "";
			this.showNum = money;
			this.awardAnimator.SetInteger ("state", 0);
			new DelayAction(1.2f, null, ()=> {
				AudioEntity.Instance.PlayRollUpEffect();
				tweener = Utils.Utilities.AnimationTo(curShowNum,showNum,5f,onUpdateTxt,null,onCompleteUpdate);
				progressiveSkipButtonClicked = false;
			}).Play();
			//			this.BtnSkip.gameObject.SetActive (false);
		}

		public void StartAddNum(){

			stateAnimation = 1;
//			this.TxtSkip.gameObject.SetActive (true);
//			this.BtnSkip.gameObject.SetActive (true);
			this.awardAnimator.SetInteger ("state", 1);

		}

		private void onUpdateTxt(long num){
			this.WinText.text =  Utils.Utilities.ThousandSeparatorNumber(num);
			curShowNum = num;
		}

		private void onCompleteUpdate()
		{
			//Libs.SoundEntity.Instance.RollEnd ();
			Libs.AudioEntity.Instance.PlayRollEndEffect();
			stateAnimation = 2;
			AudioEntity.Instance.StopRollingUpEffect ();
			AudioEntity.Instance.PlayRollEndEffect ();
//			this.TxtSkip.text = "Tap to close";
			StartCoroutine (StartEndAnimation ());
//			this.awardAnimator.SetInteger ("state", 2);
		}

		private bool progressiveSkipButtonClicked = true;

   		public override void OnButtonClickHandler(GameObject go)
    	{
			//Debug.Log("OnButtonClickHandler " + go.name);
    	    base.OnButtonClickHandler(go);
//			if (this.BtnBg.gameObject == go) {
//				if  (tweener!=null){
//					tweener.Complete();
//				}
//				this.WinText.text = this.showNum.ToString();
//				StartCoroutine(StartEndAnimation());
//				this.BtnBg.gameObject.SetActive(false);
//				UIManager.Instance.Close(this);
//			}
			if (stateAnimation == 2) {;
				this.awardAnimator.SetInteger ("state", 2);
			} else if (stateAnimation ==1){
				if  (tweener!=null){
					tweener.Complete();
				}
				this.WinText.text = Utils.Utilities.ThousandSeparatorNumber(showNum);
				onCompleteUpdate();
			}
			if (!progressiveSkipButtonClicked) {
				if (go.GetComponent<Button>() == skipButton && go.GetComponent<Button>() != null) {
					progressiveSkipButtonClicked = true;
					new DelayAction(0.5f, null, this.ShowOut).Play();
				}
				else {
					progressiveSkipButtonClicked = true;
					new DelayAction(2.0f, null, this.ShowOut).Play();
				}
				if  (tweener!=null){
					tweener.Complete(true);
				}
				this.WinText.text = Utils.Utilities.ThousandSeparatorNumber(showNum);
			}
//			this.ShowOut();
    	}

		private IEnumerator StartEndAnimation()
		{
			yield return new WaitForSeconds (2f);
			stateAnimation = 2;
			this.awardAnimator.SetInteger ("state", 2);
//			this.ShowOut();
		}

		private Tweener tweener; 
		private int stateAnimation = 0;
	}

	public class ProgressiveAward01 : ProgressiveAward
	{

	}

	public class ProgressiveAward02 : ProgressiveAward
	{
		
	}

	public class ProgressiveAward03 : ProgressiveAward
	{
		
	}
}
