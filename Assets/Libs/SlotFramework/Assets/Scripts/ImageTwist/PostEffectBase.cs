using UnityEngine;
using System.Collections;

//非运行时也触发效果
//[ExecuteInEditMode]
//屏幕后处理特效一般都需要绑定在摄像机上
//[RequireComponent(typeof(Camera))]
//提供一个后处理的基类，主要功能在于直接通过Inspector面板拖入shader，生成shader对应的材质
public class PostEffectBase : MonoBehaviour {

    //Inspector面板上直接拖入
    public Shader shader = null;
    public Material _material = null;
    public Material _Material
    {
        get
        {
            if (_material == null)
                _material = GenerateMaterial(shader);
            return _material;
        }
    }

    //根据shader创建用于屏幕特效的材质
    protected Material GenerateMaterial(Shader shader)
    {
//		Debug.Log(shader.isSupported);
		if (shader == null) {
			//Utils.Utilities.NativePlatformLog("TwistEffectTest", "shader == null");
			return null;
		}
        //需要判断shader是否支持
		if (shader.isSupported == false) {
			//Utils.Utilities.NativePlatformLog("TwistEffectTest", "shader.isSupported == false");
			return null;
		}
        Material material = new Material(shader);
        material.hideFlags = HideFlags.DontSave;
		if (material) {
			//Utils.Utilities.NativePlatformLog("TwistEffectTest", "material != null");
			return material;
		}
		//Utils.Utilities.NativePlatformLog("TwistEffectTest", "material == null");
        return null;
    }
	/// <summary>
	/// Raises the destroy event.
	/// Added By qing.liu
	///使用这个标记HideFlags.DontSave 必须手动释放，否则会造成内存溢出 
	/// </summary>
	void OnDestroy(){
		if (_material!=null) {
			DestroyImmediate (_material);
		}
	}
}