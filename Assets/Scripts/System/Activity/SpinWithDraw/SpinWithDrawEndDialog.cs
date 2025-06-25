using Libs;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

namespace Activity
{
    public class SpinWithDrawEndDialog:UIDialog
    {
        public TextMeshProUGUI contentTmp;
        public AutoFitCustomFontText awardNumTmp;
        private int cashNum;
        public LocalizedString Tmp1_Page1_Str;
        private string key = "Withdraw Instantly";
        
        public void SetUIData(int num)
        {
            cashNum = num;
            Refresh();
        }

        public override void Refresh()
        {
            base.Refresh();
            double exchageCash = OnLineEarningMgr.Instance.ConvertMoneyToDouble(cashNum);
            awardNumTmp.text = exchageCash.ToString();
            Tmp1_Page1_Str.Arguments = new object[] {exchageCash};
            contentTmp.text = Tmp1_Page1_Str.GetLocalizedString();
        }
    }
}