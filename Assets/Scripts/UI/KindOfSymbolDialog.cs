using Libs;
using UnityEngine;
using UnityEngine.UI;

namespace Classic
{
    public class KindOfSymbolDialog : UIDialog
    {
        public Image SymbolImage;

        public Text SymbolCountText;

        public GameObject Symbol_6;

        public GameObject Symbol_5;

        public Animator anim;

        public float QuitAniDuration = 0.433f;
        protected override void Awake()
        {
            base.Awake();
            AudioManager.Instance.AsyncPlayMusicAudio("of_kind_dialog",loop:false);
        }

        public void KindInit(Sprite sprite, int count)
        {
            if(SymbolImage != null)
            {
                this.SymbolImage.sprite = sprite;
                this.SymbolImage.SetNativeSize();
            }
            //防止SetNativeSize()后，有的symbol尺寸过大，文字覆盖主symbol
            if (SymbolImage.rectTransform.sizeDelta.x >= 500)
            {
                SymbolImage.rectTransform.localScale = new Vector3(0.55f, 0.55f, 0.55f);
            }
            else if (SymbolImage.rectTransform.sizeDelta.x >= 300)
            {
                SymbolImage.rectTransform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
            }
            else
            {
                SymbolImage.rectTransform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
            }
            if (SymbolCountText != null) this.SymbolCountText.text = count.ToString();
            if(count == 5 && Symbol_5 != null) Symbol_5.SetActive(true);
            if(count == 6 && Symbol_6 != null) Symbol_6.SetActive(true);
            
        }

        public override void ShowOut()
        {
            if(anim!=null) anim.SetTrigger("out");
            Libs.DelayAction da = new DelayAction(QuitAniDuration,null, () =>
            {
                base.ShowOut();
            });
            da.Play();
            AudioEntity.Instance.StopMusicAudio("of_kind_dialog");
        }
    }
}