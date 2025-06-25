using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;
namespace Classic
{
	public class ElementWithChangeBgRender : AnimatiorElement {

		protected override void SymbolChangeHandler(){
			//偶数为红，奇数为蓝
			if (IndexOfList % 2 == 0) { 
				SetStaticSprite(this.ReelContronller.elementConfigs.GetFastBySymbolIndex(SymbolIndex));
			} else if (IndexOfList % 2 == 1) { 
				SetBackGround(this.ReelContronller.elementConfigs.GetBackground(SymbolIndex));
			}

			m_SymbolImage.transform.localScale = new Vector3 (1f, 1f, 1f);
		}
//		public Image BackGroundImage;
//		public void ChangeBgImage(Sprite sprite)
//		{
//			if (BackGroundImage != null) {
//				BackGroundImage.sprite = sprite;
//			}
//		}
//
//		public override void SetStaticSprite (Sprite sprite)
//		{
//			base.SetStaticSprite (sprite);
//			if (sprite != null) {
//				staticImage.transform.localScale = new Vector3 (1f, 1f, 1f);
//			}
//		}

		public void PlaySymbolAnimation()
		{
			GameObject go = this.ReelContronller.elementConfigs.GetAnimation (SymbolIndex);

			if (go != null) {
				DestroyAnimation ();
				AnimationGO = Instantiate (go) as GameObject;
				AnimationGO.transform.SetParent (this.transform, false);
				//				AnimationGO.transform.localScale = this.ScaleFacter;
				AnimationGO.transform.SetSiblingIndex (1);

				animator = AnimationGO.transform.GetChild (0).GetComponent<Animator> ();
				if (IndexOfList % 2 == 0) {
					animator.SetTrigger ("red");
				} else {
					animator.SetTrigger ("blue");
				}
			}
		}

		public void ChangeColorAnimation(float endColorValue)
		{
			m_SymbolImage.DOColor (new Color (endColorValue, endColorValue, endColorValue, 1f), 1f);
		}
}

}
