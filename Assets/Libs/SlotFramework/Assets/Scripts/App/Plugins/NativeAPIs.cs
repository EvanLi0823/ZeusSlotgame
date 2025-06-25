#if UNITY_EDITOR
#define ENABLE_SIM_PAYMENT
#endif
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
#if ENABLE_SIM_PAYMENT || UNITY_ANDROID
#endif


namespace Plugins
{
    public class NativeAPIs
    {
#if UNITY_EDITOR || UNITY_EDITOR_OSX || UNITY_STANDALONE || UNITY_WEBGL || UNITY_IOS

        public static void _OpenBrowser(string url)
        {
            Application.OpenURL(url);
        }

        #region Configuration APIs

        public static bool _GetConfigurationData(ref IntPtr configueData, ref int dataSize)
        {
            WarningUnimplementedMethod();
            return false;
        }

        public static bool _FreeDataPointer(IntPtr data)
        {
            WarningUnimplementedMethod();
            return false;
        }

        #endregion

        #region Settings

        public static void _RateUs()
        {
            WarningUnimplementedMethod();
        }

        public static void _EnableAutomaticLockScreen(bool isAutomoatic)
        {
            WarningUnimplementedMethod();
        }

        public static bool _IsAutomaticLockScreen()
        {
            WarningUnimplementedMethod();
            return false;
        }

        public static void _EnableNotification(bool isEnable)
        {
            WarningUnimplementedMethod();
        }

        public static bool _IsEnableNotification()
        {
            WarningUnimplementedMethod();
            return false;
        }

        #endregion

        #region Mail

        public static bool _CanSendEmail()
        {
            WarningUnimplementedMethod();
            return false;
        }

        public static bool _SendMail(string recipeints, string subject, string body)
        {
            WarningUnimplementedMethod();
            return false;
        }

        #endregion

        #region System APIs

        public static int _SystemUptime()
        {
            WarningUnimplementedMethod();
            return (int)Time.realtimeSinceStartup;
        }


        public static double _AppInstallTime()
        {
            return (double)Time.realtimeSinceStartup;
        }

        public static string _LocalizedString(string str, string defaultStr, string local)
        {
            WarningUnimplementedMethod();
            return defaultStr;
        }

        #endregion

        #region openURL

        public static bool _CanOpenUrl(string url)
        {
            return false;
        }

        #endregion

        #region Rtot

        public static void _SendActionToRtot(string action)
        {
            WarningUnimplementedMethod();
        }

        public static bool _IsRtotContentReady()
        {
            return false;
        }

        public static string _GetRtotContent()
        {
            return string.Empty;
        }

        public static string _GetUserInfo()
        {
            return string.Empty;
        }

        #endregion

        public static void SetGdprGranted(bool isGranted)
        {
        }

        public static void _ShowTrackingTransparency()
        {
        }

        public static int _TrackingTransparencyStatus()
        {
            return 0;
        }

#elif UNITY_ANDROID
        public static string _GetAndroidId()
        {
	        return "";
        }
        
		public static void _OpenBrowser (string url)
		{
		}

        #region Configuration APIs
        public static bool _GetConfigurationData (ref IntPtr configueData, ref int dataSize)
        {
            WarningUnimplementedMethod();
            return false;
        }
        
        public static bool _FreeDataPointer (IntPtr data)
        {
            WarningUnimplementedMethod();
            return false;
        }
        #endregion
        
        
        #region Analytics APIs
        
        public static bool _EndAnalytics ()
        {
            return true;
        }
        #endregion
        
        #region Settings

        public static void _RateUs ()
        {
        }
        
        public static void _EnableAutomaticLockScreen(bool isAutomoatic)
        {
        }
        
        public static bool _IsAutomaticLockScreen()
        {
            return true;
        }
        
        public static void _EnableNotification(bool isEnable)
        {
        }
        
        public static bool _IsEnableNotification()
        {
            return true;
        }
        #endregion
        
        #region Mail
        public static bool _CanSendEmail()
        {
            return true;
        }
        public static bool _SendMail(string recipeints,string subject, string body)
        {
            return true;
        }
        #endregion
        
        #region System APIs
        public static int _SystemUptime()
        {
			AndroidJNI.AttachCurrentThread();
			AndroidJavaClass androidClass = new AndroidJavaClass("android.os.SystemClock");
			int s = (int) (androidClass.CallStatic<long>("elapsedRealtime")/1000);
			return s;
        }
        
        public static double _AppInstallTime() {
			return 0.0;
		}
        
        public static string _LocalizedString(string str, string defaultStr,string local)
        {
            return defaultStr;
        }
        
        #endregion

		public static string _GetConfigString(){
			return "";
		}

		public static bool _CanOpenUrl(string url)
		{
			return false;
		}

        #region Rtot

		public static void _SendActionToRtot(string action) {
		}

		public static bool _IsRtotContentReady() {
		return false;
		}

		public static string _GetRtotContent() {
		return string.Empty;
		}

        #endregion

		public static string _GetUserInfo()
		{
			return "";
		}

		public static  void	SetGdprGranted(bool isGranted){
		  
		}
		public static void _ShowTrackingTransparency()
		{
		}
	    public static int _TrackingTransparencyStatus() {
			return 0;
		}

#else
  //       public static void _OpenBrowser (string url)
  //       {
  //           Application.OpenURL(url);
  //       }
  //       #region Configuration APIs
  //       [DllImport ("__Internal")]
  //       public static extern bool _GetConfigurationData (ref IntPtr configueData, ref int dataSize);
  //
  //       [DllImport ("__Internal")]
  //       public static extern bool _FreeDataPointer (IntPtr data);
  //       #endregion
  //
  //       #region Settings
  //
  //       [DllImport ("__Internal")]
  //       public static extern void _RateUs ();
  //
  //       [DllImport ("__Internal")]
  //       public static extern void _EnableAutomaticLockScreen(bool isAutomoatic);
  //
  //       [DllImport ("__Internal")]
		// public static extern bool _IsAutomaticLockScreen();
  //
  //       [DllImport ("__Internal")]
		// public static extern void _EnableNotification(bool isEnable);
  //       
  //       [DllImport ("__Internal")]
		// public static extern bool _IsEnableNotification();
  //       #endregion
  //
  //       #region Mail API
  //       [DllImport ("__Internal")]
  //       public static extern bool _CanSendEmail();
  //   
  //       [DllImport ("__Internal")]
  //       public static extern bool _SendMail(string recipeints,string subject, string body);
  //       #endregion
  //
  //       #region System APIs
  //       [DllImport ("__Internal")]
  //       public static extern int _SystemUptime();
  //       [DllImport ("__Internal")]
		// public static extern double _AppInstallTime();
  //       [DllImport ("__Internal")]
  //       public static extern string _LocalizedString(string str, string defaultStr,string local);
  //       #endregion
  //
		// [DllImport ("__Internal")]
		// public static extern void _GetConfigString ();
  //
		// [DllImport ("__Internal")]
		// public static extern bool _CanOpenUrl(string url);
  //
		// [DllImport ("__Internal")]
  //       public static extern void _SendActionToRtot(string action);
  //
		// [DllImport ("__Internal")]
		// public static extern bool _IsRtotContentReady();
  //
  //       [DllImport ("__Internal")]
		// public static extern string _GetRtotContent();
  //
		// [DllImport ("__Internal")]
		// public static extern string _GetUserInfo();
  //
		// [DllImport ("__Internal")]
		// public static extern void SetGdprGranted(bool isGranted);
	 //    
	 //    [DllImport ("__Internal")]
		// public static extern void _ShowTrackingTransparency();
	 //    
	 //    [DllImport ("__Internal")]
		// public static extern int _TrackingTransparencyStatus();

#endif

        private static void WarningUnimplementedMethod()
        {
        }
        
#pragma warning disable
        static global::Utils.Logger logger = global::Utils.Logger.GetUnityDebugLogger(typeof(NativeAPIs), false);
#pragma warning restore
    }
}