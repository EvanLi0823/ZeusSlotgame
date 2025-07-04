using UnityEngine;
using System.Collections;

namespace Libs
{
    public class BaseActionObject : MonoBehaviour, IAction
    {
        public bool IsPlaying { get; set; }

        public bool Loop
        {
            get { return _loop; }
            set { _loop = value; }
        }

        [SerializeField] private bool
            _loop = false;

        public bool AutoRun
        {
            get { return _AutoRun; }
            set { _AutoRun = value; }
        }

        [SerializeField] private bool
            _AutoRun = false;

        public float PlayTime
        {
            get { return _PlayTime; }
            set { _PlayTime = value; }
        }

        public bool AutoPlayNextAction { get; set; }

        [SerializeField] private float
            _PlayTime = 1;

        public float DelayTime
        {
            get { return _DelayTime; }
            set { _DelayTime = value; }
        }

        [SerializeField] private float
            _DelayTime = 0;

        public float AutoStartDelayTime
        {
            get { return _AutoStartDelayTime; }
            set { _AutoStartDelayTime = value; }
        }

        [SerializeField] private float
            _AutoStartDelayTime = 1;

        public ActionCurve PositionSkyAniDuration
        {
            get { return _PositionSkyAniDuration; }
            set { _PositionSkyAniDuration = value; }
        }

        [SerializeField] private ActionCurve
            _PositionSkyAniDuration = ActionCurve.Linear;

        public ActionCallBack DelayCallBack { get; set; }

        public ActionCallBack PlayCallBack { get; set; }

        public BaseActionSequence ParentAction { get; set; }

        void Start()
        {
            Init();
            if (AutoRun)
            {
                StartCoroutine(delayTimeAction(AutoStartDelayTime, PlayLoop));
            }
        }

        public virtual void Init()
        {
            IsPlaying = false;
            ParentAction = null;
            DelayCallBack = new ActionCallBack();
            DelayCallBack.SetCompleteMethod(() => { PlayLoop(); });
            PlayCallBack = new ActionCallBack();
            PlayCallBack.SetCompleteMethod(() =>
            {
                if (PlayCallBack.OnStepCompleteMethod != null)
                {
                    PlayCallBack.OnStepCompleteMethod();
                }

                PlayNext();
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
            if (DelayCallBack.OnStartMethod != null)
            {
                DelayCallBack.OnStartMethod();
            }

            if (DelayTime > 0)
            {
                StartCoroutine(delayTimeAction(DelayTime, () => { DelayCallBack.OnCompleteMethod(); }));
            }
            else
            {
                DelayCallBack.OnCompleteMethod();
            }
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

        protected IEnumerator delayTimeAction(float delayTime, System.Action a)
        {
            yield return new WaitForSeconds(delayTime);
            a();
        }
    }
}