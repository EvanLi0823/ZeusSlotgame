using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Classic
{
	public class SpinButtonStyle : MonoBehaviour
	{
		public SpinButtonState state;
		private Image StateButtonImage = null;
		public Sprite Disable;
		public Sprite SpinNormal;
		public Sprite SpinPress;
		public Sprite SpiningNormal;
		public Sprite SpiningPress;
		public Sprite AutoSpinNormal;
		public Sprite AutoPress;
		private Button button;

        public const string END_AUTO_RUN = "EndAutoRun";

        // Use this for initialization
        void Start ()
		{
			StateButtonImage = transform.GetComponent<Image> ();
			//Messenger.AddListener (SlotControllerConstants.OnSpinEnd, SpinEnable);
			//Messenger.AddListener(GameConstants.OnApplicationResume, EndAutoRun);
			Messenger.AddListener(GameConstants.SPINNOMONEY, EndAutoRun);
            //Messenger.AddListener (GameConstants.OPEN_DIALOG, OpenDialog);
            Messenger.AddListener(END_AUTO_RUN, EndAutoRun);
        }

		void OnDestroy()
		{
			//Messenger.RemoveListener (SlotControllerConstants.OnSpinEnd, SpinEnable);
			//Messenger.RemoveListener(GameConstants.OnApplicationResume, EndAutoRun);
			Messenger.RemoveListener(GameConstants.SPINNOMONEY, EndAutoRun);
            //Messenger.RemoveListener (GameConstants.OPEN_DIALOG, OpenDialog);
            Messenger.RemoveListener(END_AUTO_RUN, EndAutoRun);
        }
	
		private void OpenDialog()
		{
			if (isTouching) {
				this.OnTouchUp ();
			}
		}

		private void SpinEnable(bool isEnable)
		{
			state = isEnable ? SpinButtonState.NORML : SpinButtonState.Disable;
			BaseSlotMachineController.Instance.reelManager.AutoRun = false;
		}

		public void EndAutoRun(){
			this.state = SpinButtonState.NORML;
			if (BaseSlotMachineController.Instance != null && BaseSlotMachineController.Instance.reelManager != null) {
				BaseSlotMachineController.Instance.reelManager.AutoRun = false;
			}
			StateButtonImage.sprite = SpinNormal;
		}

		// Update is called once per frame
//		void Update()
//		{
//			
//		}
		void Update ()
		{
			if (isTouching) {
				Libs.EventSystemUtils.IsOpen = false;
			} else {
				Libs.EventSystemUtils.IsOpen = true;
			}
				if (!BaseSlotMachineController.Instance)
					 return;
				switch (state) {
				case SpinButtonState.Disable:
					StateButtonImage.sprite = Disable;
					break;
				case SpinButtonState.NORML:
					if (!isTouching) {
						StateButtonImage.sprite = SpinNormal;

					} else {
						bool isLongPress = lastTime > 0 && Time.time > lastTime + 1;
						if (!longActive) {
							if (isLongPress) {
								longActive = true;
								StateButtonImage.sprite = AutoSpinNormal;
								BaseSlotMachineController.Instance.DoSpin ();
							}
						}
					}
					break;
				case SpinButtonState.AUTOSPIN:
				if (!BaseSlotMachineController.Instance.reelManager.AutoRun && !BaseSlotMachineController.Instance.IsAutoSpinSuspending()) {
					state = SpinButtonState.NORML;
					StateButtonImage.sprite = SpinNormal;
				} else {
					if (!isTouching) {
						StateButtonImage.sprite = AutoSpinNormal;
						state = SpinButtonState.AUTOSPIN;
					}
				}
				break;
				default:
					break;
				}
		}

//		public void OnClick ()
//		{
//			switch (state) {
//			case SpinButtonState.NORML:
//				break;
//			case SpinButtonState.AUTOSPIN:
//				break;
//			default:
//				break;
//			}
//		}

		private IEnumerator BeginTouchDown()
		{
			yield return new WaitForSeconds (1f);
			if (!BaseSlotMachineController.Instance)
				yield break;
			switch (state) {
			case SpinButtonState.Disable:
				StateButtonImage.sprite = Disable;
				break;
			case SpinButtonState.NORML:
				if (!isTouching) {
					StateButtonImage.sprite = SpinNormal;

				} else {
					bool isLongPress = lastTime > 0 && Time.time > lastTime + 1;
					if (!longActive) {
						if (isLongPress) {
							longActive = true;
							StateButtonImage.sprite = AutoSpinNormal;
							BaseSlotMachineController.Instance.DoSpin ();
						}
					}
				}
				break;
			case SpinButtonState.AUTOSPIN:
				if (!BaseSlotMachineController.Instance.reelManager.AutoRun) {
					state = SpinButtonState.NORML;
					StateButtonImage.sprite = SpinNormal;
				} else {
					if (!isTouching) {
						StateButtonImage.sprite = AutoSpinNormal;
					}
				}
				break;
			default:
				break;
			}
		}

		float lastTime;
		bool isTouching = false;
		bool longActive = false;

		public void OnTouchDown ()
		{
			if (Libs.UIManager.Instance.CheckExistDialog()) {
				return;
			}
			//Libs.SoundEntity.Instance.Spin ();//LQ 转轮启动时，播放声效
			if (BaseSlotMachineController.Instance.reelManager.isFreespinBonus&&
				!BaseSlotMachineController.Instance.reelManager.gameConfigs.EnableSpinBtnInFreeSpin)
				return;
//			StartCoroutine (BeginTouchDown());
			isTouching = true;
			switch (state) {
			case SpinButtonState.NORML:
				longActive = false; 
				StateButtonImage.sprite = SpinPress;
				lastTime = Time.time;
                if ((BaseSlotMachineController.Instance.reelManager.isSpining ())) {
					StateButtonImage.sprite = SpiningPress;
                    lastTime = Time.time;
				} else {
					StateButtonImage.sprite = SpinPress;
					lastTime = Time.time;
				}
				break;
			case SpinButtonState.AUTOSPIN:
				StateButtonImage.sprite = AutoPress;
				lastTime = -1;
				break;
			default:
				break;
			}
		}



		public void OnTouchUp ()
		{
//			StopAllCoroutines ();
			isTouching = false;
			if (Libs.UIManager.Instance.CheckExistDialog()) {
				return;
			}
			if (BaseSlotMachineController.Instance.reelManager.isFreespinBonus&&
				!BaseSlotMachineController.Instance.reelManager.gameConfigs.EnableSpinBtnInFreeSpin)
				return;
			bool isLongPress = lastTime > 0 && Time.time > lastTime + 1;
			switch (state) {
			case SpinButtonState.NORML:
				if (isLongPress) {
					state = SpinButtonState.AUTOSPIN;
                    BaseSlotMachineController.Instance.reelManager.AutoRun = true;
                    if (BaseSlotMachineController.Instance.reelManager.isSpining ()) {
                        if (!BaseSlotMachineController.Instance.reelManager.fastStopDisabled) {
                            StateButtonImage.sprite = SpinNormal;
                            BaseSlotMachineController.Instance.reelManager.StopRun ();
                        }
					} else {
						Analytics.GetInstance ().LogEvent (Analytics.AutoSpinClicked);
                        if (BaseSlotMachineController.Instance.DoSpin ()) {
							StateButtonImage.sprite = AutoSpinNormal;
						} else {
							state = SpinButtonState.NORML;
							StateButtonImage.sprite = SpinNormal;
                            BaseSlotMachineController.Instance.reelManager.AutoRun = false;
							BaseSlotMachineController.Instance.statisticsManager.SendAutoSpinTimesEvent (); 
						}
					}
				} else {
                    if (BaseSlotMachineController.Instance.reelManager.isSpining ()) {
                        if (!BaseSlotMachineController.Instance.reelManager.fastStopDisabled) {
                            StateButtonImage.sprite = SpinNormal;
                            BaseSlotMachineController.Instance.reelManager.fastStop = true;
							Messenger.Broadcast (GameConstants.ENABLE_FAST_STOP);
                            BaseSlotMachineController.Instance.reelManager.StopRun ();
			
                        }
					} else {
                        if (BaseSlotMachineController.Instance.DoSpin ()) {
							StateButtonImage.sprite = SpiningNormal;
						} else {
							StateButtonImage.sprite = SpinNormal;
                            Messenger.Broadcast(GameConstants.NOW_SPIN_CLICK);
						}
					}
				}
				break;
			case SpinButtonState.AUTOSPIN:
				state = SpinButtonState.NORML;
				StateButtonImage.sprite = AutoSpinNormal;
                BaseSlotMachineController.Instance.reelManager.AutoRun = false;

                if(!BaseSlotMachineController.Instance.reelManager.isSpining ()){
					if (ResultStateManager.Instante.slotController.reelManager.enableBetChangeAfterEpicWin) {
						Messenger.Broadcast<bool> (SlotControllerConstants.ActiveButtons,false);
					}
				}
				BaseSlotMachineController.Instance.statisticsManager.SendAutoSpinTimesEvent (); 
				break;
			default:
				break;
			}
		}

		public enum SpinButtonState
		{
			NORML,
			AUTOSPIN,
			Disable
		}
	}
}
