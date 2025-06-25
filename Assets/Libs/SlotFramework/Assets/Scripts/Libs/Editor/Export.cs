using UnityEngine;
using UnityEditor;
using System.Collections;
using System;

//using System.Collections;
using System.Collections.Generic;

//using System.Reflection;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 生成prefab和代码
/// </summary>
namespace Libs
{
	public class Export
	{
		private static readonly string TAG_UI = "_UI_";
		private static readonly string TAG_PROPERTY = "_Property_";
		private static readonly string TAG_DIALOG = "_Dialog_";
		private static readonly string TAG_Render = "_Render_";

		private class Property
		{
			public Type type;
			public string name;
			public string path;

			public Property (Type t, string n, string p)
			{
				type = t;
				name = n;
				path = p;
			}
		}

		private class Render
		{
			
		}

		public static Dictionary<Type, int> PRIORITY = new Dictionary<Type, int> ();

		delegate Util.IterationArguments UnitProcess (Transform unit,string path);

		private static string CurrentRoot;

		private static Transform root;

		private static List<Property> properties;

		private static List<Property> listProperties;

		private static string CurrentUIClassName;

		private static bool isDialog;
		private static bool isRender;
		static Export ()
		{
	        
		}

		/// <summary>
		/// Creates the dictionary.
		/// priority越大，结果越靠前
		/// </summary>
		protected static void CreateDictionary ()
		{

			PRIORITY [typeof(RawImage)] = 9;
			PRIORITY [typeof(Image)] = 9;
			PRIORITY [typeof(Text)] = 9;
			PRIORITY [typeof(TMPro.TextMeshProUGUI)] = 10;
						
			PRIORITY [typeof(Button)] = 10;
			PRIORITY [typeof(Scrollbar)] = 10;                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                   
			PRIORITY [typeof(Slider)] = 10;
			PRIORITY [typeof(Toggle)] = 10;
			PRIORITY [typeof(ToggleGroup)] = 10;
			PRIORITY [typeof(InputField)] = 10;
			PRIORITY [typeof(Animator)] = 10;
			PRIORITY [typeof(ScrollRect)] = 10;
			//自己写的组件
			PRIORITY [typeof(UIBase)] = 20;
			PRIORITY [typeof(ToggleButton)] = 20;
			PRIORITY [typeof(HorizontalSrollRect)] = 30;
			PRIORITY [typeof(VerticalScrollRect)] = 30;

			PRIORITY [typeof(UIChangeNumber)] = 19;
			PRIORITY [typeof(UIIncreaseNumber)] = 19;
			//PRIORITY[typeof()]
		}

		[MenuItem ("Libs/Export/ExportDialog(可以生成prefab和代码文件,tag为_Dialog_)")]
		public static void ExportDialog ()
		{
			CreateDictionary ();
			properties = new List<Property> ();
			listProperties = new List<Property> ();
			Util.IterateChild (GameObject.Find (Config.UIHierarchyRoot).transform, Config.UIHierarchyRoot, unitProcessor);

			if (root != null) {
				generateUI ();
			}
		}

		[MenuItem ("Libs/Export/ExportPanel(只生成代码不生成prefab，必须选中要导出的gameobject,tag为_UI_)")]
		public static void ExportPanel ()
		{
			CreateDictionary ();
			properties = new List<Property> ();
			listProperties = new List<Property> ();
			string path = Util.GetGameObjectPath (Selection.activeGameObject.transform);
			Util.IterateChild (Selection.activeGameObject.transform, path, unitProcessor);

			if (root != null) {
				generateUI ();
			}
		}

		[MenuItem ("Libs/Export/ExportRender(导出prefab和代码，tag为_Render_)")]
		public static void ExportRender ()
		{
			CreateDictionary ();
			properties = new List<Property> ();
			listProperties = new List<Property> ();
			string path = Util.GetGameObjectPath (Selection.activeGameObject.transform);
			Util.IterateChild (Selection.activeGameObject.transform, path, unitProcessor);

			if (root != null) {
				generateUI ();
			}
		}

		private static Util.IterationArguments unitProcessor (Transform unit, string path)
		{
			bool result = unit.CompareTag (TAG_UI) || unit.CompareTag (TAG_DIALOG) || unit.CompareTag (TAG_Render);
			if (result) {
				root = unit;
				CurrentRoot = path;
				CurrentUIClassName = root.name;
				isDialog = unit.CompareTag (TAG_DIALOG);
					isRender = unit.CompareTag(TAG_Render);
			}
			return result ? Util.IterationArguments.StopAll : Util.IterationArguments.Continue;
		}

		private static void generateUI ()
		{
			Util.IterateChild (root, "", generateProperty);

			generateUICodeAndPrefab (root.name);
		}

		private static Util.IterationArguments generateProperty (Transform unit, string path)
		{

			if (unit.CompareTag (TAG_PROPERTY)) {
				MonoBehaviour[] behaviours = unit.GetComponents<MonoBehaviour> ();

				Type type = null;
				int last = 1;
				foreach (MonoBehaviour uib in behaviours) {
					Type current = uib.GetType ();
					if (isUIClassOrSubClass (current)) {
						int cur = getPriority (current);
						if (cur > last) {
							type = current;
							last = cur;
						}
					}
				}
				properties.Add (new Property (type == null ? typeof(GameObject) : type, unit.name, path));
				if(type != null && type.Equals(typeof(HorizontalSrollRect)))
				{
					return Util.IterationArguments.StopCurrent;
				}
			}
			return Util.IterationArguments.Continue;
		}

		private static bool isUIClassOrSubClass (Type t)
		{
			foreach (KeyValuePair<Type, int> pair in PRIORITY) {
				if (t == pair.Key || t.IsSubclassOf (pair.Key))
					return true;
			}
			return false;
		}

		private static int getPriority (Type t)
		{
			foreach (KeyValuePair<Type, int> pair in PRIORITY) {
				if (t == pair.Key || t.IsSubclassOf (pair.Key))
					return pair.Value;
			}
			return 0;
		}

		private static void generateUICodeAndPrefab (string className)
		{
			string code = "";
			code += "using UnityEngine;\n";
			code += "using UnityEngine.UI;\n";
			code += "using Libs;\n";
			//code += "using System.Collections;\n";
			//code += "using System.Collections.Generic;\n";
			code += "\n";
			if (!string.IsNullOrEmpty (Config.OutputNameSpace)) {
				code += "namespace " + Config.OutputNameSpace + "\n";
				code += "{\n";
			}
			string baseClassName = isDialog ? "UIDialog" : "UIBase";	 
			code += "public class " + className + "Base : " + baseClassName + " \n{";
			code += "\n";
			//变量
			foreach (Property property in properties) {
				Debug.Log (property.type.FullName);
				code += "\t\t[HideInInspector]\n";
				code += "    public " + property.type.FullName + " " + property.name + ";\n";
			}
			//函数Initialize
			code += "\n";
			code += "    protected override void Awake()\n";
			code += "    {\n";
			code += "        base.Awake();\n";
			foreach (Property property in properties) {
				string str = property.path;
				code += "        this." + property.name + " = " + "Util.FindObject<" + property.type + ">(transform,\"" + property.path + "\");\n";
				if (property.type == typeof(Button)) {
					code += "        UGUIEventListener.Get(this." + property.name + ".gameObject).onClick = this.OnButtonClickHandler;\n";
				}
				if (property.type == typeof(ToggleButton) || property.type == typeof(Toggle)) {
					code += "\t\tUGUIEventListener.Get(this." + property.name + ".gameObject).onToggleChanged = this.onToggleSelect;\n";
				}

			}
			code += "    }\n";
			code += "}";
			if (!string.IsNullOrEmpty (Config.OutputNameSpace)) {
				code += "}\n";
			}
			string path = Config.UIClassPath + "Base/";
			WriteClass (code, path, className + "Base");

			////////////////////////////////////////////////////UI Class/////////////////////////////////////////////////

			path = Config.UIClassPath;

			if (!System.IO.File.Exists (path + className + ".cs")) {
				code = "";
				code += "using UnityEngine;\n";
				code += "using UnityEngine.UI;\n";
				code += "using System.Collections;\n";
				code += "using System.Collections.Generic;\n";
				code += "using Libs;\n";
				code += "\n";
				if (!string.IsNullOrEmpty (Config.OutputNameSpace)) {
					code += "namespace " + Config.OutputNameSpace + "\n";
					code += "{\n";
				}
				code += "public class " + className + " : " + className + "Base \n{";
				code += "\n";
				//函数Initialize
//	            code += "\n";
				code += "     \tprotected override void Awake()\n";
				code += "    \t{\n";
				code += "       \t\tbase.Awake();\n";
				code += "    \t}\n";
				//函数ButtonClickHandler
				code += "\n";
				code += "   \t\tpublic override void OnButtonClickHandler(GameObject go)\n";
				code += "    \t{\n";
				code += "    \t    base.OnButtonClickHandler(go);\n";
				code += "    \t}\n";
				code += "\t}\n";
				if (!string.IsNullOrEmpty (Config.OutputNameSpace)) {
					code += "}\n";
				}
				WriteClass (code, path, className);
			}
	        
			////////////////////////////////////////////////////Create prefab/////////////////////////////////////////////////
			if (isDialog ) {
				GameObject uigo = GameObject.Find (Config.UIHierarchyRoot + className);
				GameObject prefab = PrefabUtility.CreatePrefab (Config.UIPrefabPath + className + ".prefab", uigo, ReplacePrefabOptions.Default);
				GameObject.DestroyImmediate (uigo);
			}
			if (isRender) {
				GameObject uigo = Selection.activeGameObject;
				GameObject prefab = PrefabUtility.CreatePrefab (Config.RenderPrefabPath + className + ".prefab", uigo, ReplacePrefabOptions.Default);
				GameObject.DestroyImmediate (uigo);
			}
			Debug.Log ("生成完毕！");
			AssetDatabase.Refresh ();
			AssetDatabase.SaveAssets ();
		}


		private static void generateList (Transform listRoot, string listPath)
		{
			Transform item = listRoot;
			Transform grid = null;
			string itemPath = listPath;
			for (int i = 0; i < 3; i++) {
				item = item.GetChild (0);
				itemPath = itemPath + item.name + "/";

				if (i == 1)
					grid = item;
			}

			if (grid != null) {
				GridLayoutGroup layout = grid.GetComponent<GridLayoutGroup> ();
				if (layout == null)
					layout = grid.gameObject.AddComponent<GridLayoutGroup> ();
				RectTransform itemRect = item.GetComponent<RectTransform> ();
				//layout.padding;
				layout.cellSize = itemRect.sizeDelta;
				layout.spacing = new Vector2 (5, 5);
			}

			Util.IterateChild (item, itemPath, generateListProperty);

			generateListCodeAndPrefab (item, getCurrentItemClassName (root.name, listRoot.name));
		}

		private static Util.IterationArguments generateListProperty (Transform unit, string path)
		{
			Debug.Log (path);
			if (unit.tag.Equals (TAG_PROPERTY)) {
				Type type = null;
				UIBehaviour[] behaviours = unit.GetComponents<UIBehaviour> ();

				if (behaviours == null || behaviours.Length < 1) {
					type = typeof(GameObject);
				} else {
					foreach (Behaviour bb in behaviours) {
						if (type == null) {
							type = bb.GetType ();
						} else if (bb is Selectable) {
							type = bb.GetType ();
						}
					}
				}
				listProperties.Add (new Property (type, unit.name, path));
			}
			return Util.IterationArguments.Continue;
		}

		private static void generateListCodeAndPrefab (Transform listRoot, string className)
		{
			bool handleBtn = false;
			string code = "";
			code += "using UnityEngine;\n";
			code += "using UnityEngine.UI;\n";
			//code += "using System.Collections;\n";
			//code += "using System.Collections.Generic;\n";
			code += "\n";
			code += "public class " + className + "Base : GUnlimitedItem \n{";
			code += "\n";
			//变量
			foreach (Property property in listProperties) {
				code += "    public " + property.type.Name + " " + property.name + ";\n";
			}
			//函数Initialize
			code += "\n";
			code += "    protected override void Awake()\n";
			code += "    {\n";
			code += "        base.Awake();\n";
			foreach (Property property in listProperties) {
				code += "        this." + property.name + " = " + "Util.FindObject<" + property.type + ">(transform, \"" + property.path + "\");\n";
				if (property.type.IsSubclassOf (typeof(Button))) {
					handleBtn = true;
					//                code += "        this." + property.name + ".OnClick = this.OnButtonClickHandler;\n";
					code += "        UGUIEventListener.Get(this." + property.name + ".gameObject).onClick = this.OnButtonClickHandler;\n";
				}
			}

			//		Button btn = listRoot.GetComponent<Button>();
			//        if(btn!=null)
			//        {
			//            handleBtn = true;
			////            code += "        this.GetComponent<GButtonBase>().OnClick = this.OnButtonClickHandler;\n";
			//			code += "        UGUIEventListener.Get(this." + property.name + ").OnClick = this.OnButtonClickHandler;\n";
			//        }

			code += "    }\n";

			code += "}";
			string path = Config.UIClassPath + "Items/Base/";
			WriteClass (code, path, className + "Base");

			path = Config.UIClassPath + "Items/";
			if (!System.IO.File.Exists (path + className + ".cs")) {
				code = "";
				code += "using UnityEngine;\n";
				code += "using UnityEngine.UI;\n";
				//code += "using System.Collections;\n";
				//code += "using System.Collections.Generic;\n";
				code += "\n";
				code += "public class " + className + " : " + className + "Base \n{";
				code += "\n";
				code += "    void Awake()\n";
				code += "    {\n";
				code += "        base.Awake();\n";
				code += "    }\n";
				code += "    public override void Data(object data)\n";
				code += "    {\n\n";
				code += "    }\n";
				if (handleBtn) {
					code += "    public override void OnButtonClickHandler(GameObject go)\n";
					code += "    {\n";
					code += "        base.OnButtonClickHandler(go);\n";
					code += "    }\n";
				}
				code += "}\n";
				path = Config.UIClassPath + "Items/";

				WriteClass (code, path, className);
			}

			GameObject prefab = PrefabUtility.CreatePrefab (Config.UIPrefabPath + "Items/" + className + ".prefab", listRoot.gameObject, ReplacePrefabOptions.Default);
			GameObject.DestroyImmediate (listRoot.gameObject);
			Debug.Log ("生成完毕！");
			AssetDatabase.Refresh ();
			AssetDatabase.SaveAssets ();
		}

		private static void generateListCode (Transform list)
		{
			if (list.childCount <= 0)
				return;
			Transform item = list.GetChild (0);
			listProperties.Clear ();
			Util.IterateChild (item, "", generateListProperty);

			generateListCodeAndPrefab (item, getCurrentItemClassName (root.name, list.name));
		}

		private static void WriteClass (string code, string path, string className)
		{
			System.IO.File.WriteAllText (path + className + ".cs", code, System.Text.UnicodeEncoding.UTF8);
		}

		private static string getCurrentItemClassName (string className, string itemName)
		{
			return System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase (className) +
			System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase (itemName) +
			"Item";
		}
	}
}
