using UnityEngine;
using System.Collections;

public class BackgroundPanel : MonoBehaviour {
	void Awake () {
		OnAwakeBackgroundPanel ();
	}

	void OnDestroy(){
		OnDestroyBackgroundPanel ();
	}

	public virtual void OnAwakeBackgroundPanel(){
		Messenger.AddListener (SlotControllerConstants.OnEnterFreespin, OnEnterFreespin);
		Messenger.AddListener (SlotControllerConstants.OnQuitFreespin, OnQuitFreespin);
	}

	public virtual void OnDestroyBackgroundPanel(){
		Messenger.RemoveListener (SlotControllerConstants.OnEnterFreespin, OnEnterFreespin);
		Messenger.RemoveListener (SlotControllerConstants.OnQuitFreespin, OnQuitFreespin);
	}

	public virtual void OnEnterFreespin(){
	}

	public virtual void OnQuitFreespin(){
	}
}
