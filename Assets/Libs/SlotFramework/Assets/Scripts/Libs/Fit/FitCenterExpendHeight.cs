namespace Libs
{
	using UnityEngine;
	using System.Collections;

	public class FitCenterExpendHeight : MonoBehaviour
	{
		public float DefaultWidthRatio =16f;
		public float DefaultHeightRatio = 9f;
		void Awake ()
		{
            if (SkySreenUtils.GetScreenSizeType()==SkySreenUtils.ScreenSizeType.Size_16_9)
            {
                return;
            }
            float targetScale = transform.localScale.y * Screen.height * DefaultWidthRatio / (Screen.width * DefaultHeightRatio);

			transform.localScale = new Vector3 (transform.localScale.x, targetScale, transform.localScale.z);
		}
	}
}
