using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Classic;
using UnityEngine.SceneManagement;
using System;
public class CollectPoint : MonoBehaviour
{
    // Start is called before the first frame update
    public CurveCollectPathController mCurvePathController;
    public float delayCollectTime;//选出最大延迟时间
    [Header("收集目标点在棋盘Symbol位置的X坐标(从1开始)")]
    [SerializeField] 
    private int  boardX;
    [Header("收集目标点在棋盘Symbol位置的Y坐标(从1开始)")]
    [SerializeField] 
    private int boardY;
    public  string collectElementKey{
        get{
            return SceneManager.GetActiveScene ().name + "collect";
        }
    }
    void Awake()
    {
        Messenger.AddListener<BaseElementPanel,bool>(collectElementKey, OnPlayCollectPathMsg);
    }

    void OnDestroy()
    {
        Messenger.RemoveListener<BaseElementPanel,bool>(collectElementKey, OnPlayCollectPathMsg);
    }
    
    protected virtual void OnPlayCollectPathMsg(BaseElementPanel elementPanel,bool lookAtTarget)
    {
        if (mCurvePathController == null) return;
        mCurvePathController.SetTargetPostion(boardX, boardY);
        float aniDuration = mCurvePathController.DoCurvePath(elementPanel,lookAtTarget);
        if (aniDuration < 0) return;
        if (delayCollectTime > float.Epsilon && aniDuration > delayCollectTime) return;

        delayCollectTime = aniDuration;
    }
}
