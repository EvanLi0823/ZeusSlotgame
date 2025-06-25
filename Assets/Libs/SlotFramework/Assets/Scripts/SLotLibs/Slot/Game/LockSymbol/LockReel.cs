using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Classic;
public class LockReel : MonoBehaviour {

	[HideInInspector]
	public float CenterAnchorX;  //锚点中心x值
	protected int PostionOfReelData = 0;
	public int ElementNumber {
		get;
		set;
	}
	public LockElementController lockController {
		get;
		set;
	}
	public int Index {
		get;
		set;
	}
	public GameConfigs elementConfigs {
		get;
		set;
	}
	public Dictionary<int,LockElementPanel> positionMap {
		get;
		set;
	}
	public bool IsEndReel {
		get;
		set;
	}
	public List<LockElementPanel> Elements {
		get;
		set;
	}
	public List<int> Result {
		get;
		set;
	}

	public bool RunWithReelData {
		get;
		set;
	}
	protected float elementRelativelyHeight;
	protected float addWhenChange;
	public virtual void InitElements (LockElementController GameContronller, GameConfigs elementConfigs, bool IsEndReel)
	{
		InitData (GameContronller,elementConfigs,IsEndReel);
		Layout (ElementNumber + 1);
	}
	protected virtual void InitData(LockElementController GameContronller, GameConfigs elementConfigs, bool IsEndReel){
		PostionOfReelData = 0;
		this.lockController = GameContronller;
		Index = this.lockController.LockElementReels.IndexOf (this);

		positionMap = new Dictionary<int, LockElementPanel> ();
		this.IsEndReel = IsEndReel;
		Elements = new List<LockElementPanel> ();
		this.elementConfigs = elementConfigs;
		this.Result = new List<int> ();
		GameConfigs.ReelConfigs reelConfigs = this.elementConfigs.GetReelConfigs (this.Index);
		RunWithReelData = false;
		ElementNumber = reelConfigs.ElementNumbers;
		elementRelativelyHeight = reelConfigs.elementHeight / reelConfigs.PanelHeight;

	}
	protected void CreateXOffsetByY(RectTransform r){
		float offsetX = Mathf.Abs (((r.anchorMax.y + r.anchorMin.y) / 2f) - 0.5f)*elementConfigs.reelConfigs[Index].offsetX;
		r.anchorMax = new Vector2 (1+offsetX, r.anchorMax.y);
		r.anchorMin = new Vector2 (offsetX, r.anchorMin.y);
	}

	protected virtual int GenSymbol (){
		return lockController.GenSymbol ();
	}

	protected void Layout(int layoutElementNumber){
		if (elementConfigs.hasBlank) {
			addWhenChange = elementRelativelyHeight * (layoutElementNumber / 2.0f);
		} else {
			addWhenChange = elementRelativelyHeight * (layoutElementNumber);
		}

		List<LockElementPanel> elements = new List<LockElementPanel>();
		List<GameObject> animationElements = new List<GameObject>();

		for (int i = 0; i < layoutElementNumber; i++) {
			LockElementPanel element = Instantiate(lockController.lockElementCanvas.lockElementPanel) as LockElementPanel;
			element.gameObject.transform.SetParent(this.transform, false);
			elements.Add(element);
			Elements.Add(element);
		}

		float startY = 0;

		float endY = elementRelativelyHeight;


		if (elementConfigs.hasBlank) {
			startY = -0.5f*elementRelativelyHeight;
			endY = 0.5f*elementRelativelyHeight;
		}

		for (int j = 0; j < layoutElementNumber; j++) {
			LockElementPanel element = elements[j];
			RectTransform r = element.gameObject.transform as RectTransform;
			r.anchorMax = new Vector2 (1, startY + elementRelativelyHeight);
			r.anchorMin = new Vector2 (0, startY);
			CreateXOffsetByY (r);

			element.InitElement (this, GenSymbol ());
			element.IndexOfDisplay = j;

			if (elementConfigs.hasBlank) {
				startY += elementRelativelyHeight / 2f;
			} else {
				startY += elementRelativelyHeight;
			}

			element.SetOrderZ(j);
		}
	}
}
