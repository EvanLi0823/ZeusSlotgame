using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class GlobalObjectReference : MonoBehaviour {
	private static GlobalObjectReference _instance=null;
	public static GlobalObjectReference Instance{
		get{			 
			return _instance;
		}
	}
	public List<AssetReference> dialogList; 
	void Awake(){
		if (_instance == null) {
			DontDestroyOnLoad (this);
			_instance = this;
		} else if (_instance != this) {
			Destroy(this.gameObject);
		}
	}
	[Serializable]
	public class AssetReference{
		public string assetID;
		public UnityEngine.GameObject assetRef;
	}

}
