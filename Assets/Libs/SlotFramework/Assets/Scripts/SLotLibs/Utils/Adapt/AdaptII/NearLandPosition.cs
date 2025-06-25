using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NearLandPosition : MonoBehaviour
{
    [Header("20.5:9")]
    public Vector2 m_Position1;
    [Header("19.5:9")]
    public Vector2 m_Position2;
    [Header("18:9")]
    public Vector2 m_Position3;
    [Header("16:9")]
    public Vector2 m_Position4;
    [Header("16:10")]
    public Vector2 m_Position5;
    [Header("15:10")]
    public Vector2 m_Position6;
    [Header("4:3")]
    public Vector2 m_Position7;
    // Use this for initialization
    void Start()
    {
        if (SkySreenUtils.CurrentOrientation == ScreenOrientation.LandscapeLeft)
        {
            float realRadio = Screen.width *1.0f / Screen.height;
            float radio1 = 2460 / 1080f;//20.5:9
            float radio2 = 2436 / 1125f;//19.5:9
            float radio3 = 2160 / 1080f;//18:9
            float radio4 = 1920 / 1080f;//16:9
            float radio5 = 2560 / 1600f;//16:10
            float radio6 = 1133 / 744f;//15:10
            float radio7 = 1920 / 1440f;//4:3
            if (realRadio>=(radio1+radio2)/2)
            {
                transform.localPosition = m_Position1;
            }else if (realRadio>=(radio2+radio3)/2)
            {
                transform.localPosition = m_Position2;
            }else if (realRadio>=(radio3+radio4)/2)
            {
                transform.localPosition = m_Position3;
            }else if (realRadio>=(radio4+radio5)/2)
            {
                transform.localPosition = m_Position4;
            }else if (realRadio>=(radio5+radio6)/2)
            {
                transform.localPosition = m_Position5;
            }else if (realRadio>=(radio6+radio7)/2)
            {
                transform.localPosition = m_Position6;
            }
            else
            {
                transform.localPosition = m_Position7;
            }
            
        }
    } 
}
