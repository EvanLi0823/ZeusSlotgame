using System;
using System.Collections.Generic;
using Libs;
using UnityEngine;

namespace Activity
{
    public class SpinWithDrawActivity:BaseActivity
    {
        public bool NotFirstLogin
        {
            get
            {
                if (_notfirstTimeLogin == -1)
                {
                    _notfirstTimeLogin = PlayerPrefs.GetInt("_notfirstLogin", 0);
                }
				
                return _notfirstTimeLogin==1;
            }
            set
            {
                _notfirstTimeLogin = value ? 1 : 0;
                PlayerPrefs.SetInt("_notfirstLogin", _notfirstTimeLogin);
                PlayerPrefs.Save();
            }
        }
		
        private int _notfirstTimeLogin = -1;
        private Dictionary<string, object> plistData = new Dictionary<string, object>();
        
        public SpinWithDrawActivity(Dictionary<string, object> data) : base(data)
        {
            ParseTaskData();
        }

        private bool showEndDialog = false;
        protected override void ParseTaskData()
        {
            Dictionary<string, object> taskData = Utils.Utilities.GetValue<Dictionary<string,object>>(Data, ActivityConstants.TASKS, null);
            if (taskData==null || taskData.Count==0)
            {
                return;
            }
            int taskId =  Utils.Utilities.GetInt(taskData, TaskConstants.TaskId_Key, 0);
            Task = TaskManager.Instance.RegisterTask(taskId,taskData);
        }

        //广播刷新icon
        public override BaseIcon RegisterIcon(GameObject go)
        {
            icon = go.AddComponent<SpinWithDrawItem>();
            icon.OnInit(id,iconData);
            return icon;
        }
        
        private void ShowStartDialog(Action callback = null)
        {
            int spinCount = (int)(Task as SpinAwardCashTask).TargetNum;
            int cash = (Task as SpinAwardCashTask).cashTargetNum;
            Messenger.Broadcast<int,int,Action>(GameDialogManager.OpenSpinWithDrawStartDialogMsg,spinCount,cash,callback);
        }
        
        private void ShowEndDialog()
        {
            if (showEndDialog)
            {
                return;
            }
            showEndDialog = true;
            int cash = (Task as SpinAwardCashTask).cashTargetNum;
            Messenger.Broadcast<int, Action>(GameDialogManager.OpenSpinWithDrawEndDialogMsg,cash,ShowWithDrawDialog);
        }

        public override bool IsOpen()
        {
            return isOpen && Task.State == (int)TaskState.ONGOING;
        }
        
        private void ShowWithDrawDialog()
        {
            Messenger.Broadcast(GameDialogManager.OpenWithDrawDialog);
        }
        public override void AddListener()
        {
            Messenger.AddListener(GameConstants.OnSceneInit,OnSceneInit);
            Messenger.AddListener(Task.UpdateTaskDataMsg,UpdateProgress);
        }

        public override void RemoveListener()
        {
            Messenger.RemoveListener(GameConstants.OnSceneInit,OnSceneInit);
            Messenger.RemoveListener(Task.UpdateTaskDataMsg,UpdateProgress);
        }
        
        //进入了机器场景，此活动需要进入机器时检测是否第一次登录，若是第一次登录，需要弹出主界面
        private void OnSceneInit()
        {
            if (!NotFirstLogin)
            {
                Action action = ()=>
                {
                    //展示引导弹窗
                    // Messenger.Broadcast(GameDialogManager.OpenNewUserGuidDialogMsg);
                    Messenger.Broadcast<bool>(GameConstants.NEW_USER_GUIDE, true);
                };
                NotFirstLogin = true;
                ShowStartDialog(action);
            }
        }

        public void UpdateProgress()
        {
            if (icon!=null)
            {
                icon.RefreshProgress(GetProgress());
            }
            if (Task.IsTaskConditionOK && Task.State==(int)TaskState.ONGOING)
            {
                // ShowEndDialog();
                Task.State = (int)TaskState.CLOSE;
                //移除活动
                ActivityManager.Instance.RemoveActivity(id);
            }
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            ActivityManager.Instance.RemoveIcon(id);
        }
        
        public override float GetProgress()
        {
            SpinAwardCashTask task = Task as SpinAwardCashTask;
            if (task==null)
            {
                Debug.LogError("[SpinWithDrawActivity][GetProgress] task == null");
                return 0;
            }
            float f = task.cashCollectNum * 1.0f / task.cashTargetNum;
            return f;
        }
        
        public override void OnClickIcon()
        {
            ShowWithDrawDialog();
        }
    }
}