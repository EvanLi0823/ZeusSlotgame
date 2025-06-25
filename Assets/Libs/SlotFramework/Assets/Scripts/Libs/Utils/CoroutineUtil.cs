using UnityEngine;
using System.Collections;

namespace Libs
{
public class CoroutineUtil : MonoBehaviour
{
	static private CoroutineUtil _instance = null;

	static public CoroutineUtil Instance {
		get {
			if (_instance == null) {
				_instance = new GameObject("YieldUtil", typeof(CoroutineUtil)).GetComponent<CoroutineUtil>();
				_instance.gameObject.hideFlags = HideFlags.HideAndDontSave;
				DontDestroyOnLoad(_instance.gameObject);
			}
			return _instance;
		}
	}
		public static IEnumerator DelayAction (float dTime, System.Action callback)
		{
			yield return new WaitForSeconds (dTime);
			callback ();
		}

        public static IEnumerator DelayRealTimeAction(float dTime, System.Action callback)
        {
            yield return new WaitForSecondsRealtime(dTime);
            callback();
        }
        public static void  DoDelayAction (float dTime, System.Action callback)
        {
	        Instance.StartCoroutine(DelayAction(dTime,callback));
        }
        public static void DoDelayRealTimeAction(float dTime, System.Action callback)
        {
            Instance.StartCoroutine(DelayRealTimeAction(dTime, callback));
        }
        
        public static void DoDelayByContext(MonoBehaviour context,float dTime, System.Action callback)
        {
	        context.StartCoroutine(DelayAction(dTime, callback));
        }
        
        public static void DoRealDelayByContext(MonoBehaviour context,float dTime, System.Action callback)
        {
	        if (context == null)
	        {
		        Debug.LogException(new System.Exception("DoRealDelayByContext context is null"));
		        return;
	        }
	        context.StartCoroutine(DelayRealTimeAction(dTime, callback));
        }
    }
}
