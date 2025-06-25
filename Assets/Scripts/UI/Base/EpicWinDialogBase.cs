using UnityEngine;
using UnityEngine.UI;
using Libs;

namespace Classic
{
    public class EpicWinDialogBase : UIDialog 
    {
        //old
		[HideInInspector]
        public TMPro.TextMeshProUGUI WinCoinsText2;
		[HideInInspector]
        public TMPro.TextMeshProUGUI WinCoinsText1;
		[HideInInspector]
        public UnityEngine.UI.Button TapToCloseButton;

       

        protected override void Awake()
        {
            base.Awake();

            this.WinCoinsText2 = Util.FindObject<TMPro.TextMeshProUGUI>(transform, "Iphone/banner/WinCoinsText2/");
            this.WinCoinsText1 = Util.FindObject<TMPro.TextMeshProUGUI>(transform, "Ipad/banner/WinCoinsText1/");

            this.TapToCloseButton = Util.FindObject<UnityEngine.UI.Button>(transform,"TapToCloseButton/");
            UGUIEventListener.Get(this.TapToCloseButton.gameObject).onClick = this.OnButtonClickHandler;
        }
    }
}
