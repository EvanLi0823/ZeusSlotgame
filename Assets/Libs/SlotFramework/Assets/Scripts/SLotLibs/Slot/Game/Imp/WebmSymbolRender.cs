using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Classic;
using UnityEngine.UI;
using UnityEngine.Video;
using System;
/*
 * 
 * 如果gameconfig配置了对应的动画的webm，则加载webm的prefab，调用播放webm的接口
 * video的大小显示尺寸为输出的video大小，如需要更改其位置需要在调用PlayAnimation之后调用Move偏移函数。
 */
public class WebmSymbolRender : AnimatiorElement {
	protected const string WEBM_RAW_PATH = "Prefab/Shared/WebmRawImage";
//	private WebmSymbolAssets m_vedioManager;
//	public RawImage  m_vedioRawImage;

	protected WebmSymbolVedio m_SymbolVedio ;

//	public Action VideoCompleteHandler = null;
//	public Action VideoInitHandler = null;

	//video动画是否为循环的，在play播放动画之前可以设置这个值
//	[HideInInspector]
//	public bool IsVideoLoop = true;
	//播放到最后一帧的时候，重新播放的起始时间
//	public float RepeatPlayStartTime = 0f;

	public override void InitElement (BaseReel ReelContronller, int SymbolIndex)
	{
		base.InitElement (ReelContronller, SymbolIndex);
		if (SymbolIndex !=-1 && ContainsSymbolVideo()) {
			GameObject o = Instantiate (Resources.Load<GameObject> (WEBM_RAW_PATH) as GameObject, this.animationParent);
			m_SymbolVedio = o.GetComponent<WebmSymbolVedio> ();
		}
	}
	public override void PlayAnimation (int animationId,bool isLoop = true, System.Action VideoInitHandler=null, System.Action VideoCompleteHandler=null,float RepeatPlayStartTime =0f,bool isUseCache = true)
	{
		if (this.SymbolIndex == -1) {
			return;
		}

//		Debug.Log (isLoop+this.SymbolIndex.ToString() +"_"+ animationId);
//		m_SymbolVedio = null;//m_vedioManager.GetSymbolVedio(this.SymbolIndex,animationId);

//		WebmSymbolVedio vedio = Instantiate (m_SymbolVedio, this.m_vedioRawImage.transform) as WebmSymbolVedio;

//		VideoPlayer clip = WebmSymbolAssets.Instance.GetSymbolVideoPlayer (this.SymbolIndex, animationId);
//		m_SymbolVedio.ShowVideo (clip,delegate() {
//
//			EnableSymbolImage (false);
//		});

		if (animationId == 2) {
			animationId = 1;
		}

		if (ContainsSymbolVideoAnimationId(animationId)) {
			VideoClip clip = GetSymbolVideoClip (animationId);
			if (clip != null) {
				m_SymbolVedio =animationParent.GetComponentInChildren<WebmSymbolVedio> ();
				if (m_SymbolVedio== null) {
					GameObject o = Instantiate (Resources.Load<GameObject> (WEBM_RAW_PATH) as GameObject, this.animationParent);
					m_SymbolVedio = o.GetComponent<WebmSymbolVedio> ();
                    AnimationGO = o;
                }

                string id = this.SymbolIndex.ToString() + "_" + animationId;
                if (animationId == (int)AnimationID.AnimationID_SmartSymbolReminder)
                {
                    id = this.SymbolIndex.ToString() + "_" + ReelIndex + "_" + animationId;
                }
                m_SymbolVedio.CacheShow (id, clip,RepeatPlayStartTime, isLoop, delegate {
					EnableSymbolImage (false);
					if(VideoInitHandler !=null)
					{
						VideoInitHandler();
					}
				},delegate {
					if(VideoCompleteHandler != null){
						VideoCompleteHandler();
					}

//					if(m_SymbolVedio.IsLoop == false && m_SymbolVedio.RepeatPlayStartTime == 0f)
//					{
////						Debug.Log(666);
					//	this.PauseAnimation();
					//}
				},isUseCache);

				ResetVideoPosition ();
			}

		} else {
			base.PlayAnimation (animationId,true, null, null,0f);
		}
	}


	public override void PauseAnimation (bool notChange = false)
	{
        if(m_SymbolVedio==null)
        {
            m_SymbolVedio = animationParent.GetComponentInChildren<WebmSymbolVedio>();
        }

        if (m_SymbolVedio != null){
			m_SymbolVedio.CachePause ();
			EnableSymbolImage (true);
		}

		if (animator != null) {
			animator.SetInteger ("state", 0);
			if (notChange) {
				EnableSymbolImage (true);
				if (AnimationGO != null) {
					AnimationGO.SetActive (false);
				}
			}
		}
	}

	public override void StopAnimation (bool showAnimationFrame = true)
	{
        if(m_SymbolVedio == null && animationParent != null)
        {
            m_SymbolVedio = animationParent.GetComponentInChildren<WebmSymbolVedio>();
        }
        if (m_SymbolVedio != null) {
			m_SymbolVedio.CacheStop ();

            //			IsVideoLoop = true;
            //			VideoCompleteHandler = VideoInitHandler = null;
        }

		if (animator != null || AnimationGO!=null) {
			DestroyAnimation ();
			animator = null;

            AnimationGO = null;
        }

		EnableSymbolImage (true);

        //		m_vedioRawImage.texture = null;
        //		m_vedioRawImage.enabled = false;

        //		Util.DestroyChildren (this.m_vedioRawImage.transform);
    }

	public virtual void ChangeVideoScaleSize(Vector3 v)
	{
		if (m_SymbolVedio != null) {
			m_SymbolVedio.SetScaleSize (v);
		}
	}

    protected void ResetVideoPosition()
	{
		(m_SymbolVedio.transform as RectTransform).localPosition = Vector3.zero;
	}

	//参数为移动的相对位置
	//需要在playAnimation之后调用此方法，因为playAnimation方法包含ResetVideoPosition。顺序错了会出问题
	public void MoveVideoOffset(Vector3 v)
	{
		if (m_SymbolVedio == null) {
			return;
		}

		if (!ContainsSymbolVideo ()) {
			return;
		}

		RectTransform rt = m_SymbolVedio.transform as RectTransform;

		Vector3 result = new Vector3 (rt.localPosition.x + v.x, rt.localPosition.y + v.y, 0f);

		(m_SymbolVedio.transform as RectTransform).localPosition = result;
	}

	//获得video的clip
	public virtual VideoClip GetSymbolVideoClip(int animationId)
	{
		if (animationId == 1 || animationId == 2) {
			animationId = 1;
		}

		GameConfigs gameConfig = GetGameConfig();

		if (gameConfig == null) {
			return null;
		}

		GameConfigs.VideoData videoData = gameConfig.elementResources [this.SymbolIndex].VideoAnimations.Find (delegate(GameConfigs.VideoData obj) {
			return obj.AnimationId == animationId;
		});

		if (videoData == null) {
//			Debug.LogError (SymbolIndex+" symbol的video动画"+ animationId + " is null");
			return null;
		}

		return videoData.m_VideoAnimation;
	}

	//判断是否需要video，用gameconfig的视频长度来决定
	public bool ContainsSymbolVideo()
	{
		GameConfigs gameConfig = GetGameConfig();

		if (gameConfig == null) {
			return false;
		}

		if (gameConfig.elementResources.Length <= this.SymbolIndex) {
			Debug.LogError (this.SymbolIndex + "超出了所有elements的长度");
			return false;
		}

		return gameConfig.elementResources [this.SymbolIndex].VideoAnimations != null && gameConfig.elementResources [this.SymbolIndex].VideoAnimations.Count > 0;
	}

	public virtual bool ContainsSymbolVideoAnimationId(int animationId)
	{
		if (ContainsSymbolVideo ()) {
			GameConfigs gameConfig = GetGameConfig();
			bool bContainAniId = gameConfig.elementResources [this.SymbolIndex].VideoAnimations.Exists (delegate(GameConfigs.VideoData obj) {
				return obj.AnimationId == animationId;
			});
			return bContainAniId;
		}
		return false;
	}

	public WebmSymbolVedio GetWebmSymbolVedio()
	{
		return this.m_SymbolVedio;
	}
}
