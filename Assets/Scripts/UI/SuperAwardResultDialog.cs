using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Libs;

namespace Classic
{
public class SuperAwardResultDialog : SuperAwardResultDialogBase 
{
     	protected override void Awake()
    	{
       		base.Awake();
			this.bResponseBackButton = false;
			this.DisplayTime = 3f;
			this.AutoQuit = true;
    	}

   		public override void OnButtonClickHandler(GameObject go)
    	{
    	    base.OnButtonClickHandler(go);
    	}

		public override void Refresh ()
		{
			base.Refresh ();
			this.TxtAwardNum.text = this.m_Data.ToString ();
		}
	}
}
