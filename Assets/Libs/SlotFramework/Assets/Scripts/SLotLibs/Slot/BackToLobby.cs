using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Classic;

public class BackToLobby : MonoBehaviour {

	[SerializeField]
	Button myButton;


	public virtual void Awake(){
//		Messenger.AddListener (SlotControllerConstants.DisactiveButtons, DisactiveButtons);
//		Messenger.AddListener (SlotControllerConstants.ActiveButtons, ActiveButtons);
	}

	void OnDestroy() 
	{
//		Messenger.RemoveListener (SlotControllerConstants.DisactiveButtons, DisactiveButtons);
//		Messenger.RemoveListener (SlotControllerConstants.ActiveButtons, ActiveButtons);
	}
	
	void DisactiveButtons(){
		if (myButton != null) {
			myButton.interactable = false;
		}
	}

	 void ActiveButtons(){
		if (myButton != null) {
			myButton.interactable = true;
		}
	}
}
