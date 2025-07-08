// Assets/Editor/APKBuildConfigCreator.cs
using UnityEngine;
using UnityEditor;
using System.IO;

public class APKBuildConfigCreator : EditorWindow
{
    private string keystorePath = "";
    private string bundleVersion = "";
    private string productName = "";
    private string keystorePass = "";
    private string keyaliasName = "";
    private string keyaliasPass = "";
    private string outputPath = "Builds/Android";
    // 原有字段保持不变...
    private const string CONFIG_PATH = "Assets/Editor/APKBuildConfig.asset";
    [MenuItem("Tools/APK Builder/Edit Configuration")] // 新增编辑菜单项
    private static void ShowWindow()
    {
        var window = GetWindow<APKBuildConfigCreator>("APK Config Creator");
        window.LoadExistingConfig(); 
        window.minSize = new Vector2(400, 300);
        window.Show();
    }
    private void LoadExistingConfig()
    {
        var config = AssetDatabase.LoadAssetAtPath<APKBuildConfig>(CONFIG_PATH);
        if (config != null)
        {
            productName = config.productName;
            bundleVersion = config.bundleVersion;
            keystorePath = config.keystorePath;
            keystorePass = config.keystorePassword;
            keyaliasName = config.keyaliasName;
            keyaliasPass = config.keyaliasPassword;
            outputPath = config.outputPath;
        }
        else
        {
            // 保持默认值
            outputPath = "Builds/Android";
        }
    }
    private void OnGUI()
    {
        GUILayout.Label("应用信息", EditorStyles.boldLabel);
        DrawAppInfoFields();
        
        GUILayout.Label("签名配置", EditorStyles.boldLabel);
        DrawKeystoreField();
        keystorePass = EditorGUILayout.PasswordField("Keystore 密码", keystorePass);
        keyaliasName = EditorGUILayout.TextField("密钥别名", keyaliasName);
        keyaliasPass = EditorGUILayout.PasswordField("别名密码", keyaliasPass);

        EditorGUILayout.Space(15);
        GUILayout.Label("输出设置", EditorStyles.boldLabel);
        DrawOutputPathField();

        EditorGUILayout.Space(25);
        DrawActionButtons();
    }
    private void DrawAppInfoFields()
    {
        productName = EditorGUILayout.TextField("产品名称", productName);
        bundleVersion = EditorGUILayout.TextField("版本号", bundleVersion);
    
        // 版本号校验提示
        if (!IsValidVersion(bundleVersion))
        {
            EditorGUILayout.HelpBox("推荐使用语义化版本格式 (示例: 1.0.0)", MessageType.Warning);
        }
    }

    private bool IsValidVersion(string version)
    {
        return !string.IsNullOrEmpty(version) && 
               System.Text.RegularExpressions.Regex.IsMatch(version, @"^\d+\.\d+\.\d+");
    }
    
    private void DrawKeystoreField()
    {
        EditorGUILayout.BeginHorizontal();
        keystorePath = EditorGUILayout.TextField("Keystore 路径", keystorePath);
        
        if (GUILayout.Button("选择", GUILayout.Width(50)))
        {
            string path = EditorUtility.OpenFilePanel("选择Keystore文件", "", "keystore");
            if (!string.IsNullOrEmpty(path))
            {
                keystorePath = path;
            }
        }
        EditorGUILayout.EndHorizontal();

        // 拖拽区域
        Rect dropArea = GUILayoutUtility.GetRect(0, 20, GUILayout.ExpandWidth(true));
        GUI.Box(dropArea, "拖拽.keystore文件到这里");
        HandleFileDrop(dropArea);
    }

    private void DrawOutputPathField()
    {
        EditorGUILayout.BeginHorizontal();
        outputPath = EditorGUILayout.TextField("输出路径", outputPath);
        
        if (GUILayout.Button("选择", GUILayout.Width(50)))
        {
            string path = EditorUtility.SaveFolderPanel("选择输出目录", outputPath, "");
            if (!string.IsNullOrEmpty(path))
            {
                outputPath = path;
            }
        }
        EditorGUILayout.EndHorizontal();

        // 拖拽文件夹区域
        Rect folderDropArea = GUILayoutUtility.GetRect(0, 20, GUILayout.ExpandWidth(true));
        GUI.Box(folderDropArea, "拖拽文件夹到这里");
        HandleFolderDrop(folderDropArea);
    }

    private void DrawActionButtons()
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        
        if (GUILayout.Button("保存配置", GUILayout.Width(120), GUILayout.Height(30)))
        {
            SaveConfig();
        }
        
        if (GUILayout.Button("取消", GUILayout.Width(80), GUILayout.Height(30)))
        {
            Close();
        }
        
        EditorGUILayout.EndHorizontal();
    }

    private void HandleFileDrop(Rect dropArea)
    {
        Event evt = Event.current;
        if (!dropArea.Contains(evt.mousePosition)) return;

        switch (evt.type)
        {
            case EventType.DragUpdated:
                DragAndDrop.visualMode = IsValidFileDrag() ? DragAndDropVisualMode.Copy : DragAndDropVisualMode.Rejected;
                evt.Use();
                break;

            case EventType.DragPerform:
                if (IsValidFileDrag())
                {
                    DragAndDrop.AcceptDrag();
                    keystorePath = DragAndDrop.paths[0];
                    evt.Use();
                }
                break;
        }
    }

    private void HandleFolderDrop(Rect dropArea)
    {
        Event evt = Event.current;
        if (!dropArea.Contains(evt.mousePosition)) return;

        switch (evt.type)
        {
            case EventType.DragUpdated:
                DragAndDrop.visualMode = IsValidFolderDrag() ? DragAndDropVisualMode.Copy : DragAndDropVisualMode.Rejected;
                evt.Use();
                break;

            case EventType.DragPerform:
                if (IsValidFolderDrag())
                {
                    DragAndDrop.AcceptDrag();
                    outputPath = DragAndDrop.paths[0];
                    evt.Use();
                }
                break;
        }
    }

    private bool IsValidFileDrag()
    {
        return DragAndDrop.paths != null 
            && DragAndDrop.paths.Length == 1 
            && Path.GetExtension(DragAndDrop.paths[0]) == ".keystore";
    }

    private bool IsValidFolderDrag()
    {
        if (DragAndDrop.paths == null || DragAndDrop.paths.Length != 1) return false;
        return Directory.Exists(DragAndDrop.paths[0]);
    }

    private void SaveConfig()
    {
        if (!ValidateInput()) return;
        // 在保存前再次加载最新配置以确保数据同步
        var existingConfig = AssetDatabase.LoadAssetAtPath<APKBuildConfig>(CONFIG_PATH);
        if (existingConfig != null)
        {
            // 更新现有配置而不是创建新实例
            existingConfig.keystorePath = keystorePath;
            existingConfig.keystorePassword = keystorePass;
            existingConfig.keyaliasName = keyaliasName;
            existingConfig.keyaliasPassword = keyaliasPass;
            existingConfig.outputPath = outputPath;
            
            EditorUtility.SetDirty(existingConfig);
            AssetDatabase.SaveAssets();
            
            EditorUtility.DisplayDialog("保存成功", "配置已更新！", "确定");
            Close();
        }
        else
        {
            string assetPath = "Assets/Editor/APKBuildConfig.asset";
            bool configExists = File.Exists(assetPath);

            // 处理已存在配置文件的情况
            if (configExists)
            {
                if (!EditorUtility.DisplayDialog("配置文件已存在", 
                        "是否覆盖现有配置文件？", "覆盖", "取消"))
                {
                    return;
                }
        
                // 强制删除原有文件
                if (!AssetDatabase.DeleteAsset(assetPath))
                {
                    EditorUtility.DisplayDialog("错误", 
                        "无法删除旧配置文件，请手动删除后重试", "确定");
                    return;
                }
        
                // 等待资源数据库刷新
                AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
            }

            // 创建新配置
            var config = ScriptableObject.CreateInstance<APKBuildConfig>();
            config.keystorePath = keystorePath;
            config.keystorePassword = keystorePass;
            config.keyaliasName = keyaliasName;
            config.keyaliasPassword = keyaliasPass;
            config.outputPath = outputPath;

            // 确保目录存在
            string folderPath = Path.GetDirectoryName(assetPath);
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            // 保存新资产
            AssetDatabase.CreateAsset(config, assetPath);
    
            // 添加延迟确保保存完成
            EditorApplication.delayCall += () =>
            {
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
                EditorUtility.DisplayDialog("保存成功", 
                    $"配置已保存到：{assetPath}", "确定");
                Close();
            };
        }
    }


    private bool ValidateInput()
    {
        if (string.IsNullOrEmpty(keystorePath) || !File.Exists(keystorePath))
        {
            EditorUtility.DisplayDialog("错误", "请选择有效的keystore文件", "确定");
            return false;
        }

        if (string.IsNullOrEmpty(keystorePass) || string.IsNullOrEmpty(keyaliasPass))
        {
            EditorUtility.DisplayDialog("错误", "密码不能为空", "确定");
            return false;
        }

        try
        {
            string fullPath = Path.GetFullPath(outputPath);
        }
        catch
        {
            EditorUtility.DisplayDialog("错误", "输出路径无效", "确定");
            return false;
        }

        return true;
    }
}
