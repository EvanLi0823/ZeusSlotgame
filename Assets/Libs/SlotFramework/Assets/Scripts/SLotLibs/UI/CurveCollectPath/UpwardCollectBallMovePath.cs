using UnityEngine;
using System.Collections;
using DG.Tweening;
using Classic;

public class UpwardCollectBallMovePath : CollectBallMovePathBase
{
	protected Vector3 mEndPos;
	protected Vector3 mStartPos;
	public override void Init(CurveCollectPathController curvePathController,BaseElementPanel elementPanel)
	{
		base.Init(curvePathController,elementPanel);
		mStartPos = mElementPanel.transform.position;
		mEndPos = mStartPos + mCurvePathController.offsetStartPos;
	}

	public override Tweener GenerateTweener(CollectBall collectBall)
	{
		return collectBall.mTransfm.DOMoveY(mEndPos.y, Duration);
	}

	public override void OnDoPathEnd(CollectBall collectBall)
	{
		collectBall.PlayBehaviour();
	}
}

