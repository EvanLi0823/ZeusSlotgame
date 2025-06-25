using UnityEngine;
using System.Collections;
using System;

namespace Libs
{
    public class DefaultAssetLoad : IAssetLoad
    {
        public IEnumerator LoadRes<T>(OpenConfigParam<T> p, Action<T> sucCB, Action<string> failedCB)
            where T : UIBase
        {
            string resPath = p.defaultResourcePath;
            Type t = typeof(T);
            // if (ResourceCache.Instance.HasResource(t.Name))
            // {
            //     GameObject prefab = ResourceCache.Instance.GetResource<GameObject>(t.Name);
            //     if (prefab != null)
            //     {
            //         UIManager.Instance.CreateGameObjectUI(p, prefab, "", sucCB, failedCB); 
            //         yield break;
            //     }
            // }
            if (UIManager.Instance.DefaultDialogPrefabDictionary.ContainsKey(t.Name))
            {
                GameObject prefab = UIManager.Instance.DefaultDialogPrefabDictionary[t.Name];
                if (prefab != null)
                {
                    UIManager.Instance.CreateGameObjectUI(p, prefab, "", sucCB, failedCB);
                    yield break;
                }
            }

            yield return CoroutineUtil.Instance.StartCoroutine(
                ResourceLoadManager.Instance.AsyncLoadResource<GameObject>(resPath,
                    (slotName, prefab) =>
                    {
                        UIManager.Instance.CreateGameObjectUI(p, prefab, slotName, sucCB, failedCB);
                    }, bundleNmae: p.bundleName));
        }
    }
}