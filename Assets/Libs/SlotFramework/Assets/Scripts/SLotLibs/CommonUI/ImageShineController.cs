using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using MaterialCache = System.Collections.Generic.Dictionary<UnityEngine.Material,UnityEngine.Material>;

[ExecuteInEditMode]

public class ImageShineController : MonoBehaviour {

	private Material material;

	public float Percent;

	public float Angel;

	public float Width = 2;

	public Color ShineColor = new Color(1f,1f,1f,1f);
	private static readonly MaterialCache _cache = new MaterialCache();

	public bool ResetMaterial = false;

	void Awake()
	{
		if (GetComponent(typeof( VertIndexAsUV1)) == null) {
			this.gameObject.AddComponent<VertIndexAsUV1> ();
		}
	}
	Image image;
	void Start ()
	{
		image = GetComponent<Image> ();
		if (image != null) {
			material = image.material;
		}

		SaveMaterialOriginValues();
	}

	void Update (){
		if (material == null || ResetMaterial) {
			ResetMaterial = false;
//			Image image = GetComponent<Image> ();
			if (image != null) {
				#if UNITY_EDITOR
				Material cachedMat;
				if(!_cache.TryGetValue(image.material,out cachedMat))
				{
					cachedMat = new Material(image.material);
					_cache.Add(image.material,cachedMat);
				}
				material =cachedMat;
				#else
				material = image.material;
				#endif

				 
			}
		}

		if (material != null) {
			material.SetFloat ("_percent", Percent);
			material.SetFloat ("_angle", Angel);
			material.SetFloat ("_width", Width);
			material.SetColor("_shineColor",ShineColor);
		}
	}


	void OnDisable()
	{
		RestoreMaterialOriginValues();
	}

	private void SaveMaterialOriginValues()
	{
		if (material != null) {
			if (material.HasProperty("_percent"))
				originPercent = material.GetFloat("_percent");
			if (material.HasProperty("_angle"))
				originAngle = material.GetFloat("_angle");
			if (material.HasProperty("_width"))
				originWidth = material.GetFloat("_width");
			if (material.HasProperty("_shineColor"))
				originShineColor = material.GetColor("_shineColor");
		}
	}

	private void RestoreMaterialOriginValues()
	{
		if (material != null) {
			if (material.HasProperty("_percent"))
				material.SetFloat ("_percent", originPercent);
			if (material.HasProperty("_angle"))
				material.SetFloat ("_angle", originAngle);
			if (material.HasProperty("_width"))
				material.SetFloat ("_width", originWidth);
			if (material.HasProperty("_shineColor"))
				material.SetColor("_shineColor",originShineColor);
		}
	}

	private float originPercent, originAngle, originWidth;
	private Color originShineColor;
}
