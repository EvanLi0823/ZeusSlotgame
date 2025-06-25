using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NearLandScale : MonoBehaviour
{
    [Header("20.5:9")] public Vector2 m_Scale1;
    [Header("19.5:9")] public Vector2 m_Scale2;
    [Header("18:9")] public Vector2 m_Scale3;
    [Header("16:9")] public Vector2 m_Scale4;
    [Header("16:10")] public Vector2 m_Scale5;
    [Header("15:10")] public Vector2 m_Scale6;
    [Header("4:3")] public Vector2 m_Scale7;

    // Start is called before the first frame update
    void Start()
    {
        SetScale();
    }

    void OnEnable()
    {
        SetScale();
    }

    void SetScale()
    {
        if (SkySreenUtils.CurrentOrientation == ScreenOrientation.LandscapeLeft)
        {
            float realRadio = Screen.width * 1.0f / Screen.height;
            float radio1 = 2460 / 1080f; //20.5:9
            float radio2 = 2436 / 1125f; //19.5:9
            float radio3 = 2160 / 1080f; //18:9
            float radio4 = 1920 / 1080f; //16:9
            float radio5 = 2560 / 1600f; //16:10
            float radio6 = 1133 / 744f; //15:10
            float radio7 = 1920 / 1440f; //4:3
            if (realRadio >= (radio1 + radio2) / 2)
            {
                transform.localScale = m_Scale1;
            }
            else if (realRadio >= (radio2 + radio3) / 2)
            {
                transform.localScale = m_Scale2;
            }
            else if (realRadio >= (radio3 + radio4) / 2)
            {
                transform.localScale = m_Scale3;
            }
            else if (realRadio >= (radio4 + radio5) / 2)
            {
                transform.localScale = m_Scale4;
            }
            else if (realRadio >= (radio5 + radio6) / 2)
            {
                transform.localScale = m_Scale5;
            }
            else if (realRadio >= (radio6 + radio7) / 2)
            {
                transform.localScale = m_Scale6;
            }
            else
            {
                transform.localScale = m_Scale7;
            }
        }

    }
}
