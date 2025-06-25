using UnityEngine;
using System.Collections;

namespace Classic
{
	public class FixedFullIncludeZ : MonoBehaviour
	{

		public float devHeight = 9f;
		public float devWidth = 16f;

		public static float Scaler = 1f;

		void Awake ()
		{

			resetSize ();
		}
		
		private void resetSize ()
		{
            if (SkySreenUtils.GetScreenSizeType() != SkySreenUtils.ScreenSizeType.Size_16_9)
            {
                float targetScale = transform.localScale.x * Screen.width * devHeight / (Screen.height * devWidth);
                transform.localScale = new Vector3(targetScale, targetScale, targetScale);
            }
			
		}
	}
}
