using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseIdleAnimation : MonoBehaviour
{
	void Awake()
	{
		if (!RenderLevelSwitchMgr.Instance.CheckRenderLevelIsOK(GameConstants.IdleAnimationLevel_Key))
		{
			Animator animator = transform.GetComponent<Animator>();
			if (animator != null)
				animator.speed = 0;
		}
	}
}
