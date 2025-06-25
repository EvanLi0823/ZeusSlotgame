using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Libs;

namespace Classic
{
public class HighRollerDialog : HighRollerDialogBase 
{
		SlotMachineConfig config;
     	protected override void Awake()
    	{
       		base.Awake();
    	}

   		public override void OnButtonClickHandler(GameObject go)
    	{
    	    base.OnButtonClickHandler(go);
			if (go == this.CloseBtn.gameObject) {
				this.Close ();
			}
    	}

		public void InitData(int displayTime){
			this.DisplayTime = displayTime;
			this.AutoQuit = true;
		}

		public override void Refresh(){
			base.Refresh ();
			config = (this.m_Data as SlotMachineConfig);
			if (BaseSlotMachineController.Instance!=null) {
				BaseSlotMachineController.Instance.nCurrentTriggerHighRollerCount++;
			}
		}
	}
}
