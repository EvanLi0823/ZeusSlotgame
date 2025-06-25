using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CommonPhoneTopUICanvasAdapter : MonoBehaviour {
	private CanvasScaler canvasScaler;
	void Awake(){
		canvasScaler = transform.GetComponent<CanvasScaler> ();
	}
	// Use this for initialization
	void Start () {
		#if UNITY_EDITOR
		SkySreenUtils.SetScreenResolutions ();
		#endif
		float screenRadio = Screen.width*1.0f / Screen.height;
		if (!IphoneXAdapter.IsIphoneX() && !SkySreenUtils.IsRealPad())//screenRadio!=1920/1440f ) 
        {
			if (canvasScaler==null) 
            {
				return;
			}
            switch (SkySreenUtils.CurrentOrientation)
            {
                case ScreenOrientation.LandscapeLeft:
                    LandscapeCanvasAdapter();
                    break;
                case ScreenOrientation.Portrait:
                    PortraitCanvasAdapter();
                    break;
                default:
                    break;
            }
        }
	}
    void LandscapeCanvasAdapter()
    {
        canvasScaler.matchWidthOrHeight = 0;
        
        if (CommonMobileDeviceAdapter.IsWideScreen())
        {
            canvasScaler.matchWidthOrHeight = 1;
        }
        else if (CommonMobileDeviceAdapter.IsSquareScreen())
        {
            canvasScaler.matchWidthOrHeight = 0;
        }
    }

    void PortraitCanvasAdapter()
    {
       //todo 待补充
    }
}
