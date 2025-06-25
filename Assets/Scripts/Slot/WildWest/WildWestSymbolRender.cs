using UnityEngine;
using UnityEngine.UI;

public class WildWestSymbolRender : SpineSymbolRender
{
    private WildWestReelManager WildWest; 
    public bool stop = false;
    public Text count;

    public override void PlayAnimation (int animationId,bool isLoop = true, System.Action VideoInitHandler=null, System.Action VideoCompleteHandler=null,float RepeatPlayStartTime =0f,bool isUseCache = true)
	{
        stop = false;
        base.PlayAnimation(animationId, isLoop, VideoInitHandler, VideoCompleteHandler, RepeatPlayStartTime, isUseCache);
        if(this.symbolIndex == 4) SetSymbolCanvas(7);
        if(this.symbolIndex == 5) SetSymbolCanvas(6);
    }

    private void SetSymbolCanvas(int canvaIndex)
	{
		if(animationParent.GetComponent<Canvas>() != null) return;
		Canvas aniCanvas = animationParent.gameObject.AddComponent<Canvas>();
		aniCanvas.overrideSorting = true;
		aniCanvas.sortingOrder = canvaIndex;	
	}

    public override void StopAnimation (bool showAnimationFrame = true)   
	{
		base.StopAnimation(showAnimationFrame);
        if(animationParent == null) return;;
		Canvas aniCanvas = animationParent.GetComponent<Canvas>();
		if(aniCanvas != null) Destroy(aniCanvas);
	}

    protected override void SymbolChangeHandler()
	{
        this.SetCountUI(0);
	    WildWest = this.runControllerAdapter as WildWestReelManager;
        if(this.symbolIndex == 22 || this.symbolIndex == 23) {this.ChangeSymbol(this.ItemId(this.symbolIndex), SymbolChangeState.Running); return;} 
        if(this.m_SymbolChangeState == SymbolChangeState.ReelStop)
        {
            if(IsPendantId(this.symbolIndex)) this.SetCountUI(WildWest.spinresult.pendantCount[this.ReelIndex][this.PositionId]);
        }else if(this.m_SymbolChangeState == SymbolChangeState.Running)
        {
            if(IsPendantId(this.symbolIndex)) this.SetCountUI(WildWest.spinresult.PendantCount());
        }
	}

    public int ItemId(int symbolId)
    {
        if(symbolId == 22) return WildWest.spinresult.R01();
        if(symbolId == 23) return WildWest.spinresult.R02();
        return Random.Range(0, 15);
    }

    public void SetCountUI(int value)
    {        
        if(value != 0)
        {
            count.text = value.ToString();
            count.transform.parent.gameObject.SetActive(true);
        }else
        {
            count.text = "";
            count.transform.parent.gameObject.SetActive(false);
        }
    }

    public bool IsPendantId(int symbolId)
    {
        if(symbolId >= 16 && symbolId <= 21) return true;
        return false;
    }
}
