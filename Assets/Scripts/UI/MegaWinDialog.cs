using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Libs;

using DG.Tweening;

namespace Classic
{
    public class MegaWinDialog : MegaWinDialogBase 
    {
        public Text NewWinCoinsText;

        public float AniChangCoinTime = 0;

        public float AniWaitTime = 0;

        public float ChangCoinTime = 8.5f;

        public float WaitTime = 2f;

        protected override void Awake()
    	{
       		base.Awake();
    		isCloseButtonClicked = false;
    	}

    	public override void OnButtonClickHandler(GameObject go)
    	{
    		
    		base.OnButtonClickHandler(go);
            if (AniWaitTime > 0) WaitTime = AniWaitTime;
            if (isAnimationEnd) {
    			
    			CloseDialog ();
    		} else {
    			if (tween != null) {
    				tween.Kill (true);
    			}

    			initNum = LastNumber;
    			CaculateTxt (initNum);

    			new DelayAction(WaitTime, null,delegate() {
    				CloseDialog();
    			}).Play();
    			isAnimationEnd = true;
    		}
    	}

    	private bool isCloseButtonClicked = false;
    	private bool isAnimationEnd = false;
    	private long initNum = 0;
    	private bool InterruptRollingUpAudio = false;
    	private Tween tween = null;
    	private long LastNumber = 0;
    	public void SetCoinsNumber(long number){
    		LastNumber = number;
    		TapToCloseButton.interactable = false;
            initNum = 0;
            if (AniChangCoinTime > 0) ChangCoinTime = AniChangCoinTime;
            if (AniWaitTime > 0) WaitTime = AniWaitTime;
            if (this.NewWinCoinsText != null)
            {
                this.NewWinCoinsText.text = "";
            }
            else
            {
                WinCoinsText1.text = "";
                WinCoinsText2.text = "";
            }


            InterruptRollingUpAudio = false;

            tween = Utils.Utilities.AnimationTo (this.initNum,number, ChangCoinTime, CaculateTxt,null,()=>{ 
    			isAnimationEnd = true;
    			//如果是RedWhietBlue 则缩短BigWin时间
    			if(BaseSlotMachineController.Instance.slotMachineConfig.Name().Equals("RedWhiteBlue"))
    			{
                    WaitTime = 0.8f;
    			}

    			new DelayAction(WaitTime, null,delegate() {
    				CloseDialog();
    			}).Play();
			}).SetUpdate(true);
        }

    	private void CaculateTxt(long num)
        {
    		this.initNum = num;
            if (this.NewWinCoinsText != null)
            {
                this.NewWinCoinsText.text = Utils.Utilities.ThousandSeparatorNumber(num);
                return;
            }
            WinCoinsText1.text = Utils.Utilities.ThousandSeparatorNumber(num);//string.Format("{0:0,0}",initNum); //initNum.ToString(); 
            WinCoinsText2.text = Utils.Utilities.ThousandSeparatorNumber(num); //string.Format("{0:0,0}",initNum); //initNum.ToString(); 
        }  

    	private void CloseDialog()
    	{
    		if (isCloseButtonClicked) {
    			return;
    		} else {
    			isCloseButtonClicked = true;
    		}

    		if (Bg1 != null && Bg2 != null) {
    			Bg1.gameObject.SetActive (false);
    			Bg2.gameObject.SetActive (false);		
    		}
    		CommandTriggerManager.Instance.CheckMomentConditions (GameConstants.CLOSE_BIG_WIN_DIALOG);
    		Messenger.Broadcast(GameConstants.CLOSE_BIG_WIN_DIALOG);
    		this.Close ();
    		InterruptRollingUpAudio = true;

    	}

    	public override void Refresh ()
    	{
    		base.Refresh ();
    		long v = (long)this.m_Data;
    		this.SetCoinsNumber (v);
    	}
	}
}
