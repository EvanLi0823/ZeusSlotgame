using System.Collections;
using UnityEngine;
using System;

public class AnimationCurveWheelController : MonoBehaviour
{
        [Header("Curve的长度必须为整数倍，且必须包含匀速阶段")]
        public AnimationCurve animationCurve;

        private AnimationCurve finalAnimationCurve;
        [Header("Curve曲线匀速阶段的起始帧索引")]
        public int startFrameIndex = 2;
        [Header("Curve曲线匀速阶段的末尾帧索引")]
        public int endFrameIndex = 3;
        //[Header("Curve曲线的运行时长，必须为正整数")]
        //private float runTime = 10f;//实际运行时间不能低于5s
        private bool spinning;
        private float anglePerItem;
        private int awardItemIndex=0;
        private int preAwardItemIndex = 0;
        private int wheelItemTotalCount = 1;
        public const string SPIN_FINISHED = "SpinFinished";
        private Action<AnimationCurveWheelController> finishCallback = null;
        private const float ROUND_ANGLE = 360;
        private float offsetAngle = 0;
        private  bool EnableEnd = false;
        private bool counterClockwise = false;
        private float preAudioTriggerAngle = 0;
        private Action audioSpinStart;
        private Action audioSpinMiddle;
        private Action audioSpinLast;
        private Action audioSpinCB;
        //private bool isPlayNearSound;
        public void Reset()
        {
            spinning = false;
            awardItemIndex = 0;
            preAwardItemIndex = 0;
            wheelItemTotalCount = 1;
            transform.localEulerAngles = Vector3.zero;
            finishCallback = null;
            offsetAngle = 0;
            EnableEnd = false;
        }

    private void RegisterEvent(Action _audioSpinStart,Action _audioSpinMiddle,Action _audioSpinLast)
    {
        audioSpinStart = _audioSpinStart;
        audioSpinMiddle = _audioSpinMiddle;
        audioSpinLast = _audioSpinLast;

    }
   
    public void SetWheelData(int totalItemCount,bool Reset = false,bool CCW=false,Action _audioSpinStart=null,Action _audioSpinMiddle=null,Action _audioSpinLast=null)
        {
            anglePerItem = 360f / totalItemCount;
            wheelItemTotalCount = totalItemCount;
            //this.runTime = runTime;
            counterClockwise = CCW;
            if (Reset)
            {
                preAwardItemIndex = 0;
                transform.localEulerAngles = Vector3.zero;
            }

            preAudioTriggerAngle = anglePerItem / 2;
           
            RegisterEvent(_audioSpinStart,_audioSpinMiddle,_audioSpinLast);
        }

        public bool SetReadyState(int targetIndex,Action<AnimationCurveWheelController> finishCB)
        {
            int offsetItem = 0;
            awardItemIndex = targetIndex;
            finishCallback = finishCB;
            if (awardItemIndex>=preAwardItemIndex)
            {
                offsetItem = awardItemIndex - preAwardItemIndex;
            }
            else
            {
                offsetItem = awardItemIndex + wheelItemTotalCount - preAwardItemIndex;
            }
            offsetAngle = (counterClockwise ? -1 : 1)*offsetItem * anglePerItem;
            return CreateFinalAnimationCurve();
        }


        private bool CreateFinalAnimationCurve()
        {
            if (animationCurve == null)
            {
                 return false;
            }

            if (animationCurve.length == 0 || animationCurve.length + 2 <= startFrameIndex ||
                animationCurve.length + 1 <= endFrameIndex)
            {
                return false;
            }
            finalAnimationCurve = new AnimationCurve();
            //1.起始段和固定的原有匀速段
            for (int i = 0; i <= startFrameIndex; i++)
            {
                finalAnimationCurve.AddKey(animationCurve.keys[i]);
            }
            //2.添加匀速段，来保证转动到指定的中奖位置
            Keyframe startKey = animationCurve.keys[startFrameIndex];
            Keyframe endKey = animationCurve.keys[endFrameIndex];
            float k = (endKey.value - startKey.value) / (endKey.time - startKey.time);
            float offsetValue = Mathf.Abs(offsetAngle) / ROUND_ANGLE;
            float offsetTime = offsetValue / k;
            for (int i = endFrameIndex; i < animationCurve.keys.Length; i++)
            {
                Keyframe kf = animationCurve[i];
                kf.time += offsetTime;
                kf.value += offsetValue;
                finalAnimationCurve.AddKey(kf);
            }
            return true;
        }


        public void StartWheel()
        {
            if (spinning) return;

            StartCoroutine(SpinWheel());
        }


        IEnumerator SpinWheel()
        {
            spinning = true;
            //isPlayNearSound = true;
//            bool finished = false;
            float timer = 0.0f;
            float startAngle = transform.eulerAngles.z;
            float lastFrameTime = finalAnimationCurve[finalAnimationCurve.length - 1].time;
            float middleTime = finalAnimationCurve[startFrameIndex].time;
            float lastTime = finalAnimationCurve[endFrameIndex].time;
            bool isPlayFirstAudio = true;
            bool isPlayMiddleAudio = true;
            bool isPlayLastAudio = true;
//            float offsetValue = 0;
            int clockwise = counterClockwise ? -1 : 1;
            while (timer<lastFrameTime)
            {
                float angle = clockwise* ROUND_ANGLE * finalAnimationCurve.Evaluate(timer);
                if (Mathf.Abs(angle) >= preAudioTriggerAngle)
                {
                    //if (audioSpinCB != null) audioSpinCB();
                    preAudioTriggerAngle += anglePerItem;
                }
                transform.eulerAngles = new Vector3(0.0f, 0.0f, angle + startAngle);
                if (isPlayFirstAudio && audioSpinStart!=null)
                {
                    audioSpinStart();
                    isPlayFirstAudio = false;
                }
                if (isPlayMiddleAudio && audioSpinMiddle!=null && timer>=middleTime)
                {
                    audioSpinMiddle();
                    isPlayMiddleAudio = false;
                }
                if (isPlayLastAudio && audioSpinLast!=null && timer>=lastTime)
                {
                    audioSpinLast();
                    isPlayLastAudio = false;
                }
                timer += Time.unscaledDeltaTime;
                yield return 0;
            }
            //AudioEntity.Instance.StopEffectAudio("wheel_near_stop");
            float maxAngle = clockwise*ROUND_ANGLE *finalAnimationCurve[finalAnimationCurve.keys.Length - 1].value;
            transform.eulerAngles = new Vector3(0.0f, 0.0f, maxAngle + startAngle);
            spinning = false;
            preAwardItemIndex = awardItemIndex;
            if (finishCallback!=null) finishCallback(this);
        }
}
