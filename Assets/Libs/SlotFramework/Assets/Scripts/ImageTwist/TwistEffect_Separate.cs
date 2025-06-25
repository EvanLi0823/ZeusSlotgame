using UnityEngine;
using UnityEngine.UI;
using System.Collections;

//继承自PostEffectBase
public class TwistEffect_Separate : PostEffectBase {

	public bool ActivateTwistEffect;

	public Camera mainCamera;

	public RenderTexture mainRT {
		get;
		set;
	}

	public Image reelImage {
		get;
		set;
	}
	private Texture2D t2d;

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
	public Color fillInColor; private int cnter = 0;

	[Range(0.0f, 1.0f)]
	public float mainTexRatio;

    //覆写OnRenderImage函数
    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
//		Debug.Log(Camera.current.name);
        //仅仅当有材质的时候才进行后处理，如果_Material为空，不进行后处理
        if (_Material)
        {
            //通过Material.SetXXX（"name",value）可以设置shader中的参数值
			_Material.SetFloat("_ParamA", parameterA);
			_Material.SetFloat("_ParamB", parameterB);
			_Material.SetFloat("_TwistAreaUp", twistAreaUpBorder);
			_Material.SetFloat("_TwistAreaDown", twistAreaDownBorder);
			_Material.SetFloat("_TwistAreaLeft", twistAreaLeftBorder);
			_Material.SetFloat("_TwistAreaRight", twistAreaRightBorder);
			_Material.SetColor("_BackgroundColor", fillInColor);
//			_Material.SetTexture("_MainTex", src);
			_Material.SetTexture("_MainMachineTexture", mainRT);
			_Material.SetFloat("_MainTexRatio", mainTexRatio);
            //使用Material处理Texture，dest不一定是屏幕，后处理效果可以叠加的！
//			Debug.Log("Blit!");

			Graphics.Blit(mainRT, dest);
			if (ActivateTwistEffect)
				Graphics.Blit(src, dest, _Material);
        }
        else
        {
            //直接绘制
            Graphics.Blit(src, dest);
        }
    }

	void Awake() {
		mainRT = new RenderTexture(Screen.width, Screen.height, 24, RenderTextureFormat.ARGB32);
		mainRT.wrapMode = TextureWrapMode.Repeat;
		if (ActivateTwistEffect) {
			mainCamera.targetTexture = mainRT;
		}
	}
}