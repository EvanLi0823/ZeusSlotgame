using UnityEngine;
using System.Collections;

namespace Classic
{
public class FixedBottom : MonoBehaviour {

		public float devHeight = 9f;
		public float devWidth = 16f;
		void Awake(){
            if (SkySreenUtils.GetScreenSizeType() != SkySreenUtils.ScreenSizeType.Size_16_9)
            {
                RectTransform rc = transform as RectTransform;
                Vector2 min = rc.anchorMin;
                Vector2 max = rc.anchorMax;
                float height = max.y - min.y;
                float targetHeight = height * Screen.width * devHeight / (Screen.height * devWidth);
                //          max.y = min.y + targetHeight;
                //          rc.anchorMax = max;
                rc.localScale = new Vector3(rc.localScale.x, targetHeight / height, rc.localScale.z);
            }
		
		}
}
}
