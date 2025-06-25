using UnityEngine;
using System.Collections;
using DG.Tweening;
using Classic;

public class CurveCollectBallMovePath : CollectBallMovePathBase
{
	protected float AniDuratiom;
	public Ease mEase = Ease.InQuint;
	public float f_1 = 0.4f, f_2 = 0.16f;
	public ECollectPathOffsetDir mECollectPathOffsetDir;
	Vector3[] mWaypointArray = new Vector3[3];
	public Vector3 startScale = Vector3.one;
	public Vector3 targetScale = Vector3.one;
	public override void Init(CurveCollectPathController curvePathController,BaseElementPanel elementPanel)
	{
		base.Init(curvePathController,elementPanel);

		mWaypointArray[2] = mCurvePathController.mEndTransfm.position;
		mWaypointArray[0] = mElementPanel.transform.position + mCurvePathController.offsetStartPos;
		Vector3 offsetDir = mECollectPathOffsetDir == ECollectPathOffsetDir.E_PathOffset_Down ? Vector3.down:Vector3.up;
		mWaypointArray[1] = mWaypointArray[0] + (mWaypointArray[2] - mWaypointArray[0]) * f_1 + offsetDir * Vector3.Distance(mWaypointArray[0], mWaypointArray[2]) * f_2;

		AniDuratiom = Duration + mCurvePathController.GetExtraAniDurationBySymbolPos(mElementPanel);
	}

	public override Tweener GenerateTweener(CollectBall collectBall)
	{
		if (!targetScale.Equals(Vector3.one))
		{
			collectBall.mTransfm.localScale = startScale;
			collectBall.mTransfm.DOScale(targetScale, AniDuratiom);
		}
		return collectBall.mTransfm.DOPath(mWaypointArray, AniDuratiom, PathType.CatmullRom).SetEase(mEase);
	}

	public override float GetDuration()
	{
		return AniDuratiom;
	}
}
