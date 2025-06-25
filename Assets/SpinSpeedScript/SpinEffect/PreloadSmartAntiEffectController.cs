using System.Collections;
using System.Collections.Generic;
using Classic;
using UnityEngine;
using UnityEngine.Video;
public class PreloadSmartAntiEffectController : SmartAntiEffectController
{
    private const string WEBM_RAW_PATH = "Prefab/Shared/WebmRawImage";

    protected override void PreloadSmartWebm(List<ResultContent.ReelResult> reelsSpinData, ReelManager reelManager)
    {
        foreach (var reelIndex in SmartPositions.Keys)
        {
            List<int> symbols = reelsSpinData[reelIndex].SymbolResults;
            foreach (var pos in SmartPositions[reelIndex].Keys)
            {
                //TODO 有待优化
                BaseElementPanel render = reelManager.GetSymbolRender(reelIndex, pos);
                if (render != null)
                {
                    StartCoroutine(LoadSmartWebm(render, symbols[pos], reelIndex, reelManager));
                }
            }
        }
        
    }

    private IEnumerator LoadSmartWebm(BaseElementPanel render, int symbolIndex, int reelIndex,ReelManager reelManager)
    {
        yield return new WaitForEndOfFrame();
        if (render != null)
        {
            string id = symbolIndex + "_" + reelIndex + "_" +
                        (int) BaseElementPanel.AnimationID.AnimationID_SmartSymbolReminder;
            GameObject o = Instantiate(Resources.Load<GameObject>(WEBM_RAW_PATH) as GameObject,
                render.animationParent);
            WebmSymbolVedio symbolVedio = o.GetComponent<WebmSymbolVedio>();
            render.AnimationGO = o;
            if (symbolVedio != null)
            {
                VideoPlayer videoPlayer = symbolVedio.transform.GetComponent<VideoPlayer>();
                VideoClip videoClip = GetSymbolVideoClip(symbolIndex,
                    (int) BaseElementPanel.AnimationID.AnimationID_SmartSymbolReminder,
                    reelManager.GetGameConfig());
                if (videoClip != null && videoPlayer != null)
                {
                    WebmSymbolAssets.Instance.PreloadVideoForSmart(id, videoClip, videoPlayer);
                }
            }
        }
    }

    private VideoClip GetSymbolVideoClip(int symbolIndex, int animationId, GameConfigs gameConfig)
    {
        if (gameConfig == null)
            return null;
        GameConfigs.VideoData videoData = gameConfig.elementResources [symbolIndex].VideoAnimations.Find (delegate(GameConfigs.VideoData obj) {
            return obj.AnimationId == animationId;
        });
        if (videoData == null)
            return null;
        return videoData.m_VideoAnimation;
    }
    //TODO 缓存webm时的symbolrender和播放时的可能不一致，
    // 所有可能无法 停掉Smart动画，2.9.0版本在调用StopAllSmartAndAntiEffectAnimation时 先让所有的动画都停掉
    public override void StopAllSmartAndAntiEffectAnimation(ReelManager reelManager)
    {
        foreach (var reelList in reelManager.boardController.SymbolRenderDic.Values)
        {
            foreach (var render in reelList.Values)
            {
                if(render != null)
                    render.StopAnimation();
            }
        }
        WebmSymbolAssets.Instance.RemoveAllCacheVideo();
    }
}
