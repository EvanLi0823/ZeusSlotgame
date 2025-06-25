using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 在videoslots这边IPhoneX单独适配，不去调整Camera的Videoport，像classic wild luckycity spear是同一方案，需要调整IPhoneX适配
/// </summary>
public class CameraAdapter {

	public static void AdapterSpecialCamera(Camera cam){
		//IphoneXAdapter.AdapterCamera(cam);
		CommonMobileDeviceAdapter.AdapterCamera (cam);
	}

	public static void AdapterAllCameras(){

		Camera[] cameras= Camera.allCameras;
		if (cameras==null||cameras.Length==0) return;

		#if UNITY_EDITOR
		SkySreenUtils.SetScreenResolutions ();
		#endif

		for (int i = 0; i <cameras.Length; i++)
		{
			//IphoneXAdapter.AdapterCamera(cameras[i]);
			CommonMobileDeviceAdapter.AdapterCamera (cameras[i]);
		}

	}
}
