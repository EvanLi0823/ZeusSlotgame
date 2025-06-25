using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Classic;
using UnityEngine.Video;
using System;
using Libs;

public class WebmSymbolAssets
{


	// Use this for initialization
//	public GameObject m_VideoObject;
//	public VideoClip m_TestData;
//	public VideoClip m_TestBigData;

	public static WebmSymbolAssets _instance = null;

	public static WebmSymbolAssets Instance
	{
		get{
			if (_instance == null) {
				_instance = new WebmSymbolAssets ();

				Messenger.AddListener(GameConstants.MachineBacktoLobby_Key, _instance.backToLobby);
			}
			return _instance;
		}
	}

	public Dictionary<string, VideoData> mCommonVideo = new Dictionary<string, VideoData>();
	public Dictionary<string, VideoData> mCacheVideo = new Dictionary<string, VideoData>();
	void backToLobby()
	{
		mCacheVideo.Clear();
		mCommonVideo.Clear ();
	}

	public void PauseVideo(string animationKey)
	{
		if (mCacheVideo.ContainsKey(animationKey) && mCacheVideo[animationKey] != null)
		{
			if (mCacheVideo [animationKey].Video == null)
				return;
			mCacheVideo [animationKey].Video.Pause();
			mCacheVideo [animationKey].Video.frame = 0;
			return;
		}
		
		if (mCommonVideo.ContainsKey (animationKey) && mCommonVideo[animationKey] != null) {
			if (mCommonVideo [animationKey].Video == null)
				return;
			mCommonVideo [animationKey].Video.Pause();
			mCommonVideo [animationKey].Video.frame = 0;
		}
	}

	public void StopVideo(string animationKey)
	{
		if (string.IsNullOrEmpty (animationKey)) return;
		
		if (mCacheVideo.ContainsKey (animationKey) && mCacheVideo[animationKey] != null) {
			if (mCacheVideo [animationKey].Video == null)
				return;
			mCacheVideo [animationKey].Video.Stop();
			mCacheVideo [animationKey].Video.frame = 0;
			mCacheVideo.Remove(animationKey);
			return;
		}
		
		if (mCommonVideo.ContainsKey (animationKey) && mCommonVideo[animationKey] != null) {
			if (mCommonVideo [animationKey].Video == null)
				return;
			mCommonVideo [animationKey].Video.Stop();
			mCommonVideo [animationKey].Video.frame = 0;
			mCommonVideo.Remove (animationKey);
		}
	}

	public void ClearKey()
	{
		mCacheVideo.Clear();
		mCommonVideo.Clear ();
	}

	private VideoData FetchVideoPlayer(string animationKey, VideoPlayer video)
	{
		if (mCacheVideo.ContainsKey(animationKey) && mCacheVideo[animationKey] != null)
		{
			return mCacheVideo[animationKey];
		}

		if (mCommonVideo.ContainsKey (animationKey) && mCommonVideo [animationKey] != null) {
			return mCommonVideo [animationKey];
		} else {
			VideoData vData = new VideoData (video);
			mCommonVideo [animationKey] = vData;
		}
		return mCommonVideo[animationKey];
	}

//	1. 拿出一个来加载video，如加载完则回调，可做个delegate
	public void LoadVideo(string id, VideoPlayer video,VideoClip clip,bool isLoop,System.Action<string,VideoPlayer>prepareHadler,bool isUseCache = true,WebmSymbolVedio owner=null)
	{
		VideoPlayer vp = null;
		if (isUseCache) {
			VideoData vd = FetchVideoPlayer (id, video);
			vp = vd.Video;
		} else {
			vp = video;
		}

		if (BaseSlotMachineController.Instance != null && BaseSlotMachineController.Instance.reelManager != null)
		{
			BaseSlotMachineController.Instance.reelManager.StartCoroutine ( prepareVideo( id,vp,clip,isLoop,prepareHadler,owner) );
		}
	}
//	public delegate void PrepareCallback(string key,VideoPlayer videoPlayer);
//	public event PrepareCallback prepareHadler;// = new PrepareCallback(null);
	private IEnumerator prepareVideo(string id,VideoPlayer videoPlayer, VideoClip clip,bool isLoop, System.Action<string,VideoPlayer>prepareHadler,WebmSymbolVedio owner=null)
	{
		if (videoPlayer == null||owner==null)
			yield break;

		if (videoPlayer.clip != clip) {
			videoPlayer.clip = clip;
			videoPlayer.EnableAudioTrack (0, false);  //先禁用audio
		}
		videoPlayer.Prepare ();//播放之前必须准备，否则同一条带子使用相同的webm对象触发webm不会再播放webm动画
while (videoPlayer!=null && !videoPlayer.isPrepared)
		{
			if (owner==null||owner.isAniStopped)
			{
				yield break;
			}
			yield return null;
		}
        if (videoPlayer == null)
        {
            yield break;
        }
		videoPlayer.isLooping = isLoop; //为了防止抖动问题

        yield return GameConstants.FrameTime;

//		if (!videoPlayer.isPlaying) {
//			videoPlayer.frame = 0;
//			videoPlayer.Play ();
//		}

        if (prepareHadler != null) {
	        prepareHadler (id,videoPlayer);
		}
	}
	
	// 计算出smart的位置后，就提前加载webm.
	public void PreloadVideoForSmart(string id, VideoClip clip, VideoPlayer videoPlayer)
	{
		videoPlayer.renderMode = VideoRenderMode.RenderTexture;
		videoPlayer.clip = clip;
		VideoData vData = new VideoData(videoPlayer);
		mCacheVideo[id] = vData;
		vData.Video.Prepare();
	}

	public void RemoveAllCacheVideo()
	{
		mCacheVideo.Clear();
	}
	
	public class VideoData
	{
		public enum State  {
			None,
			Loading,
			Downloaded
		}

		public State LoadState = State.None;

		public VideoPlayer Video;

		public VideoData(VideoPlayer _v)
		{
			this.Video = _v;
		}
	}
}