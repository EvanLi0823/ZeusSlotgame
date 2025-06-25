using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Classic
{
	public class SlotBackground : MonoBehaviour
	{
		public Image bg1;
		public Image bg2;

		public void ChangeSprite(Sprite bgSprite){
			bg1.sprite = bgSprite;
			bg2.sprite = bgSprite;
		}

		public void ChangerColor(Color c){
			bg1.color = c;
			bg2.color = c;
		}
	}
}
