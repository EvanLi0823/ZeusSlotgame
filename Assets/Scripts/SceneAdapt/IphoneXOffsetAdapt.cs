using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IphoneXOffsetAdapt : MonoBehaviour
{
    public Vector2 offsetMin = new Vector2(0f,0f);
	public Vector2 offsetMax = new Vector2(0f,0f);

	void Start () 
    {
		if (IphoneXAdapter.IsIphoneX()) 
        {
			RectTransform rct = this.transform as RectTransform;
			rct.offsetMax = this.offsetMax;
			rct.offsetMin = this.offsetMin;
		}
	}
}
