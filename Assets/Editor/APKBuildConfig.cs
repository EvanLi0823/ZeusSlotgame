// Assets/Editor/APKBuildConfig.cs
using UnityEngine;

public class APKBuildConfig : ScriptableObject
{
    [Header("应用信息")]
    [Tooltip("示例: 1.0.0")]
    public string bundleVersion = "1.0.0";
    
    [Tooltip("产品显示名称")] 
    public string productName = "Zeus";
    
    [Tooltip("Keystore文件绝对路径")]
    public string keystorePath;
    
    [Tooltip("Keystore访问密码")]
    public string keystorePassword;
    
    [Tooltip("密钥别名")]
    public string keyaliasName;
    
    [Tooltip("别名密码")]
    public string keyaliasPassword;

    [Tooltip("APK输出目录（绝对路径）")]
    public string outputPath = "Builds/Android";
}