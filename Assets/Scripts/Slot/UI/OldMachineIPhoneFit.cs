using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OldMachineIPhoneFit : MonoBehaviour {
	[Header("iphone时向下偏移的像素")]
	public float m_MoveOffsetY;
    [Header("ipad时向下偏移的像素")]
    public float m_MoveOffsetYForIpad;
    // Use this for initialization
    void Awake () {
        float offsetY = m_MoveOffsetY;
        if(m_MoveOffsetYForIpad > 0 && SkySreenUtils.GetScreenSizeType() != SkySreenUtils.ScreenSizeType.Size_16_9)
        {
            offsetY = m_MoveOffsetYForIpad;
        }
//		if (SkySreenUtils.GetScreenSizeType () == SkySreenUtils.ScreenSizeType.Size_16_9) {
        RectTransform rect = this.transform as RectTransform;
			Vector3 v = rect.localPosition;
			v.y -= offsetY;
			rect.localPosition = v;
//		}
	}

}
