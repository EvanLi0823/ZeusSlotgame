using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

/*思路为用缓存video或非缓存
 * 1.缓存video
 *  在WebmSymbolAssets中保存索引symbolIndex+animationId的videoPlayer。
 * 需调用CacheShow函数
 * 在关卡结束需要clear所有数据
 * 2.非缓存
 * 可以直接配置videoToPlay，直接调用显示，也可以外部调用ShowVideo函数
 * 
*/
public class WebmSymbolVedio : MonoBehaviour {

	//Raw Image to Show Video Images [Assign from the Editor]
//	public RawImage image;
	//Video To Play [Assign from the Editor]
	public VideoClip videoToPlay;

	[HideInInspector]
	protected VideoPlayer videoPlayer;
	private VideoSource videoSource;

	protected RawImage m_ShowRawImage;

    protected delegate void VideoDelegate();

	protected VideoDelegate VideoCompleteHandler = null;

	protected bool IsStopState = false;

	[HideInInspector]
	public bool isAniStopped;
	public void SetRawImage(RawImage _img)
	{
		this.m_ShowRawImage = _img;
	}

	public bool IsLoop = true;

    protected bool IsFirstVideoPlay = true;//for startHandler, firstTime no response

	//Audio
//	private AudioSource audioSource;

	// Use this for initialization
	void Awake()
	{
//		image.enabled = false;
//		Application.runInBackground = true;
//		StartCoroutine(LoadVideo());
		m_ShowRawImage = GetComponent<RawImage>();
		videoPlayer = GetComponent<VideoPlayer> ();

		videoPlayer.loopPointReached += EndReached;
        videoPlayer.started += StartHandler;

		videoPlayer.skipOnDrop = true;
	}

	void Start()
	{
		if (this.videoToPlay != null) {
			this.ShowVideo (this.videoToPlay, videoPlayer.isLooping);
		}
	}

//	public void ShowVideo(VideoPlayer _video,System.Action callback=null)
//	{
////		videoPlayer = _video;
//		this.m_ShowRawImage.color = Color.white;
//		this.m_ShowRawImage.texture = _video.texture;
//
////		Debug.Log (33333);
//			_video.Pause ();
//			_video.frame = 0;
//			_video.time = 0;
//		if (!_video.isPlaying) {
//			_video.Play ();
//		}
//
//		if (callback != null) {
//			callback ();
//		}
//	}

	//先设置nativeSize，如有需要则
	public void ShowVideo(VideoClip _clip,bool isLoop=true, System.Action InitCallback=null,System.Action _videoCompleteCallback=null)
	{
		if (videoPlayer == null) {
			return;
		}
		this.VideoCompleteHandler = new VideoDelegate( _videoCompleteCallback);

		StartCoroutine(Play (_clip,isLoop,InitCallback));
	}

//	IEnumerator LoadVideo()
//	{
//
//		//Add VideoPlayer to the GameObject
//		videoPlayer = gameObject.AddComponent<VideoPlayer>();
//		videoPlayer.renderMode = VideoRenderMode.APIOnly;
//		//Add AudioSource
////		audioSource = gameObject.AddComponent<AudioSource>();
//
//		//		videoPlayer.loopPointReached += EndReached;
//		//		videoPlayer.prepareCompleted += PrepareCompleted;
//
//		//Disable Play on Awake for both Video and Audio
//		videoPlayer.playOnAwake = false;
//		audioSource.playOnAwake = true;
//
//		//We want to play from video clip not from url
//		videoPlayer.source = VideoSource.VideoClip;
//
//		//Set Audio Output to AudioSource
//		videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
//
//		//Assign the Audio from Video to AudioSource to be played
//		videoPlayer.EnableAudioTrack(0, true);
////		videoPlayer.SetTargetAudioSource(0, audioSource);
//
//		//Set video To Play then prepare Audio to prevent Buffering
//		videoPlayer.clip = videoToPlay;
//		videoPlayer.Prepare();
//		videoPlayer.waitForFirstFrame = false;
//
//		//Wait until video is prepared
//		while (!videoPlayer.isPrepared)
//		{
//			//			Debug.Log("Preparing Video");
//			yield return null;
//		}
//
//		//		Debug.Log("Done Preparing Video");
//
//		//Assign the Texture from Video to RawImage to be displayed
////		image.enabled = true;
////		image.texture = videoPlayer.texture;
//
//		//Play Video
////		m_ShowRawImage = this.GetComponentInParent<RawImage>();
//		m_ShowRawImage.enabled = true;
//		m_ShowRawImage.texture = videoPlayer.texture;
//		videoPlayer.frame= 0;
//		videoPlayer.Play ();
//	}


	/// <summary>
	/// 是否真的去停止video，设置为true的话会在一定程度上造成卡顿，最好为默认值
	/// </summary>
	/// <param name="realStop">Real stop.</param>
	public void Stop(bool stopVideo = false)
	{
		if (videoPlayer != null) {
			if (stopVideo) {
				videoPlayer.Stop ();
			} else {
//				StartCoroutine (StopVideo ());
				videoPlayer.Pause ();
			}
		}

//		videoPlayer.texture = null;
//		this.m_ShowRawImage.enabled = false;
		this.m_ShowRawImage.texture = null;
		this.m_ShowRawImage.color = Color.clear;
	}

	private IEnumerator StopVideo()
	{
		videoPlayer.Pause ();
		yield return null;
	}

	public void Pause(bool hideVideo = true)
	{
		if (videoPlayer != null) {
			videoPlayer.Pause ();
//			videoPlayer.Stop ();
//			videoPlayer.frame= 0;
		}

		if (hideVideo) {
			this.m_ShowRawImage.texture = null;
			this.m_ShowRawImage.color = Color.clear;
		}
	}

	public virtual IEnumerator Play(VideoClip _clip,bool isLoop=true,System.Action InitCallback = null)
	{
		videoPlayer.clip = _clip;
		videoPlayer.EnableAudioTrack (0, false);  //先禁用audio
		videoPlayer.Prepare ();
		videoPlayer.isLooping = isLoop;
		while (!videoPlayer.isPrepared)
		{
			yield return null;
		}


		this.m_ShowRawImage.color = Color.white;
		this.m_ShowRawImage.texture = videoPlayer.texture;
		m_ShowRawImage.SetNativeSize ();

		if (!videoPlayer.isPlaying) {
			videoPlayer.frame = 0;
			videoPlayer.time = 0;
			videoPlayer.Play ();
            this.IsFirstVideoPlay = true;
		}

		if (InitCallback != null) {
			InitCallback ();
		}
	}

	public void SetLoop (bool isLoop = false)
	{
		if (videoPlayer != null) {
			videoPlayer.isLooping = isLoop;
		}
	}

	public Texture GetVedioTexture()
	{
		if (videoPlayer != null) {
			return videoPlayer.texture;
		}
		return null;
	}

	public void SetScaleSize(Vector3 v)
	{
		this.transform.localScale = v;
	}

	void EndReached(UnityEngine.Video.VideoPlayer vp)
	{
		if (VideoCompleteHandler!=null) {
			VideoCompleteHandler ();
		}
        if (!Mathf.Approximately(this.RepeatPlayStartTime, 0f))
        {
            if (videoPlayer != null)
            {
                videoPlayer.Play();
            }
        }    
	}

    void StartHandler(VideoPlayer vp)
    {
        if(IsFirstVideoPlay)
        {
            IsFirstVideoPlay = false;
            return;
        }
        if (!Mathf.Approximately(this.RepeatPlayStartTime, 0f))
        {
            if (videoPlayer != null)
            {
                videoPlayer.time = RepeatPlayStartTime;
            }
        }
    }

	#region
	//##################################################################
	//用缓存方式
	//1.先把symbol放入缓存中
	//2.prepare的时候delegate回调
//	public 

	protected string CacheId;//缓存动画id
	public virtual void CacheShow(string id, VideoClip _clip, float _repeatStartTime = 0f,bool isLoop=true,System.Action InitCallback = null,System.Action _videoCompleteCallback=null,bool isUseCache=true)
	{
//		WebmSymbolAssets.PrepareCallback handler = delegate(string key,VideoPlayer _videoPlayer) {
//			this.m_ShowRawImage.color = Color.white;
//			this.m_ShowRawImage.texture = _videoPlayer.texture;
//			m_ShowRawImage.SetNativeSize ();
//
//			if (InitCallback != null) {
//				InitCallback ();
//			}
//		};
		isAniStopped = false;//启动携程
		this.RepeatPlayStartTime = _repeatStartTime;
		this.IsLoop = isLoop;

		IsStopState = false;

		System.Action<string,VideoPlayer> callback =  delegate(string key,VideoPlayer _videoPlayer) {
			if(!this.IsStopState){
				this.m_ShowRawImage.color = Color.white;
				this.m_ShowRawImage.texture = _videoPlayer.texture;
				m_ShowRawImage.SetNativeSize ();

//				if (!_videoPlayer.isPlaying) {
					_videoPlayer.frame = 0;
                    this.IsFirstVideoPlay = true;
                    _videoPlayer.Play ();
//				}

				if (InitCallback != null) {
					InitCallback ();
				}

				//complete handler
				//if(completeCoroutine !=null)
				//{
				//	StopCoroutine(completeCoroutine);
				//}

				//if(RepeatPlayStartTime!=0f)
				//{
				//	completeCoroutine = StartCoroutine(JudgeCompleteHandler());
				//}
			}
		};

		if (string.IsNullOrEmpty (id)) {
			return;
		}

		WebmSymbolAssets.Instance.LoadVideo (id, this.videoPlayer, _clip,isLoop, callback,isUseCache,this);
		this.CacheId = id;

        this.VideoCompleteHandler = new VideoDelegate(_videoCompleteCallback);
	}

	public void CachePause()
	{
		if (string.IsNullOrEmpty (this.CacheId)) {
			return;
		}
		if(this.m_ShowRawImage != null)
		{
			this.m_ShowRawImage.texture = null;
			this.m_ShowRawImage.color = Color.clear;
		}
		WebmSymbolAssets.Instance.PauseVideo (this.CacheId);

		IsStopState = true;

        if(this.videoPlayer != null && this.videoPlayer.isPlaying)
        {
            videoPlayer.Pause();

        }
    }

	public void CacheStop()
	{
		isAniStopped = true;//停止自身启动的携程
		WebmSymbolAssets.Instance.StopVideo (this.CacheId);
		if(this.m_ShowRawImage != null)
		{
			this.m_ShowRawImage.texture = null;
			this.m_ShowRawImage.color = Color.clear;
		}
		CacheId = null;

		IsStopState = true;

        if (this.videoPlayer != null && this.videoPlayer.isPlaying)
        {
            videoPlayer.Stop();

        }

        //清除回调
  //      if (completeCoroutine != null) {
		//	StopCoroutine (completeCoroutine);
		//}
	}

	#endregion

	//public void PlayVideoTime(float timeStart)
	//{
	//	if (videoPlayer != null) {
	//		videoPlayer.time = timeStart;
	//		videoPlayer.Play ();
	//	}
	//}
	//public VideoPlayer GetVideoPlayer()
	//{
	//	return this.videoPlayer;
	//}

	#region 结束的回调
	//private Coroutine completeCoroutine;
	private IEnumerator JudgeCompleteHandler()
	{
        yield break;

		while (true) {
			if (this.videoPlayer != null && this.videoPlayer.isPlaying) {
				//有可能最后一帧会出现问题找不到，保险起见整两帧
				if (this.videoPlayer.frame == (long)this.videoPlayer.frameCount-1 || this.videoPlayer.frame == (long)this.videoPlayer.frameCount) {
					if (VideoCompleteHandler != null) {
						VideoCompleteHandler ();
					}

					this.videoPlayer.time = RepeatPlayStartTime;
				} 
			}
			yield return GameConstants.FrameTime;
		}
	}

	//再次重复播放的时候的开始时间
	public float RepeatPlayStartTime = 0f;

	#endregion
}
