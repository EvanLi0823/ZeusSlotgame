// Assets/Editor/APKAutoBuilder.cs
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

[InitializeOnLoad]
public class APKAutoBuilder
{
    private const string CONFIG_PATH = "Assets/Editor/APKBuildConfig.asset";

    [MenuItem("Tools/Build APK")]
    public static void Build()
    {
        var config = LoadConfig();
        if (config == null) return;

        if (!ValidateConfig(config)) return;
        if (!CheckAndroidBuildTarget()) return;

        SetupPlayerSettings(config);
        BuildPlayer(config);
    }

    private static APKBuildConfig LoadConfig()
    {
        var config = AssetDatabase.LoadAssetAtPath<APKBuildConfig>(CONFIG_PATH);
        if (config == null)
        {
            config = ScriptableObject.CreateInstance<APKBuildConfig>();
            AssetDatabase.CreateAsset(config, CONFIG_PATH);
            Debug.Log("Created new APKBuildConfig at: " + CONFIG_PATH);
        }
        return config;
    }

    private static bool ValidateConfig(APKBuildConfig config)
    {
        // 新增校验规则
        if (string.IsNullOrEmpty(config.productName))
        {
            Debug.LogError("产品名称不能为空！");
            return false;
        }

        if (string.IsNullOrEmpty(config.bundleVersion))
        {
            Debug.LogError("版本号不能为空！");
            return false;
        }
        
        if (string.IsNullOrEmpty(config.keystorePath) || !File.Exists(config.keystorePath))
        {
            Debug.LogError("Keystore file not found!");
            return false;
        }

        if (string.IsNullOrEmpty(config.outputPath))
        {
            Debug.LogError("Output path not configured!");
            return false;
        }

        return true;
    }

    private static bool CheckAndroidBuildTarget()
    {
        if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android)
            return true;

        bool switchResult = EditorUtility.DisplayDialog(
            "Switch Platform",
            "需要切换到Android平台，是否继续？",
            "切换并构建", 
            "取消"
        );

        if (switchResult)
        {
            EditorUserBuildSettings.SwitchActiveBuildTarget(
                BuildTargetGroup.Android, 
                BuildTarget.Android
            );
            return false; // 需要重新触发构建
        }

        return false;
    }

    private static void SetupPlayerSettings(APKBuildConfig config)
    {
        // 设置应用信息
        PlayerSettings.bundleVersion = config.bundleVersion;
        PlayerSettings.productName = config.productName;
    
        // 自动生成VersionCode（可选）
        PlayerSettings.Android.bundleVersionCode = GenerateVersionCode(config.bundleVersion);
        // 自动签名配置
        PlayerSettings.Android.keystoreName = config.keystorePath;
        PlayerSettings.Android.keystorePass = config.keystorePassword;
        PlayerSettings.Android.keyaliasName = config.keyaliasName;
        PlayerSettings.Android.keyaliasPass = config.keyaliasPassword;
        PlayerSettings.Android.useCustomKeystore = true;
    }
    private static int GenerateVersionCode(string version)
    {
        try
        {
            string[] parts = version.Split('.');
            int major = int.Parse(parts[0]) * 10000;
            int minor = int.Parse(parts[1]) * 100;
            int patch = int.Parse(parts[2]);
            return major + minor + patch;
        }
        catch
        {
            Debug.LogWarning("自动生成VersionCode失败，使用默认值10000");
            return 10000;
        }
    }
    private static void BuildPlayer(APKBuildConfig config)
    {
        // 准备输出目录
        if (!Directory.Exists(config.outputPath))
            Directory.CreateDirectory(config.outputPath);

        // 生成带时间戳的文件名
        string fileName = $"{PlayerSettings.productName}_v{PlayerSettings.bundleVersion}_{System.DateTime.Now:yyyyMMddHHmm}.apk";
        string fullPath = Path.Combine(config.outputPath, fileName);

        // 收集启用场景
        List<string> scenes = new List<string>();
        foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
        {
            if (scene.enabled)
                scenes.Add(scene.path);
        }

        // 执行构建
        BuildPipeline.BuildPlayer(
            scenes.ToArray(),
            fullPath,
            BuildTarget.Android,
            BuildOptions.None
        );

        // 打开输出目录
        EditorUtility.RevealInFinder(fullPath);
    }
}


