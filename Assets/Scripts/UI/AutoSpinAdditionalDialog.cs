using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Libs;

namespace Classic
{
public class AutoSpinAdditionalDialog : AutoSpinAdditionalDialogBase 
{
		private readonly string AUDIO_APPEAR = "Add-Freespin";

        public float WaitTime = 0;
		[Header("增加次数文本")]
		public UIText freeCount;
        public bool IsOpenModel = true;

     	protected override void Awake()
    	{
       		base.Awake();
			this.bResponseBackButton = false;
            AudioEntity.Instance.StopFreeSpinBackgroundMusic();
            AudioEntity.Instance.PlayEffectAudio (AUDIO_APPEAR);
    	}


   		public override void OnButtonClickHandler(GameObject go)
    	{
    	    base.OnButtonClickHandler(go);
    	}

		public void SetNumAndAdditional(int coins,bool isShowAdditional = true)
		{
            if (WaitTime > 0) this.DisplayTime = WaitTime;
            if (!IsOpenModel) this.isModel = false;

            if(this.TextNum != null)
            {
                this.TextNum.text = Utils.Utilities.ThousandSeparatorNumber(coins, false); //coins.ToString ();
            }

            if(this.ImageBellow != null) 
            {
                this.ImageBellow.gameObject.SetActive(isShowAdditional);
            }

			if (freeCount!=null) {
				this.freeCount.SetText (coins.ToString());
			}
        }

        public override void ShowOut()
        {
            base.ShowOut();
            AudioEntity.Instance.PlayFreeSpinBackgroundMusic();
        }
    }
}
