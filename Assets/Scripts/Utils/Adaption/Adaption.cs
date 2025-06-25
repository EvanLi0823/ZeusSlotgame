using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Adaption : MonoBehaviour
{
    [Header("勾选scale伸缩，不勾选使用改变组件尺寸")]
    public bool IsScale;
  
    [Header("勾选IsPortrait，表示在竖版中界面不是横版旋转过来的，是本来竖版显示的")]
    public bool IsPortrait;
    
    void Awake ()
    {
        if (IsPortrait)
        {
            return;
        }
        RectTransform rect = transform as RectTransform;
        if (IsScale)
        {
            AdaptionTools.ScaleSize (transform); 
        }
        else
        {
            AdaptionTools.ExpandSize(rect);
        }
        
    }
    

}
