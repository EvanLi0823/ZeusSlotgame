using System;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Libs;
using DG.Tweening;

namespace Classic
{
	public class UIProgress : MonoBehaviour {
        //RealCurrent到Current的转变
        public float Total = 100f;
		public float LastValue = 0f;
        private float Time = 2f; //总共变化的时长
        private float CurrentShowValue = -1;

        public Image FrontImage = null;
        public Image HeadImage = null;
        public Image highlightImage = null;
		public Animator hightLightAnimator;
		
        private float Speed = 3;

        private Tweener doTween;

		public void SetTotal(float newValue)
		{
			Total = newValue;
		}

		public void SetCurrent(float newValue)
		{
			LastValue = newValue;

            DoTweenChange();
            //StopAllCoroutines ();
            //StartCoroutine (UpdateProgress());
        }

		public void SetCurrentIncreaseFromZero(float newValue){
			CurrentShowValue = -1;
			LastValue = newValue;

            DoTweenChange();

            //         StopAllCoroutines ();
            //StartCoroutine (UpdateProgress());
        }

        private void DoTweenChange()
        {
            if (FrontImage == null) 
			{
				Debug.LogError("UIProgress FrontImage not set");
			}

            if (doTween != null)
            {
                DOTween.Kill(doTween, true);
            }
            if(CurrentShowValue > LastValue)
            {
                LastValue += Total;
            }
            //tweener = DOTween.To(() => this.initNum, x => this.initNum = x, number, tweenerDuration).OnUpdate(CaculateTxt)
                                    //.OnComplete(() => { tweenerDelay = 0f; tweenerDuration = 3f; });
                                    
	       
		        doTween = DOTween.To(() => CurrentShowValue, x => this.CurrentShowValue = x, LastValue,Time)
			        .OnUpdate(ValueChanged)
			        .OnComplete(() =>
			        {
				        CurrentShowValue = LastValue;
			        });
		       
            

            if (hightLightAnimator != null)
            {
                if(hightLightAnimator.GetInteger("state") == 0)
                {
                    hightLightAnimator.SetInteger("state", 1);
                    DelayAction delay = new DelayAction(1.5f, null, () => {
                        hightLightAnimator.SetInteger("state", 0);
                    });
                    delay.Play();
                }
            }
        }
		/// <summary>
		/// 设置dotween不收Time.scale的控制
		/// </summary>
        public void SetDoTweenUpdate()
        {
	        if (doTween != null)
				doTween.SetUpdate(true);
        }
        

        void ValueChanged()
		{
			if (FrontImage == null) 
			{
				return;
			}
		
			if (Total == 0)
				Total = 100f;

            if(CurrentShowValue > Total)
            {
                CurrentShowValue -= Total;
            }

			float progress = CurrentShowValue / Total;
			
			FrontImage.type = Image.Type.Filled;
			FrontImage.fillMethod = Image.FillMethod.Horizontal;
			FrontImage.fillAmount = progress;

            if (HeadImage != null) {
                RectTransform r = HeadImage.rectTransform;
                Vector2 max = r.anchorMax;
                Vector2 min = r.anchorMin;
                r.anchorMax = new Vector2 (progress,max.y);
                r.anchorMin = new Vector2 (progress-(max.x - min.x), min.y);
            }

            if (highlightImage != null) {
                highlightImage.type = Image.Type.Filled;
                highlightImage.fillMethod = Image.FillMethod.Horizontal;
                highlightImage.fillAmount = progress;
            }

			
		}

		//IEnumerator UpdateProgress()
		//{
		//	while (CurrentShowValue != LastValue) {
		//		float nextProgress = CurrentShowValue;
		//		if (CurrentShowValue < LastValue) {
		//			nextProgress = CurrentShowValue + Time.deltaTime * Speed;
		//			nextProgress = nextProgress > LastValue ? LastValue : nextProgress;
		//		} else if (CurrentShowValue > LastValue) {
		//			nextProgress = CurrentShowValue - Time.deltaTime * Speed;
		//			nextProgress = nextProgress > LastValue ? nextProgress : LastValue;
		//		}
		//		else {
		//			nextProgress = CurrentShowValue;
		//		}
		//		CurrentShowValue = nextProgress;
		//		ValueChanged ();
		//		yield return GameConstants.FrameTime;
		//	} 
		//}
	}
}



