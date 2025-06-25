using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

namespace Classic
{
//	[ExecuteInEditMode]
	public class SlotConsole : MonoBehaviour
	{
		public SlotMiddlePanel slotMiddlePanel;
		public Transform balanceTransform;


		public virtual void Init (SlotMachineConfig slotConfig, GameCallback onStop = null, GameCallback onStart = null)
		{
			slotMiddlePanel.Init (slotConfig, onStop, onStart);
			CreateMiddlePanle ();
		}

		void Awake ()
		{
			OnAwake ();
		}

		public virtual void OnAwake(){
			Messenger.AddListener<bool> (SlotControllerConstants.DisactiveButtons, DisactiveButtons);
			Messenger.AddListener<bool> (SlotControllerConstants.ActiveButtons, ActiveButtons);
			Messenger.AddListener<SlotMachineConfig,GameCallback,GameCallback> (SlotControllerConstants.InitSlotScene, Init);
		}

		void OnDestroy() 
		{
			Messenger.RemoveListener<bool> (SlotControllerConstants.DisactiveButtons, DisactiveButtons);
			Messenger.RemoveListener<bool> (SlotControllerConstants.ActiveButtons, ActiveButtons);
			Messenger.RemoveListener<SlotMachineConfig,GameCallback,GameCallback> (SlotControllerConstants.InitSlotScene, Init);
			Destroy ();
		}

		public virtual void Destroy(){

		}

		void OnEnable ()
		{
//			CreateMiddlePanle ();
		}

		void CreateMiddlePanle ()
		{
			slotMiddlePanel = transform.GetComponentInChildren<SlotMiddlePanel> ();
			if (slotMiddlePanel == null) {
				slotMiddlePanel = UnityUtil.LoadPrefab<SlotMiddlePanel> ((SceneManager.GetActiveScene().name + "/MiddlePanel"));//资源类不可以修改
				slotMiddlePanel.transform.SetParent (transform, false);
				slotMiddlePanel.transform.SetSiblingIndex (0);
			}
            BaseSlotMachineController.Instance.reelManager = slotMiddlePanel.baseGamePanel;
			Messenger.Broadcast (SlotControllerConstants.SLOT_REELMANAGER_INIT);
		}

		public virtual void DisactiveButtons(bool ignoreSpin = false){
		}

		public virtual void ActiveButtons(bool ignoreSpin = false){
		}
	}
}
