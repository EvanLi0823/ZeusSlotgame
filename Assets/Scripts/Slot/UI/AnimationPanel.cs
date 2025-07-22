using System;
using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;

[Serializable]
public class AnimationItem
{
   public string name;
   public GameObject SkeletonPrefab;
}

/// <summary>
/// 动画层，位于最顶层，主要用来播放一些转场的全屏动画
/// </summary>
public class AnimationPanel : MonoBehaviour
{
   //动画的路径
   public List<AnimationItem> Animations = new List<AnimationItem>();

   private SkeletonGraphic skeletonGraphic;
   private string curAnimationName = string.Empty;

   private void OnEnable()
   {
      Messenger.AddListener<string>(GameConstants.PlayFullScreenAnimation, PlayAni);
   }

   private void OnDisable()
   {
      Messenger.RemoveListener<string>(GameConstants.PlayFullScreenAnimation, PlayAni);
   }

   /// <summary>
   /// 暂时只支持 spine动画的播放，暂时没有animationClip 的播放的需求，可以后续扩展
   /// </summary>
   /// <param name="name"></param>
   private void PlayAni(string name)
   {
      //当前有动画在播放
      if (curAnimationName != string.Empty)
      {
         Debug.Log($"AnimationPanel: {curAnimationName} is playing, please wait for it to finish.");
         return;
      }

      AnimationItem animationItem = Animations.Find(item => item.name == name);
      if (animationItem == null)
      {
         Debug.LogError($"AnimationPanel: Animation {name} not found.");
         return;
      }
      curAnimationName = name;
      skeletonGraphic = Instantiate(animationItem.SkeletonPrefab).GetComponent<SkeletonGraphic>();
      skeletonGraphic.gameObject.transform.SetParent(transform, false);
      skeletonGraphic.gameObject.SetActive(true);
      skeletonGraphic.Skeleton.SetToSetupPose();
      skeletonGraphic.AnimationState.Complete += OnComplete;
      skeletonGraphic.AnimationState.SetAnimation(0, "animation", false);
   }
   
   private void OnComplete(Spine.TrackEntry entry)
   {
      if (entry.animation.name != curAnimationName)
      {
         skeletonGraphic.gameObject.SetActive(false);
         Destroy(skeletonGraphic.gameObject);
         skeletonGraphic = null;
         curAnimationName = string.Empty;
      }
   }
}
