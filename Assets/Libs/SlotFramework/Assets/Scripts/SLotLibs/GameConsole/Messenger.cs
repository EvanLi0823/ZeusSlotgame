﻿/*
 * Advanced C# messenger by Ilya Suzdalnitski. V1.0
 * 
 * Based on Rod Hyde's "CSharpMessenger" and Magnus Wolffelt's "CSharpMessenger Extended".
 * 
 * Features:
 	* Prevents a MissingReferenceException because of a reference to a destroyed message handler.
 	* Option to log all messages
 	* Extensive error detection, preventing silent bugs
 * 
 * Usage examples:
 	1. Messenger.AddListener<GameObject>("prop collected", PropCollected);
 	   Messenger.Broadcast<GameObject>("prop collected", prop);
 	2. Messenger.AddListener<float>("speed changed", SpeedChanged);
 	   Messenger.Broadcast<float>("speed changed", 0.5f);
 * 
 * Messenger cleans up its evenTable automatically upon loading of a new level.
 * 
 * Don't forget that the messages that should survive the cleanup, should be marked with Messenger.MarkAsPermanent(string)
 * 
 * Based on: http://wiki.unity3d.com/index.php/Advanced_CSharp_Messenger
 */

//#define MESSENGER_LOG_ALL_MESSAGES
//#define MESSENGER_LOG_ADD_LISTENER
//#define MESSENGER_LOG_BROADCAST_MESSAGE
//#define MESSENGER_REQUIRE_LISTENER
//#define MESSENGER_THROW_EXCEPTIONS

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;




internal static class Messenger
{
	#region Internal variables

	static public Dictionary<string, Delegate> eventTable = new Dictionary<string, Delegate> ();

	public static void OnDispose()
	{
	}

	//Message handlers that should never be removed, regardless of calling Cleanup
	static public List<string> permanentMessages = new List<string> ();

	#endregion

	#region Helper methods

	//Marks a certain message as permanent.
	static public void MarkAsPermanent (string eventType)
	{
		#if MESSENGER_LOG_ALL_MESSAGES
		Debug.Log("Messenger MarkAsPermanent \t\"" + eventType + "\"");
		#endif

		if (!permanentMessages.Contains (eventType)) {
			permanentMessages.Add (eventType);
		}
	}


	static public void Cleanup ()
	{
		#if MESSENGER_LOG_ALL_MESSAGES
		Debug.Log("MESSENGER Cleanup. Make sure that none of necessary listeners are removed.");
		#endif

		List<string> messagesToRemove = new List<string> ();

		foreach (KeyValuePair<string, Delegate> pair in eventTable) {
			bool wasFound = false;

			foreach (string message in permanentMessages) {
				if (pair.Key == message) {
					wasFound = true;
					break;
				}
			}

			if (!wasFound)
				messagesToRemove.Add (pair.Key);
		}

		foreach (string message in messagesToRemove) {
			eventTable.Remove (message);
		}
	}

	static public void PrintEventTable ()
	{
		Debug.Log ("\t\t\t=== MESSENGER PrintEventTable ===");

		foreach (KeyValuePair<string, Delegate> pair in eventTable) {
			Debug.Log ("\t\t\t" + pair.Key + "\t\t" + pair.Value);
		}

		Debug.Log ("\n");
	}

	#endregion

	#region Message logging and exception throwing

	static void OnListenerAdding (string eventType, Delegate listenerBeingAdded)
	{
		#if MESSENGER_LOG_ALL_MESSAGES || MESSENGER_LOG_ADD_LISTENER
		Debug.Log("MESSENGER OnListenerAdding \t\"" + eventType + "\"\t{" + listenerBeingAdded.Target + " -> " + listenerBeingAdded.Method + "}");
		#endif

		if (!eventTable.ContainsKey (eventType)) {
			eventTable.Add (eventType, null);
		}

		Delegate d = eventTable [eventType];
		if (d != null && d.GetType () != listenerBeingAdded.GetType ()) {
			#if MESSENGER_THROW_EXCEPTIONS
			throw new ListenerException(string.Format("Attempting to add listener with inconsistent signature for event type {0}. Current listeners have type {1} and listener being added has type {2}", eventType, d.GetType().Name, listenerBeingAdded.GetType().Name));
			#endif
		}
	}
	
	

	static void OnListenerRemoving (string eventType, Delegate listenerBeingRemoved)
	{
		#if MESSENGER_LOG_ALL_MESSAGES
		Debug.Log("MESSENGER OnListenerRemoving \t\"" + eventType + "\"\t{" + listenerBeingRemoved.Target + " -> " + listenerBeingRemoved.Method + "}");
		#endif

		if (eventTable.ContainsKey (eventType)) {
			Delegate d = eventTable [eventType];
			if (d == null) {
				#if MESSENGER_THROW_EXCEPTIONS
				throw new ListenerException(string.Format("Attempting to remove listener with for event type \"{0}\" but current listener is null.", eventType));
				#endif
			} else if (d.GetType () != listenerBeingRemoved.GetType ()) {
				#if MESSENGER_THROW_EXCEPTIONS
				throw new ListenerException(string.Format("Attempting to remove listener with inconsistent signature for event type {0}. Current listeners have type {1} and listener being removed has type {2}", eventType, d.GetType().Name, listenerBeingRemoved.GetType().Name));
				#endif
			}
		} else {
			#if MESSENGER_THROW_EXCEPTIONS
			throw new ListenerException(string.Format("Attempting to remove listener for type \"{0}\" but Messenger doesn't know about this event type.", eventType));
			#endif
		}
	}

	static void OnListenerRemoved (string eventType)
	{
		if (eventTable.ContainsKey(eventType) && eventTable [eventType] == null) {
			eventTable.Remove (eventType);
		}
	}

	

	
	static void OnBroadcasting (string eventType)
	{
		#if MESSENGER_REQUIRE_LISTENER
		if (!eventTable.ContainsKey(eventType) && !luaeventTable.ContainsKey(eventType))
		{
		#if MESSENGER_THROW_EXCEPTIONS
		throw new BroadcastException(string.Format("Broadcasting message \"{0}\" but no listener found. Try marking the message with Messenger.MarkAsPermanent.", eventType));
		#endif
		}
		#endif
	}

	static public BroadcastException CreateBroadcastSignatureException (string eventType)
	{
		return new BroadcastException (string.Format ("Broadcasting message \"{0}\" but listeners have a different signature than the broadcaster.", eventType));
	}

	public class BroadcastException : Exception
	{
		public BroadcastException (string msg)
			: base (msg)
		{
		}
	}

	public class ListenerException : Exception
	{
		public ListenerException (string msg)
			: base (msg)
		{
		}
	}

	#endregion

	#region AddListener

	//No parameters
	static public void AddListener (string eventType, UnityAction handler)
	{
		OnListenerAdding (eventType, handler);
		eventTable [eventType] = (UnityAction)eventTable [eventType] + handler;
	}

	//Single parameter
	static public void AddListener<T> (string eventType, UnityAction<T> handler)
	{
		OnListenerAdding (eventType, handler);
		eventTable [eventType] = (UnityAction<T>)eventTable [eventType] + handler;
	}

	//Two parameters
	static public void AddListener<T, U> (string eventType, UnityAction<T, U> handler)
	{
		OnListenerAdding (eventType, handler);
		eventTable [eventType] = (UnityAction<T, U>)eventTable [eventType] + handler;
	}

	//Three parameters
	static public void AddListener<T, U, V> (string eventType, UnityAction<T, U, V> handler)
	{
		OnListenerAdding (eventType, handler);
		eventTable [eventType] = (UnityAction<T, U, V>)eventTable [eventType] + handler;
	}

	//4 parameters
	static public void AddListener<T, U, V,W> (string eventType, UnityAction<T, U, V,W> handler)
	{
		OnListenerAdding (eventType, handler);
		eventTable [eventType] = (UnityAction<T, U, V,W>)eventTable [eventType] + handler;
	}
	

	
	#endregion

	#region RemoveListener

	//No parameters
	static public void RemoveListener (string eventType, UnityAction handler)
	{
		OnListenerRemoving (eventType, handler);
		if (eventTable.ContainsKey (eventType)) {
			eventTable [eventType] = (UnityAction)eventTable [eventType] - handler;
		}
		OnListenerRemoved (eventType);
	}

	//Single parameter
	static public void RemoveListener<T> (string eventType, UnityAction<T> handler)
	{
		OnListenerRemoving (eventType, handler);
		if (eventTable.ContainsKey (eventType)) {
			eventTable [eventType] = (UnityAction<T>)eventTable [eventType] - handler;
		}
		OnListenerRemoved (eventType);
	}

	//Two parameters
	static public void RemoveListener<T, U> (string eventType, UnityAction<T, U> handler)
	{
		OnListenerRemoving (eventType, handler);
		if (eventTable.ContainsKey (eventType)) {
			eventTable [eventType] = (UnityAction<T, U>)eventTable [eventType] - handler;
		}
		OnListenerRemoved (eventType);
	}

	//Three parameters
	static public void RemoveListener<T, U, V> (string eventType, UnityAction<T, U, V> handler)
	{
		OnListenerRemoving (eventType, handler);
		if (eventTable.ContainsKey (eventType)) {
			eventTable [eventType] = (UnityAction<T, U, V>)eventTable [eventType] - handler;
		}
		OnListenerRemoved (eventType);
	}

	//4 parameters
	static public void RemoveListener<T, U, V,W> (string eventType, UnityAction<T, U, V,W> handler)
	{
		OnListenerRemoving (eventType, handler);
		eventTable [eventType] = (UnityAction<T, U, V,W>)eventTable [eventType] - handler;
		OnListenerRemoved (eventType);
	}
	

	#endregion

	#region Broadcast

	//No parameters
	static public void Broadcast (string eventType)
	{
		#if MESSENGER_LOG_ALL_MESSAGES || MESSENGER_LOG_BROADCAST_MESSAGE
		Debug.Log("MESSENGER\t" + System.DateTime.Now.ToString("hh:mm:ss.fff") + "\t\t\tInvoking \t\"" + eventType + "\"");
		#endif
		OnBroadcasting (eventType);

		Delegate d;
		if (eventTable.TryGetValue (eventType, out d)) {
			UnityAction action = d as UnityAction;

			if (action != null) {
				action ();
			} else {
				#if MESSENGER_THROW_EXCEPTIONS
				throw CreateBroadcastSignatureException(eventType);
				#endif
			}
		}
	}

	//Single parameter
	static public void Broadcast<T> (string eventType, T arg1)
	{
		#if MESSENGER_LOG_ALL_MESSAGES || MESSENGER_LOG_BROADCAST_MESSAGE
		Debug.Log("MESSENGER\t" + System.DateTime.Now.ToString("hh:mm:ss.fff") + "\t\t\tInvoking \t\"" + eventType + "\"");
		#endif
		OnBroadcasting (eventType);

		Delegate d;
		if (eventTable.TryGetValue (eventType, out d)) 
		{
			UnityAction<T> action = d as UnityAction<T>;

			if (action != null) {
				action (arg1);
			} else {
				#if MESSENGER_THROW_EXCEPTIONS
				throw CreateBroadcastSignatureException(eventType);
				#endif
			}
		}

	}

	//Two parameters
	static public void Broadcast<T, U> (string eventType, T arg1, U arg2)
	{
		#if MESSENGER_LOG_ALL_MESSAGES || MESSENGER_LOG_BROADCAST_MESSAGE
		Debug.Log("MESSENGER\t" + System.DateTime.Now.ToString("hh:mm:ss.fff") + "\t\t\tInvoking \t\"" + eventType + "\"");
		#endif
		OnBroadcasting (eventType);

		Delegate d;
		if (eventTable.TryGetValue (eventType, out d)) {
			UnityAction<T, U> action = d as UnityAction<T, U>;

			if (action != null) {
				action (arg1, arg2);
			} else {
				#if MESSENGER_THROW_EXCEPTIONS
				throw CreateBroadcastSignatureException(eventType);
				#endif
			}
		}
	}

	//Three parameters
	static public void Broadcast<T, U, V> (string eventType, T arg1, U arg2, V arg3)
	{
		#if MESSENGER_LOG_ALL_MESSAGES || MESSENGER_LOG_BROADCAST_MESSAGE
		Debug.Log("MESSENGER\t" + System.DateTime.Now.ToString("hh:mm:ss.fff") + "\t\t\tInvoking \t\"" + eventType + "\"");
		#endif
		OnBroadcasting (eventType);

		Delegate d;
		if (eventTable.TryGetValue (eventType, out d)) {
			UnityAction<T, U, V> action = d as UnityAction<T, U, V>;

			if (action != null) {
				action (arg1, arg2, arg3);
			} else {
				#if MESSENGER_THROW_EXCEPTIONS
				throw CreateBroadcastSignatureException(eventType);
				#endif
			}
		}
	}

	//4 parameters
	static public void Broadcast<T, U, V,W> (string eventType, T arg1, U arg2, V arg3,W arg4)
	{
		#if MESSENGER_LOG_ALL_MESSAGES || MESSENGER_LOG_BROADCAST_MESSAGE
		Debug.Log("MESSENGER\t" + System.DateTime.Now.ToString("hh:mm:ss.fff") + "\t\t\tInvoking \t\"" + eventType + "\"");
		#endif
		OnBroadcasting (eventType);

		Delegate d;
		if (eventTable.TryGetValue (eventType, out d)) {
			UnityAction<T, U, V,W> action = d as UnityAction<T, U, V,W>;

			if (action != null) {
				action (arg1, arg2, arg3,arg4);
			} else {
				#if MESSENGER_THROW_EXCEPTIONS
				throw CreateBroadcastSignatureException(eventType);
				#endif
			}
		}
	}
	#endregion
}