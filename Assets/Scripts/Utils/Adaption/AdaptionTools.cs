using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdaptionTools
{
    public const float IphoneXSize = 19.5f/9;
    public const float IpadSize = 1.33f;
    public const float IphoneSize = 16 / 9.0f;
    public const float Design_Width = 1920f;
    public const float Design_Heigth = 1080;

    /// <summary>
    ///19.5 以上，横向拉升
    ///19.5:12 - 4:3 相机裁切
    ///4:3 等比缩放 按照16：9 的展示效果为基础缩放
    /// </summary>
    /// <param name="BGImage"></param>
    public static void ExpandSize(RectTransform BGImage)
    {
        RectTransform rect = BGImage;
        if (rect != null)
        {
            float ratio = 1;
            float screenRatio = Screen.width > Screen.height ? Screen.width * 1f / Screen.height :
                Screen.height * 1f /  Screen.width;
           
            if ( screenRatio>= IphoneXSize)
            {
                ratio = screenRatio * 9 /  19.5f;
                rect.sizeDelta=new Vector2(rect.sizeDelta.x*ratio,rect.sizeDelta.y);
            }
            else if (screenRatio < IpadSize)
            {
                ratio = screenRatio * 9 / 16f;
                rect.sizeDelta=new Vector2(rect.sizeDelta.x*ratio,rect.sizeDelta.y*ratio);
            } 
        }
    }

    /// <summary>
    /// 是否使用竖版Ipad专用的Banner
    /// </summary>
    /// <returns></returns>
    public static bool isUsePortraitPadBanner()
    {
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;
			

        if (screenHeight > screenWidth)//可认为是竖屏模式
        {	
            screenWidth = Screen.height;
            screenHeight = Screen.width;
        }

        if (screenWidth / screenHeight - 1.7f < -0.05f)
            return true;
        return false;
    }
    
    /// <summary>
    ///19.5 以上，横向拉升
    ///19.5:12 - 4:3 相机裁切
    ///4:3 等比缩放 按照16：9 的展示效果为基础缩放
    /// </summary>
    /// <param name="transform"></param>
    public static void ScaleSize (Transform transform)
    {
        var scale = transform.localScale;
        float ratio = 1;
        float screenRatio = Screen.width > Screen.height ? Screen.width * 1f / Screen.height :
            Screen.height * 1f /  Screen.width;
        if (screenRatio > IphoneXSize)
        {
            ratio = screenRatio * 9 / 19.5f;
            scale.x *= ratio;
            transform.localScale = scale;
        }
        else if (screenRatio < IpadSize)
        {
            ratio = screenRatio * 9 / 16f;
            scale.x *= ratio;
            scale.y *= ratio;
            transform.localScale = scale;
        }
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="needSafeArea">安全区域</param>
    /// <param name="isLeft">向左偏移</param>
    /// <param name="offsetMul">偏移比例</param>
    public static void MoveX(Transform transform,bool needSafeArea,bool isLeft,float offsetMul=1)
    {
        float _safeArea = 0;
        if (needSafeArea)
        {
            _safeArea = Screen.safeArea.x;
        }
      
        int direction = 1;
        if (isLeft)
        {
            direction = -1;
        }
        var pos =transform.localPosition;
        float currentResolution = Screen.width * 1f / Screen.height;
        float offset = Design_Width * (currentResolution / IphoneSize - 1)/2f * offsetMul;
        pos.x += ((direction * offset)+_safeArea);
        transform.localPosition = pos;
    }
    
    public static void MoveY(Transform transform,bool needSafeArea,bool isDown,float offsetMul=1)
    {
        float _safeArea = 0;
        if (needSafeArea)
        {
            _safeArea = Screen.safeArea.y;
        }
      
        int direction = 1;
        if (isDown)
        {
            direction = -1;
        }
        var pos =transform.localPosition;
        float currentResolution = Screen.width * 1f / Screen.height;
        float offset = 1920 * (currentResolution / IphoneSize - 1)/2f * offsetMul;
        pos.x += ((direction * offset)+_safeArea);
        transform.localPosition = pos;
    }
    
}
