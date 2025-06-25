using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Libs;
using DG.Tweening;

namespace Classic
{
    public class SliderProgress : MonoBehaviour
    {
        public RectTransform level;
        public GameObject guang;

        public float Total = 100f;
        public float LastValue = 0f;
        private float Time = 1f; //总共变化的时长
        private float CurrentShowValue = -1;

        private Tweener doTween;
        public Transform FillParticle;
        public Transform StartPos;
        public Transform EndPos;
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
        public void SetCurrentIncreaseFromZero(float newValue)
        {
            CurrentShowValue = -1;
            LastValue = newValue;

            DoTweenChange();
        }

        private void DoTweenChange()
        {
            if (doTween != null)
            {
                DOTween.Kill(doTween, true);
            }
            if (CurrentShowValue > LastValue)
            {
                LastValue += Total;
            }
            //tweener = DOTween.To(() => this.initNum, x => this.initNum = x, number, tweenerDuration).OnUpdate(CaculateTxt)
            //.OnComplete(() => { tweenerDelay = 0f; tweenerDuration = 3f; });
            doTween = DOTween.To(() => CurrentShowValue, x => this.CurrentShowValue = x, LastValue, Time)
                .OnUpdate(ValueChanged)
                .OnComplete(() =>
                {
                    CurrentShowValue = LastValue;
                    new DelayAction(1f, null, delegate () { if (guang != null) guang.SetActive(false); }).Play();
                });

        }

        private void ValueChanged()
        {
            if (Total == 0) Total = 100f;

            if (CurrentShowValue > Total)
            {
                CurrentShowValue -= Total;
            }

            float progress = CurrentShowValue / Total;

            if (guang != null) guang.SetActive(true);

            float width = (progress) * level.transform.parent.GetComponent<RectTransform>().rect.width;

            level.sizeDelta = new Vector2(width, level.rect.height);
            if (FillParticle != null && StartPos != null && EndPos != null)
            {
                FillParticle.position = Vector3.Lerp(StartPos.position, EndPos.position, progress);
            }
        }

        //IEnumerator UpdateProgress()
        //{
        //	while (RealCurrent != Current) 
        //    {
        //		float nextProgress = RealCurrent;
        //		if (RealCurrent < Current)
        //        {
        //			nextProgress = RealCurrent + Time.deltaTime * Speed;
        //			nextProgress = nextProgress > Current ? Current : nextProgress;
        //		} else if (RealCurrent > Current)
        //        {
        //			nextProgress = RealCurrent - Time.deltaTime * Speed;
        //			nextProgress = nextProgress > Current ? nextProgress : Current;
        //		}
        //		else
        //        {
        //			nextProgress = RealCurrent;
        //		}
        //		RealCurrent = nextProgress;
        //		ValueChanged ();
        //		yield return null;
        //	}
        //}
    }
}
