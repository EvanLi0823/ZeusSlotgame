using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Classic;

public class InitAnimationData : MonoBehaviour {

	public GameObject bigWin_Iphone;
	public GameObject bigWin_Ipad;

	public List<GameObject> smallWin_Iphone;
	public List<GameObject> smallWin_Ipad;
	public ReelManager reelManager;
	void Awake(){
		if (reelManager==null) {
			return;
		}

		switch (SkySreenUtils.GetScreenSizeType ()) {
		case SkySreenUtils.ScreenSizeType.Size_16_9:{
				TurnOnIphone ();
			}
			break;
		case SkySreenUtils.ScreenSizeType.Size_4_3:{
				TurnOnIpad ();
			}
			break;
		case SkySreenUtils.ScreenSizeType.Size_3_2:{
				TurnOnIphone ();
			}
			break;
		default:{
				TurnOnIphone ();
			}
			break;
		}

	}

	public virtual void TurnOnIphone(){
		//reelManager.BigAnimation = bigWin_Iphone;
		bigWin_Ipad.SetActive (false);
		reelManager.reelWinCoinAnimations = smallWin_Iphone;
		foreach (GameObject item in smallWin_Ipad) {
			item.SetActive (false);
		}
	}
	public virtual void TurnOnIpad(){
		//reelManager.BigAnimation = bigWin_Ipad;
		bigWin_Iphone.SetActive (false);
		reelManager.reelWinCoinAnimations = smallWin_Ipad;
		foreach (GameObject item in smallWin_Iphone) {
			item.SetActive (false);
		}
	}
}
