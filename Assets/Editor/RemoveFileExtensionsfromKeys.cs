using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

public static class AddressablesKeyRemoveExtension
{
    [MenuItem("Tools/Addressables/Remove All File Extensions (Recursive)")]
    public static void RemoveAllFileExtensionsRecursive()
    {
        AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
        if (settings == null)
        {
            Debug.LogError("Addressable Settings not found!");
            return;
        }

        // 需要保留后缀的文件类型（如.json需要保留）
        string[] keepExtensions = { ".json", ".mp3", ".wav" }; 

        foreach (var group in settings.groups)
        {
            // 递归处理所有子资源
            ProcessGroupEntriesRecursive(group, keepExtensions);
        }

        AssetDatabase.SaveAssets();
        Debug.Log("Addressables Keys 已递归移除文件后缀！");
    }

    private static void ProcessGroupEntriesRecursive(AddressableAssetGroup group, string[] keepExtensions)
    {
        foreach (var entry in group.entries.ToList())
        {
            string originalAddress = entry.address;
            string extension = Path.GetExtension(originalAddress);

            // 跳过需要保留后缀的文件
            if (keepExtensions.Contains(extension)) 
                continue;

            // 去掉后缀（包括子文件夹路径中的资源）
            string newAddress = Path.Combine(
                Path.GetDirectoryName(originalAddress),
                Path.GetFileNameWithoutExtension(originalAddress)
            ).Replace("\\", "/"); // 确保路径格式统一

            if (newAddress != originalAddress)
            {
                entry.address = newAddress;
                Debug.Log($"Key修改: {originalAddress} -> {newAddress}", entry.TargetAsset);
            }
        }
    }
}