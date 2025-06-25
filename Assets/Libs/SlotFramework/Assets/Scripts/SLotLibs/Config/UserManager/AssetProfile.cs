using System;
using System.IO;
using UnityEngine;

namespace Core
{
    public class AssetProfile
    {
        public const string AssetFormatV2 = "V2";
        public const string AssetFormatV1 = "V1";

        private static AssetProfile _instance;
        public static AssetProfile GetInstance()
        {
            if (_instance == null)
            {
                lock(typeof(AssetProfile))
                {
                    if (_instance == null)
                    {
                        _instance = new AssetProfile();
                    }
                }
            }
            return _instance;
        }

        private AssetProfile()
        {
            CanSupportAstcBunble = SupportAstcFormatAssetBundle();
        }

        public bool CanSupportAstcBunble { get; private set; }

        private bool SupportAstcFormatAssetBundle()
        {
#if UNITY_EDITOR
            return true;
#else
            return SupportAstcFormatAssetBundleDevice();
#endif
        }

        private bool SupportAstcFormatAssetBundleDevice()
        {
            var support = true;
            support = support && SystemInfo.SupportsTextureFormat(TextureFormat.ASTC_6x6);
            support = support && (SystemInfo.systemMemorySize >= 1024);
            Debug.Log("CanSupportAstcFormatAssetBundleDevice " + support);
            return support;
        }

        public string CorrectBundleAssetPath(string originPath, string assetFormatVersion)
        {
            if (CanSupportAstcBunble && assetFormatVersion == AssetFormatV2)
            {
                // Debug.Log("CorrectBundleAssetPath " + assetFormatVersion + " " + originPath);
                return Path.Combine(AssetFormatV2, originPath);
            }
            return originPath;
        }
    }
}