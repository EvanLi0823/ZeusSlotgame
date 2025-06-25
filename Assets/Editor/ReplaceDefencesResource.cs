using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using System.Text.RegularExpressions;
using Object = UnityEngine.Object;

namespace Libs
{
    public class ReplaceDefencesResource: EditorWindow
    {
        private Object selectObj;
        private List<Object> selectObjArr= new List<Object>();
        private string oldFolderPath;
        private string newFolderPath;
        private string prefabPathList; 
        private string prefix = "-";
        private string animaControll = ".controller";
        private string matKey = ".mat";
        private string fontKey = ".fontsettings";
        private bool initOver = false;
        [MenuItem("Assets/资源引用替换",false,13)]
        static void ShowChangeGUIDToolsWindox()
        {
            ReplaceDefencesResource editor = (ReplaceDefencesResource)GetWindowWithRect (typeof(ReplaceDefencesResource), new Rect (100, 100, 1000, 500), false, "资源引用替换");
            editor.Show ();
        }
        void Init()
        {
            if (!initOver)
            {
                GetAssetsSelectFileInfo();
            }
            initOver = true;
        }
        void GetAssetsSelectFileInfo()
        {
            Debug.Log("GetAssetsSelectFileInfo -----------");
            object[] obj = Selection.objects;
            for (int i = 0; i < obj.Length; i++)
            {
                if (obj[i] as Object) selectObjArr.Add(obj[i] as Object);
            }
        }
        private void OnGUI()
        {
            Init();
            EditorGUILayout.BeginVertical();
            Func<string> selectFunc = () =>
            {
                if (selectObjArr == null || selectObjArr.Count == 0)
                {
                    if (selectObj == null) return "";
                    return selectObj.name;
                }
                string name = selectObjArr[0].name;
                if (selectObjArr.Count < 2) return name;
                for (int i = 1; i < selectObjArr.Count; i++)
                {
                    name += "," + selectObjArr[i].name;
                }

                return name;
            };
            EditorGUILayout.LabelField ("当前选中的物体名字为:"+selectFunc());
            
            EditorGUILayout.LabelField ("原资源文件夹:");
            Rect orginDirPathRect = EditorGUILayout.GetControlRect(GUILayout.Width(500));
            oldFolderPath = EditorGUI.TextField(orginDirPathRect, oldFolderPath);
            DragFileMethod(orginDirPathRect,false, ref oldFolderPath);
            
            EditorGUILayout.LabelField ("替换资源文件夹:");
            Rect targetDirPathRect2 = EditorGUILayout.GetControlRect(GUILayout.Width(500));
            newFolderPath = EditorGUI.TextField(targetDirPathRect2, newFolderPath);
            DragFileMethod(targetDirPathRect2,false, ref newFolderPath);
            
            EditorGUILayout.Space();
            
            if (GUILayout.Button("替换", GUILayout.Width(100)))
            {
                for (int i = 0; i < selectObjArr.Count; i++)
                {
                    Replace(AssetDatabase.GetAssetPath(selectObjArr[i]));
                }
            }

            EditorGUILayout.EndVertical();
        }

        private void Replace(string prefabPath)
        {
            //获取选中物体所有的引用资源
            if (!string.IsNullOrEmpty(prefabPath))
            {
                Debug.Log("预制件 路径"+prefabPath);
                string path = Path.Combine(Environment.CurrentDirectory,prefabPath);
                // Debug.Log("Environment.CurrentDirectory ===="+Environment.CurrentDirectory+"------path--->>"+path);
                string prefabFileInfo = File.ReadAllText(path);
                // Debug.Log("prefabFileInfo ===="+prefabFileInfo);
                string[] prefabFileInfoArr = prefabFileInfo.Split('\n');
                if (prefabFileInfoArr==null || prefabFileInfoArr.Length==0)
                {
                    Debug.LogError("prefab 解析错误------>>"+prefabPath);
                    return;
                }
                string[] dependenciesPaths= AssetDatabase.GetDependencies(prefabPath);
                if (dependenciesPaths==null || dependenciesPaths.Length==0)
                {
                    Debug.LogError("获取依赖资源为nil------>>"+prefabPath);
                    return;
                }   
                //筛选出在之前文件夹下所引用的资源
                List<string> oldFolderDepencies = new List<string>();
                for (int i = 0; i < dependenciesPaths.Length; i++)
                {
                    if (!dependenciesPaths[i].Equals(prefabPath) && dependenciesPaths[i].StartsWith(oldFolderPath))
                    {
                        // Debug.Log("引用资源："+dependenciesPaths[i]);
                        oldFolderDepencies.Add(dependenciesPaths[i]);
                    }
                }
                if (oldFolderDepencies==null || oldFolderDepencies.Count==0)
                {
                    Debug.LogError("当前prefab没有引用资源在当前文件目录下------>>"+prefabPath);
                    // return;
                }  
                List<string> oldGuidArray = new List<string>();
                List<string> newGuidArray = new List<string>();
                List<string> newFolderDepencies = new List<string>();

                for (int i = 0; i < oldFolderDepencies.Count; i++)
                {
                    string oldGuid = AssetDatabase.AssetPathToGUID(oldFolderDepencies[i]);
                    Debug.Log("引用资源的路径："+oldFolderDepencies[i]+"=========Guid:"+oldGuid);
                    if (string.IsNullOrEmpty(oldGuid))
                    {
                        continue;
                    }
                    string newAssetPath = (prefix+oldFolderDepencies[i])
                        .Replace(prefix + oldFolderPath, newFolderPath);
                    Debug.Log("替换资源的路径：旧的"+oldFolderDepencies[i]+"=========新的:"+newAssetPath);
                    string newGuid = AssetDatabase.AssetPathToGUID(newAssetPath);
                    // Debug.Log("新的已经替换的资源的路径："+newAssetPath+"=========Guid:"+newGuid);
                    if (string.IsNullOrEmpty(newGuid))
                    {
                        continue;
                    }
                    newFolderDepencies.Add(newAssetPath);
                    if (!oldGuid.Equals(newGuid))
                    {
                        oldGuidArray.Add(oldGuid);
                        newGuidArray.Add(newGuid);
                    }
                }
                
                //替换guid
                for (int i = 0; i < prefabFileInfoArr.Length; i++)
                {
                    string guid = GetGUIDFromContext(prefabFileInfoArr[i]);
                    if(string.IsNullOrEmpty(guid)) continue;
                    for (int j = 0; j < oldGuidArray.Count; j++)
                    {
                        if (guid == oldGuidArray[j])
                        {
                            prefabFileInfoArr[i] = prefabFileInfoArr[i].Replace(oldGuidArray[j], newGuidArray[j]);
                            break;
                        }
                    }
                }
                //文件重新写入
                prefabFileInfo = string.Join("\n", prefabFileInfoArr);
                prefabFileInfo.Remove(prefabFileInfo.Length - 1);
                File.WriteAllText(path,prefabFileInfo);
                
                
                List<string> aniMatorlList = new List<string>();
                List<string> matList = new List<string>();
                List<string> fontList = new List<string>();

                for (int i = 0; i < newFolderDepencies.Count; i++)
                {
                    if (newFolderDepencies[i].EndsWith(animaControll))
                    {
                        aniMatorlList.Add(newFolderDepencies[i]); 
                    }
                    if (newFolderDepencies[i].EndsWith(matKey))
                    {
                        matList.Add(newFolderDepencies[i]); 
                    }
                    if (newFolderDepencies[i].EndsWith(fontKey))
                    {
                        fontList.Add(newFolderDepencies[i]); 
                    }
                }
                //替换动画状态机
                for (int i = 0; i < aniMatorlList.Count; i++)
                {
                    Replace(aniMatorlList[i]);
                }
                //替换材质图片引用
                for (int i = 0; i < matList.Count; i++)
                {
                    Replace(matList[i]);
                }
                //替换材质图片引用
                for (int i = 0; i < fontList.Count; i++)
                {
                    Replace(fontList[i]);
                }
                Debug.Log("替换成功 ----->>"+prefabPath);
            }
        }

        /// <summary>
        /// 文件拖拽
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="m_filePaths"></param>
        private void DragFileMethod(Rect rect,bool canContact, ref string m_filePaths)
        {
            if (!rect.Contains(Event.current.mousePosition)) return;
            if (Event.current.type == EventType.DragUpdated || Event.current.type == EventType.DragPerform)
            {
                DragAndDrop.AcceptDrag();
                DragAndDrop.visualMode = DragAndDropVisualMode.Link;
            }

            
            if (Event.current.type== EventType.DragExited)
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Generic;
                if (DragAndDrop.paths!=null && DragAndDrop.paths.Length>0)
                {
                    string path = DragAndDrop.paths[0];
                    if (!canContact)
                    {
                        m_filePaths = path;
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(m_filePaths))
                        {
                            m_filePaths = path;
                        }
                        else if (!m_filePaths.Contains(path))
                        {
                            m_filePaths += ";" + path;
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 获取guid
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private string GetGUIDFromContext(string text){
            Regex regex = new Regex("guid: ([a-fA-F0-9]{32})");
            Match match = regex.Match(text);

            if (match.Success)
            {
                return match.Value.Substring(6);
            }      
            return "";
        }
    }
}