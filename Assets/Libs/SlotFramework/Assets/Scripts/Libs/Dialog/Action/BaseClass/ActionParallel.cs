using UnityEngine;
using System.Collections;

namespace Libs
{
    public class ActionParallel : BaseActionSequence
    {
        public override void AppendAction(IAction skyAction)
        {
            AnimationSequence.Add(skyAction);
            setAction(skyAction);
        }

        public override void AddHead(IAction skyAction)
        {
            AnimationSequence.Insert(0, skyAction);
            setAction(skyAction);
        }

        private void setAction(IAction skyAction)
        {
            skyAction.ParentAction = this;
            if (skyAction.PlayTime > PlayTime)
            {
                PlayTime = skyAction.PlayTime;
                if (ParentAction != null)
                    this.ParentAction.ReComputePlaytime();
            }
        }

        public override void RemoveAction(IAction skyAction)
        {
            AnimationSequence.Remove(skyAction);
            skyAction.ParentAction = null;
            if (PlayTime == skyAction.PlayTime)
            {
                ReComputePlaytime();
                if (ParentAction != null)
                    this.ParentAction.ReComputePlaytime();
            }
        }

        public override void ReComputePlaytime()
        {
            PlayTime = 0;
            foreach (IAction skyAction in AnimationSequence)
            {
                if (skyAction.PlayTime > PlayTime)
                    PlayTime = skyAction.PlayTime;
            }
        }

        public override void PlayLoop()
        {
            base.PlayLoop();
            if (AnimationSequence.Count > 0)
            {
                foreach (IAction skyAction in AnimationSequence)
                {
                    skyAction.Play();
                }

                DelayTimeAction(PlayTime, PlayCallBack);
            }
            else
            {
                PlayCallBack.OnCompleteMethod();
            }
        }

        public override void PlayNext(IAction skyAction)
        {
        }
    }
}