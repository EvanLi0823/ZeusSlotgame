using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Linq;
using Beebyte.Obfuscator;

/// <summary>
/// 工具类
/// </summary>
///
[Skip]
public class Util {

    public delegate IterationArguments UnitProcess(Transform unit, string path);
    public enum IterationArguments
    {
        Continue,       //继续迭代
        StopCurrent,    //跳过当前节点的迭代
        StopAll         //停止所有迭代
    }

    /// <summary>
    /// child 迭代器
    /// 迭代当前root的每一个孩子，以及孩子的孩子。。。。
    /// 并且把每个孩子交给processor进行处理
    /// </summary>
    /// <param name="r"></param>当前迭代过程的根节点
    /// <param name="path"></param>当前迭代节点的节点目录
    /// <param name="controller"></param>节点控制器
    public static void IterateChild(Transform r, string path, UnitProcess controller)
    {
        IterationArguments ctrler = controller(r, path);
        if (ctrler == IterationArguments.StopAll)
        {
            return;
        }
        else if (ctrler == IterationArguments.StopCurrent)
        { }
        else if (ctrler == IterationArguments.Continue)
        {
            foreach (Transform child in r)
            {
                IterateChild(child, path + child.name + "/", controller);
            }
        }
    }

    public static T FindUnder<T>(Transform root, string path) where T : UIBehaviour
    {
        T result = null;
        UnitProcess finder = (unit, p) =>
        {
            if (p != path) 
                return IterationArguments.Continue;

            result = unit.GetComponent<T>();

            if (result == null) 
                return IterationArguments.Continue;

            return IterationArguments.StopAll;
        };

        IterateChild(root, root.name + "/", finder);
        return result;
    }

    /// <summary>
    /// 销毁所有child
    /// </summary>
    /// <param name="transform">根</param>
    /// <param name="activeIncluded">是否包含activeSelf为false的对象</param>
    public static void DestroyChildren(Transform transform,bool activeIncluded=true)
    {
        if (transform != null)
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                Transform t = transform.GetChild(i);
                if (t != null)
                {
                    if (t.gameObject != null)
                    {
                        if (activeIncluded)
							GameObject.Destroy(t.gameObject);
                        else
                        {
                            if(t.gameObject.activeSelf)
                            {
								GameObject.Destroy(t.gameObject);
                            }
                        }
                    }
                }
            }
        }
    }
	/// <summary>
    /// 根据名字查找对象
    /// 1.返回名字符合的第一个对象
    /// 2.没找到就返回Default
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="transform"></param>
    /// <param name="objectName"></param>
    /// <returns></returns>
	public static T FindObjectByName<T>(Transform transform,string objectName) where T : UnityEngine.Object
    {
		if (string.IsNullOrEmpty(objectName)) return default(T);
		T com = null;
		for (int i = 0; i < transform.childCount; i++)
        {
			Transform t = transform.GetChild(i);
			if (t.name == objectName)
			{
				if (typeof(T) == typeof(Transform)) return t as T;
				if (typeof(T) == typeof(GameObject)) return t.gameObject as T;
			    com =  t.GetComponent<T>();
				if (com != null)
				{
					return com;
				}
                else
				{
					return default(T);
				}
			}
			com = FindObjectByName<T>(t, objectName);
			if (com != null) break;
		}
		return com;
	}

	public static  T GetComponentByName<T>(Transform trans,string objectName)where T : UnityEngine.Component
    {
		T t = FindObjectByName<T>(trans, objectName);
		if(t == null)
        {
			GameObject go = FindObjectByName<GameObject>(trans, objectName);
			t = go?.AddComponent<T>();
		}
		return t;
	}

    public static T FindObject<T>(Transform transform, string path) where T:UnityEngine.Object
    {
        //Debug.Log(path);
        if (string.IsNullOrEmpty(path)) 
            return default(T);

        Transform child = transform.Find(path);
        if (child == null) 
            return default(T);

        if (typeof(T) == typeof(GameObject))
            return child.gameObject as T;

        T com = child.GetComponent<T>();
        if (com == null) 
            return default(T);

        return com;
    }

    /// <summary>
    /// 支持对禁用对象的查找
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="path"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T FindObjectEx<T>(Transform transform, string path) where T:UnityEngine.Object
    {
	    T com = FindObject<T>(transform, path);
	    if (com != null) return com;
	    List<string> namesList = path.Split('/').ToList().Where((a) => { return !string.IsNullOrEmpty(a);}).ToList();
	    com = FindRecursive<T>(transform, namesList);
	    return com;
    }

    private static T FindRecursive<T>(Transform trans, List<string> names)
    {
	    if (names == null || names.Count == 0) return default(T);
	    foreach(Transform child in trans)
	    {
		    if (child.name.Equals(names[0]))
		    {
			    if (names.Count==1)
			    {
				    return child.GetComponent<T>();
			    }
			    else
			    {
				    names.RemoveAt(0);
				    return FindRecursive<T>(child, names);
			    }
		    }
	    }

	    return default(T);
    }
	public static string GetGameObjectPath(Transform transform)
	{
		string path = transform.name;
		while (transform.parent != null)
		{
			transform = transform.parent;
			path = transform.name + "/" + path;
		}
		return path;
	}

	public static void BecomeGrey (Transform transform, bool enable)
	{
		float rgb = enable ? 1f : 0.5f;
		Image[] images = transform.parent.GetComponentsInChildren<Image> ();
		foreach (Image image in images) {
			image.color = new Color (rgb, rgb, rgb, 1f);
		}
	}

	/// <summary>
	/// Loads the sprite.
	/// </summary>
	/// <returns>The sprite.</returns>
	/// <param name="resourcePath">Resource path.</param>
	public static Sprite loadSprite (string resourcePath)
	{
		Sprite result = null;
		GameObject go = Resources.Load<GameObject> (resourcePath);
		if (go == null) {
			return null;
		}
//		#if GAME_SLOTS

//		Image sr = go.GetComponent<Image>();
//		#else
		SpriteRenderer sr = go.GetComponent<SpriteRenderer>();

//		#endif
		if (sr != null) {
			result = sr.sprite;
		} else{ 
			Image image = go.GetComponent<Image>();
			if (image != null) {
				result = image.sprite;
			}
		}
		return result;
	}

	/// <summary>
	/// Sets the transform visible.
	/// </summary>
	/// <param name="root">Root.</param>
	/// <param name="isVisible">If set to <c>true</c> is visible.</param>
	/// <param name="ignoreRoot">If set to <c>true</c> ignore root.</param>
	public static void SetTransformVisible(Transform root,bool isVisible,bool ignoreRoot = false)
	{
		for (int i = root.childCount - 1; i >= 0; i--)
		{
			Transform t = root.GetChild(i);
			SetTransformVisible(t,isVisible);
			t.gameObject.SetActive(isVisible);
		}
		if (!ignoreRoot) {
			root.gameObject.SetActive (isVisible);
		}
	}

	public static List<T> FindChildrenIn<T>(GameObject parent) where T:Component
	{
		List<T> result = new List<T> ();
		T c= parent.GetComponent<T> ();
		if (c != null) {
			result.Add (c);
		}

		for (int i = 0; i < parent.transform.childCount; i++) {
			result.AddRange (FindChildrenIn<T> (parent.transform.GetChild (i).gameObject));
		}
		return result;
	}

	/// <summary>
	/// Sets the children layer.
	/// </summary>
	/// <param name="r">The red component.</param>
	/// <param name="layer">Layer.</param>
	public static void SetChildrenLayer(Transform r, int layer)
	{		
		r.gameObject.layer = layer;
		foreach (Transform child in r)
		{
			SetChildrenLayer (child, layer);
		}

	}
}
