using UnityEngine;
using UnityEngine.UI;
using Libs;
using App.SubSystems;

namespace Classic
{
	public class SettingDialog : SettingDialogBase
	{
		public const string RefreshSettingDialogMsg = "RefreshSettingDialogMsg";
		protected override void Awake ()
		{
			base.Awake ();

			Messenger.Broadcast(SlotMiddlePanel.StopAutoRun);

			Settings.GetInstance ().OnNotificationEnabledChanged += settingNotificationChangedListener;
			BaseGameConsole.ActiveGameConsole().LogBaseEvent(Analytics.OpenSettingDialog);
			Messenger.AddListener (SettingDialog.RefreshSettingDialogMsg,Refresh);
		}

		protected override void OnDestroy ()
		{
			Settings.GetInstance ().OnNotificationEnabledChanged -= settingNotificationChangedListener;
			Messenger.RemoveListener (SettingDialog.RefreshSettingDialogMsg,Refresh);
		}

		public override void OnButtonClickHandler (GameObject go)
		{
			if (go == this.TermsButton.gameObject)
			{
				TermsOfUSOnClick();
			}else if (go == this.PolicyButton.gameObject)
			{
				PrivacyOnClick();
			}
			else if (go == this.FeedBackButton.gameObject)
			{
				FeedBackOnClick();
			}
			
			Libs.AudioEntity.Instance.PlayClickEffect();
		}

		public override void onToggleSelect (GameObject go, bool isSelect)
		{
			//Libs.SoundEntity.Instance.Click ();
			Libs.AudioEntity.Instance.PlayClickEffect();
			if (go == this.ToggleSound.gameObject) {
				SoundsEnabledChanged ();
			} else if (go == this.ToggleScreenLock.gameObject)
			{
				ScreenLockDuringAutoSpinChanged();
			}
		}

		public override void Refresh ()
		{
			base.Refresh ();
			this.ToggleSound.isOn = Settings.GetInstance ().IsSoundsEnabled ();
			this.ToggleScreenLock.isOn = Settings.GetInstance ().IsScreenLockDuringAutoSpin ();
		}

		public void SoundsEnabledChanged ()
		{
			if(ToggleSound.isOn) AudioManager.Instance.AsyncPlayEffectAudio("open_function_switch");
			Settings.GetInstance ().SetSoundsEnabled (ToggleSound.isOn);
            Analytics.GetInstance().LogEvent(Analytics.SoundsEnabledChanged, "on", ToggleSound.isOn);
            AudioListener.pause = !ToggleSound.isOn;
			//Libs.SoundManager.Instance.EnableSound = ToggleSound.isOn;
			Libs.AudioEntity.Instance.EnableSound = ToggleSound.isOn;
		}

		public void ScreenLockDuringAutoSpinChanged ()
		{
			if(ToggleScreenLock.isOn) AudioManager.Instance.AsyncPlayEffectAudio("open_function_switch");
			if (ToggleScreenLock.isOn != Settings.GetInstance ().IsScreenLockDuringAutoSpin ()) {
				Settings.GetInstance ().SetScreenLockDuringAutoSpin (ToggleScreenLock.isOn);
				ToggleScreenLock.isOn = Settings.GetInstance ().IsScreenLockDuringAutoSpin ();
                Analytics.GetInstance().LogEvent(Analytics.ScreenLockDuringAutoSpinChanged, "on", ToggleScreenLock.isOn.ToString());
            }
        }

		void CloseSettingDialog ()
		{
			UIManager.Instance.Close (this);
		}

		void PrivacyOnClick()
		{
			PlatformManager.Instance.SendMsgToPlatFormByType(MessageType.PrivacyPolicy);
		}
		
		void TermsOfUSOnClick()
		{
			PlatformManager.Instance.SendMsgToPlatFormByType(MessageType.TermsofUse);
		}
		
		void FeedBackOnClick()
		{
			PlatformManager.Instance.SendMsgToPlatFormByType(MessageType.FeedBack);
		}

		private Settings.NotifyNotificationEnabledChanged settingNotificationChangedListener;

		
		private static global::Utils.Logger logger = global::Utils.Logger.GetUnityDebugLogger (typeof(SettingDialog), true);
	}
}
