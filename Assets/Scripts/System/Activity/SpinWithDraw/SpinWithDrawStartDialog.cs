using System.Collections;
using System.Collections.Generic;
using Libs;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.UIElements;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;

namespace Activity
{
    public class SpinWithDrawStartDialog : UIDialog
    {
        private TextMeshProUGUI contentTMP;
        private Button StartBtn;
        protected override void Awake()
        {
            base.Awake();
            contentTMP = Util.FindObject<TextMeshProUGUI>(this.transform, "Anchor/contentTMP");
            StartBtn = Util.FindObject<Button>(this.transform, "Anchor/StartBtn");
            if (StartBtn!=null)
            {
                UGUIEventListener.Get(this.StartBtn.gameObject).onClick = this.OnButtonClickHandler;
            }
        }

        public void SetUIData(int spinCount,int cashNum)
        {
            // string arg1 = string.Format("<font=\"Baloo2-Bold_Origin Atlas Material\">{0}</font>",spinCount);
            int arg1 = spinCount;
            string arg2 = string.Format("<color=#2CF0FF>{0}</color>",OnLineEarningMgr.Instance.GetMoneyStr(cashNum,needIcon:false));
            string contentString = LocalizationSettings.StringDatabase.GetLocalizedString(LocalizationManager.Instance.tableName, "Rotateandwithdraw", new object[] {arg1 ,arg2});
            contentTMP.text = contentString;
        }
        
        public override void OnButtonClickHandler(GameObject go)
        {
            base.OnButtonClickHandler(go);
            if (go == StartBtn.gameObject)
            {
                this.Close();
            }
        }
    }
}
