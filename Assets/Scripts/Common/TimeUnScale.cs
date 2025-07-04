﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeUnScale : MonoBehaviour {

	private Animator _animator;

	void Awake()
	{
		_animator = GetComponent<Animator> ();
		if (_animator != null) {
			_animator.updateMode = AnimatorUpdateMode.UnscaledTime;
		}
	}
}
