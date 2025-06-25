using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public class MiddleMessagePanel : MonoBehaviour {
	public static  string HIDE_MSG="GOODLUCK_HIDE";
	public static  string SHOW_MSG="GOODLUCK_SHOW";
	[SerializeField]
	private  TextMeshProUGUI messageInfo;

	[SerializeField]
	private  Image bgImage;


	// Use this for initialization
	void Start () {
		Messenger.AddListener<string> (SlotControllerConstants.ChangeMiddleMessage, ChangeMessage);
		Messenger.AddListener (SHOW_MSG, showPanel);
		Messenger.AddListener (HIDE_MSG, hidePanel);
	}
	
	void OnDestroy() 
	{
		Messenger.RemoveListener (SHOW_MSG, showPanel);
		Messenger.RemoveListener (HIDE_MSG, hidePanel);
		Messenger.RemoveListener<string> (SlotControllerConstants.ChangeMiddleMessage, ChangeMessage);
	}

	void showPanel()
	{
		this.gameObject.SetActive (true);
	}

	void hidePanel()
	{
		this.gameObject.SetActive (false);
	}

	void ChangeMessage(string message){
		messageInfo.text = message;
		if (message.Equals ("")) {
			bgImage.gameObject.SetActive (false);
		} else {
			bgImage.gameObject.SetActive (true);
		}
	}
}
