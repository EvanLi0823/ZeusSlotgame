using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Classic;

public class StripsHintAnticipationAnimationController : MonoBehaviour {

	public List<GameObject> anticipationList= new List<GameObject>();
	void Awake(){
		//Reel Blink hyccong
		if (RenderLevelSwitchMgr.Instance.CheckRenderLevelIsOK(GameConstants.ReelBlinkAnimationLevel_Key))
		{
			Messenger.AddListener<int>(SlotControllerConstants.PLAY_ANTICIPATION_EFFECT, PlayAnimation);
			Messenger.AddListener(SlotControllerConstants.STOP_ANTICIPATION_EFFECT, StopAnimation);
		}
		StopAnimation ();
	}
	void OnDestroy(){
		if (RenderLevelSwitchMgr.Instance.CheckRenderLevelIsOK(GameConstants.ReelBlinkAnimationLevel_Key))
		{
			Messenger.RemoveListener<int>(SlotControllerConstants.PLAY_ANTICIPATION_EFFECT, PlayAnimation);
			Messenger.RemoveListener(SlotControllerConstants.STOP_ANTICIPATION_EFFECT, StopAnimation);
		}
	}
	void PlayAnimation(int reelIndex){
		for (int i = 0; i < anticipationList.Count; i++) {
			if (i==reelIndex&&anticipationList[i]!=null) {
				anticipationList [i].SetActive (true);
			} 
		}

	}

	void StopAnimation(){
		for (int i = 0; i < anticipationList.Count; i++) {			
			if (anticipationList[i]!=null) {
				anticipationList [i].SetActive(false);
			}
		}

	}
}
