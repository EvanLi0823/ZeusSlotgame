using UnityEngine;
using System.Collections;
 
public class ToggleImageRatio : MonoBehaviour {
	public GameObject IphoneGO;
	public GameObject IpadGo;
	void Awake ()
	{
		switch (SkySreenUtils.GetScreenSizeType ()) {
		case SkySreenUtils.ScreenSizeType.Size_16_9:{
				if (IphoneGO != null) {
					IphoneGO.SetActive (true);
				}
				if (IpadGo != null) {
					IpadGo.SetActive (false);
				}
			}
				break;
		case SkySreenUtils.ScreenSizeType.Size_4_3:{
				if (IphoneGO != null) {
					IphoneGO.SetActive (false);
				}
				if (IpadGo != null) {
					IpadGo.SetActive (true);
				}
			}
				break;
		case SkySreenUtils.ScreenSizeType.Size_3_2:{
				
			}
				break;
			default:
			break;
		}
	}
}
