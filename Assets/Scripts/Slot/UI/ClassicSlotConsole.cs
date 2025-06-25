using UnityEngine;
using System.Collections;

namespace Classic
{
	public class ClassicSlotConsole : SlotConsole 
	{
		public Vector2 offsetMin_Ipad = new Vector2(0.5f,0.5f);
		public Vector2 offsetMax_Ipad = new Vector2(0.5f,0.5f);
        public float scale_Factor_Ipad = 1.0f;
		private void RelayoutBannerImage(){
			SkySreenUtils.ScreenSizeType currentType = SkySreenUtils.GetScreenSizeType ();
			if (currentType == SkySreenUtils.ScreenSizeType.Size_16_9) {
			} else if(currentType == SkySreenUtils.ScreenSizeType.Size_4_3) {
				RectTransform rc2 = slotMiddlePanel.MachinePanel.transform as RectTransform;
				rc2.anchorMin = offsetMin_Ipad;
				rc2.anchorMax = offsetMax_Ipad;
                rc2.localScale = new Vector3(scale_Factor_Ipad, scale_Factor_Ipad, scale_Factor_Ipad);
            }
		}

		public override void Init (SlotMachineConfig slotConfig, GameCallback onStop = null, GameCallback onStart = null)
		{
			this.RelayoutBannerImage ();
			base.Init (slotConfig, onStop, onStart);
		}
	}
}
