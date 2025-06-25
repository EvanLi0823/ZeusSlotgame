using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using MaterialCache = System.Collections.Generic.Dictionary<UnityEngine.Material,UnityEngine.Material>;

[ExecuteInEditMode]
public class Effect_Shine_WithStencil : MonoBehaviour
{
    private Material material;

	public float Prcent;
	
	public bool ResetMaterial = false;
	
	private float originPrcent, originMoveType;

	void Start ()
	{
		image = GetComponent<Image> ();
		if (image != null) {
			material = image.material;
		}

		SaveMaterialOriginValues();
	}
	Image image;
	void Update (){
		if (material == null || ResetMaterial) {
			ResetMaterial = false;
			image = GetComponent<Image> ();
			if (image != null) {
				material = image.material;
			}
		}

		if (material != null) {
			material.SetFloat ("_Precent", Prcent);
			material.SetFloat ("_MoveType", 10);
		}
	}


	void OnDisable()
	{
		RestoreMaterialOriginValues();
	}

	private void SaveMaterialOriginValues()
	{
		if (material != null) {
			if (material.HasProperty("_Precent"))
				originPrcent = material.GetFloat("_Precent");
			if (material.HasProperty("_MoveType"))
				originMoveType = material.GetFloat("_MoveType");
		}
	}

	private void RestoreMaterialOriginValues()
	{
		if (material != null) {
			if (material.HasProperty("_Precent"))
				material.SetFloat ("_Precent", originPrcent);
			if (material.HasProperty("_MoveType"))
				material.SetFloat("_MoveType", originMoveType);
		}
	}

}

