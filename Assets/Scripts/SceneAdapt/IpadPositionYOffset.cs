using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IpadPositionYOffset : MonoBehaviour {

	public Vector2 m_Offset;
	
    // Use this for initialization
	void Start () {
		#if UNITY_EDITOR
		SkySreenUtils.SetScreenResolutions();
		#endif
        if (SkySreenUtils.CurrentOrientation == ScreenOrientation.LandscapeLeft)
        {
            if (SkySreenUtils.GetScreenSizeType() == SkySreenUtils.ScreenSizeType.Size_4_3)
            {
                RectTransform rct = this.transform as RectTransform;
                Vector3 v = rct.localPosition;
                v.x += m_Offset.x;
                v.y += m_Offset.y;
                rct.localPosition = v;
            }
        }
       
	}

}
