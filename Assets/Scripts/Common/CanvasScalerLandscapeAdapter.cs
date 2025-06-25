using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;
using RealYou.Unity.UIAdapter;
public class CanvasScalerLandscapeAdapter : MonoBehaviour
{
	private static readonly float iphoneXScaleFactor = 0.88f;
	private static readonly float ipadScaleFactor = 1.2f;
	private static readonly float screenSizeXIphoneX = 2436f;

    private static readonly float SCREEN_NORMAL_WIDTH = 1920f;
    private static readonly float SCREEN_NORMAL_HEIGHT = 1080f;
    private static readonly float SCREEN_4X3_NORMAL_HEIGHT = 1440f;
    private static string[] UI_CANVAS_CHANGE_NAMES = { "UICanvas", "BGCanvas"};
    private static string[] BANNER_CANVAS_CHANGE_NAME = { "BannerCanvas" };
    private Vector3 initScale = new Vector3(0.01f,0.01f,0.01f);
	// Use this for initialization
	void Awake ()
	{
//		if (IphoneXAdapter.IsIphoneX () || SkySreenUtils.GetScreenSizeType() == SkySreenUtils.ScreenSizeType.Size_4_3 ) {
//			DontDestroyOnLoad (this);
		SceneManager.sceneLoaded += this.SceneChangeHandler;
	}

	void OnDestroy(){
		SceneManager.sceneLoaded -= this.SceneChangeHandler;
	}

	void SceneChangeHandler (Scene s, LoadSceneMode mode)
	{
        if (SkySreenUtils.CurrentOrientation != ScreenOrientation.LandscapeLeft) return;
#if UNITY_EDITOR
        SkySreenUtils.SetScreenResolutions();
#endif
		Camera[] cameras = Camera.allCameras;
		if (cameras == null || cameras.Length < 1) {
			return;
		}
		for (int i = 0; i < cameras.Length; i++) {
			ChangeCamera (cameras [i]);
		}
	}

	private void ChangeCamera (Camera camera)
	{
		CanvasScalerAdapt adapt = camera.GetComponent<CanvasScalerAdapt>();
		if (adapt != null)
		{
			adapt.SetCanvasScaler(SkySreenUtils.CurrentOrientation == ScreenOrientation.Portrait,camera);
			return;
		}
		int count = camera.transform.childCount;
//		Canvas[] canvas = camera.GetComponentsInChildren<Canvas> ();
//		for (int i = 0; i < canvas.Length; i++) {
//			if (canvas [i].worldCamera == camera) {
//				Debug.Log (canvas [i].name);
//			}
//		}
		BaseSlotMachineController slot = camera.GetComponent<BaseSlotMachineController> ();

		if (slot != null) {
			camera.clearFlags = CameraClearFlags.SolidColor;
			camera.backgroundColor = Color.black;
		}
		
        


		float screenScale = 1f;
		if (IphoneXAdapter.IsIphoneX()) {
			screenScale = SkySreenUtils.DEVICE_WIDTH / screenSizeXIphoneX;
		}
		else if (CommonMobileDeviceAdapter.IsWideScreen()) {
			screenScale = SkySreenUtils.DEVICE_HEIGHT / SCREEN_NORMAL_HEIGHT;
		}
		else if (CommonMobileDeviceAdapter.IsSquareScreen()) {
			screenScale = SkySreenUtils.DEVICE_WIDTH / SCREEN_NORMAL_WIDTH;
		} 
		else
		{
			screenScale = SkySreenUtils.DEVICE_WIDTH / SCREEN_NORMAL_WIDTH;
		}
        CanvasScaler[] canvasScaler = camera.GetComponentsInChildren<CanvasScaler> ();
		for (int i = 0; i < canvasScaler.Length; i++) {
			Canvas c = canvasScaler [i].transform.GetComponent<Canvas> ();
			if (c.isRootCanvas) {
				if (c.renderMode == RenderMode.WorldSpace)
				{
					c.transform.localScale = initScale;
				}

				if (slot != null) {
					GoldGameConfig config = FindObjectOfType<GoldGameConfig> ();
					if (config != null) {
						if (config.IsSelfAdapter) {
							return;
						}
					}
					
					if (IphoneXAdapter.IsIphoneX())
                    {
                        canvasScaler[i].uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
                        if (UI_CANVAS_CHANGE_NAMES.Contains(canvasScaler[i].name))
                        {
                            if (config != null && config.iphoneXScaleFactor != -1)
                            {
                                canvasScaler[i].scaleFactor = config.iphoneXScaleFactor * screenScale;
                            }
                            else
                            {
                                canvasScaler[i].scaleFactor = iphoneXScaleFactor * screenScale;
                            }
                        }
                        else if (BANNER_CANVAS_CHANGE_NAME.Contains(canvasScaler[i].name))
                        {
                            canvasScaler[i].scaleFactor = screenScale;
                        }
                    }
                    else if (SkySreenUtils.GetScreenSizeType () == SkySreenUtils.ScreenSizeType.Size_4_3) {
						canvasScaler [i].uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
						if (UI_CANVAS_CHANGE_NAMES.Contains (canvasScaler [i].name)) {
							if (config != null && config.ipadScaleFactor != -1) {
								if (SkySreenUtils.IsRealPad())
								{
									canvasScaler [i].scaleFactor = config.ipadScaleFactor * screenScale;
								}
								else
								{
									float realRadio = (float)Screen.width / Screen.height;
									float scale = realRadio/ (4f/3);
									canvasScaler [i].scaleFactor = config.ipadScaleFactor * screenScale/scale;
								}
							} else {
								canvasScaler [i].scaleFactor = ipadScaleFactor * screenScale;
							}
						} 
                        else if (BANNER_CANVAS_CHANGE_NAME.Contains(canvasScaler[i].name))
                        {
                            canvasScaler[i].scaleFactor =  screenScale;
                        }
                    }
                } 
            }
		}
	}
}
