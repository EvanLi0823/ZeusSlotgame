using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Libs;

namespace Classic
{
public class BlackOutPanel : BlackOutPanelBase 
{
     	protected override void Awake()
    	{
       		base.Awake();
    	}

   		public override void OnButtonClickHandler(GameObject go)
    	{
    	    base.OnButtonClickHandler(go);
    	}
	}
}
