using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Classic;

public class CollectBallMovePathBase : MonoBehaviour
{
	public int PathIndex = 0;
	public float Duration;
	protected BaseElementPanel mElementPanel;
	protected CurveCollectPathController mCurvePathController;

	public virtual void Init(CurveCollectPathController curvePathController,BaseElementPanel elementPanel)
	{
		mElementPanel = elementPanel;
		mCurvePathController = curvePathController;
	}

	public virtual Tweener GenerateTweener(CollectBall collectBall){
		return null;
	}

	public virtual void OnDoPathEnd(CollectBall collectBall)
	{
		return;
	}

	public virtual float GetDuration()
	{
		return Duration;
	}
}
