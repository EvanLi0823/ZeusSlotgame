using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
public class FindUnDoTag  {
	[MenuItem("Libs/查找未打成图集的图片")]
	private static void FindUnTagResource()
	{
		int resultCount = 0;
		List<string> WithOutFolders = new List<string>(){"Assets/Libs","Assets/Platforms","Assets/Plugins","Assets/Resources","Assets/Fabric"};
		List<string> ConatainLastExtensions = new List<string>(){".png"};
		string[] files = Directory.GetFiles (Application.dataPath, "*.*", SearchOption.AllDirectories)
			.Where (s =>{ 
				for(int k=0;k<WithOutFolders.Count;k++){
					if(s.Contains(WithOutFolders[k])){
						return false;
					}
				}
				return ConatainLastExtensions.Contains (Path.GetExtension (s).ToLower ());
			}).ToArray ();
		string splitString = "Assets/";
		for (int i = 0; i < files.Length; i++) {
			string fullName = files [i].Substring (files [i].IndexOf(splitString));
			TextureImporter importer = AssetImporter.GetAtPath(fullName) as TextureImporter;
//			TextureImporter textureImporter = (TextureImporter)TextureImporter.GetAtPath (fullName);
			if (importer.spritePackingTag == "") {
				resultCount++;
				Log.Error (fullName);
			}
	
		}
		Log.Trace (string.Format("查找结束,{0}个图片需要处理",resultCount));
	}


//	[MenuItem("Libs/查找没有被使用的图片")]
	private static void FIndUnUseImage()
	{
		List<string> needSearchFolders = new List<string> (){ "Assets/Textures/Lobby/Common"};//"Assets/Libs","Assets/Platforms","Assets/Plugins","Assets/Resources"};
		List<string> ConatainLastExtensions = new List<string>(){".png",".tga",".psd"};
		string[] needSearchImageFiles = Directory.GetFiles (Application.dataPath, "*.*", SearchOption.AllDirectories)
			.Where (s =>{ 
				for(int k=0;k<needSearchFolders.Count;k++){
					if(!s.Contains(needSearchFolders[k])){
						return false;
					}
				}
				return ConatainLastExtensions.Contains (Path.GetExtension (s).ToLower ());
			}).ToArray ();
		//被引用的unity文件
		List<string> withoutExtensions = new List<string>(){".prefab",".unity",".mat",".asset"};
		string[] allUnityFiles = Directory.GetFiles(Application.dataPath, "*.*", SearchOption.AllDirectories)
			.Where(s => withoutExtensions.Contains(Path.GetExtension(s).ToLower())).ToArray();

		for (int startIndex =0; startIndex < needSearchImageFiles.Length; startIndex++) {
			string searchPath = needSearchImageFiles [startIndex];
			string guid = AssetDatabase.AssetPathToGUID(searchPath);

			EditorUtility.DisplayCancelableProgressBar ("匹配资源中", searchPath, (float)startIndex / (float)needSearchImageFiles.Length);

			for (int unityIndex =0; unityIndex < allUnityFiles.Length; unityIndex++) {
				string file = allUnityFiles [unityIndex];


				if (Regex.IsMatch (File.ReadAllText (file), guid)) {
//					Debug.Log (file, AssetDatabase.LoadAssetAtPath<Object> (GetRelativeAssetsPath (file)));
					break;
				}

			}
			Debug.LogError (searchPath);
		}
	}

	static private string GetRelativeAssetsPath(string path)
	{
		return "Assets" + Path.GetFullPath(path).Replace(Path.GetFullPath(Application.dataPath), "").Replace('\\', '/');
	}
}
