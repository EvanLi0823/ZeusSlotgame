using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace UnityEngine.UI
{
	public class AutoFitCustomFontText : Text {
		private float WidthScope = 100f;
		private float HeightScope = 100f;
		public RectTransform rect = null;
		float widthScale = 1f;
		float heightScale = 1f;
		float useScale = 1f;

		protected override void OnEnable (){
			base.OnEnable ();//必须调用父类方法，否则无法添加默认材质
			rect = this.transform as RectTransform;
			alignByGeometry = true;
			horizontalOverflow = HorizontalWrapMode.Overflow;
			verticalOverflow = VerticalWrapMode.Overflow;
			alignment = TextAnchor.MiddleCenter;
			supportRichText = false;
		}
			
		protected override void UpdateGeometry(){
			AdjustTextComponent();
			base.UpdateGeometry ();
		}

		void AdjustTextComponent(){
			WidthScope = rect.sizeDelta.x;
			HeightScope = rect.sizeDelta.y;
            if (Mathf.Approximately(WidthScope,0f)||Mathf.Approximately(HeightScope,0f))
            {
                WidthScope = rect.rect.width;
                HeightScope = rect.rect.height;
            }
            widthScale = 1f;
			heightScale = 1f;
			useScale = 1f;
            if (preferredWidth > WidthScope)
            {
                widthScale = WidthScope / preferredWidth;
            }
			
            if (preferredHeight > HeightScope) {
				heightScale = HeightScope / preferredHeight;
            }

            useScale = Mathf.Min (widthScale, heightScale);
			rect.localScale = new Vector3 (useScale,useScale,1f);
		}
	}

}
