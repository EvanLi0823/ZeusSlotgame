using UnityEngine;
using System.Collections;

namespace Classic
{
	public class SlotMiddlePanel : MonoBehaviour
	{
		public const string StopAutoRun ="StopAutoRun";
		public ReelManager baseGamePanel;
		public GameObject MachinePanel;

		public void Init (SlotMachineConfig slotConfig, GameCallback onStop = null, GameCallback onStart = null)
		{
			baseGamePanel.InitReels (slotConfig, onStop, onStart);
		}
	}
}
