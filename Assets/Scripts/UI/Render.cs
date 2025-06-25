using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Libs;

namespace Classic
{
public class Render : RenderBase 
{
     	protected override void Awake()
    	{
       		base.Awake();
    	}

   		public override void OnButtonClickHandler(GameObject go)
    	{
    	    base.OnButtonClickHandler(go);
    	}

		public override void SetData (object data,bool needRefresh = false)
		{
			base.SetData (data,needRefresh);
			this.Image.color = new Color (1, 0, 0);
		}
	}
}
