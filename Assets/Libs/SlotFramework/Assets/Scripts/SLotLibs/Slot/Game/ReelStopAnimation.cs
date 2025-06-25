using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReelStopAnimation : MonoBehaviour {
	public List<GameObject> reelStopAnimation;
	void Awake(){
		Messenger.AddListener<int> (SlotControllerConstants.PLAY_ANTICIPATION_EFFECT,PlayReelStopEffect);
		Messenger.AddListener (SlotControllerConstants.OnSpinEnd, StopReelStopEffect);
	}

	void OnDestroy(){
		Messenger.RemoveListener<int> (SlotControllerConstants.PLAY_ANTICIPATION_EFFECT,PlayReelStopEffect);
		Messenger.RemoveListener (SlotControllerConstants.OnSpinEnd, StopReelStopEffect);
	}
	void PlayReelStopEffect(int idx){
		if (reelStopAnimation!=null&&idx<reelStopAnimation.Count&&reelStopAnimation [idx]!=null) {
			reelStopAnimation [idx].SetActive (true);
		}
	}
	void StopReelStopEffect(){
		if (reelStopAnimation!=null) {
			for (int i = 0; i < reelStopAnimation.Count; i++) {
				if (reelStopAnimation[i]!=null) {
					reelStopAnimation [i].SetActive (false);
				}
			}
		}
	}
}
