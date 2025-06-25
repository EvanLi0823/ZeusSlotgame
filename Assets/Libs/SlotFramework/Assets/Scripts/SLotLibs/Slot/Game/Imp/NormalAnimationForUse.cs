using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalAnimationForUse : MonoBehaviour {
	[SerializeField]
	private  List<int> m_ShowAnimationIds = new List<int>();
	public List<int> ShowAnimationIds
	{
		get{
			return m_ShowAnimationIds;
		}
	}
}
