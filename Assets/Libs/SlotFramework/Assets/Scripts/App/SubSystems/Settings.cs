using System;
using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;
using System.Collections.Generic;
using Plugins;

namespace App.SubSystems
{
    public class Settings
    {

        public delegate void NotifySoundsEnabledChanged (bool isEnabled);

        public NotifySoundsEnabledChanged OnSoundsEnabledChanged;

        public delegate void NotifyScreenLockDuringAutoSpinChanged (bool isLock);

        public NotifyScreenLockDuringAutoSpinChanged OnScreeLockDuringAutoSpinChanged;

        public delegate void NotifyNotificationEnabledChanged (bool isEnabled);

        public event NotifyNotificationEnabledChanged OnNotificationEnabledChanged;

        public delegate void NotifyRateUsClicked ();

        public NotifyRateUsClicked OnRateUsClicked;

        public delegate void NotifyAboutClicked ();

        public NotifyAboutClicked OnAboutClicked;

        public static Settings GetInstance ()
        {
            if (instance == null) {
                lock (syncRoot) {
                    if (instance == null) {
                        instance = new Settings ();
                        instance.LoadSettings ();
                    }
                }
            }
            return instance;
        }

        #region Sound


        public void SetSoundsEnabled (bool isEnabled)
        {
            if (isSoundsEnabled != isEnabled) {
                isSoundsEnabled = isEnabled;
                if (OnSoundsEnabledChanged != null) {
                    OnSoundsEnabledChanged (isSoundsEnabled);
                }
                SaveSettings ();
            }

        }

        public bool IsSoundsEnabled ()
        {
            return isSoundsEnabled;
        }

		public void SetWinnerAnnounce(bool isEnabled)
		{
			if (isEnabled != this.isPushWinnerEnabled) {
				this.isPushWinnerEnabled = isEnabled;

				SaveSettings ();
			}
		}

		public bool IsWinnerAnnounce()
		{
			return isPushWinnerEnabled;
		}

		public void SetupSound ()
		{
			AudioListener.pause = !this.IsSoundsEnabled ();
			//Libs.SoundManager.Instance.EnableSound = this.IsSoundsEnabled ();
            Libs.AudioEntity.Instance.EnableSound =this.IsSoundsEnabled();
		}

        #endregion


        #region Screen Lock

        public void SetScreenLockDuringAutoSpin (bool isAutomaticLock)
        {
            //if (IsScreenLockDuringAutoSpin() != isAutomaticLock) {
                SetPlatformScreenLockDuringAutoSpin(isAutomaticLock);

                if (isAutomaticLock == isScreenLockDuringAutoSpin && OnScreeLockDuringAutoSpinChanged != null) {
                    OnScreeLockDuringAutoSpinChanged (isAutomaticLock);
                }
                SaveSettings();
           // }

        }

        public bool IsScreenLockDuringAutoSpin ()
        {
            return isScreenLockDuringAutoSpin;
        }

        internal void SetPlatformScreenLockDuringAutoSpin(bool isAutomaticLock)
        {
            NativeAPIs._EnableAutomaticLockScreen(isAutomaticLock);
#if UNITY_ANDROID
			isScreenLockDuringAutoSpin = isAutomaticLock;
#else
			isScreenLockDuringAutoSpin = NativeAPIs._IsAutomaticLockScreen();
#endif
           

        }

        private bool IsPlatformScreenLockDuringAutoSpin()
        {
			#if UNITY_ANDROID
			    return isScreenLockDuringAutoSpin;
			#else
			    return Plugins.NativeAPIs._IsAutomaticLockScreen();
			#endif

           
        }

        #endregion

        public bool IsNotificationEnabled ()
        {
            return isNotificationEnabled;
        }

        internal void SetPlatformNotificationEnable (bool isEnabled)
        {
			#if UNITY_ANDROID
			SetSettingNotificationEnable(isEnabled);
			#else
			Plugins.NativeAPIs._EnableNotification(isEnabled);
			#endif
        }

        internal void SetSettingNotificationEnable( bool isEnabled)
        {
            if (isEnabled != isNotificationEnabled) {
                isNotificationEnabled = isEnabled;
                SaveSettings();
            }
        }

        internal void NotifyNotificationChanged(bool isEnabled)
        {
            if (OnNotificationEnabledChanged != null) {
                OnNotificationEnabledChanged (isEnabled);
            }
        }

        private bool isPlatformNotificatonEnabled()
        {
			#if UNITY_ANDROID
			return isNotificationEnabled;
			#else
			return Plugins.NativeAPIs._IsEnableNotification();
			#endif
        }
        

        public bool IsJackpotAnnouncementsEnabled ()
        {
	        return isJackpotAnnouncementsEnabled;
        }
        private void SaveSettings ()
        {
            PlayerPrefs.SetInt (SOUNDS_ENABLED_KEY, IsSoundsEnabled () ? 1 : 0);
			PlayerPrefs.SetInt (SCREEN_LOCK_DURING_AUTO_SPIN_KEY, IsScreenLockDuringAutoSpin () ? 1 : 0);
			PlayerPrefs.SetInt (NOTIFICATION_KEY, IsNotificationEnabled () ? 1 : 0);
			PlayerPrefs.SetInt(JACKPOT_ANNOUNCEMENTS_KEY,IsJackpotAnnouncementsEnabled()?1:0);
			PlayerPrefs.SetInt (PUSH_WINNER_KEY, isPushWinnerEnabled ? 1 : 0);
            PlayerPrefs.Save ();
        }

        private void LoadSettings ()
        {
			int DefaultWinnerNotificationEnabled = Utils.Utilities.CastValueInt( Plugins.Configuration.GetInstance ().GetValueWithPath<int> ("Module/Jackpot/WinnerAnnouncementSwitch", 1));
            isSoundsEnabled = (PlayerPrefs.GetInt (SOUNDS_ENABLED_KEY, DefaultSoundsEnabled) == 1);
			isScreenLockDuringAutoSpin = (PlayerPrefs.GetInt (SCREEN_LOCK_DURING_AUTO_SPIN_KEY, DefaultScreenLockDuringAutoSpin) == 1);
			isNotificationEnabled = (PlayerPrefs.GetInt (NOTIFICATION_KEY, DefaultNotificationEnabled) == 1);
			isJackpotAnnouncementsEnabled = (PlayerPrefs.GetInt (JACKPOT_ANNOUNCEMENTS_KEY, DefaultJackpotAnnouncementsEnabled) == 1);
			isPushWinnerEnabled = (PlayerPrefs.GetInt (PUSH_WINNER_KEY, DefaultWinnerNotificationEnabled) == 1);
		}

        private static void _OpenBrowser (string url)
        {
            Plugins.NativeAPIs._OpenBrowser (url);
        }

        private static void _RateUs ()
        {
            Plugins.NativeAPIs._RateUs ();
        }

		public void SetupPlatform ()
		{
			SetPlatformScreenLockDuringAutoSpin (IsScreenLockDuringAutoSpin ());
			SetPlatformNotificationEnable (IsNotificationEnabled ());
			SetupSound ();
		}

        private bool isSoundsEnabled;
		private bool isScreenLockDuringAutoSpin;
		private bool isNotificationEnabled;
		private bool isJackpotAnnouncementsEnabled;
		private bool isPushWinnerEnabled;
        private readonly int DefaultSoundsEnabled = 1;
//		private readonly int DefaultWinnerNotificationEnabled = 1;
		private readonly int DefaultScreenLockDuringAutoSpin = 0;
		private readonly int DefaultNotificationEnabled = 1;
		private readonly int DefaultJackpotAnnouncementsEnabled = 0;
        private static readonly string SOUNDS_ENABLED_KEY = "soundsEnabled";
		private static readonly string SCREEN_LOCK_DURING_AUTO_SPIN_KEY = "screenLockDuringAutoSpin";
		private static readonly string NOTIFICATION_KEY = "notificationEnabled";
		private static readonly string JACKPOT_ANNOUNCEMENTS_KEY = "jackpotAnnouncementsEnabled";
		private const string  PUSH_WINNER_KEY = "winnerPushEnabled";
        private static Settings instance;
        private static object syncRoot = new System.Object ();
    }
}
