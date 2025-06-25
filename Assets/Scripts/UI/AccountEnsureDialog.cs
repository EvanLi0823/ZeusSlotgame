using System;
using Libs;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Classic
{
    public class AccountEnsureDialog:UIDialog
    {
        public TextMeshProUGUI cashTmp;
        public Image image;
        private int platformIndex;
        public TextMeshProUGUI emailTMP; 
        public TextMeshProUGUI dataTmp;
        public Button ensureBtn;
        private int money = 0;
        protected override void Awake()
        {
            base.Awake();
            ensureBtn.onClick.AddListener(EnsureBtnClick);
            // UGUIEventListener.Get(closeBtn.gameObject).onClick = CloseBtnClick;
            // UGUIEventListener.Get(ensureBtn.gameObject).onClick = EnsureBtnClick;
        }
        
        public void SetUIData(int spriteIndex,string account,int cash)
        {
            money = cash;
            cashTmp.text = OnLineEarningMgr.Instance.GetMoneyStr(cash, needIcon: false);
            Sprite sp = Resources.Load<Sprite>("Platform/"+spriteIndex);
            if (sp != null)
            {
                image.sprite = sp;
            }

            emailTMP.text = account;
            DateTime now = DateTime.Now;
            dataTmp.text = now.ToString("MM/dd/yyyy");
        }
        private void EnsureBtnClick()
        {
            Debug.Log("[AccountEnsureDialog][EnsureBtnClick]");
            this.Close();
            WithDrawManager.Instance.ReduceCash(money);
        }
    }
}