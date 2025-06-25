using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Plugins;
using UnityEngine;

namespace Libs
{
    [System.Serializable]
    public class TaskDictItem
    {
        public int taskId;   // 
        public BaseTask task;
    }
    
    [System.Serializable]
    public class TaskDictWrapper
    {
        public List<TaskDictItem> list = new List<TaskDictItem>();
    }
    
    public class TaskManager:MonoSingleton<TaskManager>
    {
        private Dictionary<int, BaseTask> taskDict = new Dictionary<int, BaseTask>();
        public void OnInit()
        {
            //先加载存储在本地的任务进度数据
            LoadTaskDictPlayerPrefers();
            // //创建在plist节点下配置的任务信息。跳过已存储在本地的
            // CreatePlistTask();
        }
        
        public void SaveTaskDictPlayerPrefers()
        {
            var wrapper = new TaskDictWrapper();
            foreach (var kvp in taskDict)
            {
                wrapper.list.Add(new TaskDictItem(){taskId = kvp.Key, task = kvp.Value});
            }
            string taskStr = Newtonsoft.Json.JsonConvert.SerializeObject(wrapper,new JsonSerializerSettings {
                TypeNameHandling = TypeNameHandling.All});
            Debug.Log($"[TaskManager][SaveTaskDictPlayerPrefers] [taskStr]:{taskStr}");
            PlayerPrefs.SetString(TaskConstants.SaveTaskDict_Key,taskStr);
        }

        private void LoadTaskDictPlayerPrefers()
        {
            string taskStr = PlayerPrefs.GetString(TaskConstants.SaveTaskDict_Key,"");
            if (string.IsNullOrEmpty(taskStr))
            {
                return;
            }

            TaskDictWrapper taskDictWrapper = JsonConvert.DeserializeObject<TaskDictWrapper>(taskStr,new JsonSerializerSettings {
                TypeNameHandling = TypeNameHandling.All});
            if (taskDictWrapper.list == null || taskDictWrapper.list.Count == 0)
            {
                Debug.LogError($"[TaskManager][LoadTaskDictPlayerPrefers] taskDictWrapper.list is empty");
                return;
            }
            //从 json数据中加载存储在本地的任务进度
            foreach (var item in taskDictWrapper.list)
            {
                taskDict.Add(item.taskId,item.task); 
            }
        }

        
        /// <summary>
        /// 外部系统通过此方法获取任务并注册，已缓存的直接获取，未缓存的直接创建
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="dict"></param>
        /// <returns></returns>
        public BaseTask RegisterTask(int taskId,Dictionary<string,object> dict)
        {
            //创建对象，主要是走一遍构造函数
            BaseTask task = TaskFactory.CreateTask(dict);
            BaseTask savedTask = GetTaskById(taskId);
            if (savedTask != null)
            {
                //将已保存任务进度和状态相关信息克隆
                task.Clone(savedTask);
            }
            taskDict[task.TaskId] = task;
            return task;
        }
        
        private void CreatePlistTask()
        {
            Dictionary<string, object> taskInfoDict =
                Configuration.GetInstance().GetValue<Dictionary<string,object>>(TaskConstants.PlistTask_Key,null);
            if (taskInfoDict == null)
            {
                return;
            }
            foreach (var taskItem in taskInfoDict)
            {
                int taskId = Int32.Parse(taskItem.Key);
                if (taskDict.ContainsKey(taskId))
                {
                    continue;
                }
                Dictionary<string, object> taskInfos = taskItem.Value as Dictionary<string, object>;
                if (taskInfos == null || taskInfos.Count == 0)
                {
                    Debug.LogError("[TaskManager][CreatePlistTask] taskInfos is null");
                    continue;
                }

                BaseTask task = TaskFactory.CreateTask(taskInfos);
                taskDict.Add(taskId,task);
            }
        }
        
        public BaseTask GetTaskById(int taskId)
        {
            if (!taskDict.ContainsKey(taskId))
            {
                return null;
            }

            return taskDict[taskId];
        }
    }
}