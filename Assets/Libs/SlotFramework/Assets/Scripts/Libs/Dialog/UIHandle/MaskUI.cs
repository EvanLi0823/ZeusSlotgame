using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
namespace Libs
{
    public class MaskUI : MonoBehaviour
    {
        private float endAlpha = 0.7f;
        private float startAlpha = 0.2f;
        private Image maskImg;
        private float fadeInDuratiion = 0.3f;
        private float fadeOutDuration = 0.3f;
        private bool startedFadeIn = false;

        public void SetAlpha(float start,float end)
        {
            this.startAlpha = start;
            this.endAlpha = end;
        }

        public void SetFadeDuration(float fadeinDuration,float fadeoutDuration)
        {
            this.fadeInDuratiion = fadeinDuration;
            this.fadeOutDuration = fadeoutDuration;
        }
        
        public void FadeIn()
        {
            if (startedFadeIn) return;
            startedFadeIn = true;
            
            if (maskImg == null)
            {
                maskImg = gameObject.AddComponent<Image>();
                maskImg.color = new Color(0,0,0,startAlpha);
            }

            maskImg.DOFade(this.endAlpha, fadeInDuratiion).SetEase(Ease.Linear).SetUpdate(true);
        }

        public void FadeOut()
        {
            maskImg.DOFade(startAlpha, fadeOutDuration).SetEase(Ease.Linear).SetUpdate(true);
        }

        private void OnDestroy()
        {
            DOTween.Kill(maskImg);
        }
    }
}