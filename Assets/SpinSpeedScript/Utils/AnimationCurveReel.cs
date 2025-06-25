using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationCurveReel : MonoBehaviour {
	private float WholeLength;
//	private int RenderCount = 3;
	public AnimationCurve m_Curve;
	public List<SymbolRender> AllRenders= new List<SymbolRender>();

	public float RunTime = 3f;
	public float RunMultiple = 10f; 

	private int NextShowIndex = 4;

	// Use this for initialization
	void Start () {
		WholeLength = (this.transform as RectTransform).sizeDelta.y;

		Reset ();
	}

	void Reset()
	{
		float minY = -WholeLength / 2 - WholeLength / 6;
		float maxY = WholeLength/2 +WholeLength / 6;
		AllRenders.ForEach (delegate(SymbolRender obj) {
			obj.RecordPosition (minY , maxY);	
		});
	}

	void Update()
	{
		if (Input.GetKeyDown (KeyCode.Space)) {
			StartCoroutine (SpinTheWheel ());
		}
	}

	IEnumerator SpinTheWheel()
	{
		Reset ();

		float timer = 0f;
		while (timer < RunTime) {
			timer += Time.deltaTime;
			float x = timer / RunTime;
			float s = m_Curve.Evaluate (x) * WholeLength * RunMultiple;

			for (int i = 0; i < AllRenders.Count; i++) {
				
//				if (AllRenders [i].MoveDistance (s)) {
////					AllRenders [i].ChangeSymbol (this.NextShowIndex);
//					this.NextShowIndex++;
//				}
			}

			yield return 0;
		}
	}

}


