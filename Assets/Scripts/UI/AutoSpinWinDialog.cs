using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Libs;
using TMPro;

namespace Classic
{
	public class AutoSpinWinDialog : AutoSpinWinDialogBase
	{
        public Button StartBtn;
        public string BtnSound;
        public string DialogSound;
		public UIText freeCount;
        public TMPro.TextMeshProUGUI ExtraAwardTxt;
		public static int AwardExtraAdd = 0;
        public bool isOpenModel = true;
		protected override void Awake ()
		{
			base.Awake ();

			ExtraAwardTxt = Util.FindObject<TextMeshProUGUI>(transform,"ImageBellow/TxtExtraAward/");
//			if (T != null) {
//				ExtraAwardTxt = T.GetComponent<TextMeshProUGUI> ();
//			}

            if (this.StartBtn == null)
            {
                this.bResponseBackButton = false;
                this.AutoQuit = true;
                this.DisplayTime = 4;
            }else
            {
                UGUIEventListener.Get(this.StartBtn.gameObject).onClick = this.OnButtonClickHandler;
            }

        }

		public override void OnButtonClickHandler (GameObject go)
		{
			base.OnButtonClickHandler (go);
            if (BtnSound != null) AudioEntity.Instance.PlayEffect(BtnSound);
            this.Close();
        }

        public void SetNum (long coins, bool isShow = true)
		{
            if (DialogSound != null) AudioEntity.Instance.PlayEffect(DialogSound);

            if (!isOpenModel) this.isModel = false;

            if (ImageBellow != null) {
				this.ImageBellow.gameObject.SetActive (isShow);
			}
			if (isShow) {
				//freespin
                if (this.TextNum!=null) this.TextNum.text = Utils.Utilities.ThousandSeparatorNumber(coins, false);
                if (this.TextNum2 != null) this.TextNum2.text = Utils.Utilities.ThousandSeparatorNumber(coins, false);
				if (freeCount!=null) freeCount.SetText (coins.ToString());
                //				this.TextNum.text = coins.ToString ();
				if (ExtraAwardTxt != null) {
					ExtraAwardTxt.text = Utils.Utilities.ThousandSeparatorNumber(AwardExtraAdd);
				}
				AwardExtraAdd = 0;
			} else {
				//awardcoins
				if(this.TextNum!=null)
					this.TextNum.text = Utils.Utilities.ThousandSeparatorNumber(coins);		
                if (this.TextNum2 != null) 
                    this.TextNum2.text = Utils.Utilities.ThousandSeparatorNumber(coins);
				
			}
		}
	}
}
