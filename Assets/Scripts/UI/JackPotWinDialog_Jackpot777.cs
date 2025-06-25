using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Libs;
using DG.Tweening;

namespace Classic
{
public class JackPotWinDialog_Jackpot777 : JackPotWinDialog_Jackpot777Base 
{
        private System.Action closeDialogCallback;
        private Tween tween = null;
        public long initNum = 1;
     	protected override void Awake()
    	{

       		base.Awake();
    	}

        public override void OnButtonClickHandler(GameObject go)
        {
            // TODO:增加飞金币动画，按下Click之后；分支合并之后再加。@yiyang
            AudioEntity.Instance.PlayEffect("Jackpot-off", false); 
            AudioEntity.Instance.PlayClickEffect();
            base.OnButtonClickHandler(go);
            Messenger.Broadcast<Transform, Libs.CoinsBezier.BezierType, System.Action>(GameConstants.CollectBonusWithType, go.transform, Libs.CoinsBezier.BezierType.DailyBonus, closeDialogCallback);
            Messenger.Broadcast(SlotControllerConstants.AUTO_SPIN_RESUME);
            Messenger.Broadcast(BaseSlotMachineController.RESUME_FREESPIN);
            if (closeDialogCallback != null)
            {
                new DelayAction(0.5f, null, ()=>{closeDialogCallback();}).Play();

            }
            this.Close();
        }

        private void SetImage(bool bo1,bool bo2,bool bo3,bool bo4)
        {
            MINI.gameObject.SetActive(bo1);
            MINOR.gameObject.SetActive(bo2);
            MAJOR.gameObject.SetActive(bo3);
            MEGA.gameObject.SetActive(bo4);
            MINI_TEXTTURE.gameObject.SetActive(bo1);
            MINOR_TEXTTURE.gameObject.SetActive(bo2);
            MAJOR_TEXTTURE.gameObject.SetActive(bo3);
            MEGA_TEXTTURE.gameObject.SetActive(bo4); 
        }
        private void ChangeImage(int index)
        {
            if (index == 0)
            {
                SetImage(true,false,false,false);
            }
            if (index == 1)
            {
                SetImage(false,true,false,false);
            }
            if (index == 2)
            {
                SetImage(false,false,true,false);
            }
            if (index == 3)
            {
                SetImage(false,false,false,true);
            }
        }

        public void Init(int count, long winValue, System.Action callback)
        {
            ChangeImage(count);
            Libs.AudioEntity.Instance.PlayRollUpEffect(1, false);
			tween = Utils.Utilities.AnimationTo (this.initNum,winValue,2f,CaculateTxt,null,() => {

				//                      this.Close();
				Libs.AudioEntity.Instance.StopRollingUpEffect();
				Libs.AudioEntity.Instance.PlayRollEndEffect();

			});
            closeDialogCallback = callback;
        }
		private void CaculateTxt(long num)
        {
			this.initNum = num;
            this.TxtWinAward.text = Utils.Utilities.ThousandSeparatorNumber(num);
        }


	}
}
