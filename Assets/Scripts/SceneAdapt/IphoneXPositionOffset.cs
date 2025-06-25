using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Libs;
public class IphoneXPositionOffset : MonoBehaviour {
	[Header("x为向右边偏移，y为向上偏移")]
	public Vector2 m_Offset;
	// Use this for initialization
	void Start () {
        if (SkySreenUtils.CurrentOrientation== ScreenOrientation.LandscapeLeft)
        {
            if (IphoneXAdapter.IsIphoneX())
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
