using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Classic;
/// <summary>
/// Lock element controller.
/// 尽量将功能模块拆开，不要强行绑定到ReelManager上，降低耦合度。易于扩展和维护
/// </summary>
public class LockElementController : MonoBehaviour {
	public const string RESET_LOCKED_SYMBOLS = "ResetLockedSymbols";
	public const string ACTIVE_SPEICAL_SYMBOL = "ActiveSpecialSymbol";
	[HideInInspector]
	public List<LockReel> LockElementReels = new List<LockReel>();
	public LockElementCanvas lockElementCanvas;
	public string symbolName = "B01";
	public int maskLayer = 10;
	//public AnimationCanvas animationCanvas;
	private GameConfigs gameConfigs;
	[HideInInspector]
	public SymbolMap symbolMap;
	void Awake(){
		Messenger.AddListener (GameConstants.OnSlotMachineSceneInit, InitData);
		Messenger.AddListener (RESET_LOCKED_SYMBOLS,ResetLockSymbol);
		Messenger.AddListener<int,int,int,bool> (ACTIVE_SPEICAL_SYMBOL,ActiveSymbol);
	}

	void OnDestroy(){
		Messenger.RemoveListener (GameConstants.OnSlotMachineSceneInit, InitData);
		Messenger.RemoveListener (RESET_LOCKED_SYMBOLS,ResetLockSymbol);
		Messenger.RemoveListener<int,int,int,bool> (ACTIVE_SPEICAL_SYMBOL,ActiveSymbol);
	}
	/// <summary>
	/// Inits the data after initing ReelManager.
	/// </summary>
	void InitData(){
		if (gameConfigs==null) {
			gameConfigs = GetComponent<GameConfigs> ();
			symbolMap = GetComponent<ReelManager> ().symbolMap;
			LayoutReels ();
			Messenger.Broadcast (SpriteMaskInit.SPRITE_MASK_INIT_EVENT);
			ResetLockSymbol ();
		}
	}

	void LayoutReels(){
		float width = GetReelPanelWidth(0) / gameConfigs.PanelWidth;

		float startX = 0f;
		float endX = width;
		for (int j = 0; j < gameConfigs.GetReelNumber(); j++)
		{
			float reelOffsetX = gameConfigs.GetReelConfigs(j).reelOffsetX;
			float tempSpace = (1 - gameConfigs.GetReelConfigs(j).relativeHeight) / 2f;

			LockReel element = Instantiate(lockElementCanvas.lockReel) as LockReel;
			element.gameObject.transform.SetParent(lockElementCanvas.transform, false);
			RectTransform r = element.gameObject.transform as RectTransform;

			r.anchorMax = new Vector2(endX + reelOffsetX, 1 - tempSpace);
			r.anchorMin = new Vector2(startX + reelOffsetX, tempSpace);


			element.CenterAnchorX = (endX + startX) / 2f;

			//element.AnimationReel = animationCanvas.animationReel;
			LockElementReels.Add(element);
			bool IsEndReel = false;
			if (gameConfigs.isReverseRollReel)
			{
				IsEndReel = (j == 0);
			}
			else
			{
				IsEndReel = (j == gameConfigs.GetReelNumber() - 1);
			}
			element.InitElements(this, gameConfigs, IsEndReel);

			float space = GetReelSpacePercent(j);
			startX = endX + space;
			width = GetReelPanelWidth(j) / gameConfigs.PanelWidth;
			endX = startX + width;
		}
	}

	protected virtual float GetReelPanelWidth(int reelIndex){
		return gameConfigs.ReelPanelWidth;
	}
	protected virtual float GetReelSpacePercent(int reelIndex)
	{
		float space = 0;

		if (gameConfigs.GetReelNumber() > 1)
		{
			space = GetTotalReelPanelSpaceWidthPercent() / (gameConfigs.GetReelNumber() - 1);
		}
		return space;
	}

	protected virtual float GetTotalReelPanelSpaceWidthPercent()
	{
		float totalReelPanelSpaceWidthPercent = 1 - GetTotalReelPanelWidthPercent();
		if (totalReelPanelSpaceWidthPercent < 0)
		{
			totalReelPanelSpaceWidthPercent = 0;
		}
		return totalReelPanelSpaceWidthPercent;
	}

	protected virtual float GetTotalReelPanelWidthPercent()
	{
		float totalReelPanelWidth = 0;
		for (int i = 0; i < gameConfigs.GetReelNumber(); i++)
		{
			totalReelPanelWidth += GetReelPanelWidth(i);
		}

		if (gameConfigs.PanelWidth > 0)
		{
			float totalPanelWidthPercent = totalReelPanelWidth / gameConfigs.PanelWidth;
			if (totalPanelWidthPercent > 1)
			{
				totalPanelWidthPercent = 1;
			}
			return totalPanelWidthPercent;
		}
		return 0;
	}

	public virtual int GenSymbol(){
		return symbolMap.getSymbolIndex(symbolName);
	}

	public void ResetLockSymbol(){
		for (int i = 0; i < LockElementReels.Count; i++) {
			for (int j = 0; j < LockElementReels [i].Elements.Count; j++) {
				LockElementReels [i].Elements [j].EnableSymbolImage (false);
			}
		}
	}

	public void ActiveSymbol(int columnIndex,int rowIndex,int symbolIndex,bool active){
		
		if (columnIndex<0||columnIndex >= LockElementReels.Count)
			return;
		if (LockElementReels [columnIndex] == null)
			return;
		if (rowIndex < 0 || rowIndex >= LockElementReels [columnIndex].Elements.Count)
			return;
		if (LockElementReels [columnIndex].Elements [rowIndex] == null)
			return;
		
		LockElementPanel element = LockElementReels [columnIndex].Elements [rowIndex];
		element.SymbolIndex = symbolIndex;
		element.EnableSymbolImage (active);

	}
}
