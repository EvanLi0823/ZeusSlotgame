using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Classic;

public class CollectPathUnit : MonoBehaviour
{
	public bool IsPathReady { get; private set;}
	CurveCollectPathController mCurveCollectPathController;
	protected List<CollectBallMovePathBase> mCollectBallMovePathList = new List<CollectBallMovePathBase>();

	public void MakePath(CurveCollectPathController curvePathController,BaseElementPanel elementPanel)
	{
		mCollectBallMovePathList.Clear();
		CollectBallMovePathBase[] collectBallMovePathArray = GetComponents<CollectBallMovePathBase>();
		for (int i = 0; i < collectBallMovePathArray.Length; i++)
		{
			CollectBallMovePathBase collectBallMovePath = collectBallMovePathArray[i];
			mCollectBallMovePathList.Add(collectBallMovePath);
		}

		mCollectBallMovePathList.Sort((CollectBallMovePathBase item1, CollectBallMovePathBase item2 )=>{
			return item1.PathIndex - item2.PathIndex;
		});

		for (int i = 0; i < mCollectBallMovePathList.Count; i++)
		{
			CollectBallMovePathBase collectBallMovePathBase = mCollectBallMovePathList[i];
			collectBallMovePathBase.Init(curvePathController,elementPanel);
		}
		mCurveCollectPathController = curvePathController;
		IsPathReady = true;
	}

	public float DoPath(CollectBall collectBall)
	{
		float retAniDuration = 0;
		Sequence mySequence = DOTween.Sequence();
		for (int i = 0; i < mCollectBallMovePathList.Count; i++)
		{
			CollectBallMovePathBase collectBallMovePath = mCollectBallMovePathList[i];
			if(collectBallMovePath == null) return 0;

			Tweener tweener = collectBallMovePath.GenerateTweener(collectBall);
			if (tweener == null) return 0;

			tweener.OnComplete(() =>{
				collectBallMovePath.OnDoPathEnd(collectBall);
			});

			mySequence.Append(tweener);
			retAniDuration += collectBallMovePath.GetDuration();
		}

		mySequence.OnComplete(()=>{
			mCurveCollectPathController.RecycleCollectBall(collectBall);
		});

		return retAniDuration;
	}
}