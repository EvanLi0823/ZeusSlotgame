using Spine;
using System;
using UnityEngine;
using Spine.Unity;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class SpineAnimation : MonoBehaviour
{
    public SkeletonGraphic Skeleton;
    public SpineGraphic SpineGraphic;
    public Mask SpineMask;
    
    private Spine.AnimationState animationState;
    private Spine.TrackEntry trackEntry;
    private System.Action InitHandler;
    private System.Action CompleteHandler;
    private bool isLoop = false;
    private bool callBackOnEachComplete = false;
    private int loopCount = 0;
    private int completeNum = 0;
    private float loopStartTime;
    private float scale = 0.01f;
    
    public void Play(SkeletonDataAsset skeletonDataAsset, string animationName, bool _isLoop = true,
        float RepeatPlayStartTime = 0f, int _loopCount = 0, Action _InitHandler = null,
        Action _CompleteHandler = null, bool _useSpineSelfClip = false, string skin = "default",bool _callBackOnEachComplete = false,bool isParticalLineType=false)
    {
        InitHandler = _InitHandler;
        CompleteHandler = _CompleteHandler;
        callBackOnEachComplete = _callBackOnEachComplete;
        isLoop = _isLoop;
        loopStartTime = RepeatPlayStartTime;
        loopCount = _loopCount;
        completeNum = 0;

        if(Skeleton != null)
        {
            
            if (isParticalLineType && Skeleton.AnimationState != null)
            {
                if (Skeleton.AnimationState.ToString() == animationName && Skeleton.enabled && isLoop)
                    return;
            }
            Skeleton.enabled = true;
            Skeleton.skeletonDataAsset = skeletonDataAsset;
            Skeleton.initialSkinName = skin;
            Skeleton.startingAnimation = animationName;
            Skeleton.Initialize(true, _useSpineSelfClip);
        }
        
        if(Skeleton.AnimationState != null)
        {
            animationState = Skeleton.AnimationState;
            animationState.Start += OnStart;
            animationState.Complete += OnComplete;
            trackEntry = animationState.SetAnimation(0, animationName, isLoop);
        }

        if (!_useSpineSelfClip)
        {
            if (Skeleton.Skeleton != null && SpineGraphic !=null &&SpineMask !=null)
            {
                foreach (var item in Skeleton.Skeleton.Slots)
                {
                    ClippingAttachment clippingAttachment = item.attachment as ClippingAttachment;
                    if(clippingAttachment != null && clippingAttachment.vertices !=null &&  clippingAttachment.vertices.Length != 0 && SpineGraphic != null)
                    {
                        SpineGraphic.vertices.Clear();
                        foreach (var vertices in clippingAttachment.vertices)
                        {
                            SpineGraphic.vertices.Add(vertices/scale);
                            SpineGraphic.enabled = true;
                            SpineMask.enabled = true;
                        }
                        break;
                    }
                }
            }
        }
    }
    
    public void Pause()
    {
        
    }

    public void Stop()
    {
        if(Skeleton != null)
        {
            Skeleton.Clear();
            Skeleton.enabled = false;
        }
        
        if (SpineGraphic != null && SpineMask != null)
        {
            if (SpineGraphic.IsActive()) SpineGraphic.enabled = false;
            if (SpineMask.IsActive()) SpineMask.enabled = false;
        }
    }
    
    public void OnStart(Spine.TrackEntry entry)
    {
        if(this.InitHandler != null)
        {
            this.InitHandler();
            this.InitHandler = null;
        }
    }

    public void OnComplete(Spine.TrackEntry entry)
    {
        completeNum++;

        if (loopStartTime != 0 && trackEntry != null)
        {
            trackEntry.animationStart = loopStartTime / 30f;
            trackEntry.animationLast = loopStartTime / 30f;
        }

        if (this.CompleteHandler != null)
        {
            if (!isLoop)
            {
                this.CompleteHandler();
                this.CompleteHandler = null;
            }
            else if (loopCount != 0 && completeNum == loopCount)
            {
                this.CompleteHandler();
                this.CompleteHandler = null;
            }
            else if (callBackOnEachComplete)
            {
                this.CompleteHandler();
            }
        }
    }
}
