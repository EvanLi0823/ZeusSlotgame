using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
namespace Classic
{
	[ExecuteInEditMode]
	public class SlotBackgroundParent : MonoBehaviour
	{

		public SlotBackground slotBackgournd;

		public static SlotBackground BackgroundController;

		void Awake ()
		{
			CreateMiddlePanle ();
		}
		
//		void OnEnable ()
//		{
//			CreateMiddlePanle ();
//		}
		
		void CreateMiddlePanle ()
		{

			slotBackgournd = transform.GetComponentInChildren<SlotBackground> ();

			if (slotBackgournd == null) {
				slotBackgournd = UnityUtil.LoadPrefab<SlotBackground> ((SceneManager.GetActiveScene().name + "/BGPanel"));//资源类不可以改
				if (slotBackgournd != null) {
					slotBackgournd.transform.SetParent (transform, false);
					slotBackgournd.transform.SetSiblingIndex (0);
				}
			}
			BackgroundController = slotBackgournd;
			float scaler =(3f * Screen.width)/ (4f * Screen.height);
			float scaler2 =(9f * Screen.width)/ (16f * Screen.height);
			slotBackgournd.transform.localScale  = new Vector3(scaler2,scaler,1);
		}
	}
}

	
