using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
public class WebmVideoRender : MonoBehaviour
{
    public VideoClip videoToPlay;
    [HideInInspector]
    private VideoPlayer videoPlayer;
    public RawImage m_ShowRawImage;
    //[Tooltip("默认是否开始就播放")]
    //public bool InitialToPlay = true;
    private System.Action VideoCompleteHandler = null;

    [HideInInspector]
    //public RenderTexture VideoRT;
    void Awake()
    {
        //m_ShowRawImage = GetComponent<RawImage>();
        videoPlayer = GetComponent<VideoPlayer>();
        videoPlayer.loopPointReached += EndReached;

        //VideoRT = new RenderTexture(100, 100, 16, RenderTextureFormat.ARGB32);

        //VideoRT.Create();
    }

    public void PlayVideo(VideoClip _clip, bool isLoop = true, System.Action InitCallback = null, System.Action _videoCompleteCallback = null, float _scale = 1f)
    {
        if (videoPlayer == null)
        {
            return;
        }
        this.VideoCompleteHandler = _videoCompleteCallback;

        StartCoroutine(Play(_clip, isLoop, InitCallback, _scale));
    }

    public void StopVideo(bool stopVideo = true)
    {
        if (videoPlayer != null)
        {
            if (stopVideo)
            {
                //if (VideoRT != null)
                //{
                //    VideoRT.Release();
                //    VideoRT.DiscardContents();
                //    //RenderTexture.ReleaseTemporary(VideoRT);
                //}
                if (videoPlayer.targetTexture != null)
                {
                    videoPlayer.targetTexture.Release();
                    videoPlayer.targetTexture.DiscardContents();
                }


                if (videoPlayer.texture != null)
                {
                    (videoPlayer.texture as RenderTexture).Release();
                    //Destroy(videoPlayer.texture);
                    //RenderTexture.ReleaseTemporary(videoPlayer.texture as RenderTexture);

                }
                videoPlayer.Stop();
            }
            else
            {
                videoPlayer.Pause();
            }
        }
        if (this.m_ShowRawImage.texture != null)
        {
            //(this.m_ShowRawImage.texture as RenderTexture).Release();
            //RenderTexture.ReleaseTemporary(this.m_ShowRawImage.texture as RenderTexture);
        }
        this.m_ShowRawImage.texture = null;
        this.m_ShowRawImage.color = Color.clear;
    }
    public void PauseVideo(bool hideVideo = true)
    {
        if (videoPlayer != null)
        {
            videoPlayer.Pause();
        }

        if (hideVideo)
        {
            this.m_ShowRawImage.texture = null;
            this.m_ShowRawImage.color = Color.clear;
        }
    }

    private IEnumerator Play(VideoClip _clip, bool isLoop = true, System.Action InitCallback = null, float _scale = 1f)
    {
        videoPlayer.clip = _clip;
        videoPlayer.EnableAudioTrack(0, false);  //先禁用audio
        videoPlayer.Prepare();
        videoPlayer.isLooping = isLoop;
        videoPlayer.renderMode = VideoRenderMode.RenderTexture;

        while (!videoPlayer.isPrepared)
        {
            yield return null;
        }

        PrepareStart(InitCallback, _scale);
    }

    private void EndReached(UnityEngine.Video.VideoPlayer vp)
    {
        if (VideoCompleteHandler != null)
        {
            VideoCompleteHandler();
        }
    }

    public void SetNativeSize(float _scale)
    {
        float w = videoPlayer.width * _scale;
        float h = videoPlayer.height * _scale;
        RectTransform rect = m_ShowRawImage.transform as RectTransform;
        rect.sizeDelta = new Vector2(w, h);

    }

    public void PlayVideo(string _clipPath, bool isLoop = true, System.Action InitCallback = null, System.Action _videoCompleteCallback = null, float _scale = 1f)
    {
        if (videoPlayer == null)
        {
            return;
        }
        this.VideoCompleteHandler = _videoCompleteCallback;

        StartCoroutine(Play(_clipPath, isLoop, InitCallback, _scale));
    }
    #region 按照path播放
    private IEnumerator Play(string _clipPath, bool isLoop = true, System.Action InitCallback = null, float _scale = 1f)
    {
        videoPlayer.source = VideoSource.Url;
        videoPlayer.url = _clipPath;
        videoPlayer.EnableAudioTrack(0, false);  //先禁用audio
        videoPlayer.Prepare();
        videoPlayer.isLooping = isLoop;
        videoPlayer.renderMode = VideoRenderMode.RenderTexture;
        while (!videoPlayer.isPrepared)
        {
            yield return null;
        }

        PrepareStart(InitCallback, _scale);
    }

    private void PrepareStart(System.Action InitCallback = null, float _scale = 1f)
    {

        //videoPlayer.targetTexture = VideoRT;
        //VideoRT.width = (int)videoPlayer.width;
        //VideoRT.height = (int)videoPlayer.height;

        this.m_ShowRawImage.color = Color.white;
        this.m_ShowRawImage.texture = videoPlayer.texture;
        //m_ShowRawImage.SetNativeSize ();
        SetNativeSize(_scale);



        if (!videoPlayer.isPlaying)
        {
            videoPlayer.frame = 0;
            videoPlayer.time = 0;
            videoPlayer.Play();
        }


        if (InitCallback != null)
        {
            InitCallback();
        }
    }

    #endregion

    private void OnDestroy()
    {
        //if (this.VideoRT != null)
        //{
        //    this.VideoRT.Release();
        //}
    }

    public void ResumeVideo()
    {
        if (videoPlayer != null)
        {
            if (videoPlayer.isPaused)
            {
                videoPlayer.Play();
            }
        }
    }

    public void SuspendVideo()
    {
        if (videoPlayer != null)
        {
            if (videoPlayer.isPlaying)
            {
                videoPlayer.Pause();
            }
        }
    }

    public void SetCacheVideo(RenderTexture rt)
    {
        this.videoPlayer.targetTexture = rt;
        this.videoPlayer.Play();
        this.m_ShowRawImage.texture = rt;
        this.m_ShowRawImage.color = Color.white;
    }
}
