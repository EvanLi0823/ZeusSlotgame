using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Libs;

namespace Classic
{
    public class ExpirePromoCodeDialog : ExpirePromoCodeDialogBase
	{
		protected override void Awake ()
		{
			base.Awake();
			Messenger.Broadcast(SlotMiddlePanel.StopAutoRun);
		}

   		public override void OnButtonClickHandler(GameObject go)
    	{
    	    base.OnButtonClickHandler(go);
			if (go == this.btnOK.gameObject) {
				Libs.AudioEntity.Instance.PlayClickEffect();
			}
			this.Close();
    	}
	}
}
