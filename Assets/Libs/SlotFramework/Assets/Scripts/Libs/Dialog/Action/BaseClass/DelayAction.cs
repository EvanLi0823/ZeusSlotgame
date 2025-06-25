using UnityEngine;
using System.Collections;
using DG.Tweening;

namespace Libs
{
    public class DelayAction : BaseActionNormal
    {
        public DelayAction()
        {
            this.Init();
        }

        public DelayAction(float delayTime, System.Action startAction, System.Action callback)
        {
            this.Init();
            this.PlayTime = delayTime;
            this.PlayCallBack.AddCompleteMethod(callback);
            this.PlayCallBack.AddStartMethod(startAction);
        }

        public override void PlayLoop()
        {
            base.PlayLoop();
            DelayTimeAction(PlayTime, PlayCallBack);
        }
    }
}