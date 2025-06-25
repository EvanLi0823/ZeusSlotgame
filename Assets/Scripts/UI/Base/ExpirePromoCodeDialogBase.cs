using UnityEngine;
using UnityEngine.UI;
using Libs;

namespace Classic
{
    public class ExpirePromoCodeDialogBase : UIDialog
	{
		[HideInInspector]
	    public UnityEngine.UI.Button btnOK;

	    protected override void Awake()
	    {
	        base.Awake();
			this.btnOK = Util.FindObject<UnityEngine.UI.Button>(transform,"BtnOK/");
	        UGUIEventListener.Get(this.btnOK.gameObject).onClick = this.OnButtonClickHandler;
	    }
	}
}
