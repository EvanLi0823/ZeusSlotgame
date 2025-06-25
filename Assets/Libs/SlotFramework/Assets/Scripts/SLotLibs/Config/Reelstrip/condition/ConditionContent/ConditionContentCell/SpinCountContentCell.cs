using UnityEngine;
using System.Collections;
using Classic;
public class SpinCountContentCell : ConditionContentCell {

	public SpinCountContentCell(object info):base(1,0){
		try {
			string[] temp = info.ToString().Split (',');
			min = Utils.Utilities.CastValueInt(temp[0]);
			max = Utils.Utilities.CastValueInt (temp[1]);
		} catch (System.Exception ex) {
			Utils.Utilities.LogPlistError ("SpinCount:"+info.ToString());
		}
	}

	public override bool ConditionIsOK(){
		int spinNum = 0 ;
		SlotMachineConfig machineConfig = null;
		StatisticsManager statisManager = null;
		ReelManager reelManager = null;
		if (BaseSlotMachineController.Instance!=null) {
			machineConfig = BaseSlotMachineController.Instance.slotMachineConfig;
			statisManager = BaseSlotMachineController.Instance.statisticsManager;
			reelManager = BaseSlotMachineController.Instance.reelManager;
		} else if(TestController.Instance!=null){
			machineConfig = TestController.Instance.classicMachineConfig;
			statisManager = TestController.Instance.statisticsManager;
			reelManager = TestController.Instance.reelManager;
		}
		if (machineConfig!=null) {
			spinNum = machineConfig.GetAllSpinCount ();//统计的为玩家付费的spin次数，如果接口发生改变，此处需要指定付费spin次数
			spinNum += statisManager.SpinNum;
		}
		if ((reelManager!=null&&(reelManager.IsCostBetSpin()))&&spinNum>=min&&spinNum<max) {
			return true;
		}

		return false;
	}
}
