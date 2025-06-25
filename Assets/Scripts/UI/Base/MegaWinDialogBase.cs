using UnityEngine;
using UnityEngine.UI;
using Libs;

namespace Classic
{
    public class MegaWinDialogBase : UIDialog 
    {
        [HideInInspector]
        public UnityEngine.UI.Image Bg1;
    	[HideInInspector]
        public TMPro.TextMeshProUGUI WinCoinsText2;
    	[HideInInspector]
        public UnityEngine.UI.Image Bg2;
    	[HideInInspector]
        public TMPro.TextMeshProUGUI WinCoinsText1;
    	[HideInInspector]
        public UnityEngine.UI.Button TapToCloseButton;

        protected override void Awake()
        {
            base.Awake();
        
            this.Bg1 = Util.FindObject<UnityEngine.UI.Image>(transform, "Iphone/Bg1/");
            this.WinCoinsText2 = Util.FindObject<TMPro.TextMeshProUGUI>(transform, "Iphone/banner/WinCoinsText2/");
            this.Bg2 = Util.FindObject<UnityEngine.UI.Image>(transform, "Ipad/Bg2/");
            this.WinCoinsText1 = Util.FindObject<TMPro.TextMeshProUGUI>(transform, "Ipad/banner/WinCoinsText1/");
    
            this.TapToCloseButton = Util.FindObject<UnityEngine.UI.Button>(transform,"TapToCloseButton/");
            UGUIEventListener.Get(this.TapToCloseButton.gameObject).onClick = this.OnButtonClickHandler;
        }
    }
}
