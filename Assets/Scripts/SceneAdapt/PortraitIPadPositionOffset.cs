using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortraitIPadPositionOffset : MonoBehaviour
{
    public Vector2 m_Offset;
    public bool EnableMesseageRecived = false;
    public bool setPos = false;

    private void Awake()
    {
        Messenger.AddListener(Libs.DialogOrientationAdapter.ADJUST_SCREEN_ORIENTATION_SETTINGS,AdjustLoadingLayout);
    }
    private void OnDestroy()
    {
        Messenger.RemoveListener(Libs.DialogOrientationAdapter.ADJUST_SCREEN_ORIENTATION_SETTINGS, AdjustLoadingLayout);
    }
    private void Start()
    {
        if (!EnableMesseageRecived)
        {
            AdjustLoadingLayout();
        }
    }
    void AdjustLoadingLayout()
    {
        if (SkySreenUtils.CurrentOrientation == ScreenOrientation.Portrait)
        {
            if (SkySreenUtils.GetScreenSizeType() == SkySreenUtils.ScreenSizeType.Size_4_3)
            {
                if(setPos)
                {
                    RectTransform pos = this.transform as RectTransform;
                    pos.localPosition = new Vector2(m_Offset.x, m_Offset.y);
                    return;
                }
                
                RectTransform rct = this.transform as RectTransform;
                Vector3 v = rct.localPosition;
                v.x += m_Offset.x;
                v.y += m_Offset.y;
                rct.localPosition = v;
            }
        }
    }
}
