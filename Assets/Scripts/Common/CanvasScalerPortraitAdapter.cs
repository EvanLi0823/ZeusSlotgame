using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;
public class CanvasScalerPortraitAdapter : MonoBehaviour
{
    private static readonly float iphoneXScaleFactor = 0.88f;
    private static readonly float ipadScaleFactor = 1.2f;
    private static readonly float iphoneScaleFactor = 1f;
    private static readonly float screenSizeXIphoneX = 2436f;


    private static readonly float SCREEN_NORMAL_WIDTH = 1920f;
    private static readonly float SCREEN_NORMAL_HEIGHT = 1080f;
    private static readonly float SCREEN_4X3_NORMAL_HEIGHT = 1440f;
    private static string[] UI_CANVAS_CHANGE_NAMES = { "UICanvas", "BGCanvas" };
    private static string[] BANNER_CANVAS_CHANGE_NAME = { "BannerCanvas" };
    private Vector3 initScale = new Vector3(0.01f,0.01f,0.01f);

    // Use this for initialization
    void Awake()
    {
        SceneManager.sceneLoaded += this.SceneChangeHandler;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= this.SceneChangeHandler;
    }

    void SceneChangeHandler(Scene s, LoadSceneMode mode)
    {
        if (SkySreenUtils.CurrentOrientation != ScreenOrientation.Portrait) return;
#if UNITY_EDITOR
        SkySreenUtils.SetScreenResolutions();
#endif
        Camera[] cameras = Camera.allCameras;
        if (cameras == null || cameras.Length < 1)
        {
            return;
        }
        for (int i = 0; i < cameras.Length; i++)
        {
            ChangeCamera(cameras[i]);
        }
    }

    private void ChangeCamera(Camera camera)
    {
        int count = camera.transform.childCount;
        //      Canvas[] canvas = camera.GetComponentsInChildren<Canvas> ();
        //      for (int i = 0; i < canvas.Length; i++) {
        //          if (canvas [i].worldCamera == camera) {
        //              Debug.Log (canvas [i].name);
        //          }
        //      }
        BaseSlotMachineController slot = camera.GetComponent<BaseSlotMachineController>();
        //UI.Lobby.BaseLobbyController lobby = camera.GetComponent<UI.Lobby.BaseLobbyController>();

        if (slot == null)
        {
            return;
        }

        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.backgroundColor = Color.black;


        float screenScale = 1f;
        if (IphoneXAdapter.IsIphoneX())
        {
            screenScale = SkySreenUtils.DEVICE_WIDTH / screenSizeXIphoneX;
        }
        else if (CommonMobileDeviceAdapter.IsWideScreen())
        {
            screenScale = SkySreenUtils.DEVICE_HEIGHT / SCREEN_NORMAL_HEIGHT;
        }
        else if (CommonMobileDeviceAdapter.IsSquareScreen())
        {
            screenScale = SkySreenUtils.DEVICE_WIDTH / SCREEN_NORMAL_WIDTH;
        }
        else
        {
            screenScale = SkySreenUtils.DEVICE_WIDTH / SCREEN_NORMAL_WIDTH;
        }
        
        CanvasScaler[] canvasScaler = camera.GetComponentsInChildren<CanvasScaler>();
        for (int i = 0; i < canvasScaler.Length; i++)
        {
            Canvas c = canvasScaler[i].transform.GetComponent<Canvas>();
            if (c.isRootCanvas)
            {
                if (slot != null)
                {
                    GoldGameConfig config = FindObjectOfType<GoldGameConfig>();
                    if (config != null)
                    {
                        if (config.IsSelfAdapter)
                        {
                            return;
                        }
                    }

                    
//                    float bannerScaleV = Screen.width / 1080f;

                    if (IphoneXAdapter.IsIphoneX() )
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
                        else if(BANNER_CANVAS_CHANGE_NAME.Contains(canvasScaler[i].name))
                        {
                            if (config != null && config.iphoneXScaleFactor != -1)
                            {
                                canvasScaler[i].scaleFactor = config.iphoneXScaleFactor * screenScale;
                            }
                            else
                            {
                                canvasScaler[i].scaleFactor = screenScale;
                            }
                        }
                    }
                    else if (CommonMobileDeviceAdapter.IsWideScreen())
                    {
                        canvasScaler[i].uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
                        if (UI_CANVAS_CHANGE_NAMES.Contains(canvasScaler[i].name))
                        {
                          
                            float origin = (float)Screen.height / Screen.width;
                            float v = origin / (2436f / 1125f);

                            canvasScaler[i].scaleFactor = screenScale;// config.iphoneXScaleFactor * screenScale * v;
                        }
                        else if(BANNER_CANVAS_CHANGE_NAME.Contains(canvasScaler[i].name))
                        {
                                canvasScaler[i].scaleFactor = screenScale;
                        }
                    }
                    else if (SkySreenUtils.GetScreenSizeType() == SkySreenUtils.ScreenSizeType.Size_4_3)
                    {
                        canvasScaler[i].uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
                        if (UI_CANVAS_CHANGE_NAMES.Contains(canvasScaler[i].name))
                        {
                            if (config != null && config.ipadScaleFactor != -1)
                            {
                                canvasScaler[i].scaleFactor = config.ipadScaleFactor * screenScale;
                            }
                            else
                            {
                                canvasScaler[i].scaleFactor = ipadScaleFactor * screenScale;
                            }
                        }
                        else if (BANNER_CANVAS_CHANGE_NAME.Contains(canvasScaler[i].name))
                        {
                            float origin = (float)Screen.width / Screen.height;
                            float factor =  SCREEN_NORMAL_HEIGHT /SCREEN_NORMAL_WIDTH  ;

                            float bannerScaleV = (float) Screen.width / SCREEN_4X3_NORMAL_HEIGHT; // origin / factor;

                            canvasScaler[i].scaleFactor = bannerScaleV;
//                            canvasScaler[i].scaleFactor = 1;
                        }
                    }
                    else if (SkySreenUtils.GetScreenSizeType()== SkySreenUtils.ScreenSizeType.Size_16_9)
                    {
                        canvasScaler[i].uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
                        if (UI_CANVAS_CHANGE_NAMES.Contains(canvasScaler[i].name))
                        {
                            if (config != null && config.iphoneScaleFactor != -1)
                            {
                                canvasScaler[i].scaleFactor = config.iphoneScaleFactor * screenScale;
                            }
                            else
                            {
                                canvasScaler[i].scaleFactor = iphoneScaleFactor * screenScale;
                            }
                        }
                        else if (BANNER_CANVAS_CHANGE_NAME.Contains(canvasScaler[i].name))
                        {
                            float origin = (float)Screen.width / Screen.height;
                            float factor =  SCREEN_NORMAL_HEIGHT /SCREEN_NORMAL_WIDTH  ;
                        
                            float bannerScaleV = origin / factor;
                            if (SkySreenUtils.IsRealIphone())
                            {
                                canvasScaler[i].scaleFactor = screenScale;
                            }
                            else
                            {
                                canvasScaler[i].scaleFactor = (float) Screen.width / SCREEN_4X3_NORMAL_HEIGHT; ;
                            }

                            
                        }
                    }
                    
                    if (c.renderMode == RenderMode.WorldSpace)
                    {
                        float screenWidth = Screen.width;
                        float screenHeight = Screen.height;
                        screenWidth = Screen.height;
                        screenHeight = Screen.width;
                        float realRadio = screenWidth / screenHeight;
                        float commonRadio = 16 / 9f;
                        float scale = commonRadio / realRadio;
                        if (realRadio > commonRadio)
                        {
                            c.transform.localScale = new Vector3(scale * initScale.x, scale * initScale.y,
                                scale * initScale.z);
                        }
                    }
                }
                //else if (lobby != null)
                //{
                //    if (IphoneXAdapter.IsIphoneX())
                //    {
                //        canvasScaler[i].uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
                //        canvasScaler[i].scaleFactor = screenScale;
                //    }
                //}
            }
        }
    }
}
