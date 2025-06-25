using UnityEngine;
using System.Collections;
using System;
namespace UnityEngine.UI
{
	public class ToggleButton : Toggle {
		[SerializeField]
		public GameObject ChoosePanel;

		protected override void Start()
		{
			base.Start ();
			this.onValueChanged.AddListener (RefreshState);
			RefreshState (isOn);
		}

		private void RefreshState(bool v)
		{
			if (this.ChoosePanel != null) {
				ChoosePanel.SetActive (v);
			}
		}
	}
}
