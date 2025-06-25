using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddPrefabCell : MonoBehaviour {

	public GameObject m_Cell;
	public void CreateCell()
	{
		if (m_Cell == null) {
			Debug.LogError ("cell prefab can not be null");
			return;
		}

		for (int i = transform.childCount - 1; i >= 0; i--) {
			Transform t = transform.GetChild (i);
			if (t != null) {
				if (t.gameObject != null) {
					DestroyImmediate (t.gameObject);
				}
			}

		}


		GameObject o = Instantiate (m_Cell, transform);
		o.name = m_Cell.name;
	}
}
