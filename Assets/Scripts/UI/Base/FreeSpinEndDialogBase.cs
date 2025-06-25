using UnityEngine;
using UnityEngine.UI;
using Libs;

namespace Classic
{
    public class FreeSpinEndDialogBase : UIDialog 
    {
        [HideInInspector]
        public TMPro.TextMeshProUGUI BackGround;
        [HideInInspector]
        public UnityEngine.UI.Text TextNum;
        [HideInInspector]
        public UnityEngine.UI.Text TextNum1;
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
            if (this.TextNum==null)
            {
                this.TextNum2 = Util.FindObject<TMPro.TextMeshProUGUI>(transform,"TextNum/");
            }
            this.TextNum1 = Util.FindObject<UnityEngine.UI.Text>(transform, "animation/TextNum/");
            this.ImageBellow = Util.FindObject<TMPro.TextMeshProUGUI>(transform,"ImageBellow/");
            this.ImageTitle = Util.FindObject<TMPro.TextMeshProUGUI>(transform,"ImageTitle/");
        }
    }
}
