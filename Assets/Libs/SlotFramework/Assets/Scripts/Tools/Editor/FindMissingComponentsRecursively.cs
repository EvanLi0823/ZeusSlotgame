using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine.UI;

// from: http://wiki.unity3d.com/index.php?title=FindMissingScripts
public class FindMissingComponentsRecursively : EditorWindow 
{
	static int go_count = 0, components_count = 0, missing_count = 0;
	
	[MenuItem("Window/Find Missing Scripts (All)")]
	static void FindInAll()
	{
		go_count = 0;
		components_count = 0;
		missing_count = 0;
		foreach (var root in SceneRoots()) 
		{
			//Debug.Log(root);
			FindInGO(root);
		}
		Debug.Log(string.Format("Searched {0} GameObjects, {1} components, found {2} missing", go_count, components_count, missing_count));
	}

	public static bool FindInSelection(GameObject go)
	{
		go_count = 0;
		components_count = 0;
		missing_count = 0;
		FindInGO(go);
		if (missing_count > 0)
			Debug.LogAssertion(string.Format("Searched {0} GameObjects, {1} components, found {2} missing", go_count, components_count, missing_count));
		return (missing_count > 0);
	}

	public static void FindInGO(GameObject g)
	{
		go_count++;
		Component[] components = g.GetComponents<Component>();
		for (int i = 0; i < components.Length; i++)
		{
			components_count++;
			if (components[i] == null)
			{
				missing_count++;
				string s = g.name;
				Transform t = g.transform;
				while (t.parent != null) 
				{
					s = t.parent.name +"/"+s;
					t = t.parent;
				}
				Debug.LogAssertion (s + " has an empty script attached in position: " + i, g);
			}
		}
		// Now recurse through each child GO (if there are any):
		foreach (Transform childT in g.transform)
		{
			//Debug.Log("Searching " + childT.name  + " " );
			FindInGO(childT.gameObject);
		}
	}
	
	static IEnumerable<GameObject> SceneRoots()
	{
		var prop = new HierarchyProperty(HierarchyType.GameObjects);
		var expanded = new int[0];
		while (prop.Next(expanded)) {
			yield return prop.pptrValue as GameObject;
		}
	}
	
	[MenuItem("Assets/AssetBundles/系统/检测image_texture是否丢失", false, 200)]
	public static void CheckSelectionImageMissing()
	{
		for (int i = 0; i < Selection.objects.Length; i++)
		{
			var obj = Selection.objects[i] as GameObject;
			CheckImageMissing(obj);
		}
	}
	public static bool CheckImageMissing(GameObject obj)
    {
        if (obj != null)
        {
            var images = obj.GetComponentsInChildren<Image>();
            int count1 = 0;
            foreach (var item in images)
            {
                if (item.sprite == null)
                {
                    count1 += 1;
                    string componentName = GetPath(item.transform, item.name);
                    Debug.LogError(count1 + " missing image: " + componentName);
                }
            }

            var raw = obj.GetComponentsInChildren<RawImage>();
            int count2 = 0;
            foreach (var item in raw)
            {
                if (item.texture == null)
                {
                    count2 += 1;
                    string componentName = GetPath(item.transform, item.name);
                    Debug.LogError(count2 + " missing raw image: " + componentName);
                }
            }

            if (count1 + count2 > 0) return true;
        }
        return false;
    }
    static string GetPath(Transform transform, string originName)
    {
        if (transform.parent == null)
        {
            return originName;
        }
        string tmpName = transform.parent.name + "/" + originName;
        return GetPath(transform.parent, tmpName);
    }

    [MenuItem("Assets/修改弹窗层级(层级减10)")]
    public static void ChangedDialogLayer()
    {
	    for (int i = 0; i < Selection.objects.Length; i++)
	    {
		    GameObject go = Selection.objects[i] as GameObject;
		    Log.LogWhiteColor(go.name);
		    Change(go,10);
	    }
    }
    [MenuItem("Assets/修改弹窗层级(层级减17)")]
    public static void ChangeDialogLayer()
    {
	    for (int i = 0; i < Selection.objects.Length; i++)
	    {
		    GameObject go = Selection.objects[i] as GameObject;
		    Log.LogWhiteColor(go.name);
		    Change(go,17);
	    }
    }

    public static void Change(GameObject go,int layerNum)
    {
	    if (go)
	    {
		    string path = AssetDatabase.GetAssetPath(go);
		    go = Instantiate(go);
		    var canvas = go.GetComponentsInChildren<Canvas>();
		    for (int i = 0; i < canvas.Length; i++)
		    {
			    if (canvas[i].overrideSorting)
			    {
				    int old = canvas[i].sortingOrder;
				    canvas[i].sortingOrder -= layerNum;
				    Log.LogLimeColor(canvas[i].name+"   "+old+"=====>"+canvas[i].sortingOrder);
			    }
		    }
		    
		    var particle = Util.FindChildrenIn<ParticleSystem> (go);
		    for (int i = 0; i < particle.Count; i++)
		    {
			    int old = particle[i].GetComponent<Renderer>().sortingOrder;
			    particle[i].GetComponent<Renderer>().sortingOrder -= layerNum;
			    Log.LogYellowColor(particle[i].name+"   "+old+"=====>"+particle[i].GetComponent<Renderer>().sortingOrder);
			    
		    }
		    PrefabUtility.SaveAsPrefabAsset(go,path);
		    DestroyImmediate(go);
	    }
    }
    
    [MenuItem("Assets/检查MissingReference资源")]
    public static void FindMissing()
    {
        for (int i = 0; i < Selection.objects.Length; i++)
        {
			Find(Selection.objects[i] as GameObject);
        }
    }
    private static Dictionary<UnityEngine.Object, List<UnityEngine.Object>> prefabs = new Dictionary<UnityEngine.Object, List<UnityEngine.Object>>();
    private static Dictionary<UnityEngine.Object, string> refPaths = new Dictionary<UnityEngine.Object, string>();
    public static bool Find(GameObject go)
    {
        prefabs.Clear();
        refPaths.Clear();
        if (go)
        {
            Component[] cps = go.GetComponentsInChildren<Component>(true);//获取这个物体身上所有的组件
            foreach (var cp in cps)//遍历每一个组件
            {
                if (!cp)
                {
                    if (!prefabs.ContainsKey(go))
                    {
                        prefabs.Add(go, new List<UnityEngine.Object>() { cp });
                    }
                    else
                    {
                        prefabs[go].Add(cp);
                    }
                    continue;
                }
                SerializedObject so = new SerializedObject(cp);//生成一个组件对应的S俄日阿里则对Object对象 用于遍历这个组件的所有属性
                var iter = so.GetIterator();//拿到迭代器
                var objRefValueMethod = typeof(SerializedProperty).GetProperty("objectReferenceStringValue",
	                    BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                while (iter.NextVisible(true))//如果有下一个属性
                {
					//如果这个属性类型是引用类型的
                    if (iter.propertyType == SerializedPropertyType.ObjectReference)
                    {
						string objectReferenceStringValue = string.Empty;

						if (objRefValueMethod != null)
						{
							objectReferenceStringValue = (string) objRefValueMethod.GetGetMethod(true)
								.Invoke(iter, new object[] { });
						}

						if (iter.objectReferenceValue == null && (iter.objectReferenceInstanceIDValue != 0|| objectReferenceStringValue.StartsWith("Missing")))
                        {
                            if (!refPaths.ContainsKey(cp)) refPaths.Add(cp, AssetDatabase.GetAssetPath(go)+" : "+cp.gameObject.name+" : "+iter.propertyPath);
                            else refPaths[cp] += " | " + iter.propertyPath;
                            if (prefabs.ContainsKey(go))
                            {
                                if(!prefabs[go].Contains(cp))prefabs[go].Add(cp);
                            }
                            else
                            {
                                prefabs.Add(go, new List<UnityEngine.Object>() { cp });
                            }
                        }
                    }
                }
            }
        }
        return  PrintData();
    }
     //以下只是将查找结果显示
    private static bool PrintData()
    {
        int count = prefabs.Count;
        foreach (var item in prefabs)
        {
            foreach (var cp in item.Value)
            {
                if (cp)
                {
                    if (refPaths.ContainsKey(cp))
                    {
                        Debug.LogAssertion("引用丢失："+cp.GetType().Name+" : "+refPaths[cp]);
                    }
                }
                else
                {
                    Debug.LogAssertion("引用丢失："+item.Key.name+" : "+"Script missing");
                }
            }
        }
        prefabs.Clear();
        refPaths.Clear();
        return count > 0;
    }

    [MenuItem("Assets/检查弹窗贴图")]
    public static void FindImageOnDialog()
    {
	    for (int i = 0; i < Selection.objects.Length; i++)
	    {
		    GameObject go = Selection.objects[i] as GameObject;
		    Component[] cps = go.GetComponentsInChildren<Component>(true); //获取这个物体身上所有的组件
		    foreach (var cp in cps) //遍历每一个组件
		    {
			    if (!cp)
			    {
				    return;
			    }

			    if (cp.GetType().Name.Equals("Image"))
			    {
				    if(!((cp as Image).sprite == null))
						Debug.LogAssertion(AssetDatabase.GetAssetPath((cp as Image).sprite),AssetDatabase.LoadAssetAtPath<Object> (GetRelativeAssetsPath (AssetDatabase.GetAssetPath((cp as Image).sprite))));
				    else
					    Debug.LogAssertion("贴图丢失");
			    }
		    }
	    }
    }
    static private string GetRelativeAssetsPath (string path)
    {
	    return "Assets" + Path.GetFullPath (path).Replace (Path.GetFullPath (Application.dataPath), "").Replace ('\\', '/');
    }
}