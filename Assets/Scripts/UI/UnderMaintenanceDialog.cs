using UnityEngine;
using UnityEngine.UI;
using Libs;

namespace Classic
{
    public class UnderMaintenanceDialog : UIDialog
    {
        private TMPro.TextMeshProUGUI TxtTitle;
        private TMPro.TextMeshProUGUI TxtDesc;
        private Button BtnOk;
        private TMPro.TextMeshProUGUI Txt;
        private Button BtnClose;

        protected override void Awake()
        {
            base.Awake();
            this.TxtTitle = Util.FindObject<TMPro.TextMeshProUGUI>(transform, "Anchor/TxtTitle/");
            this.TxtDesc = Util.FindObject<TMPro.TextMeshProUGUI>(transform, "Anchor/TxtDesc/");
            this.BtnOk = Util.FindObject<Button>(transform, "Anchor/BtnOk/");
            UGUIEventListener.Get(this.BtnOk.gameObject).onClick = this.OnButtonClickHandler;
            this.Txt = Util.FindObject<TMPro.TextMeshProUGUI>(transform, "Anchor/BtnOk/Txt/");
            this.BtnClose = Util.FindObject<Button>(transform, "Anchor/BtnClose/");
            UGUIEventListener.Get(this.BtnClose.gameObject).onClick = this.OnButtonClickHandler;
        }

        public override void OnButtonClickHandler(GameObject go)
        {
            if (go == this.BtnOk.gameObject) Close();
            if (go == BtnClose.gameObject) Close();
            base.OnButtonClickHandler(go);
        }
    }
}