using UnityEngine;
using System.Collections;

public class AlignPosition : MonoBehaviour {

	public float devHeight = 9f;
	public float devWidth = 16f;
	public const int NormalHight =1080;
	public enum AlignType
	{
		AlignType_None,
		AlignType_Bottom,
		AlignType_Top,
		AlignType_Center,
	}
	public AlignType currentType = AlignType.AlignType_None;
	private float offset = 0f;
	void Start()
	{
		RePosition();
	}

	void RePosition()
	{
		float targetScale = 1f * Screen.height * devWidth /(Screen.width* devHeight) ;
		RectTransform rect = (transform as RectTransform);
		if (!Mathf.Approximately(targetScale,1f)) {
		 	offset = NormalHight*(targetScale-1f);
			switch (currentType) {
			case AlignType.AlignType_Bottom:
				rect.offsetMin = new Vector2 (0,-offset);
				break;
			case AlignType.AlignType_Top:
				rect.offsetMax = new Vector2 (0,offset);
				break;
			default:
				break;
			}

		}
	}
}
