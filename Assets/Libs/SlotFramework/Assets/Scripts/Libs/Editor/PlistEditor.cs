using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
namespace Libs
{
	public class PlistEditor :EditorWindow
	{
		string plistPath;
		string txtPath;
		string outputPath;
		bool isSuccess = false;
		List<ModifyContainer> ModifyDatas = new List<ModifyContainer> ();

		[MenuItem("Libs/Plist修改工具")]
		static void ShowPlistWindow()
		{
			PlistEditor editor = (PlistEditor)EditorWindow.GetWindowWithRect (typeof(PlistEditor), new Rect (100, 100, 600, 600), false, "Plist修改器");
			editor.Show ();
		}

		private void ReadModifyTxt()
		{
			ModifyDatas.Clear ();
			if (!string.IsNullOrEmpty (txtPath)) {
				FileStream stream = new FileStream (txtPath, FileMode.Open);
				StreamReader read = new StreamReader (stream);
				string strLine = null;
				while ((strLine = read.ReadLine ()) != null) {

					string[] txts = Regex.Split (strLine, "<<<", RegexOptions.IgnoreCase); //strLine.Split (new char[]{'<','<','<'});
					ModifyContainer container = new ModifyContainer(txts[0].Trim(), txts[1].Trim());
					Debug.Log (container.SourceTxt +"---------"+container.TargetTxt);
					this.ModifyDatas.Add (container);
				}
				read.Close ();
				stream.Close ();
			}
		}

		private void SavePlist()
		{
			isSuccess = false;
			if (!string.IsNullOrEmpty (plistPath)) {
				FileStream stream = new FileStream (plistPath, FileMode.Open);
				StreamReader read = new StreamReader (stream);
				string needModify = read.ReadToEnd ();
				read.Close ();
				stream.Close ();
				for(int i = 0; i< ModifyDatas.Count;i++)
				{
					needModify = needModify.Replace(ModifyDatas[i].SourceTxt,ModifyDatas[i].TargetTxt);
				}

				Debug.Log (needModify);
				if(!string.IsNullOrEmpty(outputPath))
				{
					FileStream WriteStream = new FileStream(outputPath,FileMode.Create);
					StreamWriter write  = new StreamWriter(WriteStream);
					write.Write(needModify);
					write.Close ();
					WriteStream.Close ();
					isSuccess = true;
				}
			}
		}

		void Awake()
		{
			plistPath = EditorPrefs.GetString ("EditorPlistPath");
			txtPath = EditorPrefs.GetString ("EditorTxtPath");
			outputPath = EditorPrefs.GetString ("EditorOutputPath");
			this.ReadModifyTxt ();
		}

		void OnGUI()
		{
			
			EditorGUILayout.BeginVertical ();
			EditorGUILayout.LabelField ("plist源路径");
			Rect rect = EditorGUILayout.GetControlRect (GUILayout.Width (300));
			plistPath = EditorGUI.TextField (rect, plistPath);

			if (( Event.current.type == EventType.DragExited) && rect.Contains (Event.current.mousePosition)) {
				DragAndDrop.visualMode = DragAndDropVisualMode.Generic;
				if (DragAndDrop.paths != null && DragAndDrop.paths.Length > 0) { 
					plistPath = DragAndDrop.paths [0];
				}
			}
			EditorGUILayout.LabelField ("替换文件保存路径");
			rect = EditorGUILayout.GetControlRect (GUILayout.Width (300));	
			txtPath = EditorGUI.TextField (rect, txtPath);

			if ((Event.current.type == EventType.DragExited) && rect.Contains (Event.current.mousePosition)) {
				DragAndDrop.visualMode = DragAndDropVisualMode.Generic;
				if (DragAndDrop.paths != null && DragAndDrop.paths.Length > 0) {
					txtPath = DragAndDrop.paths [0];
				}
			}
			EditorGUILayout.Space ();
			EditorGUILayout.LabelField ("输出文件保存路径");
			rect = EditorGUILayout.GetControlRect (GUILayout.Width (300));	
			outputPath = EditorGUI.TextField (rect, outputPath);

			for(int i = 0; i< ModifyDatas.Count;i++)
			{				
						EditorGUILayout.BeginHorizontal();
				rect = EditorGUILayout.GetControlRect (GUILayout.Width (500));	
						EditorGUI.LabelField(rect,ModifyDatas[i].SourceTxt);
				EditorGUILayout.Space();
				rect = EditorGUILayout.GetControlRect (GUILayout.Width (500));	
						EditorGUI.LabelField(rect,ModifyDatas[i].TargetTxt);
						EditorGUILayout.EndHorizontal();
			}
						EditorGUILayout.Space();

			EditorGUILayout.BeginHorizontal ();

			if (GUILayout.Button ("刷新", GUILayout.Width (100))) {
				ReadModifyTxt ();
			}

			if(GUILayout.Button("保存",GUILayout.Width(100)))
			{
				EditorPrefs.SetString ("EditorPlistPath", plistPath);
				EditorPrefs.SetString ("EditorTxtPath", txtPath);
				EditorPrefs.SetString ("EditorOutputPath", outputPath);

				SavePlist ();
			}
			EditorGUILayout.EndHorizontal ();

			if (isSuccess) {
				EditorGUILayout.LabelField ("保存成功");
			}

			EditorGUILayout.EndVertical ();
		}
	}

	[System.Serializable]
	class ModifyContainer
	{
		[HideInInspector]
		[SerializeField]
		private string sourceTxt;
		public string SourceTxt {
			get{
				return sourceTxt;
			}
			set{
				sourceTxt = value;
			}
		}

		[HideInInspector]
		[SerializeField]
		private string targetTxt;
		public string TargetTxt {
			get{
				return targetTxt;
			}
			set{
				targetTxt = value;
			}
		}

		public ModifyContainer(string source, string target)
		{
			this.sourceTxt = source;
			this.targetTxt = target;
		}
	}
}