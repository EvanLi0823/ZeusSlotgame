using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[RequireComponent(typeof(Image))]
[ExecuteInEditMode]
public class ClassicImageShineCommon : MonoBehaviour {
	public float Percent;

	public float Angel;

	public float Width = 2;

	public Color ShineColor = new Color(1f,1f,1f,1f);

	public Material m_ShareMaterial;
	private Material m_material;
	private Image m_Image;

	void Awake()
	{
		if (GetComponent(typeof( VertIndexAsUV1)) == null) {
			this.gameObject.AddComponent<VertIndexAsUV1> ();
		}
	}

	// Use this for initialization
	void Start () {
		m_Image = GetComponent<Image> ();
//		if (m_Image.material != null && !m_Image.material.name.Equals("Default UI Material")) {
//			Percent =m_Image.material.GetFloat ("_percent");
//			Angel = m_Image.material.GetFloat ("_angle");
//			Width = m_Image.material.GetFloat ("_width");
//			ShineColor = m_Image.material.GetColor("_shineColor");
//			m_material = m_Image.material;
//			Debug.Log (33333);
//		}
	}
	
	// Update is called once per frame
	void Update () {
		if( m_ShareMaterial !=null && (m_Image.material ==null || !m_Image.material.name.Equals(m_ShareMaterial.name)))
		{
			m_material = new Material (m_ShareMaterial);
			m_Image.material = m_material;

//			Percent =m_Image.material.GetFloat ("_percent");
//			Angel = m_Image.material.GetFloat ("_angle");
//			Width = m_Image.material.GetFloat ("_width");
//			ShineColor = m_Image.material.GetColor("_shineColor");
		}

		if (m_Image.material != null) {
			m_Image.material.SetFloat ("_percent", Percent);
			m_Image.material.SetFloat ("_angle", Angel);
			m_Image.material.SetFloat ("_width", Width);
			m_Image.material.SetColor("_shineColor",ShineColor);
		}
	}
}
