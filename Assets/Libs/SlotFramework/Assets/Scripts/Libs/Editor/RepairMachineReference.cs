using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using System.Linq;
using UnityEditor;
using System.IO;
using System;

public class AssetsReference
{
    private static string MACHINES_RESOURCE_PATH = "Assets/Machines";
    private static string MACHINES_SCENES_PATH = "Assets/Scenes";
    private static string MACHINES_MATCH_FORMAT = "Assets/Machines/";
    private const string NO_CHANGE_FOLDOR = "ManualPicture";
    private static string currentSceneName = "";

    /// <summary>
    /// Finds the prefab peferences.
    /// 目前是直接找直接引用，间接引用的话，需要将找出的资源进行递归查找。
    /// </summary>
    [MenuItem("Assets/Find Prefab References", false, 100)]
    static private void FindPrefabPeferences()
    {
        //1.先获取prefab路径
        string prefabPath =
            Path.Combine(Environment.CurrentDirectory, AssetDatabase.GetAssetPath(Selection.activeObject));
        Debug.Log("FindPrefabPeferences prefabPath:" + prefabPath);
        if (!File.Exists(prefabPath))
        {
            Debug.LogError("FindPrefabPeferences prfabPath is invalid!");
            return;
        }

        try
        {
            ///读取原数据
            MemoryStream mem = new MemoryStream(File.ReadAllBytes(prefabPath));
            StreamReader sr = new StreamReader(mem);
            Dictionary<string, int> dict = new Dictionary<string, int>();
            string str = sr.ReadLine();
            ///判定文件是否是末尾，可以通过判定为null来表示，不能够通过空串表示，上述英文描述来自MSDN
            while (str != null)
            {
                string guid = GetGUIDFromContext(str);
                if (!string.IsNullOrEmpty(guid))
                {
                    //Debug.Log ("guid:"+guid);
                    string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                    if (!string.IsNullOrEmpty(assetPath))
                    {
                        if (!dict.ContainsKey(assetPath))
                        {
                            dict.Add(assetPath, 0);
                        }
                    }
                }

                str = sr.ReadLine();
            }

            sr.Close();
            mem.Close();
            foreach (string item in dict.Keys)
            {
                Debug.Log(Selection.activeObject.name + ":" + item);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Exception:" + ex.Message + " stack:" + ex.StackTrace);
        }
    }

    [MenuItem("Assets/Repair Machine Assets References", false, 100)]
    static private void RepairMachineAssetsReferences()
    {
        ReMappingMachinesAssetsReference(Selection.activeObject.name);
        ReMappingSlotMachineImagePackagingTag(Selection.activeObject.name);
        AssetDatabase.Refresh();
    }

    [MenuItem("Assets/Repair All Machine Assets References", false, 100)]
    static public void RepairAllMachinesAssetsReference()
    {
        List<string> withExtensions = new List<string>() { ".unity" };
        string scenesPath = Path.Combine(System.Environment.CurrentDirectory, MACHINES_SCENES_PATH);
        string[] files = Directory.GetFiles(scenesPath, "*.*", SearchOption.AllDirectories)
            .Where(s => withExtensions.Contains(Path.GetExtension(s).ToLower())).ToArray();
        Debug.Log("path:" + files.Length);
        for (int i = 0; i < files.Length; i++)
        {
            string sceneName = Path.GetFileNameWithoutExtension(files[i]);
            Debug.Log("path:" + files[0] + " sceneName:" + sceneName);
            ReMappingMachinesAssetsReference(sceneName);
            ReMappingSlotMachineImagePackagingTag(sceneName);
        }

        AssetDatabase.Refresh();
    }

    static private void ReMappingMachinesAssetsReference(string sceneName)
    {
        Debug.Log("开始处理关卡:" + sceneName);
        EditorSettings.serializationMode = SerializationMode.ForceText;
        string path = Path.Combine(MACHINES_SCENES_PATH, sceneName + ".unity");
        string resourcePath = Path.Combine(MACHINES_RESOURCE_PATH, sceneName);
        string assetPath = GetFileFullPath(resourcePath);
        //Debug.Log ("assetPath:"+assetPath+" ok:"+Directory.Exists(assetPath));
        if (!string.IsNullOrEmpty(resourcePath) && resourcePath.StartsWith(MACHINES_RESOURCE_PATH) &&
            Directory.Exists(assetPath))
        {
            ReMappingGUID(path, sceneName); //处理Unity场景文件的关联引用
            List<string> withExtensions = new List<string>()
                { ".prefab", ".unity", ".mat", ".controller", ".anim", ".asset", ".fontsettings" }; //asset为字体文件
            string[] files = Directory.GetFiles(GetMachineAssetsPath(sceneName), "*.*", SearchOption.AllDirectories)
                .Where(s => withExtensions.Contains(Path.GetExtension(s).ToLower())).ToArray();
            //Debug.Log ("sceneName:"+sceneName+" file.Count:"+files.Length);
            int startIndex = 0;
            for (; startIndex < files.Length; startIndex++)
            {
                string file = files[startIndex];
                EditorUtility.DisplayCancelableProgressBar("重新关联资源中。。。", file, (float)startIndex / (float)files.Length);
                string relativePath = file.Replace(System.Environment.CurrentDirectory + "/", "");
                ReMappingGUID(relativePath, sceneName);
            }

            EditorUtility.ClearProgressBar();
        }

        Debug.Log("重新关联引用结束" + sceneName);
    }

    //static int count = 0;
    //static int rowcount = 0;
    /// <summary>
    /// Res the mapping GUI.
    /// 根据序列化资源文件都为YAML格式。而此格式每行只可能有一个guid，所以不需要考虑guid同一行会存在多个的情况。
    /// </summary>
    /// <param name="fileRelativePath">File relative path.</param>
    static private void ReMappingGUID(string fileRelativePath, string sceneName)
    {
        string filePath = GetFileFullPath(fileRelativePath);
        //Debug.Log ("fileRelativePath:"+fileRelativePath+" filePath:"+filePath);
        if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath)) return;

        try
        {
            ///读取原数据
            MemoryStream mem = new MemoryStream(File.ReadAllBytes(filePath));
            StreamReader sr = new StreamReader(mem);
            ///用于存储修改数据，待所有数据处理完毕后，在写入到指定的文件内
            //Debug.Log("ReMappingGUID:"+filePath+" size(byte):"+mem.Length);
            MemoryStream saveMem = new MemoryStream(File.ReadAllBytes(filePath));
            StreamWriter sw = new StreamWriter(saveMem);
//			if (fileRelativePath.Contains("PaytablePanel")) {
//				count =0;
//				rowcount =0;
//			}
            string str = sr.ReadLine();
            ///  // Read and display lines from the file until the end of the file is reached.   while ((line = sr.ReadLine()) != null) 
            ///判定文件是否是末尾，可以通过判定为null来表示，不能够通过空串表示，上述英文描述来自MSDN
            while (str != null)
            {
                string guid = GetGUIDFromContext(str);
//				if (fileRelativePath.Contains("PaytablePanel")) {
//					rowcount++;
//				}
//				if (fileRelativePath.Contains("PaytablePanel")&&!string.IsNullOrEmpty(guid)) {
//					Debug.Log("fileRelativePath:"+fileRelativePath+" guid:"+guid);
//					count++;
//				}
                if (!string.IsNullOrEmpty(guid))
                {
                    //Debug.Log ("guid:"+guid);
                    string assetPath = AssetDatabase.GUIDToAssetPath(guid);
//					if (fileRelativePath.Contains("MiddlePanel")) {
//						Debug.Log("assetPath:"+assetPath+" guid:"+guid);
//					}
                    if (!string.IsNullOrEmpty(assetPath))
                    {
                        string newPath = GetMatchMachineAssetsPath(assetPath, sceneName, fileRelativePath);
                        //Debug.Log("newPath:"+newPath);
                        if (!string.IsNullOrEmpty(newPath))
                        {
                            string newGuid = AssetDatabase.AssetPathToGUID(newPath);
                            str = str.Replace(guid, newGuid);
                            //Debug.Log ("str:"+str);
                        }
                    }
                }

                sw.WriteLine(str);
                str = sr.ReadLine();
            }

//			if (fileRelativePath.Contains("PaytablePanel")) {
//				Debug.Log("Total:"+fileRelativePath+" count:"+count+" rowCount:"+rowcount);
//
//			}
            sr.Close();
            sw.Flush();
            sw.Close();
            File.WriteAllBytes(filePath, saveMem.ToArray());
            mem.Close();
            saveMem.Close();
        }
        catch (Exception ex)
        {
            Debug.LogError("Exception:" + ex.Message + " stack:" + ex.StackTrace);
        }
    }

    static private string GetFileFullPath(string fileRelativePath)
    {
        return Path.Combine(System.Environment.CurrentDirectory, fileRelativePath);
    }

    static private string GetMachineAssetsPath(string sceneName)
    {
        return Path.Combine(System.Environment.CurrentDirectory, Path.Combine(MACHINES_RESOURCE_PATH, sceneName));
    }

    static private string GetGUIDFromContext(string text)
    {
        Regex regex = new Regex("guid: ([a-fA-F0-9]{32})");
        Match match = regex.Match(text);

        if (match.Success)
        {
            return match.Value.Substring(6);
        }

        return "";
    }

    static private string GetMatchMachineAssetsPath(string path, string mathName, string assetFile)
    {
        if (!path.Contains(MACHINES_RESOURCE_PATH)) return "";
        if (!path.Contains('/')) return "";
        string temp = path.Substring(path.IndexOf('/') + 1);
        if (!temp.Contains('/')) return "";
        temp = temp.Substring(temp.IndexOf('/') + 1);
        if (!temp.Contains('/')) return "";
        string preSlotName = temp.Substring(0, temp.IndexOf('/'));
        //Debug.Log ("preSlotName:"+preSlotName);
        string newPath = path.Replace("/" + preSlotName + "/", "/" + mathName + "/"); //防止文件名中的相关字眼被替换
        string filefullPath = Path.Combine(System.Environment.CurrentDirectory, newPath);
        //Debug.Log ("GetMatchMachineAssetsPath newPath:"+newPath+" filefullPath:"+filefullPath);
        if (!newPath.Equals(path))
        {
            if (File.Exists(filefullPath))
            {
                return newPath;
            }
            else
            {
                Debug.Log("请美术修复相关资源引用 资源:" + assetFile + "引用了资源:" + path);
            }
        }

        return "";
    }

    static private void ReMappingSlotMachineImagePackagingTag(string sceneName)
    {
        string machineResourcePath =
            Path.Combine(System.Environment.CurrentDirectory, MACHINES_MATCH_FORMAT + sceneName);
        string[] files = Directory.GetFiles(machineResourcePath, "*.*", SearchOption.AllDirectories).Where(s =>
        {
            if (s.EndsWith(".meta") || s.EndsWith(".DS_Store"))
            {
                return false;
            }

            //if(s.Contains(NO_CHANGE_FOLDOR)) return false;
            if (s.EndsWith(".png") || s.EndsWith(".jpg") || s.EndsWith("bmp") || s.EndsWith("exr") ||
                s.EndsWith(".tga") || s.EndsWith(".tiff") || s.EndsWith("psd") || s.EndsWith("iff"))
            {
                return true;
            }

            return false;
        }).ToArray();

        string splitString = "Assets/";
        for (int i = 0; i < files.Length; i++)
        {
            string fullName = files[i].Substring(files[i].IndexOf(splitString));
            TextureImporter importer = AssetImporter.GetAtPath(fullName) as TextureImporter;
            string fileName = Path.GetFileNameWithoutExtension(fullName);
            //Debug.Log ("fileName:"+fileName);
            string prePackingTag = importer.spritePackingTag;
            //Debug.Log ("orign:"+prePackingTag);
            prePackingTag = RemovePreFix(prePackingTag);
            //Debug.Log ("modify:"+prePackingTag);
            string curPackingTag = sceneName + "_" + prePackingTag;
            if (string.IsNullOrEmpty(prePackingTag))
            {
                Debug.Log("图片:" + fullName + " 没有设置图集，默认图集为机器名，考虑当前关卡播Sysmbol动画是否，会发生不一致情况。如果有请去掉图集名。");
                curPackingTag = sceneName;
            }

            importer.spritePackingTag = curPackingTag;
            importer.SaveAndReimport();
        }
    }

    static private string RemovePreFix(string prePackingTag)
    {
        if (string.IsNullOrEmpty(prePackingTag))
            return "";
        string machineResourcePath = Path.Combine(System.Environment.CurrentDirectory, MACHINES_SCENES_PATH);
        string[] files = Directory.GetFiles(machineResourcePath, "*.*", SearchOption.TopDirectoryOnly).Where(s =>
        {
            if (s.EndsWith(".meta") || s.EndsWith(".DS_Store"))
            {
                return false;
            }

            return true;
        }).ToArray();
        string longestKey = "";
        foreach (string item in files)
        {
            string key = Path.GetFileNameWithoutExtension(item);
            if (prePackingTag.Contains(key) && longestKey.Length < key.Length)
            {
                longestKey = key;
            }
        }

        if (string.IsNullOrEmpty(longestKey))
            return prePackingTag;
        return prePackingTag.Replace(longestKey, "").TrimStart('_');
    }
}