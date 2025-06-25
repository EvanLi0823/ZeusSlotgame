using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class IPhoneXCanvasScalerAdapter : MonoBehaviour {

	private CanvasScaler canvasScaler;
	void Awake(){
#if UNITY_EDITOR
        SkySreenUtils.SetScreenResolutions();
#endif
        if (IphoneXAdapter.IsIphoneX()) {
			canvasScaler = GetComponent<CanvasScaler> ();
			if (canvasScaler!=null) {
				canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
				canvasScaler.scaleFactor = 1f;
			}
		}
	}
}
