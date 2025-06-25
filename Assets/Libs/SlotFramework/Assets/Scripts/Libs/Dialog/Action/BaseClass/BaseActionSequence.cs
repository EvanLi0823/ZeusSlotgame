using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Libs
{
    public class BaseActionSequence : BaseActionNormal
    {
        public List<IAction> AnimationSequence;

        public override void Init()
        {
            base.Init();
            AnimationSequence = new List<IAction>();
            PlayTime = 0;
        }

        public BaseActionSequence()
        {
            this.Init();
        }

        public virtual void AppendAction(IAction skyAction)
        {
        }

        public virtual void RemoveAction(IAction skyAction)
        {
        }

        public void RemoveAll()
        {
            foreach (IAction skyAction in AnimationSequence)
            {
                skyAction.ParentAction = null;
            }

            AnimationSequence.Clear();
            PlayTime = 0;
            if (this.ParentAction != null)
                this.ParentAction.ReComputePlaytime();
        }

        public virtual void PlayNext(IAction skyAction)
        {
        }

        public virtual void ReComputePlaytime()
        {
        }

        public virtual void AddHead(IAction skyAction)
        {
        }
    }
}