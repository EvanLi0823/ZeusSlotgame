using DG.Tweening;
using Libs;
using TMPro;
using UnityEngine;
using Utils;
namespace Classic
{
    public class TipsDialog:UIDialog
    {
        private TextMeshProUGUI _tipsTxt;
        private CanvasGroup _canvasGroup;
        private float _curY;
        protected override void Awake()
        {
            base.Awake();
            
            _tipsTxt = Utilities.RealFindObj<TextMeshProUGUI>(transform,"Anchor/Animation/Bg/TipsTxt");

            _canvasGroup = GetComponent<CanvasGroup>();
        }
        
        public void SetUIData(string text)
        {
            _canvasGroup.alpha = 1;
            _tipsTxt.text = text;
            var seq = DOTween.Sequence();
            seq.AppendInterval(1f);
            seq.Append(transform.DOLocalMoveY(transform.localPosition.y + 50, 0.2f));
            seq.Join(_canvasGroup.DOFade(0f, 0.2f)).OnComplete(Close);
        }
    }
}