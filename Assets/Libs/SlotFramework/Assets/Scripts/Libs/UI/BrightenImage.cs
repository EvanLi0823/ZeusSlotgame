using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[ExecuteInEditMode]
[RequireComponent(typeof(Image))]
public class BrightenImage : MonoBehaviour {
	private float m_oldColorBrighten = 1f,original;

	[Range(1f,2f)]
	public float m_ColorBrighten = 1f;
	public Material m_ShareMaterial; //origanl material
	private Material m_material;

	private Image m_Image;
	void Awake()
	{
//		Canvas.GetDefaultCanvasMaterial ().shader = Shader.Find ("Custom/UI/Default");
	}
	void Start () {
		
		m_Image = GetComponent<Image> ();
		if (m_Image.material != null) {
			if (m_Image.material.HasProperty ("_Brightness")) {
				original = m_Image.material.GetFloat ("_Brightness");
				m_oldColorBrighten = original;
				m_material = m_Image.material;
			}
		}
	}
	
	// Update is called once per frame
//	void Update () {
////		#if UNITY_EDITOR
//		if (m_Image == null)
//		{
//			return;
//		}
//		if( m_ShareMaterial !=null && (m_Image.material ==null || m_ShareMaterial != m_Image.material || m_Image.material.name.Equals("Default UI Material") ))
//		{
//			m_material = new Material (m_ShareMaterial);
//			original = m_material.GetFloat ("_Brightness");
//			m_oldColorBrighten = original;
//			m_Image.material = m_material;
//		}
////		#endif
//		if ( m_ColorBrighten == m_oldColorBrighten) {
//			return;
//		}
//
//		if (m_material != null)
//		{
//			m_material.SetFloat ("_Brightness", m_ColorBrighten);
//			m_oldColorBrighten = m_ColorBrighten;
//		}
//
//	}
//
}
