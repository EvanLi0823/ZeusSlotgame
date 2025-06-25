using UnityEngine;
using System.Collections;

namespace Libs
{
    public interface IAction
    {
        bool IsPlaying { get; set; }

        bool Loop { get; set; }

        bool AutoRun { get; set; }

        bool AutoPlayNextAction { get; set; }

        float PlayTime { get; set; }

        float DelayTime { get; set; }

        float AutoStartDelayTime { get; set; }

        ActionCurve PositionSkyAniDuration { get; set; }

        ActionCallBack DelayCallBack { get; set; }

        ActionCallBack PlayCallBack { get; set; }

        BaseActionSequence ParentAction { get; set; }

        void Init();

        void PlayLoop();

        void Play();

        void Delay();

        void PlayNext();

        void RemoveFromParent();
    }
}