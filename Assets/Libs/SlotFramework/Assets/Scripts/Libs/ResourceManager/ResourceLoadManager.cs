using UnityEngine;
using System.Collections;
using System.IO;
using Classic;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Libs
{
	public class ResourceLoadManager
	{
		private ResourceLoadManager(){
			
		}
		public static ResourceLoadManager Instance {
			get {
				return Singleton<ResourceLoadManager>.Instance;	
			}
		}
		private static global::Utils.Logger logger = global::Utils.Logger.GetUnityDebugLogger (typeof(ResourceLoadManager), true);

		public T LoadResource <T> (string path, string specialSceneName = null, bool removeExtension = true) where T: UnityEngine.Object
		{
			string resourceName;
			if (removeExtension) {
				resourceName = Path.GetFileNameWithoutExtension (path);
			} else {
				resourceName = path;
			}
			T currentResult;
			if (string.IsNullOrEmpty (specialSceneName)) {
				string currentSlotName = UnityEngine.SceneManagement.SceneManager.GetActiveScene ().name; //Application.loadedLevelName;
				currentResult = (T)Resources.Load<T> (currentSlotName + "/" + resourceName);
			} else {
				currentResult = (T)Resources.Load<T> (specialSceneName + "/" + resourceName);
			}
			if (currentResult != null)
				return currentResult;
			
			T result = (T)Resources.Load<T> (path);
			if (result != null)
				return result;
			else {
				logger.LogWarning (path + " is null object");
				return null;
			}
		}
		
		//转到addressable系统异步加载资源
		public IEnumerator AsyncLoadResource <T> (string path, System.Action<string,T> completedCallback, System.Action<float> progressCallback = null,string bundleNmae = null,System.Action<string> failedCallback = null) where T: UnityEngine.Object
		{
			string name = Path.GetFileNameWithoutExtension(path);
			string currentSlotName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
			string addressableKey = currentSlotName + "/" + name;
			addressableKey = AddressableManager.Instance.ProcessAddressWithExtension<T>(addressableKey);
			// 1. 检查资源是否存在
			var checkOp = Addressables.LoadResourceLocationsAsync(addressableKey);
			yield return checkOp;
			if (checkOp.Status != AsyncOperationStatus.Succeeded || checkOp.Result.Count == 0)
			{
				Debug.LogWarning($"机器资源不存在: {addressableKey}, 尝试使用原始路径: {path}");
				addressableKey = path;
			}
			Addressables.Release(checkOp);

			// 2. 加载资源
			bool loadingCompleted = false;
			bool loadingSuccess = false;
			T loadedAsset = default;
    
			AddressableManager.Instance.LoadAsset<T>(
				addressableKey,
				(asset) => {
					loadedAsset = asset;
					loadingSuccess = true;
					loadingCompleted = true;
				},
				(error) => {
					Debug.LogError(error);
					loadingCompleted = true;
					failedCallback?.Invoke(error);
				},
				(progress) => {
					progressCallback?.Invoke(progress);
				}
			);
    
			// 等待加载完成
			while (!loadingCompleted)
			{
				yield return null;
			}
    
			// 3. 返回结果
			if (loadingSuccess)
			{
				completedCallback?.Invoke(name, loadedAsset);
			}
		}
	}
}
