using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;
public class PictureFindWithAtlasName : EditorWindow {

	[MenuItem("Libs/根据图集名字查找picture")]
	static void ShowPlistWindow()
	{
		PictureFindWithAtlasName editor = (PictureFindWithAtlasName)EditorWindow.GetWindowWithRect (typeof(PictureFindWithAtlasName), new Rect (100, 100, 300, 150), false, "图集名字找picture");
		editor.Show ();
	}
	string _atlasName;
	string SPLIT_STRING = "Assets/";
	void OnGUI()
	{
		EditorGUILayout.BeginVertical ();
		EditorGUILayout.LabelField ("请输入图集的名字");
		Rect rect = EditorGUILayout.GetControlRect (GUILayout.Width (300f));
		_atlasName = EditorGUI.TextField (rect, _atlasName);
		EditorGUILayout.Space ();
		if (GUILayout.Button ("查找", GUILayout.Width (100))) {
			if (!string.IsNullOrEmpty (_atlasName)) {
				FindByName (_atlasName);
			}
		}
	}

	private void FindByName(string atlasName)
	{
		Debug.Log (atlasName);
		List<string> withoutExtensions = new List<string>(){".png",".jpg"};
		string[] files = Directory.GetFiles(Application.dataPath, "*.*", SearchOption.AllDirectories)
			.Where(s => withoutExtensions.Contains(Path.GetExtension(s).ToLower())).ToArray();
		int startIndex = 0;
		for (; startIndex < files.Length; startIndex++) {
			string file = files [startIndex];
			string assetPath =	file.Substring (file.LastIndexOf (SPLIT_STRING) );
			if (assetPath.Contains ("Editor") || assetPath.Contains ("Plugins")) {
				continue;
			} 
			TextureImporter importer = AssetImporter.GetAtPath (assetPath) as TextureImporter;
			if (importer == null || importer.spritePackingTag == null) {
				Debug.LogError (assetPath);
				continue;
			}
			if (importer.spritePackingTag !=null && importer.spritePackingTag.Contains (atlasName)) {
				Debug.Log (file, AssetDatabase.LoadAssetAtPath<Object> (GetRelativePath (file)));
			}
		}
	}

	private string GetRelativePath(string path)
	{
		return "Assets" + Path.GetFullPath (path).Replace (Path.GetFullPath (Application.dataPath), "").Replace ('\\', '/');
	}
}
