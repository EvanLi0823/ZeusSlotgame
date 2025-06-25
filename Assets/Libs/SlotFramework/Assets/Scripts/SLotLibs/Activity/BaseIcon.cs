using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ResourceManagement.Util;

namespace Activity
{
    public class BaseIcon:MonoBehaviour
    {
        [HideInInspector]
        public int Priority = 0;
        [HideInInspector]
        public string ResourceName = "";
        [HideInInspector]
        public int PosIndex = -1;
        [HideInInspector]
        //icon绑定的 id
        public int activityId;
        /// <summary>
        /// 初始化方法，实例创建时调用
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="data"></param>
        public virtual void OnInit(int Id,Dictionary<string,object> data)
        {
            //活动类型
            this.activityId = Id;
            if (data == null)
            {
                return;
            }
            Priority = Utils.Utilities.GetInt(data, "priority", 0);
            ResourceName= Utils.Utilities.GetString(data, "resource", "");
        }

        private void OnEnable()
        {
            AddListener();
        }

        public virtual void RefreshProgress(float progress,string info=null)
        {
            
        }

        protected virtual void AddListener()
        {
            
        }
        
        protected virtual void RemoveListener()
        {
            
        }

        private void OnDisable()
        {
            RemoveListener();
        }

        public virtual void OnDestroy()
        {
            Destroy(this.gameObject);
        }
    }
}