﻿using System;
using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;
using Libs;
using UnityEngine.SceneManagement;

namespace Classic
{
	public class CoinsPanel : MonoBehaviour
	{        
 		public Transform coinTarget;
		public TextMeshProUGUI coinText;

        private float tweenerDelay = 0f;
        private float tweenerDuration = 3f;
		private long initNum = 0;
        private Libs.DelayAction tweenAction;
		private Tweener tweener;
        private long finalNum;

        public long GetCurrentBalance()
        {
	        return finalNum;
        }
		public void SetCoinsNumber(long number){
            if (tweenAction != null && tweenAction.IsPlaying) {
                tweenAction.Stop(true);
            }
			if (tweener != null) { 
				tweener.Kill (true);
            }

            finalNum = number;

            if (number > initNum)
			{
                tweenAction = new Libs.DelayAction(tweenerDelay, null, () => {
                    tweener = DOTween.To(() => this.initNum, x => this.initNum = x, number, tweenerDuration).OnUpdate(CaculateTxt)
                                     .OnComplete(()=>{tweenerDelay = 0f; tweenerDuration = 3f; CompleteShow(); }).SetUpdate(true);
                });
                tweenAction.Play();
			}
			else {
                CompleteShow();
            }
		}

        private void CompleteShow()
        {
            this.initNum = finalNum;
            this.CaculateTxt();
        }

        private void CaculateTxt()
		{
			if (coinText == null) 
			{
				if (tweener != null) 
				{
					tweener.Kill ();
				}
				return;
			}
			if(initNum ==0)
			{
				coinText.text = "0";
			}else 
			{
				coinText.text = Utils.Utilities.ThousandSeparatorNumber (initNum); //string.Format("{0:0,0}",initNum);
			}
		}

        public void SetTweenerParam(float delay = 0f, float duration = 3f) 
		{
            tweenerDelay = delay;
            tweenerDuration = duration;
        }

		public void OnClickShop()
		{
		}

		void Start()
		{
			UGUIEventListener.Get(this.gameObject).onClick = AllClick;
		}
		

		void AllClick(GameObject o)
		{
		}

        void Awake()
        {
			Messenger.AddListener<float, float>(GameConstants.SET_COINSPANEL_TWEENER_PARAM, SetTweenerParam);
        }

        void OnDestroy()
        {
			Messenger.RemoveListener<float, float>(GameConstants.SET_COINSPANEL_TWEENER_PARAM, SetTweenerParam);
        }
    }
}
