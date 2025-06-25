using System;
using UnityEngine;
using System.Collections;

public class SlotInit : MonoBehaviour {
	private bool m_HasInit = false;
	// Use this for initialization
//	void Start () {
//		if (!m_HasInit) {
//			m_HasInit = true;
//			ClassicGameConsole.ActiveGameConsole ();
//		}
//	}

void OnEnable()
	{
		if (!m_HasInit) {
			m_HasInit = true;
			AsyncLogger.Instance.StartTraceLog ("ActiveGameConsole");
			SlotGameconsole.ActiveGameConsole();
			
			AsyncLogger.Instance.EndTraceLog ("ActiveGameConsole");
		}
	}

}
