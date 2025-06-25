using UnityEngine;
using System.Collections;

namespace Classic
{
	public class FixedTop : MonoBehaviour
	{
		public float devHeight = 9f;
		public float devWidth = 16f;
		void Awake(){
            if (SkySreenUtils.GetScreenSizeType() != SkySreenUtils.ScreenSizeType.Size_16_9)
            {
                RectTransform rc = transform as RectTransform;
                Vector2 min = rc.anchorMin;
                Vector2 max = rc.anchorMax;
                float height = max.y - min.y;
                float factor = Screen.width * devHeight / (Screen.height * devWidth);
                float targetHeight = height * factor;
                min.y = max.y - targetHeight;
                rc.anchorMin = min;
            }
		}
	}
}
