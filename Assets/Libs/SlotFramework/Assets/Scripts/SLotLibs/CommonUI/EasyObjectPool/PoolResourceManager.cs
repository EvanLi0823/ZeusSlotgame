using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace MarchingBytes {
    [DisallowMultipleComponent]
    [AddComponentMenu("")]
    public class PoolResourceManager : MonoBehaviour {
        //obj pool
        private Dictionary<string, Pool> poolDict = new Dictionary<string, Pool>();

        private static PoolResourceManager mInstance = null;

        public static PoolResourceManager Instance {
            get {
                if (mInstance == null) {
                    GameObject GO = new GameObject("PoolResourceManager", typeof(PoolResourceManager));
                    // Kanglai: if we have `GO.hideFlags |= HideFlags.DontSave;`, we will encounter Destroy problem when exit playing
                    // However we should keep using this in Play mode only!
                    mInstance = GO.GetComponent<PoolResourceManager>();
                    if (Application.isPlaying) {
                        DontDestroyOnLoad(mInstance.gameObject);
                    } else {
                        Debug.LogWarning("[PoolResourceManager] You'd better ignore PoolResourceManager in Editor mode");
                    }
                }

                return mInstance;
            }
        }
        public void InitPool(string poolName,GameObject go,int size, PoolInflationType type = PoolInflationType.DOUBLE) {
            if (poolDict.ContainsKey(poolName)) {
                return;
            } else {
                GameObject pb = Instantiate(go);
                if (pb == null) {
                    Debug.LogError("[PoolResourceManager] Invalide prefab name for pooling :" + poolName);
                    return;
                }
                poolDict[poolName] = new Pool(poolName, pb, Instance.gameObject, size, type);
            }
        }

        /// <summary>
        /// Returns an available object from the pool 
        /// OR null in case the pool does not have any object available & can grow size is false.
        /// </summary>
        /// <param name="poolName"></param>
        /// <returns></returns>
        public GameObject GetObjectFromPool(string poolName, bool autoActive = true, int autoCreate = 0) {
            GameObject result = null;

            if (!poolDict.ContainsKey(poolName) && autoCreate > 0) {
                // InitPool(poolName, autoCreate, PoolInflationType.INCREMENT);
            }

            if (poolDict.ContainsKey(poolName)) {
                Pool pool = poolDict[poolName];
                result = pool.NextAvailableObject(autoActive);
                //scenario when no available object is found in pool
#if UNITY_EDITOR
                if (result == null) {
                    Debug.LogWarning("[PoolResourceManager]:No object available in " + poolName);
                }
#endif
            }
#if UNITY_EDITOR
        else {
                Debug.LogError("[PoolResourceManager]:Invalid pool name specified: " + poolName);
            }
#endif
            return result;
        }

        /// <summary>
        /// Return obj to the pool
        /// </summary>
        /// <param name="go"></param>
        public void ReturnObjectToPool(GameObject go) {
            PoolObject po = go.GetComponent<PoolObject>();
            if (po == null) {
#if UNITY_EDITOR
                Debug.LogWarning("Specified object is not a pooled instance: " + go.name);
#endif
            } else {
                Pool pool = null;
                if (poolDict.TryGetValue(po.poolName, out pool)) {
                    pool.ReturnObjectToPool(po);
                }
#if UNITY_EDITOR
            else {
                    Debug.LogWarning("No pool available with name: " + po.poolName);
                }
#endif
            }
        }

        /// <summary>
        /// Return obj to the pool
        /// </summary>
        /// <param name="t"></param>
        public void ReturnTransformToPool(Transform t) {
            if (t == null) {
#if UNITY_EDITOR
                Debug.LogError("[PoolResourceManager] try to return a null transform to pool!");
#endif
                return;
            }
            //set gameobject active flase to avoid a onEnable call when set parent
            t.gameObject.SetActive(false);
            t.SetParent(null, false);
            ReturnObjectToPool(t.gameObject);
        }
    }
}