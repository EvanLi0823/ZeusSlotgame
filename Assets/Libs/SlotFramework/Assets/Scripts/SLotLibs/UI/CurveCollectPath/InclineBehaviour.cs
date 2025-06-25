using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class InclineBehaviour : CollectBallBehaviour {
	public float inclineAniDuration = 1;
	public Vector3 targetEulerAngles = Vector3.back * 60f;

    Tweener mTweener;
	public override void DoBehaviour()
	{
		Vector3 vecEulerAngles = mTransfm.localEulerAngles;
        mTweener = DOTween.To(() => mTransfm.localEulerAngles, x => vecEulerAngles = x,targetEulerAngles, inclineAniDuration).OnUpdate(() =>{
			mTransfm.localEulerAngles = vecEulerAngles;
		});
    }

    public override void EndBehaviour(){
        if (mTweener == null) return;
        mTweener.Kill();
    }
}
