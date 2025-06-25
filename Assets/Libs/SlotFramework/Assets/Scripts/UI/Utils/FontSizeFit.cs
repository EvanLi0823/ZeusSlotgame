using UnityEngine;
using System.Collections;
using UnityEngine.UI;
[RequireComponent(typeof(Text))]
public class FontSizeFit : MonoBehaviour {
	private readonly int SCREEN_WIDTH_16_9 = 1920;
	private readonly int SCREEN_HEIGHT_16_9 = 1080;
	private readonly int SCREEN_WIDTH_4_3 = 2048;
	private readonly int SCREEN_HEIGHT_4_3 = 1536;
	private Text text;
	void Awake(){
		text = transform.GetComponent<Text> ();
	}

	// Use this for initialization
	void Start () {
		changeFontScale ();
	}
	
	// Update is called once per frame
//	void Update () {
//		changeFontScale ();
//	}

	private void changeFontScale(){
		float scaleX = 1.0f;
		float scaleY = 1.0f;
		SkySreenUtils.ScreenSizeType screenType = SkySreenUtils.GetScreenSizeType ();
		if (screenType == SkySreenUtils.ScreenSizeType.Size_16_9) {
			scaleX = (float)Screen.width / SCREEN_WIDTH_16_9;
			scaleY = (float)Screen.height / SCREEN_HEIGHT_16_9;
		} else if (screenType == SkySreenUtils.ScreenSizeType.Size_4_3) {
			scaleX = (float)Screen.width / SCREEN_WIDTH_4_3;
			scaleY = (float)Screen.height / SCREEN_HEIGHT_4_3;
		}
		if (this.text !=null){
			this.text.transform.localScale = new Vector3(scaleX,scaleY,1.0f);
		}
	}
}
