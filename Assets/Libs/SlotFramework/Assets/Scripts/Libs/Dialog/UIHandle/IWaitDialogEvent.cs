using System;
using Libs;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Libs
{
    public interface IWaitDialogEvent
    {
        bool NeedDo(); //排到自己时，是否还需要执行
        void Do(); // 执行的回调
        float GetDuration(); //执行后的等待的时间
    }

    // 在指定场景中才需要执行的WaitEvent类
    public class WaitDialogSpecifyScene : IWaitDialogEvent
    {
        private string _sceneName;
        private float _duration;
        private Action _doCB;

        public WaitDialogSpecifyScene(string sceneName, float seconds, Action doCB)
        {
            _sceneName = sceneName;
            _duration = seconds;
            _doCB = doCB;
        }

        public bool NeedDo()
        {
            string currentSceneName = SceneManager.GetActiveScene().name;
            if (currentSceneName.Equals(_sceneName))
                return true;
            return false;
        }

        public void Do()
        {
            _doCB?.Invoke();
        }

        public float GetDuration()
        {
            return _duration;
        }
    }


    /// <summary>
    /// 等待某个队列为空时才执行的Event
    /// </summary>
    public class WaitEvent
    {
        private IWaitDialogEvent _waitDialogEvent;
        private int _eid;
        private string _queueID;

        public WaitEvent(string queueID, int eid, IWaitDialogEvent waitDialogEvent)
        {
            _queueID = queueID;
            _eid = eid;
            _waitDialogEvent = waitDialogEvent;
        }

        /// 轮到事件执行时，先回调NeedDo判断是否还需要执行；
        /// 如果需要，给业务层回调doCB，然后等待duration秒后执行下一个事件
        /// 如果不需要，直接执行下一个事件
        public void DoEvent()
        {
            if (_waitDialogEvent == null)
                return;
            bool needDo = false;

            try
            {
                needDo = _waitDialogEvent.NeedDo();
            }
            catch (Exception e)
            {
                BaseGameConsole.ActiveGameConsole().SendCatchExceptionToServer(e);
            }

            if (needDo)
            {
                float duration = 0f;
                try
                {
                    _waitDialogEvent.Do();
                    duration = _waitDialogEvent.GetDuration();
                }
                catch (Exception e)
                {
                    BaseGameConsole.ActiveGameConsole().SendCatchExceptionToServer(e);
                }
                finally
                {
                    new DelayAction(duration, null,
                            () => { UIEventMgr.Instance.RemoveEventAndDoNext(_queueID, _eid); })
                        .Play();
                }
            }
            else
            {
                UIEventMgr.Instance.RemoveEventAndDoNext(_queueID, _eid);
            }
        }
    }
}