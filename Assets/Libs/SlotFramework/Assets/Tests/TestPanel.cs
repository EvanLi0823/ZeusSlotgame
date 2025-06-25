using UnityEngine;
public class TestPanel : MonoBehaviour {
	
	#if DEBUG||UNITY_EDITOR
	public static bool enableVipMachineTest = false;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Debug.isDebugBuild) {
			if (Input.touchCount == 2||Input.GetKeyDown(KeyCode.Return)) {
				if (!enableVipMachineTest) {
					enableVipMachineTest = true;
				}
			}
			if (Input.touchCount == 3||Input.GetKeyDown(KeyCode.Space)) {
				if (enableVipMachineTest) {
					enableVipMachineTest = false;
				}
			}
		}

	}
	bool initGUI = false;
	GUIStyle fontStyle;
	void OnGUI(){
		if (Debug.isDebugBuild&&enableVipMachineTest) {
			if (!initGUI) {
				initGUI = true;
				fontStyle = new GUIStyle ();  
				fontStyle.normal.background = null;    //设置背景填充  
				fontStyle.normal.textColor = new Color (1, 1, 1);   //设置字体颜色  
				fontStyle.fontSize = 24;       //字体大小  
			}
			
			GUI.Label (new Rect (300, 60, 250, 100), "EnableVIPMachineTest:" + enableVipMachineTest, fontStyle);
	
		}
	}
	#endif
}
