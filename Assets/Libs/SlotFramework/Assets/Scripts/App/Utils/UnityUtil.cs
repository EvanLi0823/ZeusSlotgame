using UnityEngine;
using System.Collections;
using App;
using System.Threading;
using System.Text;
using System.IO;
using App.SubSystems;
using System.Collections.Generic;
using System;


public class UnityUtil
{
	public static string CrashReport (string message, string stack)
	{
		var errorMessage = new StringBuilder ();

		errorMessage.AppendLine ("Slot: " + Application.platform);

		errorMessage.AppendLine ();
		errorMessage.AppendLine (message);
		errorMessage.AppendLine (stack);

		errorMessage.AppendFormat
			(
			"{0} {1} {2} {3}\n{4}, {5}, {6}, {7}x {8}\n{9}x{10} {11}dpi FullScreen {12}, {13}, {14} vmem: {15} Max Texture: {16}\n\nScene {17}, Unity Version {18}",
			SystemInfo.deviceModel,
			SystemInfo.deviceName,
			SystemInfo.deviceType,
			SystemInfo.deviceUniqueIdentifier,

			SystemInfo.operatingSystem,
			Application.systemLanguage,
			SystemInfo.systemMemorySize,
			SystemInfo.processorCount,
			SystemInfo.processorType,

			Screen.currentResolution.width,
			Screen.currentResolution.height,
			Screen.dpi,
			Screen.fullScreen,
			SystemInfo.graphicsDeviceName,
			SystemInfo.graphicsDeviceVendor,
			SystemInfo.graphicsMemorySize,
			SystemInfo.maxTextureSize,

			Application.loadedLevelName,
			Application.unityVersion
		);


		return errorMessage.ToString ();

	}


	public static void RegisterGlobalExceptionHandler ()
	{
		Application.logMessageReceived += delegate(string condition, string stackTrace, LogType type) {
			GlobalExceptionHandler (condition, stackTrace, type);
		};
	}

	public static void GlobalExceptionHandler (string condition, string stackTrace, LogType type)
	{
		if (type == LogType.Error || type == LogType.Exception) {
			Classic.Analytics.GetInstance ().LogEvent (type.ToString (), "report", UnityUtil.CrashReport (condition, stackTrace));
		}

	}

	public static string DumpPlatformInfo ()
	{
		string systemInfo = "SystemInfo ->" +
		                     string.Format ("Resolution:{0}x{1}@{2},", Screen.currentResolution.width, Screen.currentResolution.height, Screen.currentResolution.refreshRate) +
		                     "DeviceModel:" + SystemInfo.deviceModel + "," +
		                     "DeviceName:" + SystemInfo.deviceName + "," +
		                     "DeviceType:" + SystemInfo.deviceType.ToString () + "," +
		                     "DeviceUniqueId:" + SystemInfo.deviceUniqueIdentifier + "," +
		                     "GraphicsDeviceID:" + SystemInfo.graphicsDeviceID + "," +
		                     "GraphicsDeviceName:" + SystemInfo.graphicsDeviceName + "," +
		                     "GraphicsDeviceType:" + SystemInfo.graphicsDeviceType + "," +
		                     "GraphcisMeorySize:" + SystemInfo.graphicsMemorySize + "," +
		                     "GraphicsMultiThreaded:" + SystemInfo.graphicsMultiThreaded + "," +
		                     "graphicsShaderLevel:" + SystemInfo.graphicsShaderLevel + "," +
		                     "operatingSystem:" + SystemInfo.operatingSystem + "," +
		                     "processorCount:" + SystemInfo.processorCount + "," +
		                     "processorType:" + SystemInfo.processorType + "," +
		                     "supportedRenderTargetCount:" + SystemInfo.supportedRenderTargetCount + "," +
		                     "supportsInstancing:" + SystemInfo.supportsInstancing + "," +
		                     "supportsRenderTextures:" + SystemInfo.supportsRenderTextures;
		return systemInfo;
	}

	public static bool GetConfigurationData (ref IntPtr configueData, ref int dataSize)
	{
		return Plugins.NativeAPIs._GetConfigurationData (ref configueData, ref dataSize);
	}

	public static bool FreeDataPointer (IntPtr data)
	{
		return Plugins.NativeAPIs._FreeDataPointer (data);
	}

	public static string OSName {
		get {
			string osName = "Unknown";
			switch (Application.platform) {
			case RuntimePlatform.Android:
				osName = "Android";
				break;
			case RuntimePlatform.IPhonePlayer:
				osName = "iOS";
				break;
			case RuntimePlatform.OSXEditor:
			case RuntimePlatform.OSXPlayer:
				osName = "OSX";
				break;
			case RuntimePlatform.WebGLPlayer:
				osName = "WebGL";
				break;
			case RuntimePlatform.WindowsEditor:
			case RuntimePlatform.WindowsPlayer:
				osName = "Windows";
				break;

			}
			return osName;
		}

	}

	public static string ApplicationVersion {
		get {
			return Application.version;
		}
	}

	public static string persistentDataPath { get { return Application.persistentDataPath; } }

    public static string SavedConfigurationFileFullPath(string path, string fileName) {
        return UnityUtil.persistentDataPath + System.IO.Path.DirectorySeparatorChar +
            "Configs" + System.IO.Path.DirectorySeparatorChar +
            (string.IsNullOrEmpty(path) ? string.Empty : (path + System.IO.Path.DirectorySeparatorChar)) +
            fileName;
    }

    public static int CompareVersion (string version0, string version1)
	{
		if (version0 == null && version1 == null||
            (!string.IsNullOrEmpty(version0)&&!string.IsNullOrEmpty(version1)&&version0.Equals(version1))) {//相等判定，无论是版本号还是非版本号，均可
			return 0;
		} else if (string.IsNullOrEmpty(version0)) {
			return -1;
		} else if (string.IsNullOrEmpty(version1)) {
			return 1;
		} else {
			Version v0 = new Version (version0);
			Version v1 = new Version (version1);

			return v0.CompareTo (v1);
		}
	}
	public static T LoadPrefab<T> (string loadPath) where T:Component
	{
		UnityEngine.Object prefab = Resources.Load (loadPath);
		if (prefab == null) {
			return default(T);
		}

		GameObject dialogGO = GameObject.Instantiate (prefab) as GameObject;
		return (dialogGO.GetComponent<T> ());
	}
}
