using UnityEngine;
using System.Collections;
using Beebyte.Obfuscator;
using DG.Tweening;

namespace Libs
{
    [Skip]
    public class BaseActionNormal : IAction
    {
        public BaseActionNormal()
        {
            this.Init();
        }

        public bool IsPlaying { get; set; }

        public bool Loop { get; set; }

        public bool AutoPlayNextAction { get; set; }

        public bool AutoRun { get; set; }

        public float PlayTime { get; set; }

        public float DelayTime { get; set; }

        public float AutoStartDelayTime { get; set; }

        public ActionCurve PositionSkyAniDuration { get; set; }

        public ActionCallBack DelayCallBack { get; set; }

        public ActionCallBack PlayCallBack { get; set; }

        public BaseActionSequence ParentAction { get; set; }

        public virtual void Init()
        {
            IsPlaying = false;
            AutoPlayNextAction = true;
            PositionSkyAniDuration = ActionCurve.Linear;
            ParentAction = null;
            DelayCallBack = new ActionCallBack();
            DelayCallBack.AddCompleteMethod(() => { PlayLoop(); });
            PlayCallBack = new ActionCallBack();
            PlayCallBack.AddCompleteMethod(() =>
            {
                if (PlayCallBack.OnStepCompleteMethod != null)
                {
                    PlayCallBack.OnStepCompleteMethod();
                }

                if (AutoPlayNextAction)
                {
                    PlayNext();
                }

                if (Loop)
                    Delay();
                else
                    IsPlaying = false;
            });
        }

        public virtual void PlayLoop()
        {
            if (PlayCallBack.OnStartMethod != null)
            {
                PlayCallBack.OnStartMethod();
            }
        }

        public void Play()
        {
            IsPlaying = true;
            if (DelayTime > 0)
            {
                Delay();
            }
            else
            {
                PlayLoop();
            }
        }

        public virtual void Delay()
        {
            DelayTimeAction(DelayTime, DelayCallBack);
        }

        public virtual void PlayNext()
        {
            if (ParentAction != null)
            {
                ParentAction.PlayNext(this);
            }
        }

        public virtual void RemoveFromParent()
        {
            if (ParentAction != null)
            {
                ParentAction.RemoveAction(this);
            }
        }

        private float time;
        private Tweener tw;

        public void DelayTimeAction(float delayTime, ActionCallBack skyAnicallBack)
        {
            tw = null;
            tw = runDelayTime(delayTime, delayTime);
            tw.OnComplete(skyAnicallBack.OnCompleteMethod);
        }

        private Tweener runDelayTime(float endValue, float Duration)
        {
            this.time = 0;
            return DOTween.To(() => this.time, delegate(float x) { this.time = x; }, endValue, Duration).SetTarget(this)
                .SetEase(Ease.Linear).SetUpdate(true);
        }

        public float GetLeftTime()
        {
            return PlayTime - time;
        }

        public virtual void Stop(bool withCallbacks = false)
        {
            if (tw != null)
            {
                tw.Complete(withCallbacks);
            }
        }

        public virtual void Kill()
        {
            if (tw != null)
            {
                tw.Kill();
            }
        }

        public virtual void Pause()
        {
            if (tw != null)
            {
                tw.Pause();
            }
        }

        public virtual void Resume()
        {
            if (tw != null)
            {
                tw.Play();
            }
        }

        public virtual void Restart()
        {
            if (tw != null)
            {
                tw.Restart();
            }
        }
    }
}