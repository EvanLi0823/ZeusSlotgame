using UnityEngine;
using System.Collections;

namespace Libs{
	public static class Config {

		public static readonly string UIPrefabPath = "Assets/Resources/Prefab/UI/";//UI prefab 生成路径
		public static readonly string RenderPrefabPath = "Assets/Resources/Prefab/Render/";
		public static readonly string UIClassPath = Application.dataPath + "/Scripts/UI/"; //UI 

	//    public static string DataClassPath = Application.dataPath + "/Script/Data/Generate/";//数据类生成路径
	//    public static string DataBytePath = Application.dataPath + "/Resources/";//数据二进制文件生成路径

		public static readonly string UIHierarchyRoot = "/DialogCamera/DialogCanvas/Panel/";

		public static string OutputNameSpace ="Classic";
//		public static string[] AssetBundleExcludeScenes = {"EmptyScene","Loading","ClassicLobby","DoubleDiamond","3XDiamond","FruitBar","Cleopatra","Wonderland"};

		}
	}
