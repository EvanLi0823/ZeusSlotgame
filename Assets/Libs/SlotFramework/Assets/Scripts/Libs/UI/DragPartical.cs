using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using UnityEngine.UI;

public class DragPartical : MonoBehaviour
{
	public GameObject DragGo;

	private RectTransform m_RectTransform;
	private ParticleSystem m_ParticleSystem;
	ParticleSystem.MainModule mainModule;

	void Start()
	{
		m_RectTransform = DragGo.GetComponent<RectTransform> ();
		m_ParticleSystem = DragGo.GetComponent<ParticleSystem> ();
		mainModule = m_ParticleSystem.main;
	}

	void Update ()
	{
		Vector3 pos = Input.mousePosition;

		bool isTouched = false;

		#if UNITY_EDITOR
		isTouched = pos.x > 2 && Screen.width - 2 > pos.x && pos.y > 2 && Screen.height - 2 > pos.y;
		#else
		isTouched = Input.touchCount>0;
		#endif

		if (isTouched) {
			pos.x /= Screen.width;
			pos.y /= Screen.height;

			m_RectTransform.anchorMax = new Vector2 (pos.x, pos.y);
			m_RectTransform.anchorMin = new Vector2 (pos.x, pos.y);
			if (!m_ParticleSystem.isPlaying) {
				mainModule.loop = true;
				m_ParticleSystem.Play ();
			}
		} else {
			mainModule.loop = false;
		}
	}

}