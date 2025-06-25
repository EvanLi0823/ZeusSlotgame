using UnityEngine;
using UnityEngine.UI;
using Libs;

namespace Classic
{
	public class ProgressiveAward01Base : UIDialog 
	{
	    public UnityEngine.UI.Button BtnBg, skipButton;
	    public UnityEngine.GameObject Animation;
		public Text WinText;
//	    public TMPro.TextMeshProUGUI TxtSkip;

	    protected override void Awake()
	    {
			base.Awake();
//			Debug.LogError("ProgressiveAward01Base Awake beginning Pause");
			this.skipButton = Util.FindObject<UnityEngine.UI.Button>(transform,"Animation/fruit_bar_BUTTON/");
			UGUIEventListener.Get(this.skipButton.gameObject).onClick = this.OnButtonClickHandler;
			this.BtnBg = Util.FindObject<UnityEngine.UI.Button>(transform,"Animation/BtnBg/");
			UGUIEventListener.Get(this.BtnBg.gameObject).onClick = this.OnButtonClickHandler;
	        this.Animation = Util.FindObject<UnityEngine.GameObject>(transform,"Animation/");
			this.WinText = Util.FindObject<Text>(transform,"Animation/fruit_bar_number_back/TextNum/");
//	        this.TxtSkip = Util.FindObject<TMPro.TextMeshProUGUI>(transform,"TxtSkip/");
//			Debug.LogError("ProgressiveAward01Base Awake end Pause");
	    }
	}
}
