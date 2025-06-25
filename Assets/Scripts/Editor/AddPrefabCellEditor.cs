using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(AddPrefabCell))]
public class AddPrefabCellEditor : Editor {

	public override void OnInspectorGUI ()
	{
//		base.OnInspectorGUI ();
		DrawDefaultInspector();
		AddPrefabCell addPrefab = (AddPrefabCell)target;
		if (GUILayout.Button ("create prefab")) {
			addPrefab.CreateCell ();
		}
	}
}
