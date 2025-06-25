using UnityEngine;

public class AdaptionContentSize:MonoBehaviour
{
    public const float IphoneSize = 16 / 9.0f;
    
    [Header("是否是竖版弹窗（竖屏旋转的此处仍算是横版）")]
    public  bool isPortarit;
    void Awake()
    {
        ChangeSize();
    }
    private  void ChangeSize()
    {
        RectTransform rect = transform as RectTransform;
        if (rect != null)
        {
            float ratio = 1;
            float screenRatio = Screen.width > Screen.height ? Screen.width * 1f / Screen.height :
                Screen.height * 1f /  Screen.width;
           
            if ( screenRatio>= IphoneSize)
            {
                ratio = screenRatio * 9 /  16f;

                if (isPortarit && Screen.width < Screen.height)
                {
                    rect.sizeDelta=new Vector2(rect.sizeDelta.x/ratio,rect.sizeDelta.y);
                }
                else
                {
                    rect.sizeDelta=new Vector2(rect.sizeDelta.x*ratio,rect.sizeDelta.y);
                }
                
            }
            else
            {
                ratio = 16f/9 /screenRatio;
                if (isPortarit && Screen.width < Screen.height)
                {
                    rect.sizeDelta=new Vector2(rect.sizeDelta.x * ratio * ratio,rect.sizeDelta.y*ratio);
                }
                else
                {
                    rect.sizeDelta=new Vector2(rect.sizeDelta.x,rect.sizeDelta.y*ratio);
                }
            }
        }
    }

}
