using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Classic;
using UnityEngine.UI;
using UnityEngine.Video;
using System;
/*
 * 
 * 一种在自己身上播放Anticipation动画的webmSymbolRender，除了Anticipation动画以外的其他动画还是和父类一样的方式播放
 */
public class WebmSymbolRenderAnticipation : WebmSymbolRender{
    [Header("播放anticipation动画的video")]
    public WebmSymbolVedio m_anticipationVideo;
    public override void PlayAnimation (int animationId,bool isLoop = true, System.Action VideoInitHandler=null, System.Action VideoCompleteHandler=null,float RepeatPlayStartTime =0f,bool isUseCache = true)
	{
		if (this.SymbolIndex == -1) {
			return;
		}

		if (animationId == 2) {
			animationId = 1;
		}
		if (ContainsSymbolVideoAnimationId(animationId)) {
			VideoClip clip = GetSymbolVideoClip (animationId);
			if (clip != null) {
                //如果是anicipation动画
                if(animationId == (int)BaseElementPanel.AnimationID.AnimationID_SmartSymbolReminder)
                {
                    m_SymbolVedio = m_anticipationVideo;
                }
                else
                {
                    m_SymbolVedio = animationParent.GetComponentInChildren<WebmSymbolVedio>();
                    if (m_SymbolVedio == null)
                    {
                        GameObject o = Instantiate(Resources.Load<GameObject>(WEBM_RAW_PATH) as GameObject, this.animationParent);
                        m_SymbolVedio = o.GetComponent<WebmSymbolVedio>();
                    }
                }

				m_SymbolVedio.CacheShow (this.SymbolIndex.ToString() +"_"+ animationId, clip,RepeatPlayStartTime, isLoop, delegate {
					EnableSymbolImage (false);
					if(VideoInitHandler !=null)
					{
						VideoInitHandler();
					}
				},delegate {
					if(VideoCompleteHandler != null){
						VideoCompleteHandler();
					}

				},isUseCache);

				ResetVideoPosition ();
			}

		} else {
			base.PlayAnimation (animationId,true, null, null,0f);
		}
	}
}
