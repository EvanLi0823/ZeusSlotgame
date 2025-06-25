using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonusTransitionOffsetAdapter : MonoBehaviour
{
   [Header("x为向右边偏移，y为向上偏移")]
    public Vector2 IPhoneOffset;
    [Header("x为向右边偏移，y为向上偏移")]
    public Vector2 IPhoneXOffset;

    [Header("x为向右边偏移，y为向上偏移")] 
    public Vector2 IPadOffset;
    
    [Header("20.5:9的偏移，x为向右，y为向上")]
    public Vector2 Offset;
    // Use this for initialization
    void Start()
    {
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;
			
//#if UNITY_EDITOR
        if (screenHeight > screenWidth)//可认为是竖屏模式
        {	
            screenWidth = Screen.height;
            screenHeight = Screen.width;
        }
        
        float m = screenWidth / screenHeight;// 
        float n = 2460 / 1080f;
        if (m==n)
        {
            if (Offset == Vector2.zero) return;
            RectTransform rct = this.transform as RectTransform;
            Vector3 v = rct.localPosition;
            v.x += Offset.x;
            v.y += Offset.y;
            rct.localPosition = v;
        }
         if (IphoneXAdapter.IsIphoneX())
        {
            if (IPhoneXOffset == Vector2.zero) return;
            RectTransform rct = this.transform as RectTransform;
            Vector3 v = rct.localPosition;
            v.x += IPhoneXOffset.x;
            v.y += IPhoneXOffset.y;
            rct.localPosition = v;
        }
        else if (SkySreenUtils.GetScreenSizeType()== SkySreenUtils.ScreenSizeType.Size_4_3)
        {
            if (IPadOffset == Vector2.zero) return;
            RectTransform rct = this.transform as RectTransform;
            Vector3 v = rct.localPosition;
            v.x += IPadOffset.x;
            v.y += IPadOffset.y;
            rct.localPosition = v;
        }
        else if (SkySreenUtils.GetScreenSizeType()== SkySreenUtils.ScreenSizeType.Size_16_9)
        {
            if (IPhoneOffset == Vector2.zero) return;
            RectTransform rct = this.transform as RectTransform;
            Vector3 v = rct.localPosition;
            v.x += IPhoneOffset.x;
            v.y += IPhoneOffset.y;
            rct.localPosition = v;
        }
    }
}
