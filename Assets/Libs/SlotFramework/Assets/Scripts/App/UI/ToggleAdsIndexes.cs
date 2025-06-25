using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ToggleAdsIndexes : MonoBehaviour {

	public List<Image> bgList = new List<Image> ();
	public List<Image> imgList = new List<Image> (); 
	void Awake(){
		//Messenger.AddListener<int,int> (GameConstants.SetCurrentAdsIndex,ToggleAdsIdx);
		InitIndexes ();
	}

	void OnDestroy(){
		//Messenger.RemoveListener<int,int> (GameConstants.SetCurrentAdsIndex,ToggleAdsIdx);
	}

	public void ToggleAdsIdx(int curIdx,int totalCount){
		if (imgList.Count == 0||(curIdx < 0||curIdx>= imgList.Count))
			return;
		InitIndexes ();
		if (totalCount<=1) return;
		if (imgList.Count >= totalCount) {
			int diff = Mathf.Max (imgList.Count - totalCount,0);
			List<Image> tmpList = imgList.GetRange (0,imgList.Count-diff);
			List<Image> bgtmpList = bgList.GetRange (0, bgList.Count - diff);
			for (int i = 0; i < bgtmpList.Count; i++) {
				if (bgtmpList [i] == null)
					continue;
				bgtmpList [i].gameObject.SetActive(true);
			}
			for (int i = 0; i < tmpList.Count; i++) {
				if (tmpList [i] == null)
					continue;
				if (i==curIdx) tmpList [i].gameObject.SetActive(true);
				else tmpList [i].gameObject.SetActive(false);
			}
		} 
		else {
			for (int i = 0; i < bgList.Count; i++) {
				if (bgList [i] == null)
					continue;
				bgList [i].gameObject.SetActive(true);
			}
			for (int i = 0; i < imgList.Count; i++) {
				if (imgList [i] == null)
					continue;
				if (i==curIdx) imgList [i].gameObject.SetActive(true);
				else imgList [i].gameObject.SetActive(false);
			}
		}
	}

	void InitIndexes(){
		for (int i = 0; i < imgList.Count; i++) {
			if (imgList [i] == null) continue;
			if(imgList [i].enabled) imgList [i].gameObject.SetActive(false);
		}
		for (int i = 0; i < bgList.Count; i++) {
			if (bgList [i] == null) continue;
			if(bgList[i].enabled) bgList [i].gameObject.SetActive(false);
		}
	}
}
