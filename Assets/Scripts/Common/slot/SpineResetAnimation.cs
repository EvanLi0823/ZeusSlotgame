using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
public class SpineResetAnimation : MonoBehaviour {
	public SkeletonGraphic skeletonAnimation;

	private string animationId="";
	void Awake()
	{
		if (skeletonAnimation != null) {
			animationId = skeletonAnimation.AnimationState.GetCurrent (0).Animation.Name;
		}
	}

	void OnEnable()
	{
		Reset ();

	}

	void Reset(){
		skeletonAnimation.AnimationState.ClearTracks ();
		skeletonAnimation.AnimationState.SetAnimation (0, animationId, false);
//		skeletonAnimation.Skeleton.SetToSetupPose();
//		Debug.Log(skeletonAnimation.AnimationState.GetCurrent (0).Animation.Name);
//		skeletonAnimation.AnimationState.ClearTracks ();

//		skeletonAnimation.AnimationState.SetAnimation(0,
	}
}
