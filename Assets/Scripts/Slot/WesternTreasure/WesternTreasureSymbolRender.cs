using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Classic;
using UnityEngine.UI;
using System;


public class WesternTreasureSymbolRender : SpineSymbolRender
{
    private WesternTreasureReelManager reelManager;

    protected override void SymbolChangeHandler()
    {
        if (this.m_SymbolSprite == null && this.m_SymbolImage == null) return;
        
        reelManager = runControllerAdapter as WesternTreasureReelManager;
        

        if (symbolIndex == 13)
        {
            symbolIndex = reelManager.spinResult.SpinBonusItem();
            //Debug.LogError("$$$$$$$$$$$$$$   22222222  "+symbolIndex);
            if (m_SymbolSprite != null) this.m_SymbolSprite.sprite = runControllerAdapter.GetGameConfig().GetBySymbolIndex(symbolIndex);
            if (m_SymbolImage != null) this.m_SymbolImage.sprite = runControllerAdapter.GetGameConfig().GetBySymbolIndex(symbolIndex);
        }
        EnableSymbolImage(true);
//        if (m_SymbolSprite != null) this.m_SymbolSprite.sprite = runControllerAdapter.GetGameConfig().GetBySymbolIndex(symbolIndex);
//        Debug.LogError("       symbolIndex: "+symbolIndex+"       PositionId: "+PositionId+"       ReelIndex : "+ReelIndex+"       m_SymbolSprite : ");
    }

    
    public override void PlayAnimation(int animationId, bool isLoop = true, System.Action VideoInitHandler = null, System.Action VideoCompleteHandler = null, float RepeatPlayStartTime = 0f, bool isUseCache = true)
    {
        base.PlayAnimation(animationId, isLoop, () =>
        {
            
        }, VideoCompleteHandler, RepeatPlayStartTime, isUseCache);
    }
   
    public override void PauseAnimation(bool notChange = false)
    {
        base.PauseAnimation(notChange);
    }

    public override void StopAnimation(bool showAnimationFrame = true)
    {
        base.StopAnimation(showAnimationFrame);  
    }
   
}
