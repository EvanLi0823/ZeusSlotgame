using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

//继承自PostEffectBase
public class TwistEffect : PostEffectBase {

	public static TwistEffect instance;

	public bool ActivateTwistEffect;

	public Texture background_16_9, background_4_3;

	private Texture2D t2d;

	public List<float> params_16_9;
	public List<float> params_4_3;

	private const string TWIST_FAIL_EVENT_SENT = "TwistFailEventSent";

	[Header("变形参数A")]
	public float parameterA;
	[Header("变形参数B")]
	public float parameterB;
	[Header("变形区域上边界")]
	[Range(0.0f, 1.0f)]
	public float twistAreaUpBorder = 1.0f;
	[Header("变形区域下边界")]
	[Range(0.0f, 1.0f)]
	public float twistAreaDownBorder = 0.0f;
	[Header("变形区域左边界")]
	[Range(0.0f, 1.0f)]
	public float twistAreaLeftBorder = 0.0f;
	[Header("变形区域右边界")]
	[Range(0.0f, 1.0f)]
	public float twistAreaRightBorder = 1.0f;

	[Header("空白处填充颜色")]
	public Color fillInColor;

    //覆写OnRenderImage函数
    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
//		Debug.Log(Camera.current.name);
        //仅仅当有材质的时候才进行后处理，如果_Material为空，不进行后处理
//		Debug.Log(_Material == null);
		if (ActivateTwistEffect && _Material != null)
        {
//			Debug.Log("Setting shader param");
            //通过Material.SetXXX（"name",value）可以设置shader中的参数值
			_Material.SetFloat("_ParamA", parameterA / 10);
			_Material.SetFloat("_ParamB", parameterB);
			_Material.SetFloat("_TwistAreaUp", twistAreaUpBorder);
			_Material.SetFloat("_TwistAreaDown", twistAreaDownBorder);
			_Material.SetFloat("_TwistAreaLeft", twistAreaLeftBorder);
			_Material.SetFloat("_TwistAreaRight", twistAreaRightBorder);
            //使用Material处理Texture，dest不一定是屏幕，后处理效果可以叠加的！
//			Debug.Log("Blit!");

			Graphics.Blit(src, dest, _Material);
        }
        else
        {
            //直接绘制
            Graphics.Blit(src, dest);
        }
    }

	void Awake() {
		if (instance == null) {
			instance = this;
		}

		Material mat = _Material;
		if (mat == null) {
			ActivateTwistEffect = false;
			bool isEventSent = SharedPlayerPrefs.GetPlayerBoolValue(TWIST_FAIL_EVENT_SENT, false);
			if (!isEventSent) {
				SharedPlayerPrefs.SetPlayerPrefsBoolValue(TWIST_FAIL_EVENT_SENT, true);
				var dict = new Dictionary<string, object> {
					{"DeviceModule", SystemInfo.deviceModel},
					{"OperatingSystem", SystemInfo.operatingSystem},
					{"OperatingSystemFamily", SystemInfo.operatingSystemFamily},
					{"GraphicsDeviceID", SystemInfo.graphicsDeviceID},
					{"GraphicsDeviceName", SystemInfo.graphicsDeviceName},
				};
				BaseGameConsole.singletonInstance.LogBaseEvent("TwistEffectFailure", dict);
			}
			return;
		}

		if (SkySreenUtils.GetScreenSizeType () == SkySreenUtils.ScreenSizeType.Size_16_9) {
			mat.SetTexture("_BackgoundTex", background_16_9);
			SetParam (params_16_9);
		}
		else {
			mat.SetTexture("_BackgoundTex", background_4_3);
			SetParam (params_4_3);
		}

        if (gameObject.GetComponent<Camera>() == null) {
            Classic.Analytics.GetInstance().LogEvent("TwistEffect_Null_Camera");
            //GameObject.Find("Main Camera").GetComponent<Camera>().cullingMask &= (0x7FFFFBFF);
        }
        else {
            // HACK: 这个语句会强制将camera的culling mask中的Layer10（BannerUI）去除，但也会将Layer31去除。因为Layer数量应该不会那么多，所以就先不管了。
            //gameObject.GetComponent<Camera>().cullingMask &= (0x7FFFFBFF);
        }
	}

	void SetParam(List<float> param) {
		if (param == null || param.Count == 0) {
			return;
		}
		if (param.Count < 6) {
			Debug.LogError("Twist Param not enough!");
			return;
		}
		parameterA = param [0];
		parameterB = param [1];
		twistAreaUpBorder = param [2];
		twistAreaDownBorder = param [3];
		twistAreaLeftBorder = param [4];
		twistAreaRightBorder = param [5];
	}
}