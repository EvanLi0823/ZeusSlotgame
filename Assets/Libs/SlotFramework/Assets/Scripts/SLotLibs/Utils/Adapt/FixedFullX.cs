using UnityEngine;
using System.Collections;

namespace Classic
{
	public class FixedFullX : MonoBehaviour
	{
		public float devHeight = 9f;
		public float devWidth = 16f;
		void Awake(){
			resetSize ();
		}

		private void resetSize ()
		{
            if (SkySreenUtils.GetScreenSizeType() != SkySreenUtils.ScreenSizeType.Size_16_9)
            {
                float targetScale = transform.localScale.x * Screen.width * devHeight / (Screen.height * devWidth);
                transform.localScale = new Vector3 (targetScale, transform.localScale.y, transform.localScale.z);
            }
			
		}
	}
}
