using UnityEngine;
using System.Collections;
using DG.Tweening;

namespace Libs
{
    public class ActionCallBack
    {
        public TweenCallback OnCompleteMethod;
        public TweenCallback OnStartMethod;
        public TweenCallback OnStepCompleteMethod;

        public ActionCallBack()
        {
        }

        public void SetCompleteMethod(System.Action a)
        {
            if (a != null)
                OnCompleteMethod = new TweenCallback(a);
        }

        public void SetStartMethod(System.Action a)
        {
            if (a != null)
                OnStartMethod = new TweenCallback(a);
        }

        public void SetStepCompleteMethod(System.Action a)
        {
            if (a != null)
                OnStepCompleteMethod = new TweenCallback(a);
        }

        public void AddCompleteMethod(System.Action a)
        {
            if (a != null)
                OnCompleteMethod += new TweenCallback(a);
        }

        public void AddStartMethod(System.Action a)
        {
            if (a != null)
                OnStartMethod += new TweenCallback(a);
        }

        public void AddStepCompleteMethod(System.Action a)
        {
            if (a != null)
                OnStepCompleteMethod += new TweenCallback(a);
        }
    }
}