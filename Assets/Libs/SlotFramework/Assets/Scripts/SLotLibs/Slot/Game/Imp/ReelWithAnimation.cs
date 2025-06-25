using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Classic
{
	public class ReelWithAnimation : RollReelPanel
	{
		public GameObject AnimationGO;
		private GameObject go;
		public Image elementBackgournd;

		public override void InitElements (ReelManager GameContronller, GameConfigs elementConfigs, bool IsEndReel)
		{
			base.InitElements (GameContronller, elementConfigs, IsEndReel);
			for (int i=0; i<Elements.Count; i++) {
				Image elementbg = Instantiate (elementBackgournd) as Image;
				elementbg.transform.SetParent (transform, false);
				((BaseElementPanel)Elements[i]).bgImage = elementbg;
				Elements[i].ChangeBackGroundInfo(((BaseElementPanel)Elements[i]).transform as RectTransform);
				Elements[i].SetBackGround(elementConfigs.GetBackground(Elements[i].SymbolIndex));
				elementbg.transform.SetAsFirstSibling();
			}
		}

		public void LoadAnimation ()
		{
			go = Instantiate (AnimationGO) as GameObject;
			go.transform.SetParent (transform, false);
			RectTransform rt = go.transform as RectTransform;
		
			rt.anchorMin = new Vector2 (0, 0);
			rt.anchorMax = new Vector2 (1, 1);
			rt.offsetMax = new Vector2 (0, 0);
			rt.offsetMin = new Vector2 (0, 0);
		}

		public void StopAnimation ()
		{
			if (go != null) {
				Destroy (go);
				go = null;
			}
		}
	}
}
