using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif


public class SkySreenUtils : MonoBehaviour
{

    public enum ScreenSizeType
    {
        Size_16_9,
        Size_4_3,
        Size_3_2
    }

    public class ScreenStandard
    {
        public float width;
        public float height;
        public ScreenSizeType type;

        public ScreenStandard(ScreenSizeType type)
        {
            string temp = type.ToString();
            string[] temps = temp.Split('_');
            this.width = float.Parse(temps[1]);
            this.height = float.Parse(temps[2]);
            ;
            this.type = type;
        }
    }

    public static ScreenSizeType GetScreenSizeType()
    {
        SetScreenResolutions();
        return GetNearestSize().type;
    }

    public static ScreenStandard GetNearestSize()
    {
        float ratio = DEVICE_WIDTH * 1f / DEVICE_HEIGHT;
        float factor = 0;
        ScreenStandard result = defaultSize;
        foreach (ScreenStandard size in screenStandards)
        {
            float tempRatio = (size.width / size.height) / ratio;
            if (tempRatio > 1)
            {
                tempRatio = 1 / tempRatio;
            }

            if (tempRatio > factor)
            {
                factor = tempRatio;
                result = size;
            }
        }
        return result;
    }

    public static int FIXED_WIDTH = 960;
    public static int FIXED_HEIGHT = 640;
    public static int DEVICE_WIDTH = 1920;
    public static int DEVICE_HEIGHT = 1080;
    private static ScreenOrientation currentOrientation;
    public static ScreenOrientation CurrentOrientation
    {
        get { return currentOrientation; }
        set
        {
            if (value == currentOrientation)
                return;
            currentOrientation = value;
            if (currentOrientation != Screen.orientation)
            {
                switch (currentOrientation)
                {
                    case ScreenOrientation.LandscapeLeft:
                        {
                            Screen.autorotateToLandscapeLeft = true;
                            Screen.autorotateToLandscapeRight = true;
                            Screen.autorotateToPortrait = false;
                            Screen.autorotateToPortraitUpsideDown = false;
#if UNITY_ANDROID
                        Screen.orientation = ScreenOrientation.LandscapeLeft;
                        Libs.CoroutineUtil.Instance.StartCoroutine(LandscapeToAuto());
#endif
                        }
                        break;
                    case ScreenOrientation.Portrait:
                        {
                            // Screen.autorotateToLandscapeLeft = false;
                            // Screen.autorotateToLandscapeRight = false;
                            // Screen.autorotateToPortrait = true;
                            // Screen.autorotateToPortraitUpsideDown = false;
                            Screen.orientation = ScreenOrientation.Portrait;
                        }
                        break;
                    default:
                        break;
                }
            }
//
// #if UNITY_IOS
//
//             Screen.orientation = ScreenOrientation.AutoRotation;
// #endif

#if UNITY_EDITOR
            Screen.orientation = ScreenOrientation.AutoRotation;
            GameViewUtils.AdjustGameViewResolution();
#endif
        }
    }

    private static IEnumerator LandscapeToAuto()
    {
        while (true)
        {
            if (Screen.orientation == ScreenOrientation.LandscapeLeft)
            {
                Screen.orientation = ScreenOrientation.AutoRotation;
                yield break;
            }
            yield return GameConstants.FrameTime;
        }
    }
 
    private static float iPhoneRate = 16f / 9f;
    public static bool IsLessThanIphone()
    {
        if (Screen.width > Screen.height)
        {
            float screenRate = (float)Screen.width/Screen.height;
            float aa = Mathf.Abs(screenRate - iPhoneRate);
            return aa>0.1f && screenRate < iPhoneRate;
        }
        else
        {
            float screenRate = (float)Screen.height/Screen.width;
            float aa = Mathf.Abs(screenRate - iPhoneRate);
            return aa>0.1f && screenRate < iPhoneRate;
        }
    }

    public static bool hasSet = false;
    private static int scaleWidth = 0;
    private static int scaleHeight = 0;
    //硬件放缩
    public static void SetScreenResolutions(bool resetScreen = false)
    {
        if (!hasSet)
        {
            if (Screen.width > Screen.height)
            {
                SkySreenUtils.CurrentOrientation = ScreenOrientation.LandscapeLeft;
            }
            else
            {
                SkySreenUtils.CurrentOrientation = ScreenOrientation.Portrait;
            }
            hasSet = true;
            resetScreen = true;
            SetScreenResolution();
        }

        if (resetScreen)
        {
#if UNITY_ANDROID

            //比设定的比例低的话不做处理
            if (scaleWidth > 0 && scaleHeight > 0) {
                Screen.SetResolution (scaleWidth, scaleHeight, true);
            }
#endif
        }
    }

    private static void SetScreenResolution()
    {
        switch (CurrentOrientation)
        {
            case ScreenOrientation.LandscapeLeft:
                {
                    DEVICE_WIDTH = Screen.width;
                    DEVICE_HEIGHT = Screen.height;
                }
                break;
            case ScreenOrientation.Portrait:
                {
                    DEVICE_WIDTH = Screen.height;
                    DEVICE_HEIGHT = Screen.width;
                }
                break;
            default:
                {
                    DEVICE_WIDTH = Screen.width;
                    DEVICE_HEIGHT = Screen.height;
                }
                break;
        }
    }
    private static List<ScreenStandard> screenStandards = new List<ScreenStandard>(new ScreenStandard[] {
        new ScreenStandard (ScreenSizeType.Size_16_9),
        new ScreenStandard (ScreenSizeType.Size_4_3)
    });
    private static ScreenStandard defaultSize = new ScreenStandard(ScreenSizeType.Size_16_9);

    public const string UseScaledResolution = "UseScaledResolution";
    public static bool IsHighPerformanceDevice()
    {
        if (!System.Environment.Is64BitProcess) return false;
        if (SystemInfo.processorCount < 4) return false;
        if (SystemInfo.processorFrequency < 2000) return false;
        if (SystemInfo.systemMemorySize < 3000) return false;
        return true;
    }

    public static bool IsPortraitOrthographicSize(Camera camera)
    {
        if (camera == null) return false;
        return !Mathf.Approximately(camera.orthographicSize, 5.4f);
    }
    /// <summary>
    /// Sets the size of the camera orthographic.
    /// </summary>
    public static void SetCameraOrthographicSize(Camera dialogCamera, bool isPortraitMode = false)
    {
        if (dialogCamera == null) return;
        if (isPortraitMode)
        {
            if (!IsPortraitOrthographicSize(dialogCamera))
            {
                if (IphoneXAdapter.IsIphoneX())
                {
                    dialogCamera.orthographicSize = 11.695f;//5.4*2339/1080
                }
                else if (GetScreenSizeType()== ScreenSizeType.Size_16_9)
                {
                    dialogCamera.orthographicSize = 9.6f;//1920/100/2
                }
                else if (GetScreenSizeType()== ScreenSizeType.Size_4_3)
                {
                    dialogCamera.orthographicSize = 7.22f;//5.4*1440/1080
                }
                
            }

        }
        else
        {
            if (IsPortraitOrthographicSize(dialogCamera))
            {
                dialogCamera.orthographicSize = 5.4f;
            }
        }
    }
    
    public static float GetDlgScale()
    {
        if (CurrentOrientation == ScreenOrientation.Portrait)
        {
//            return v;
            if (IphoneXAdapter.IsIphoneX())
            {
                return 5.4f/11.695f; //  5.4*2339/1080
            }

            if (CommonMobileDeviceAdapter.IsWideScreen())
            {
                float v = (float)Screen.width /(float)Screen.height;
                if (v > 1)
                {
                    v = 1 / v;
                }
                return v;   //之所以改成这样是因为要解决
            }
            else if (GetScreenSizeType()== ScreenSizeType.Size_16_9)
            {
                //return 5.4f/9.6f; //1920/100/2
                
                float v = (float)Screen.width /(float)Screen.height;
                if (v > 1)
                {
                    v = 1 / v;
                }
                return v; 
            }
            else if (GetScreenSizeType()== ScreenSizeType.Size_4_3)
            {   
                float v = (float)Screen.width /(float)Screen.height;
                if (v > 1)
                {
                    v = 1 / v;
                }
                Debug.Log("GetDlgScale  44444");
                return v;   //之所以改成这样是因为要解决15：10的分辨率，
//                return 5.4f/7.22f; //5.4*1440/1080
            }
        }

        return 1;
    }
    public static  bool IsRealPad(){
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;
		
        float v = screenHeight / screenWidth;
			
        if (v > 1)
        {
            v = 1 / v;
        }

        if (Mathf.Abs(v - 0.75f ) <0.05)
        {
            return true;
        }

        return false;
    }
    
    public static  bool IsRealIphone(){
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;
		
        float v = screenHeight / screenWidth;
			
        if (v > 1)
        {
            v = 1 / v;
        }

        if (Mathf.Abs(v - 0.5625f ) <0.05)
        {
            return true;
        }

        return false;
    }
}
