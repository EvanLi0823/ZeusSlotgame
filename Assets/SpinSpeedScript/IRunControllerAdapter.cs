
using UnityEngine;
//棋盘的适配器
public interface IRunControllerAdapter {
//	Sprite GetSpriteBySymbolIndex(int symbolIndex);

//	SymbolMap.SymbolElementInfo GetSymbolInfoByName(string symbolName);

	void DoRunStopHandler(bool isFastStop);

	void DoEachReelStopHandler(int reelIndex);
	
	//轴开始减速的时间节点回调
	void DoEachReelSlowDownHandler(int reelIndex);

	void ReelBounceBackHandler(int reelIndex);

    void LastReelDistanceHandler(int reelIndex,SymbolRender render);

    void ChangeSmartAnimationPosition(int reelId, float offsetY);

    SymbolMap.SymbolElementInfo GetSymbolInfoByIndex(int symbolIndex);

	Classic.GameConfigs GetGameConfig();

    //void DoLastReel
}

//轮子的适配
public interface IReelAdapter{
	int GetReelShowNumber();
	int GetReelIndex();

	BoardConfigs GetBoardConfig();
}
