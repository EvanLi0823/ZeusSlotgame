using UnityEngine;
using System.Collections;

public class ShineAnimation : MonoBehaviour {

	public GameObject IphoneGO;
	public GameObject IpadGo;
	public Animator GetCurrentUsedShineAnimation(){
		Animator ani = null;
		switch (SkySreenUtils.GetScreenSizeType ()) {
		case SkySreenUtils.ScreenSizeType.Size_16_9:{
				if (IpadGo != null) {
					IpadGo.SetActive (false);
				}
				if (IphoneGO != null) {
					IphoneGO.SetActive (true);
					ani = IphoneGO.GetComponent<Animator> ();
				}
			
			}
			break;
		case SkySreenUtils.ScreenSizeType.Size_4_3:{
				if (IphoneGO != null) {
					IphoneGO.SetActive (false);
				}
				if (IpadGo != null) {
					IpadGo.SetActive (true);
					ani = IpadGo.GetComponent<Animator> ();
				}
			}
			break;
		case SkySreenUtils.ScreenSizeType.Size_3_2:{
				IphoneGO.SetActive (true);
				IpadGo.SetActive (false);
				ani = IphoneGO.GetComponent<Animator> ();
			}
			break;
		default:{
				IphoneGO.SetActive (true);
				IpadGo.SetActive (false);
				ani = IphoneGO.GetComponent<Animator> ();
			}
			break;
		}
		return ani;
	}
}
