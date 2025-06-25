using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Libs;

public class CollectBall : MonoBehaviour
{
    public Transform  mTransfm;
    public GameObject mParticleObj;
    public GameObject mAnimatorObj;
    public float      RecycleDelay = 0;

    public AudioSource CollisionAudio;

    Animator                   mAnimator;
    List<CollectBallBehaviour> BehaviourList = new List<CollectBallBehaviour> ();

    public void OnCreate (bool isOpenAnimation = true)
    {
        mAnimator         = mAnimatorObj.GetComponent<Animator> ();
        if(mAnimator!=null)mAnimator.enabled = isOpenAnimation;
        BehaviourList.Clear ();
        CollectBallBehaviour[] BehaviourArray = transform.GetComponents<CollectBallBehaviour> ();
        for (int i = 0; i < BehaviourArray.Length; i++)
        {
            BehaviourArray[i].Init ();
            BehaviourList.Add (BehaviourArray[i]);
        }

        BehaviourList.Sort ((CollectBallBehaviour kvp1, CollectBallBehaviour kvp2) =>
        {
            return kvp1.Index - kvp2.Index;
        });
    }

    public void OnDoPathEnd (System.Action callback)
    {
        PlayCollisionAudio ();
        if(mAnimator!=null)mAnimator.enabled = false;
        mAnimatorObj.SetActive (false);
        StopBehaviour ();
        new DelayAction (RecycleDelay, null, () =>
        {
            mParticleObj.SetActive (false);
            mTransfm.localEulerAngles = Vector3.zero;
            if (callback != null) callback ();
        }).Play ();
    }

    public void Init (Vector3 startPos)
    {
        mTransfm.position         = startPos;
        mTransfm.localEulerAngles = Vector3.zero;
        if(mAnimator!=null)mAnimator.enabled         = true;
        mAnimatorObj.SetActive (true);
        mParticleObj.SetActive (true);
    }

    public void PlayCollisionAudio ()
    {
        if (CollisionAudio == null) return;
        CollisionAudio.Play ();
    }

    public void PlayBehaviour ()
    {
        if (BehaviourList == null) return;

        for (int i = 0; i < BehaviourList.Count; i++)
        {
            if (BehaviourList[i] == null) continue;
            BehaviourList[i].DoBehaviour ();
        }
    }

    public void StopBehaviour ()
    {
        if (BehaviourList == null) return;

        for (int i = 0; i < BehaviourList.Count; i++)
        {
            if (BehaviourList[i] == null) continue;
            BehaviourList[i].EndBehaviour ();
        }
    }

    public Ease mEase = Ease.InQuint;

    public void DoMove (Vector3[] path, System.Action callback)
    {
        Tweener tweener = mTransfm.DOPath (path, 2f, PathType.CatmullRom).SetEase (mEase);
        ;
        if (tweener == null) return;
        tweener.OnComplete (() =>
        {
            if (callback != null) callback ();
            Destroy (mTransfm.gameObject);
        });
        tweener.Play ();
    }

    public void DoMove (Vector3[] path, bool destroyOnEnd = true, Action<CollectBall> moveEndCallBack = null)
    {
        Tweener tweener = mTransfm.DOPath (path, 2f, PathType.CatmullRom).SetEase (mEase);
        if (tweener == null) return;
        tweener.OnComplete (() =>
        {
            moveEndCallBack?.Invoke (this);
            if (destroyOnEnd)
                Destroy (mTransfm.gameObject);
        });
        tweener.Play ();
    }
    public void LookAtTarget(Transform target)
    {
        Vector3 dir = target.position - mTransfm.position;
        mTransfm.rotation = Quaternion.FromToRotation(Vector3.up, dir) * mTransfm.rotation;
    }

}