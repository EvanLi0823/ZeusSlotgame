using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace Classic{
	public class CustomContentCell :ConditionContentCell {
		public Dictionary<string,object> flagInfo;
		public string PointerToR;
		public CustomContentCell(Dictionary<string,object> flagInfo){
			this.flagInfo = flagInfo;
		}
		public override bool ConditionIsOK(){
			ReelManager reelManager= null;
			if (BaseSlotMachineController.Instance!=null) {
				reelManager = BaseSlotMachineController.Instance.reelManager;
			} else {
				reelManager = TestController.Instance.reelManager;
			}
			return reelManager.CheckCustomCondition (flagInfo);
		}
	}
}
