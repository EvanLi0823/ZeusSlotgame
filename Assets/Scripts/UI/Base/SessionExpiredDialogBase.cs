using UnityEngine;
using UnityEngine.UI;
using Libs;

namespace Classic
{
	public class SessionExpiredDialogBase : UIDialog 
	{
		[HideInInspector]
	    public UnityEngine.UI.Button btnReload;
		[HideInInspector]
	    public TMPro.TextMeshProUGUI TxtAward;

	    protected override void Awake()
	    {
	        base.Awake();
			this.btnReload = Util.FindObject<UnityEngine.UI.Button>(transform,"btnReload/");
	        UGUIEventListener.Get(this.btnReload.gameObject).onClick = this.OnButtonClickHandler;
	    }
	}
}
