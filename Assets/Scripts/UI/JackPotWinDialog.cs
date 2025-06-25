using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Libs;
using DG.Tweening;
namespace Classic
{
public class JackPotWinDialog : JackPotWinDialogBase 
{
		protected Tween tween = null;
		public long initNum = 0;
        [Header("金币延时播放时间")]
        public float delayTime = 0f;
		protected System.Action m_OnDialogCloseCallBack;
        public const string INIT_DIALOG_DATA = "InitJackPotWinDialogData";
     	protected override void Awake()
    	{
       		base.Awake();
//			Messenger.Broadcast (SlotControllerConstants.AUTO_SPIN_SUSPEND);
    	}

   		public override void OnButtonClickHandler(GameObject go)
    	{
			AudioEntity.Instance.PlayClickEffect ();
    	    base.OnButtonClickHandler(go);
			Messenger.Broadcast (SlotControllerConstants.AUTO_SPIN_RESUME);
			Messenger.Broadcast (BaseSlotMachineController.RESUME_FREESPIN);
			Messenger.Broadcast<Transform, Libs.CoinsBezier.BezierType, System.Action>(GameConstants.CollectBonusWithType, go.transform, Libs.CoinsBezier.BezierType.DailyBonus, null);
			Libs.AudioEntity.Instance.PlayCoinCollectionEffect ();
			if (m_OnDialogCloseCallBack != null) {
				m_OnDialogCloseCallBack ();
			}
            if (AutoQuit)
            {
                OnDialogCloseCallBack = null;
            }
            DOTween.Kill(tween);
			this.Close ();
    	}

		public void Init(int count, long winValue,System.Action _OnDialogCloseCallBack)
		{
            Messenger.Broadcast<int,long>(JackPotWinDialog.INIT_DIALOG_DATA,count,winValue);
            if (this.TxtNumber!=null)
            {
                this.TxtNumber.text = count.ToString();
            }
			m_OnDialogCloseCallBack = _OnDialogCloseCallBack;
            if (AutoQuit)
            {
                OnDialogCloseCallBack = OnCloseCallBack;
            }
			DelayAction da = new DelayAction (delayTime, null, () => {
				Libs.AudioEntity.Instance.PlayRollUpEffect (1, false);
				tween = Utils.Utilities.AnimationTo (this.initNum, winValue, 2f, CaculateTxt, null, () => {
						Libs.AudioEntity.Instance.StopRollingUpEffect ();
						Libs.AudioEntity.Instance.PlayRollEndEffect ();
				}).SetUpdate(true);
				});
                
            da.Play();
			
		}
		private void CaculateTxt(long num){
			this.initNum = num;
            if (this.TxtWinAward!=null)
            {
				this.TxtWinAward.text = Utils.Utilities.ThousandSeparatorNumber(num);
            }

            if (this.TxtWinAward1 != null)
            {
                this.TxtWinAward1.text = Utils.Utilities.ThousandSeparatorNumber(num);
            }
            if (this.TxtWinAward2!=null)
            {
                this.TxtWinAward2.text = Utils.Utilities.ThousandSeparatorNumber(num);
            }
        }
        protected void OnCloseCallBack(){
            Messenger.Broadcast(SlotControllerConstants.AUTO_SPIN_RESUME);
            Messenger.Broadcast(BaseSlotMachineController.RESUME_FREESPIN);
            Messenger.Broadcast<Transform, Libs.CoinsBezier.BezierType, System.Action>(GameConstants.CollectBonusWithType, this.gameObject.transform, Libs.CoinsBezier.BezierType.DailyBonus, null);
            Libs.AudioEntity.Instance.PlayCoinCollectionEffect();
            if (m_OnDialogCloseCallBack != null)
            {
                m_OnDialogCloseCallBack();
            }
            DOTween.Kill(tween);
        }
    }
}
