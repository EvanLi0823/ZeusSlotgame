using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AndroidDialogAdapter : MonoBehaviour  //修改大小
{
//    public float devHeight = 9f;
//    public float devWidth = 16f;
    
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
            float scale =realRadio / commonRadio ;
            if (isPortrait) scale = 1 / scale;
            transform.localScale = new Vector3(scale, scale, scale);
        }
    }

}
