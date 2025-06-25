using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelXOffset : MonoBehaviour
{
    [Header("16:10")]
    public Vector2 m_Offset1;
    [Header("16:9")]
    public Vector2 m_Offset2;
	
    // Use this for initialization
    void Start () {

        SkySreenUtils.SetScreenResolutions();
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;
        if (Screen.width < Screen.height)
        {
            screenWidth = Screen.height;
            screenHeight = Screen.width;
        }

        if (SkySreenUtils.CurrentOrientation == ScreenOrientation.LandscapeLeft)
        {
            RectTransform rct = this.transform as RectTransform;
            Vector3 v = rct.localPosition;
            if (screenHeight / screenWidth == 1080/1920f)
            {
                v.x += m_Offset2.x;
                v.y += m_Offset2.y;
                
            }

            if (screenHeight / screenWidth == 1600/2560f)
            {
                v.x += m_Offset1.x;
                v.y += m_Offset1.y;

            }
            rct.localPosition = v;
        }
       
    }
}
