using UnityEngine;
using System.Collections;

public class DontDestroy : MonoBehaviour
{
	public static DontDestroy instance;

	void Awake () 
	{
		if (!instance) 
		{
			instance = this;
			DontDestroyOnLoad (gameObject);
		}
		else 
		{
			Destroy(gameObject);
		}
	}
}