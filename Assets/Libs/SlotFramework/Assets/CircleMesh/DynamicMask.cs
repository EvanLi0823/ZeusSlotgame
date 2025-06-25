using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// Dynamic mask.
/// 与粒子遮罩有冲突，需要动态添加来解决冲突问题
/// 以达到Mask遮图片，粒子遮罩遮粒子的问题
/// </summary>
public class DynamicMask : MonoBehaviour {
	public Sprite maskImg;
	private Mask mask;
	private Image img;
	private Transform mTrans;
	void Awake(){
		AddMask ();
	}
	void AddMask(){
		mTrans = transform;
		mask = mTrans.GetComponent<Mask> ();
		if (mask==null) {
			mask = mTrans.gameObject.AddComponent<Mask> ();
			if (maskImg!=null) {
				mask.showMaskGraphic = false;
			}
		}
		img = mTrans.GetComponent<Image> ();
		if (img==null) {
			img = transform.gameObject.AddComponent<Image> ();
			img.sprite = maskImg;
		}
		img.raycastTarget = false;
	}

}
