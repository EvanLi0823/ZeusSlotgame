using System.Collections.Generic;

namespace Activity
{
    public interface IActivity
    {
        /// <summary>
        /// 初始化
        /// </summary>
        void OnInit();
        
        /// <summary>
        /// 销毁
        /// </summary>
        void OnDestroy();
        
        /// <summary>
        /// 刷新
        /// </summary>
        void OnRefresh();
    }
}