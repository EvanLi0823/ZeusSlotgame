using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Libs
{
    public class ActionAnimator
    {
        public static Tweener moveTo(GameObject obj, float time, Vector3 target, bool isLocal,
            ActionCurve vkDurType = ActionCurve.AnimationCurve, ActionCallBack completeObj = null)
        {
            Tweener tw = null;
            if (isLocal)
                tw = obj.transform.DOLocalMove(target, time, false).SetEase((Ease)vkDurType);
            else
                tw = obj.transform.DOMove(target, time, false).SetEase((Ease)vkDurType);
            if (completeObj != null)
            {
                tw.OnComplete(completeObj.OnCompleteMethod);
                tw.OnStart(completeObj.OnStartMethod);
                tw.OnStepComplete(completeObj.OnStepCompleteMethod);
            }

            return tw;
        }

        public static Tweener moveFrom(GameObject obj, float time, Vector3 target, bool isLocal,
            ActionCurve vkDurType = ActionCurve.AnimationCurve, ActionCallBack completeObj = null)
        {
            Tweener tw = null;
            if (isLocal)
                tw = obj.transform.DOLocalMove(target, time, false).SetEase((Ease)vkDurType).From();
            else
                tw = obj.transform.DOMove(target, time, false).SetEase((Ease)vkDurType).From();
            if (completeObj != null)
            {
                tw.OnComplete(completeObj.OnCompleteMethod);
                tw.OnStart(completeObj.OnStartMethod);
                tw.OnStepComplete(completeObj.OnStepCompleteMethod);
            }

            return tw;
        }

        public static Tweener moveBy(GameObject obj, float time, Vector3 offset, bool isLocal,
            ActionCurve vkDurType = ActionCurve.AnimationCurve, ActionCallBack completeObj = null)
        {
            Tweener tw = null;
            if (isLocal)
                tw = obj.transform.DOLocalMove(offset, time, false).SetEase((Ease)vkDurType).SetRelative(true);
            else
                tw = obj.transform.DOMove(offset, time, false).SetEase((Ease)vkDurType).SetRelative(true);
            if (completeObj != null)
            {
                tw.OnComplete(completeObj.OnCompleteMethod);
                tw.OnStart(completeObj.OnStartMethod);
                tw.OnStepComplete(completeObj.OnStepCompleteMethod);
            }

            return tw;
        }

        public static Tweener scaleTo(GameObject obj, float time, Vector3 scale,
            ActionCurve vkDurType = ActionCurve.AnimationCurve, ActionCallBack completeObj = null)
        {
            Tweener tw = null;
            tw = obj.transform.DOScale(scale, time).SetEase((Ease)vkDurType);
            if (completeObj != null)
            {
                tw.OnComplete(completeObj.OnCompleteMethod);
                tw.OnStart(completeObj.OnStartMethod);
                tw.OnStepComplete(completeObj.OnStepCompleteMethod);
            }

            return tw;
        }

        public static Tweener scaleFrom(GameObject obj, float time, Vector3 scale,
            ActionCurve vkDurType = ActionCurve.AnimationCurve, ActionCallBack completeObj = null)
        {
            Tweener tw = null;
            tw = obj.transform.DOScale(scale, time).SetEase((Ease)vkDurType).From();
            if (completeObj != null)
            {
                tw.OnComplete(completeObj.OnCompleteMethod);
                tw.OnStart(completeObj.OnStartMethod);
                tw.OnStepComplete(completeObj.OnStepCompleteMethod);
            }

            return tw;
        }

        public static Tweener scaleBy(GameObject obj, float time, Vector3 offsetScale,
            ActionCurve vkDurType = ActionCurve.AnimationCurve, ActionCallBack completeObj = null)
        {
            Tweener tw = null;
            tw = obj.transform.DOScale(offsetScale, time).SetEase((Ease)vkDurType).From().SetRelative(true);
            if (completeObj != null)
            {
                tw.OnComplete(completeObj.OnCompleteMethod);
                tw.OnStart(completeObj.OnStartMethod);
                tw.OnStepComplete(completeObj.OnStepCompleteMethod);
            }

            return tw;
        }

        public static Tweener colorTo(Image obj, float time, Color target,
            ActionCurve vkDurType = ActionCurve.AnimationCurve, ActionCallBack completeObj = null)
        {
            Tweener tw = null;
            tw = obj.DOColor(target, time).SetEase((Ease)vkDurType);
            if (completeObj != null)
            {
                tw.OnComplete(completeObj.OnCompleteMethod);
                tw.OnStart(completeObj.OnStartMethod);
                tw.OnStepComplete(completeObj.OnStepCompleteMethod);
            }

            return tw;
        }

        public static Sequence moveToSequence(GameObject obj, List<float> times, List<Vector3> targets, bool isLocal,
            ActionCurve vkDurType = ActionCurve.AnimationCurve, ActionCallBack completeObj = null)
        {
            Sequence sequence = DOTween.Sequence();
            if (completeObj != null)
            {
                sequence.OnComplete(completeObj.OnCompleteMethod);
            }

            for (int i = 0; i < times.Count; i++)
            {
                sequence.Append(moveTo(obj, times[i], targets[i], true, vkDurType, null));
            }

            sequence.Play();
            return sequence;
        }

        public static Tweener moveToAlpha(GameObject obj, float time, float target,
            ActionCurve vkDurType = ActionCurve.AnimationCurve, ActionCallBack completeObj = null)
        {
            Tweener tw = null;
//		tw = startSkyAnimoteObj (obj, time, "alpha", target, vkDurType, completeObj);
//		DOTween.to
            return tw;
        }
//
//	public static Tweener colorTo (object obj, float time, Color target, SkyAniDuration vkDurType = SkyAniDuration.AnimationCurve, SkyAniComplete completeObj = null)
//	{
//		Tweener tw = null;
//		tw = startSkyAnimoteObj (obj, time, "color", target, vkDurType, completeObj);
//		return tw;
//	}


        public static DelayAction delayTo(float time, System.Action completeObj = null)
        {
            DelayAction skyDelayAnimation = new DelayAction();
            skyDelayAnimation.PlayTime = time;
            if (completeObj != null)
            {
                skyDelayAnimation.PlayCallBack.AddCompleteMethod(completeObj);
            }

            return skyDelayAnimation;
        }
    }
}