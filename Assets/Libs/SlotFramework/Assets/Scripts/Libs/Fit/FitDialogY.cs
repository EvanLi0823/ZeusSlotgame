using UnityEngine;
using System.Collections;
namespace Libs
{
	/// <summary>
	/// Fit dialog y. 用于dialog中tips的显示
	/// 参照BetChangeDialog，如果是在屏幕下面则需要在dialog中 上对齐，相对于dialog上面顶点为0，如果在屏幕上面则需要在dialog中下对齐。
	/// </summary>
	public class FitDialogY : MonoBehaviour {

		private float defaultWidth = 16f;
		private float defaultHeight= 9f;
		void Awake () {
            if (SkySreenUtils.GetScreenSizeType() == SkySreenUtils.ScreenSizeType.Size_16_9)
            {
                return;
            }
			RectTransform mTransform = this.transform as RectTransform;
			float scale =  Screen.width * defaultHeight / (Screen.height * defaultWidth);
			float targetY = mTransform.anchoredPosition.y / scale;
			mTransform.anchoredPosition = new Vector3 (mTransform.anchoredPosition.x, targetY,0f);
	 	}
	}
}
