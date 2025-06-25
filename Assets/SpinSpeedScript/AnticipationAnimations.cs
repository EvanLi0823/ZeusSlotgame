using System.Collections;
using System.Collections.Generic;
//using Classic;
using UnityEngine;

public class AnticipationAnimations : MonoBehaviour {
    //索引从第一个轮子开始
    public List<GameObject> Animations;

	public virtual void ShowAnimation(int reelIndex)
	{
		if (Animations == null || reelIndex >= Animations.Count) {
			return;
		}

        if (Animations [reelIndex] != null) {
			Animations [reelIndex].SetActive (true);
		}
	}

	public virtual void HideAnimation(int reelIndex)
	{
		if (Animations == null || reelIndex >= Animations.Count) {
			return;
		}
    
        if (Animations [reelIndex] != null) {
			Animations [reelIndex].SetActive (false);
		}
	}

	public virtual void HideAllAnimation()
	{
		if (Animations == null ) {
			return;
		}

		for (int i = 0; i < Animations.Count; i++) {
			if (Animations [i] != null) {
				Animations [i].SetActive (false);
			}
		}
	}
}
