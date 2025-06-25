using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace UnityEngine.UI
{
	public class AutoFitCustomWithParent : Image {
		private float WidthScope = 250f;
		private float HeightScope = 5;
		public RectTransform rect = null;
		float widthScale = 1f;
		float heightScale = 1f;
		float useScale = 1f;

		protected void OnEnable (){
			rect = this.transform as RectTransform;
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
			var parentPreferredWidth = this.transform.parent.GetComponent<Text>().preferredWidth;
            if (preferredWidth > WidthScope)
            {
                widthScale = WidthScope / parentPreferredWidth;
            }
            
            rect.localScale = new Vector3 (useScale,1f,1f);
		}
	}

}
