using UnityEngine;
using System.Collections;

namespace Classic
{
	public class FixedFull : MonoBehaviour
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
			float screenWidth = Screen.width;
			float screenHeight = Screen.height;
			
			
			if (screenHeight > screenWidth)//可认为是竖屏模式
			{	
				screenWidth = Screen.height;
				screenHeight = Screen.width;
			}
			
			
            if (SkySreenUtils.GetScreenSizeType() != SkySreenUtils.ScreenSizeType.Size_16_9)
            {
                float targetScale = transform.localScale.x * screenWidth * devHeight / (screenHeight * devWidth);
                if (targetScale < 1)
                {
                    transform.localScale = new Vector3(targetScale, targetScale, transform.localScale.z);
                }
                else
                {
                    transform.localScale = new Vector3(1 / targetScale, 1 / targetScale, transform.localScale.z);
                }
            }
			
		}
	}
}
