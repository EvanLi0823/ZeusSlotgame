using System;

public class BlankReel : Reel
{
	protected override void InitMaxMinPosition()
	{
		int blankReelShow = int.Parse(Math.Floor(ReelShowNum/2F).ToString());
		float DisplayLength = blankReelShow * this.m_boardConfig.SymbolHeight *  this.m_boardConfig.ReelConfigs[ReelIndex].RenderMultiple;
		float halfHeight = DisplayLength / blankReelShow / 2;
		minY = -DisplayLength / 2 - halfHeight;
		maxY = DisplayLength/2 + halfHeight *(2*(ReelSymbolNum-ReelShowNum)-1) ; //考虑多余1个随机的情况
	}

	protected override float RenderInitialBotttomPosition()
	{
		return -m_boardConfig.ReelShowHeight(this.ReelIndex)/2 - this.m_boardConfig.SymbolHeight/2;
	}

	protected override float SymbolRenderDistance(float height)
	{
		return height/2;
	}

	protected override bool IsRunStop(float s)
	{
		return  this.m_TotalLength - s < (ReelShowNum ) * this.m_boardConfig.SymbolHeight/2*this.m_boardConfig.ReelConfigs[this.ReelIndex].RenderMultiple;
	}
}
