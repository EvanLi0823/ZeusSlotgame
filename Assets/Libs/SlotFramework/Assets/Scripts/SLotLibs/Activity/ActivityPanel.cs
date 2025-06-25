using System;
using System.Collections;
using System.Collections.Generic;
using Libs;
using UnityEditor;
using UnityEngine;
using Utils;

namespace Activity
{
    /// <summary>
    /// 创建活动角标，提供Activity 到 icon的映射
    /// </summary>
    public class ActivityPanel:MonoBehaviour
    {
        //icon列表，通过 string 来跟活动绑定
        private Dictionary<int, BaseIcon> iconDict = new Dictionary<int, BaseIcon>();
        private Coroutine loadCor;
        public void Awake()
        {
            Messenger.AddListener<int>(ActivityConstants.RemoveIconMsg,RemoveIcon);
            loadCor = CoroutineUtil.Instance.StartCoroutine(LoadActivityIcon());
        }

        void RemoveIcon(int activityId)
        {
            if (iconDict.ContainsKey(activityId))
            {
                BaseIcon icon = iconDict[activityId];
                iconDict.Remove(activityId);
                icon.OnDestroy();
            }
        }
        private void OnDisable()
        {
            Messenger.RemoveListener<int>(ActivityConstants.RemoveIconMsg,RemoveIcon);
        }

        IEnumerator LoadActivityIcon()
        {
            //等待一秒
            yield return GameConstants.OneSecondWait;
            Dictionary<int,BaseActivity> activities = ActivityManager.Instance.GetActivities();
            foreach (var item in activities)
            {
                BaseActivity activity = item.Value;
                string prefabName = activity.GetIconResourceName();
                if (string.IsNullOrEmpty(prefabName))
                {
                    continue;
                }
                string path = prefabName+".prefab";
                AddressableManager.Instance.LoadAsset<GameObject>(path, (asset) =>
                {
                    if (asset != null)
                    {
                        GameObject go= Instantiate(asset,gameObject.transform, false);
                        go.transform.SetAsLastSibling();
                        BaseIcon icon= activity.RegisterIcon(go);
                        iconDict.Add(activity.id,icon);
                        Debug.Log("[ActivityPanel][LoadActivityIcon] load icon success: " + activity.id + " " + path);
                    }
                    else
                    {
                        Debug.LogError("[ActivityPanel][LoadActivityIcon] load icon failed: " + activity.id + " " + path);
                    }
                });
                //等待一帧
                yield return GameConstants.FrameTime;
            }
        }
    }
}