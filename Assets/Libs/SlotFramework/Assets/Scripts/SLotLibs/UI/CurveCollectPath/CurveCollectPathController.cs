using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Classic;
using Libs;

public class CurveCollectPathController : MonoBehaviour
{
	public Transform mEndTransfm;
	int TargetIndex_X, TargetIndex_Y;
	
	public Vector3 offsetStartPos = Vector3.zero;
    public int CollectBallNum = 9;
	public Dictionary<int,CollectPathUnit> mCollectPathDict = new Dictionary<int,CollectPathUnit>();

    void Start()
	{
		InitPath();
		StartCoroutine(GenerateCollectBallCoroutine());
		Messenger.AddListener<ReelManager>(GameConstants.ChangeGameConfigsMsg, InitPath);
    }

	void OnDestroy()
	{
		Messenger.RemoveListener<ReelManager>(GameConstants.ChangeGameConfigsMsg, InitPath);
	}

    public void SetTaskTargetPodIndex_XY(){
        StartCoroutine(SetTaskTargetPodIndex_XYCoroutine());
    }

    IEnumerator SetTaskTargetPodIndex_XYCoroutine(ReelManager reelManager = null)
    {
        while (reelManager == null){
            yield return GameConstants.FiveIn100SecondWait;
            if (BaseSlotMachineController.Instance == null) continue;
            if(BaseSlotMachineController.Instance.reelManager == null) continue;
            reelManager = BaseSlotMachineController.Instance.reelManager;
            TargetIndex_X = reelManager.Reels.Count;
            TargetIndex_Y = 0;
            break;
        }
        yield return null;
    }

    private IEnumerator GenerateCollectBallCoroutine()
    {
        if (CollectBallPrefab == null){
            //Bundle 出问题了
            yield break;
        }

        for (int i = 0; i < CollectBallNum; i++)
        {
            GameObject collectBallObj = Instantiate(CollectBallPrefab);
            if (collectBallObj == null) continue;

            CollectBall collectBall = collectBallObj.GetComponent<CollectBall>();
            if (collectBall == null) yield break;//Bundle 出问题了

            collectBallObj.SetActive(true);
            collectBall.OnCreate();

            Transform collectBallTransfm = collectBall.transform;
			collectBallTransfm.SetParent(CollectBallParent);
            collectBallTransfm.localPosition = Vector3.zero;
            collectBallTransfm.localScale = Vector3.one;
            mCollectObjDataQueue.Enqueue(collectBall);

            yield return GameConstants.FiveIn100SecondWait;
        }
    } 

	Coroutine pathCoroutine;
	protected  virtual void InitPath(ReelManager reelManager = null){
		if (pathCoroutine != null) return;
		pathCoroutine = StartCoroutine(GenerateCollectPathCoroutine(reelManager));
	}

	IEnumerator GenerateCollectPathCoroutine(ReelManager reelManager = null)
    {
		while (reelManager == null){
            yield return GameConstants.FiveIn100SecondWait;
            if (BaseSlotMachineController.Instance == null) continue;
            if(BaseSlotMachineController.Instance.reelManager == null) continue;
            reelManager = BaseSlotMachineController.Instance.reelManager;
            break;
        }

		foreach (int key in mCollectPathDict.Keys)
		{
			CollectPathUnit curvePathUnit = mCollectPathDict[key];
			if (curvePathUnit == null) continue;

			Destroy(curvePathUnit.gameObject);
		}

		mCollectPathDict.Clear();
        for (int i = 0; i < reelManager.GetReelCount(); i++)
        {
            for (int j = 0; j < reelManager.GetReelSymbolRenderCount(i); j++)
            {
               int pathIndex = (i << 10) + j;//生成key的规则

                GameObject curvePathObj = Instantiate(CollectPathPrefab);
                if (curvePathObj == null) continue;

                CollectPathUnit curvePathUnit = curvePathObj.GetComponent<CollectPathUnit>();
                if (curvePathUnit == null) continue;

                curvePathObj.SetActive(true);

                Transform CurvePathTransfm = curvePathObj.transform;
				CurvePathTransfm.SetParent(CollectPathParent);
                CurvePathTransfm.localPosition = Vector3.zero;

                mCollectPathDict[pathIndex] = curvePathUnit;
                yield return GameConstants.FiveIn100SecondWait;
            }
        }
		
		pathCoroutine = null;
    }

    public Vector3 StartPosVec3 = Vector3.zero;

    public void SetTargetPostion(int reelIndex, int positionId)
    {
	    TargetIndex_X = reelIndex;
	    TargetIndex_Y = positionId;
    }
	public virtual float DoCurvePath(BaseElementPanel elementPanel,bool pointerToTarget = false)
	{
		if (mEndTransfm == null) return -1;
		CollectPathUnit curvePathUnit = GetPathByIndex(elementPanel);
		if (curvePathUnit == null) return -1;

		CollectBall collectBall = GetCollectObj();
		if (collectBall == null) return -1;
        //elementPanel.gameObject.transform
        collectBall.Init(elementPanel.transform.position + StartPosVec3);
        if (pointerToTarget)
        {
	        collectBall.LookAtTarget(mEndTransfm);
        }
		return curvePathUnit.DoPath(collectBall);
	}

	#region CollectPath
	public float GetExtraAniDurationBySymbolPos(BaseElementPanel elementPanel)
	{
		return Mathf.Abs(elementPanel.ReelIndex - TargetIndex_X) * 0.1f + Mathf.Abs(elementPanel.PositionId - TargetIndex_Y) * 0.05f;
	}

	public Transform CollectPathParent;
	public GameObject CollectPathPrefab;
	CollectPathUnit GetPathByIndex(BaseElementPanel elementPanel)
	{
		int pathIndex = (elementPanel.ReelIndex << 10) + elementPanel.PositionId;//生成key的规则
		if (!mCollectPathDict.ContainsKey(pathIndex)) return null;

		CollectPathUnit curvePathUnit = mCollectPathDict[pathIndex];
		if (curvePathUnit == null) return null;
		if (!curvePathUnit.IsPathReady) curvePathUnit.MakePath(this, elementPanel);
		return curvePathUnit;
	}
	#endregion

	#region CollectBall

	public Transform CollectBallParent;
	public GameObject CollectBallPrefab;
	private Queue<CollectBall> mCollectObjDataQueue = new Queue<CollectBall>();
	public CollectBall GetCollectObj()
	{
		if (mCollectObjDataQueue.Count > 0) return mCollectObjDataQueue.Dequeue();

		GameObject collectBallObj = Instantiate(CollectBallPrefab);
		if (collectBallObj == null) return null;

		CollectBall collectBall = collectBallObj.GetComponent<CollectBall>();
		if (collectBall == null) return null;//Bundle 出问题了

		collectBallObj.SetActive(true);
		collectBall.OnCreate();

		Transform collectBallTransfm = collectBall.transform;
		collectBallTransfm.parent = CollectBallParent;
		collectBallTransfm.localPosition = Vector3.zero;
		collectBallTransfm.localScale = Vector3.one;
		//mCollectObjDataQueue.Enqueue(collectBall);
		return collectBall;
	}

	public void RecycleCollectBall(CollectBall collectBall)
	{
		collectBall.OnDoPathEnd(()=>{
			mCollectObjDataQueue.Enqueue(collectBall);
		});
	}
	#endregion
}

public enum ECollectPathOffsetDir
{
	E_PathOffset_Up,
	E_PathOffset_Down
}
