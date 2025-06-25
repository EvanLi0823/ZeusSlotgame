using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using UnityEngine.UI;

public class FindReferences
{

	[MenuItem ("Assets/Find References", false, 100)]
	static private void Find ()
	{
		Debug.Log ("开始查找" + Selection.activeObject.name);	
		int resultCount = 0;
		EditorSettings.serializationMode = SerializationMode.ForceText;
		string path = AssetDatabase.GetAssetPath (Selection.activeObject);
		if (!string.IsNullOrEmpty (path)) {
			string guid = AssetDatabase.AssetPathToGUID (path);
			List<string> withoutExtensions = new List<string> (){ ".prefab", ".unity", ".mat", ".asset" };
			string[] files = Directory.GetFiles (Application.dataPath, "*.*", SearchOption.AllDirectories)
				.Where (s => withoutExtensions.Contains (Path.GetExtension (s).ToLower ())).ToArray ();
			int startIndex = 0;

			for (; startIndex < files.Length; startIndex++) {
				string file = files [startIndex];

				EditorUtility.DisplayCancelableProgressBar ("匹配资源中", file, (float)startIndex / (float)files.Length);

				if (Regex.IsMatch (File.ReadAllText (file), guid)) {
					resultCount++;
					Debug.Log (file, AssetDatabase.LoadAssetAtPath<Object> (GetRelativeAssetsPath (file)));
				}
			
			}

			EditorUtility.ClearProgressBar (); 
			Debug.Log (string.Format ("匹配结束，共有{0}个引用", resultCount));	
		}
	}

	[MenuItem ("Assets/Find Folder UnUse Image")]
	static private void FindUnUsePng ()
	{
		EditorSettings.serializationMode = SerializationMode.ForceText;
		if (Selection.assetGUIDs == null || Selection.assetGUIDs.Length == 0) {
			Debug.LogError ("没有选择文件夹");
			return;
		}
		string currentFold = AssetDatabase.GUIDToAssetPath (Selection.assetGUIDs[0]);

		if (!string.IsNullOrEmpty (currentFold)) {
			Debug.Log ("开始查找");
			List<string> pngExtensions = new List<string> (){ ".png", ".jpg"};
			string[] originFiles = Directory.GetFiles (currentFold, "*.*", SearchOption.AllDirectories)
				.Where (s => pngExtensions.Contains (Path.GetExtension (s).ToLower ())).ToArray ();
		
			for(int i= 0; i < originFiles.Length;i++)
			{		
				string guid = AssetDatabase.AssetPathToGUID (originFiles[i]);
				List<string> withoutExtensions = new List<string> (){ ".prefab", ".unity", ".mat", ".asset" };
				string[] files = Directory.GetFiles (Application.dataPath, "*.*", SearchOption.AllDirectories)
					.Where (s => withoutExtensions.Contains (Path.GetExtension (s).ToLower ())).ToArray ();
				EditorUtility.DisplayCancelableProgressBar ("匹配资源中", originFiles[i], (float)i / (float)originFiles.Length);

				bool hasExist = false;
				int startIndex = 0;
				for (; startIndex < files.Length; startIndex++) {
					string file = files [startIndex];			

					if (Regex.IsMatch (File.ReadAllText (file), guid)) {
						hasExist = true;
						break;					
					}
				}

				if (!hasExist) {
					Debug.Log (originFiles[i], AssetDatabase.LoadAssetAtPath<Object> (GetRelativeAssetsPath (originFiles[i])));
				}
			}
			EditorUtility.ClearProgressBar ();
			Debug.Log ("查找结束");
		}
	}


	static private string GetRelativeAssetsPath (string path)
	{
		return "Assets" + Path.GetFullPath (path).Replace (Path.GetFullPath (Application.dataPath), "").Replace ('\\', '/');
	}
}