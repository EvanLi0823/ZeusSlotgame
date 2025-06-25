using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SkyReSizeFont : MonoBehaviour {

    private float width = 1087f;
//    private float height = 611f;
    private  Text myText;
    public float InitFontSize = 20f;

	// Use this for initialization
	void Start () {
        myText = GetComponent<Text> ();
        myText.fontSize =(int) (InitFontSize * Screen.width / width);
	}

}
