using System;

namespace Libs
{
    using UnityEngine;
    using System.Collections;

    public class Loading : MonoBehaviour
    {
        public static Loading GetRoot()
        {
            return GameObject.FindObjectOfType(typeof(Loading)) as Loading;
        }
        public static Loading instance;

        public UIText UiText;
        private Action<int> removeCB;
        private int EID;
        void Awake () 
        {
            if (!instance) {
                instance = this;
                DontDestroyOnLoad (gameObject);
            }
            else {
                DestroyObject(gameObject);
            }
        }
        

        float startShowTime = 0f;
        float endShowTime = 0f;
        bool finished = false;
        private Action hideCb;
        private Action endCB;
        private int LockID;
        public void Init(float duration,string title = "",int lockId = 0,Action endCB = null,Action hideCB=null)
        {
            if(UiText!=null) UiText.SetText(title);
            LockID = lockId;
            this.endCB = endCB;
            SetShowDuration(duration,hideCB);
        }

        public bool CheckHideLoadingUI(int lockId)
        {
            return this.LockID == lockId;
        }

        public void SetEventUICB(Action<int> removeCB,int eID)
        {
            this.removeCB = removeCB;
            this.EID = eID;
        }
        private void SetShowDuration(float duration,Action hideCB)
        {
            finished = false;
            startShowTime = Time.timeSinceLevelLoad;
            endShowTime = startShowTime + duration;
            this.hideCb = hideCB;
        }
        
        /// <summary>
        /// 控制Loading延迟退出，不使用协程，协程稳定性较差，受系统逻辑影响较为严重
        /// </summary>
        void Update()
        {
            if (!finished)
            {
                if (endShowTime > startShowTime)
                {
                    startShowTime += Time.deltaTime;
                }

                if (startShowTime >= endShowTime)
                {
                    DoHideEvent();
                }
            }
        }

        public void DoHideEvent()
        {
            if (finished) return;
            finished = true;
            if (endCB != null) endCB();
            if (removeCB != null) removeCB(EID);
            if (hideCb != null) hideCb();
            endCB = null;
            removeCB = null;
            hideCb = null;
        }
    }
}