using UnityEngine;
using System.Collections;
using Libs;
using Classic;
using System;
using App.SubSystems;

public class GoldSlotController : BaseSlotMachineController {

	protected override void GetGameConsole(){
		SlotGameconsole.ActiveGameConsole ();
	}
}
