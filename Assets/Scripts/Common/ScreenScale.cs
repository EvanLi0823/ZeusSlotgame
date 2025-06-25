using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG;
using DG.Tweening;
public class ScreenScale : MonoBehaviour {
	private Transform m_Transform;

//	public float Force = 1f;
//	public float duration = 0f;
	public Vector3 scale1 = new Vector3(1.3f,1.3f,1f);
	public Vector3 scale2 = new Vector3(1.15f,1.15f,1f);
	public float m_Time = 0.2f;
	private Vector3 origin;
	private Camera[] cameras;
	// Use this for initialization
	void Start () {
		m_Transform = this.transform;
		origin = m_Transform.position;

		cameras = FindObjectsOfType<Camera> ();
	}
	


	public void DoShake()
	{
		// if (cameras == null) {
		// 	return;
		// }

		// for(int i =0 ; i < cameras.Length; i++){
		// 	Transform _Transform = cameras [i].transform;
		// 	Sequence t = DOTween.Sequence();
		// 	t.SetUpdate (true);
		// 	t.Append (_Transform.DOScale (scale1, 0.2f).SetUpdate(true));
		// 	t.Append(_Transform.DOScale(scale2,0.2f).SetEase(Ease.OutBack).SetUpdate(true));
		// 	t.Play ();


		// }
//		Vector3 newOffset = new Vector3(UnityEngine.Random.Range(-Force, Force),UnityEngine.Random.Range(-Force, Force),0);
//
//		m_Transform.position = m_Transform.position - m_oldOffset + newOffset;
//
//		m_oldOffset = newOffset;


//		this.m_Transform.DOPunchScale (new Vector3 (1.02f, 1.02f, 1f),1f);
//		this.m_Transform.DOShakeScale (2f,new Vector3(1f,1f,0f),4,180f);
//		Vector3 v = new Vector3 (this.m_Transform.localScale.x, this.m_Transform.localScale.x, this.m_Transform.localScale.x);
	}

	public void Reset()
	{
		if (cameras == null) {
			return;
		}


		for (int i = 0; i < cameras.Length; i++) {
			Transform _Transform = cameras [i].transform;

			_Transform.localScale = Vector3.one;
		}
	}

	void OnEnable()
	{
//		DoShake ();
	}
}
