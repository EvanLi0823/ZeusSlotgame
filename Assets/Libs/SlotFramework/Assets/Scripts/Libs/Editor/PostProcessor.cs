using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Build;
using UnityEditor;
using System.IO;
using System;

public class PostProcessor : IPostprocessBuild
{
    public int callbackOrder { get; }

    public void OnPostprocessBuild(BuildTarget target, string path)
    {
        MoveInTextAssets();
    }

    static void MoveInTextAssets()
    {
        string plistSrcPath = Path.Combine(Environment.CurrentDirectory, "Assets/Resources/Plist/MachineSlots/");
        string plistDstPath = Path.Combine(Environment.CurrentDirectory, "PackageTextAssets/MachineSlots/");

        if (Directory.Exists(plistDstPath))
        {
            if (Directory.Exists(plistSrcPath))
            {
                Directory.Delete(plistSrcPath, true);
            }

            Directory.Move(plistDstPath, plistSrcPath);
        }

        string configSrcPath = Path.Combine(Environment.CurrentDirectory, GameConstants.COMMON_CONFIG_PATH);
        string configDstPath = Path.Combine(Environment.CurrentDirectory, "PackageTextAssets/GameConfig.plist.xml");
        string differentConfigSrcPath = Path.Combine(Environment.CurrentDirectory, GameConstants.CLASSIC_CONFIG_PATH);
        string differentConfigDstPath =
            Path.Combine(Environment.CurrentDirectory, "PackageTextAssets/Config_classicIOS.plist.xml");

        if (File.Exists(configDstPath))
        {
            File.Move(configDstPath, configSrcPath);
        }

        if (File.Exists(differentConfigDstPath))
        {
            File.Move(differentConfigDstPath, differentConfigSrcPath);
        }
    }
}