using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using System.Linq;

public class Test : MonoBehaviour
{
	public  ReelController Controller;
	// Use this for initialization
	void Start ()
	{
		Application.targetFrameRate = 60;
	}

	void Update ()
	{
		if (Input.GetKeyDown (KeyCode.Space)) {
			//			Debug.Log (SpinState);
//			Controller.DoReelsSpin ();
		} else if (Input.GetKeyDown (KeyCode.S)) {
			if (Time.timeScale == 1f) {
				Time.timeScale = 0f;
			} else {
				Time.timeScale = 1f;
			}
		} else if (Input.GetKeyDown (KeyCode.R)) {
			//			Reset ();
//			Controller.LayOut();
		}

//		if (Input.touchCount > 0) {
//			Controller.DoReelsSpin ();
//		}

		if (Input.touches.Any (x => x.phase == TouchPhase.Ended)) {

//			Controller.DoReelsSpin ();
		}

		UpdateTick ();
	}




	#region test

	void OnGUI ()
	{
		if (GUI.Button (new Rect (0f, 0f, 150f, 150f), "normal")) {
			Controller.SetAnitiReelIds (new List<int> ());
			Controller.SetFastMode (false);
		} else if (GUI.Button (new Rect (0f, 200f, 150f, 150f), "fast")) {
			Controller.SetFastMode (true);
//			Controller.SetAnitiReelIds(new List<int>{1});
		} else if (GUI.Button (new Rect (0f, 400f, 150f, 150f), "anti2")) {
//			Controller.SetFastPattern (false);
			Controller.SetAnitiReelIds (new List<int>{ 1, 2, 3 });
		} else if (GUI.Button (new Rect (0f, 600f, 150f, 150f), "scene")) {
			//			Controller.SetFastPattern (false);
//		Controller.SetAnitiReelIds(new List<int>{1,2,3});
			UnityEngine.SceneManagement.SceneManager.LoadScene (1);
		}

		DrawFPS ();
	}




	#endregion


	#region FPS

	long framesCount = 0;
	int currentFPS = 0;
	long currentFPSUpdateCount = 0;
	float timeLeft = 0;
	float accumulated = 0;
	const float updateInterval = 0.5f;

	private GUIStyle labelStyle;
	private string segment = "";

	protected GUIStyle LabelStyle {
		get {
			if (this.labelStyle == null) {
				this.labelStyle = new GUIStyle (GUI.skin.label);
				this.labelStyle.fontSize = (int)(Screen.dpi / 5f);
			}

			return this.labelStyle;
		}
	}

	private void DrawFPS ()
	{
		Color oldColor = GUI.color;
		if (currentFPS > 50) {
			GUI.color = new Color (0, 1, 0);
		} else if (currentFPS > 40) {
			GUI.color = new Color (1, 1, 0);
		} else {
			GUI.color = new Color (1, 0, 0);
		}
		GUI.Label (new Rect (128, 35, 1024, 128), "FPS: " + currentFPS + " SEG: " + segment, this.LabelStyle);
		GUI.color = oldColor;
	}

	private void UpdateTick ()
	{
		framesCount++;
		timeLeft -= Time.deltaTime;
		accumulated += Time.timeScale / Time.deltaTime;

		if (timeLeft <= 0) {
			currentFPS = (int)(accumulated / framesCount);
			currentFPSUpdateCount++;

			timeLeft = updateInterval;
			accumulated = 0;
			framesCount = 0;
		}
	}

	#endregion

}
