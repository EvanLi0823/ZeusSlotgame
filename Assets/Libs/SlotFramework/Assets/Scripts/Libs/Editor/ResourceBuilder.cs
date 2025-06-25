using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;

public class ResourceBuilder
{
//    private static string spritesPath = Application.dataPath + "/Textures/Dialog/SevenDay";
    private static string savePath = Application.dataPath + "/Resources/Prefab/Atlas";

    [MenuItem("Assets/BuildSpritesToPrefabs")]
    public static void BuildSpritesToPrefabs()
    {
        string path = null;
        foreach (UnityEngine.Object obj in Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets))
        {
            path = AssetDatabase.GetAssetPath(obj);
            break;
        }


        string savePathDir = savePath + path.Substring(path.LastIndexOf("/"));

        if (!Directory.Exists(savePathDir))
        {
            Directory.CreateDirectory(savePathDir);
        }

        //if (!Directory.Exists(curSavePath))
        //    Directory.CreateDirectory(curSavePath);
        DirectoryInfo dirInfo = new DirectoryInfo(path);
        if (dirInfo.Exists)
        {
            string[] extensions = new[] { "*.jpg", "*.png", "*.bmp" };

            for (int i = 0; i < extensions.Length; i++)
            {
                foreach (FileInfo pngFile in dirInfo.GetFiles(extensions[i], SearchOption.AllDirectories))
                {
                    string allPath = pngFile.FullName;
                    string assetPath = allPath.Substring(allPath.IndexOf("Assets"));
                    Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
                    if (sprite == null)
                    {
                        Debug.Log(assetPath + "isNull");
                        continue;
                    }

                    Debug.Log(assetPath + " success!");
                    GameObject go = new GameObject(sprite.name);
                    go.AddComponent<SpriteRenderer>().sprite = sprite;
                    allPath = savePathDir + "/" + sprite.name + ".prefab";
                    string prefabPath = allPath.Substring(allPath.IndexOf("Assets"));
                    PrefabUtility.CreatePrefab(prefabPath, go);
                    GameObject.DestroyImmediate(go);
                }
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("parfab success");
    }
}