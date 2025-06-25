using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommonPhoneCameraAdapter : MonoBehaviour {


	private Camera mainCamera;
	void Awake(){
		mainCamera = transform.GetComponent<Camera> ();
	}
	// Use this for initialization
	void Start () {
		#if UNITY_EDITOR
		SkySreenUtils.SetScreenResolutions ();
		#endif
		if (!IphoneXAdapter.IsIphoneX()) {
			if (mainCamera==null) {
				return;
			}
			CommonMobileDeviceAdapter.AdapterCamera (mainCamera);
		}
	}
}
