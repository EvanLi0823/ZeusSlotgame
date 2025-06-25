using UnityEngine;
using UnityEngine.UI;
using Libs;

namespace Classic
{
    public class AutoSpinWinDialogBase : UIDialog 
    {
    	[HideInInspector]
        public TMPro.TextMeshProUGUI BackGround;
    	[HideInInspector]
        public UnityEngine.UI.Text TextNum;
        [HideInInspector]
        public TMPro.TextMeshProUGUI TextNum2;
    	[HideInInspector]
        public TMPro.TextMeshProUGUI ImageBellow;
    	[HideInInspector]
        public TMPro.TextMeshProUGUI ImageTitle;

        protected override void Awake()
        {
            base.Awake();
            this.BackGround = Util.FindObject<TMPro.TextMeshProUGUI>(transform,"BackGround/");
            this.TextNum = Util.FindObject<UnityEngine.UI.Text>(transform,"TextNum/");
            this.TextNum2 = Util.FindObject<TMPro.TextMeshProUGUI>(transform, "TextNum/");
            this.ImageBellow = Util.FindObject<TMPro.TextMeshProUGUI>(transform,"ImageBellow/");
            this.ImageTitle = Util.FindObject<TMPro.TextMeshProUGUI>(transform,"ImageTitle/");
        }
    }
}
