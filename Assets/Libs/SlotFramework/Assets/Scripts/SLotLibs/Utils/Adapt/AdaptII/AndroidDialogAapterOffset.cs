
using UnityEngine;

public class AndroidDialogAapterOffset : MonoBehaviour 
{
    void Start()
    {
        bool isPortrait = false;
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;
			
//#if UNITY_EDITOR
        if (screenHeight > screenWidth)//可认为是竖屏模式
        {	
            screenWidth = Screen.height;
            screenHeight = Screen.width;
            isPortrait = true;
        }
//#endif
        float realRadio = screenWidth / screenHeight;
        float commonRadio = 16 / 9f;
        if (realRadio > commonRadio)
        {
//            float scale = realRadio/commonRadio  ;
//            transform.localScale = new Vector3(scale, scale, scale);
        }
        else
        {
            float scale = commonRadio /  realRadio;
            //if (isPortrait) scale = 1 / scale;
            if (SkySreenUtils.CurrentOrientation == ScreenOrientation.Portrait) scale = 1 / scale;
                transform.localScale = new Vector3(scale, scale, scale);
        }
    }
}
