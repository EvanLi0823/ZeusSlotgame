using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Libs;
namespace Classic
{
	public class PrivacyPolicyDialog : UIDialog
	{
		[HideInInspector]
		public UnityEngine.UI.Button ContinueBtn;
		[HideInInspector]
		public UnityEngine.UI.Button CloseDialogBtn;

		public override void OnButtonClickHandler (GameObject go)
		{
			this.Close();
			Libs.AudioEntity.Instance.PlayClickEffect();
		}

		public void OnInitDialog()
		{
			this.ContinueBtn = Util.FindObject<UnityEngine.UI.Button>(transform,"ContinueBtn/");
			UGUIEventListener.Get(this.ContinueBtn.gameObject).onClick = this.OnButtonClickHandler;

			this.CloseDialogBtn = Util.FindObject<UnityEngine.UI.Button>(transform,"CloseDialogBtn/");
			UGUIEventListener.Get(this.CloseDialogBtn.gameObject).onClick = this.OnButtonClickHandler;
		}
	}
}
