using UnityEngine;
using UnityEngine.UI;
using Libs;
using System.Collections.Generic;
namespace Classic
{
public class RewardSpinDialogBase : UIDialog,ICommandEvent 
{
    public UnityEngine.UI.Text RewardSpinTimes;
    public TMPro.TextMeshProUGUI FixedBet;
    public TMPro.TextMeshProUGUI RewardUp;
    public TMPro.TextMeshProUGUI RewardDown;
    public UnityEngine.UI.Image MachinePic;
    public UnityEngine.UI.Button BtnDecline;
    public TMPro.TextMeshProUGUI DeclineBtnTxt;
    public UnityEngine.UI.Button BtnAccept;
    public TMPro.TextMeshProUGUI AcceptBtnTxt;
    public GameObject slotPicLoading;
    protected override void Awake()
    {
        base.Awake();
        //this.RewardSpinTimes = Util.FindObject<UnityEngine.UI.Text>(transform,"RewardSpinTimes/");
        //this.FixedBet = Util.FindObject<TMPro.TextMeshProUGUI>(transform,"FixedBet/");
        //this.RewardUp = Util.FindObject<TMPro.TextMeshProUGUI>(transform,"RewardUp/");
        //this.RewardDown = Util.FindObject<TMPro.TextMeshProUGUI>(transform,"RewardDown/");
        //this.MachinePic = Util.FindObject<UnityEngine.UI.Image>(transform,"SlotMask/MachinePic/");
		//this.DeclineBtnTxt = Util.FindObject<TMPro.TextMeshProUGUI>(transform,"BtnDecline/DeclineBtnTxt/");
		//this.AcceptBtnTxt = Util.FindObject<TMPro.TextMeshProUGUI>(transform,"BtnAccept/AcceptBtnTxt/");
        this.BtnDecline = Util.FindObject<UnityEngine.UI.Button>(transform,"BtnDecline/");
        UGUIEventListener.Get(this.BtnDecline.gameObject).onClick = this.OnButtonClickHandler;
        
        this.BtnAccept = Util.FindObject<UnityEngine.UI.Button>(transform,"BtnAccept/");
        UGUIEventListener.Get(this.BtnAccept.gameObject).onClick = this.OnButtonClickHandler;
        
    }

		public virtual void OnEventComplete(bool isAccept = true){
			
		}
		public virtual void DisableAcceptBtn(){
			BtnAccept.enabled = false;
			BtnAccept.interactable = false;
			Libs.AudioEntity.Instance.PlayClickEffect();
		}
		public virtual void DisableDeclineBtn(){
			BtnDecline.enabled = false;
			BtnDecline.interactable = false;
			Libs.AudioEntity.Instance.PlayClickEffect();
		}
		public virtual void SendESLog(Dictionary<string, object> dict){
			
		}
		public virtual void PlayCollectCoinsAnimation(){
			
		}
}}
