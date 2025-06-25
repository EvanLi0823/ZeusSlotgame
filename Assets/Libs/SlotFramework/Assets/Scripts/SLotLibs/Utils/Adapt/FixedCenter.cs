using UnityEngine;
using System.Collections;

namespace Classic
{
public class FixedCenter : MonoBehaviour {

		
		public float devHeight = 9f;
		public float devWidth = 16f;
		void Awake(){
			resetSize ();
		}
		private void resetSize ()
		{
            if (SkySreenUtils.GetScreenSizeType()!=SkySreenUtils.ScreenSizeType.Size_16_9)
            {
                float targetScale = transform.localScale.y * Screen.width * devHeight / (Screen.height * devWidth);
                transform.localScale = new Vector3(transform.localScale.x, targetScale, transform.localScale.z);
            }
		}
}
}
