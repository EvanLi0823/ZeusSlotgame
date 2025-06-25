using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

namespace Libs
{
	public class EventSystemUtils
	{
		static EventSystem eventSystem;

		public static bool IsOpen = true;

		public static void EnableEventSystem ()
		{
			if (eventSystem == null) {
				if (GameObject.FindObjectOfType<EventSystem> () != null) {
				
					eventSystem = GameObject.FindObjectOfType<EventSystem> ().GetComponent<EventSystem> ();
				}
			
			}	
			if (eventSystem != null && !eventSystem.enabled) {
				eventSystem.enabled = true;
			}
		}

		public static void DisableEventSystem ()
		{
			if (IsOpen) {
				eventSystem = EventSystem.current;
				if (eventSystem != null) {
					eventSystem.enabled = false;
				}
			}
		}

		public static IEnumerator AutoEventSystem ()
		{
			if (IsOpen) {
				Libs.EventSystemUtils.DisableEventSystem ();
                //				yield return new WaitForSeconds (0.5f);
                yield return new WaitForSecondsRealtime(0.5f);
				Libs.EventSystemUtils.EnableEventSystem ();
			}
		}
	}
}
