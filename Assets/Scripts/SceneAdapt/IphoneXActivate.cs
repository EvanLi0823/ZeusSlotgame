using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IphoneXActivate : MonoBehaviour 
{
	void Start () {
		if (!IphoneXAdapter.IsIphoneX()) {
			this.gameObject.SetActive(false);
		}
	}
}
