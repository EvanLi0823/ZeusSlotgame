using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
public class SymbolSpineAnimationRender : MonoBehaviour {

	#region inspector
	[Header("普通中奖动画 1")]
	// [SpineAnimation]
	public string normalAnimationName;

	[Header("大中奖动画 2")]
	// [SpineAnimation]
	public string highAnimationName;

	[Header("触发bonus中奖动画 3")]
	// [SpineAnimation]
	public string bonusTriggerAnimationName;

	[Header("smartSound动画 4")]
	// [SpineAnimation]
	public string smartSoundAnimationName;

	[Header("播放当前awardline中奖时不在awardline的动画 5")]
	// [SpineAnimation]
	public string otherAwardLineAnimationName;

	[Header("变成wildsymbol的动画 6")]
	// [SpineAnimation]
	public string ToWildSymbolAnimationName;

	[Header("轮子停止前一直做动画的symbol 7")]
	// [SpineAnimation]
	public string SmartLoopAnimationName;
	#endregion

	SkeletonGraphic m_SpineSkeletonAnimation;
	void Awake()
	{
		m_SpineSkeletonAnimation = GetComponent<SkeletonGraphic>();
	}

	private bool hasSkeletonAnimation()
	{
		return m_SpineSkeletonAnimation != null;
	}

	public virtual void PlaySpineNormalAnimation(int animationId)
	{
		if (!hasSkeletonAnimation ()) {
			return;
		}

		switch(animationId)
		{
		case 1:
			this.m_SpineSkeletonAnimation.AnimationState.SetAnimation(0,normalAnimationName,false);
			break;
		case 2:
			this.m_SpineSkeletonAnimation.AnimationState.SetAnimation(0,highAnimationName,false);
			break;
		case 3:
			this.m_SpineSkeletonAnimation.AnimationState.SetAnimation(0,bonusTriggerAnimationName,false);
			break;
		case 4:
			this.m_SpineSkeletonAnimation.AnimationState.SetAnimation(0,smartSoundAnimationName,false);
			break;
		case 5:
			this.m_SpineSkeletonAnimation.AnimationState.SetAnimation(0,otherAwardLineAnimationName,false);
			break;
		case 6:
			this.m_SpineSkeletonAnimation.AnimationState.SetAnimation(0,ToWildSymbolAnimationName,false);
			break;
		case 7:
			this.m_SpineSkeletonAnimation.AnimationState.SetAnimation(0,SmartLoopAnimationName,false);
			break;
		default:
			break;
		}
	}

	public virtual void PlaySpineCustomAnimation(string animationName)
	{
		this.m_SpineSkeletonAnimation.AnimationState.SetAnimation(0,animationName,false);
	}
}
