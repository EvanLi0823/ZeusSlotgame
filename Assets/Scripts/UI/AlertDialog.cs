using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Libs;
namespace Classic
{
public class AlertDialog : AlertDialogBase 
{
		public static void Show (string title,string content,int type = 1,System.Action okEvent=null,System.Action cancelEvent=null,System.Action closeEvent =null,bool isModel = false,string btntxt="OK",bool popupInLoading = false,string reason = null, int eid = -1)
		{
			OpenType curType = OpenType.AtOnce;
			int curId = 0;
			if (eid != -1)
			{
				curType = OpenType.InFrontOfCurrent;
				curId = eid;
			}
			OpenConfigParam<AlertDialog> param = new OpenConfigParam<AlertDialog>(curId,curType,Constants.ALERT,new DefaultUIPopupStrategy(),
				(dialog)=>{
					dialog.SetContent (title, content, type,okEvent,cancelEvent,btntxt);
					if (SkySreenUtils.CurrentOrientation == ScreenOrientation.Portrait)
					{
						dialog.isPortrait = true;
					}
				},closeEvent,loadingType: LoadingLock.None);

			UIManager.Instance.OpenSystemDialog(param);
			if (!string.IsNullOrEmpty(reason))
			{
				Dictionary<string, object> dic = new Dictionary<string, object>();
				dic.Add("reason",reason);
				BaseGameConsole.ActiveGameConsole().LogBaseEvent(GameConstants.NetworkErrorAlertInLogin_Show,dic);
			}
		}

		public static void ShowOnNetworkError(Action  ok,Action cancel,Action close,bool popupInLoading = false,string reason = null)
		{
			Show("OOPS...", "The internet connection was lost,\nPlease try again.",1,ok,cancel,close,btntxt:"OK",reason:reason);
		}
		
		public static void ShowOnMachineDataError(Action  ok,Action cancel,Action close,string reason = null)
		{
			Show("OOPS !", "Seems your game has unexpectedly stopped. \nNo worries, click PLAY to try again.",
				1, ok, cancel,close,false,"PLAY",reason:reason);
		}

//		public delegate void OKClickedEvent ();
//		public event OKClickedEvent OnOKClicked;
//
//		public delegate void CancelClickedEvent ();
//		public event CancelClickedEvent OnCancelClicked;
		public System.Action OnOKClicked;
		public System.Action OnCancelClicked;
     	protected override void Awake()
    	{
       		 base.Awake();
    	}

   		 public override void OnButtonClickHandler(GameObject go)
    	{
			//Libs.SoundEntity.Instance.Click ();
			Libs.AudioEntity.Instance.PlayClickEffect();
    	    base.OnButtonClickHandler(go);
			if (go == this.BtnOk.gameObject) {
				if (OnOKClicked != null) {
					OnOKClicked ();

				}
				this.Close();
			}
//			else if (go == this.BtnCancel.gameObject){
//				if (OnCancelClicked !=null)
//				{
//					OnCancelClicked();
//				}
//				this.Close();
//			}
    	}

		public void SetContent(string title,string content,int type = 1,System.Action okEvent=null,System.Action cancelEvent=null,string btntxt="OK")
		{
			this.TxtTitle.text = title;
			this.TxtContent.text = content;
			if (type == 3)
			{
				this.BtnOk.gameObject.SetActive(false);
			}
			this.TxtBtn.text = btntxt;
//			if (type == 1) {
//				this.BtnCancel.gameObject.SetActive (false);
//				Vector3 v = (this.BtnOk.transform as RectTransform).localPosition;
//				v.x = 0;
//				(this.BtnOk.transform as RectTransform).localPosition = v;
//			} else if (type == 2) {
//				this.BtnOk.gameObject.SetActive(false);
//				this.BtnCancel.gameObject.SetActive (false);
//			}
			this.OnOKClicked = okEvent;
			this.OnCancelClicked = cancelEvent;
//			this.OnCloseClicked += closeEvent;
		}

	}
}
