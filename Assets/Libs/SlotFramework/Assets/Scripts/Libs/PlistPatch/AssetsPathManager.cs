using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif
using Core;

/// <summary>
/// Assets path manager.
/// 1.主要目的是统一管理访问的路径。对路径信息进行解读和合并。易于模块化管理，复用性更强 （起因：目前感觉路径代码结构过于混乱，而且路径拼装过于频繁，统一管理比较差）
/// 2.目前针对资源下载到本地后，有一个路径管理器类，也可以是本地的路径管理
/// 3.注意实现时，各个平台可能实现不同 注意区分
/// 4.为了让资源路径管理用起来更加灵活，默认不包含文件名。
/// 5.现在随着重构，主要用于自动化生成校验数据文件，保存文件等情况
/// </summary>
namespace Libs
{
    public class AssetsPathManager
    {
        #region Plist Make

        public const string PLIST_OUTPUT_PATH = "Resources/Plist/";
        public static string VERSION_FILE_NAME = "PlistVersions.txt";
        public static string COMPRESSED_VERSION_FILE_NAME = "PlistVersions.zip";
        public static string PLIST_FILE_NAME = "{0}.plist.zip";
        public static string PLIST_TMP_FILE_NAME = "{0}.plist.zip_tmp";

        public static string GetPlistCompressedCheckFilePath()
        {
            return Path.Combine(GetPlistBaseOutputPath(), COMPRESSED_VERSION_FILE_NAME);
        }

        public static string GetPlistCompressedFilePath()
        {
            return Path.Combine(GetPlistBaseOutputPath(), "CompressedPlist");
        }

        public static string GetPlistRawCheckFilePath()
        {
            return Path.Combine(GetPlistBaseOutputPath(), VERSION_FILE_NAME);
        }

        public static string GetPlistRawFilePath()
        {
            return Path.Combine(GetPlistBaseOutputPath(), "RawPlist/");
        }

        public static string GetPlistResourcePathInPackage()
        {
            return Application.dataPath + "/" + PLIST_OUTPUT_PATH + "MachineSlots" + "/";
        }

        public static string GetPlistResourcePathInPackageOnMobile()
        {
            return "Plist" + "/" + "MachineSlots" + "/";
        }

        public static string GetPlistFinalResourcePathInPackageOnMobile()
        {
            return "Plist" + "/" + "Machines" + "/";
        }

        public static string GetPlistRawCheckFilePathOnMobile()
        {
            return Path.Combine("Plist/", "PlistVersions");
        }

        public static string GetPlistBaseOutputPath()
        {
            return Path.Combine(Application.dataPath, PLIST_OUTPUT_PATH);
        }

        #endregion

        #region Plist Download

        public static string GetMachineLocaRemotelPlistPath(string machineName)
        {
            return Path.Combine(Application.persistentDataPath,
                Path.Combine("Plist", Path.Combine("Machines", string.Format(PLIST_FILE_NAME, machineName))));
        }

        public static string GetMachineLocalRemotePlistRelativePath(string machineName)
        {
            return Path.Combine("Plist", Path.Combine("Machines", string.Format(PLIST_FILE_NAME, machineName)));
        }

        public static string GetMachineLocalRemotePlistTmpSavePath(string machineName)
        {
            return Path.Combine(Application.persistentDataPath,
                Path.Combine("Plist", Path.Combine("Machines", string.Format(PLIST_TMP_FILE_NAME, machineName))));
        }
        #endregion

        #region local Log

        public static string GetMachineLocalLogFileSavePath()
        {
            string fileName = Application.productName.Replace(" ", "") +
                              System.DateTime.Now.Date.ToString("_yyyy-MM-dd") + ".profile";
            return Path.Combine(Application.persistentDataPath, Path.Combine("Log", fileName));
        }

        #endregion

        #region Image Make

        #endregion

        public static string GetPlatformName()
        {
#if UNITY_EDITOR
            return GetPlatformForAssets(EditorUserBuildSettings.activeBuildTarget);
#else
			return GetPlatformForAssets(Application.platform);
#endif
        }

        public const string PLATFROM_ANDROID = "Android";
        public const string PLATFROM_IOS = "iOS";
        public const string PLATFROM_WEB_GL = "WebGL";
        public const string PLATFROM_WEB_PLAYER = "WebPlayer";
        public const string PLATFROM_WINDOWS = "Windows";
        public const string PLATFROM_OSX = "OSX";
#if UNITY_EDITOR
        private static string GetPlatformForAssets(BuildTarget target)
        {
            switch (target)
            {
                case BuildTarget.Android:
                    return PLATFROM_ANDROID;
                case BuildTarget.iOS:
                    return PLATFROM_IOS;
                case BuildTarget.WebGL:
                    return PLATFROM_WEB_GL;
                //case BuildTarget.WebPlayer:
                //return PLATFROM_WEB_PLAYER;
                case BuildTarget.StandaloneWindows:
                case BuildTarget.StandaloneWindows64:
                    return PLATFROM_WINDOWS;
                case BuildTarget.StandaloneOSXIntel:
                case BuildTarget.StandaloneOSXIntel64:
                case BuildTarget.StandaloneOSX:
                    return PLATFROM_OSX;
                // Add more build targets for your own.
                // If you add more targets, don't forget to add the same platforms to GetPlatformForAssetBundles(RuntimePlatform) function.
                default:
                    return null;
            }
        }
#endif

        public static string GetInitFileKeyPath()
        {
            return "launch.config";
        }

        private static string GetPlatformForAssets(RuntimePlatform platform)
        {
            switch (platform)
            {
                case RuntimePlatform.Android:
                    return PLATFROM_ANDROID;
                case RuntimePlatform.IPhonePlayer:
                    return PLATFROM_IOS;
                case RuntimePlatform.WebGLPlayer:
                    return PLATFROM_WEB_GL;
                //case RuntimePlatform.OSXWebPlayer:
                //case RuntimePlatform.WindowsWebPlayer:
                //return PLATFROM_WEB_PLAYER;
                case RuntimePlatform.WindowsPlayer:
                    return PLATFROM_WINDOWS;
                case RuntimePlatform.OSXPlayer:
                    return PLATFROM_OSX;
                // Add more build targets for your own.
                // If you add more targets, don't forget to add the same platforms to GetPlatformForAssetBundles(RuntimePlatform) function.
                default:
                    return null;
            }
        }
    }
}