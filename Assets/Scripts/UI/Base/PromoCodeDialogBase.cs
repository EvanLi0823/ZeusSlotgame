using UnityEngine;
using UnityEngine.UI;
using Libs;
using System.Collections.Generic;

namespace Classic
{
	public class PromoCodeDialogBase : UIDialog
	{
		[HideInInspector]
		public InputField mInputFiled;
		[HideInInspector]
		public TMPro.TextMeshProUGUI mErrorTipText;
		[HideInInspector]
		public UnityEngine.UI.Button BtnSubmit;
		[HideInInspector]
		public UnityEngine.UI.Button BtnGetCodeLink;
		[HideInInspector]
		public CanvasGroup giftCanvasGroup;
		[HideInInspector]
		public Image Press_bg;
		Transform mTransfm;

		protected override void Awake()
		{
			base.Awake();
			try{
				mTransfm = transform;	
				this.mInputFiled = Util.FindObject<InputField>(mTransfm,"InputField/");
				this.mErrorTipText = Util.FindObject<TMPro.TextMeshProUGUI>(mTransfm,"error_kuang/text/");

				this.BtnSubmit = Util.FindObject<UnityEngine.UI.Button>(mTransfm,"BtnSubmit/");
				UGUIEventListener.Get(this.BtnSubmit.gameObject).onClick = this.OnButtonClickHandler;

				this.BtnGetCodeLink = Util.FindObject<UnityEngine.UI.Button>(mTransfm,"BtnGetCodeLink/");
				UGUIEventListener.Get(this.BtnGetCodeLink.gameObject).onClick = this.OnButtonClickHandler;

				giftCanvasGroup = Util.FindObject<CanvasGroup>(mTransfm,"error_kuang/");
				Press_bg = Util.FindObject<Image>(mTransfm,"BtnGetCodeLink/Press_bg/");

				if(this.mInputFiled == null)
						this.mInputFiled = mTransfm.GetComponentInChildren<InputField>();
			}
			catch
			{
				if (this.mInputFiled != null)
					BaseGameConsole.ActiveGameConsole().LogBaseEvent("PromoCode_InputField_OK");
				else
					BaseGameConsole.ActiveGameConsole().LogBaseEvent("PromoCode_InputField_NULL");
			}
		}
	}
}
