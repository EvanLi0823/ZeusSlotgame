using System;
using UnityEngine;
using Libs;

public class AudioManager : MonoSingleton<AudioManager>
{
    private const int MaxEffectSource = 10;

    private const int MaxMusicSource = 10;

    private const string audioPath = "Audio/";

    protected static object syncRoot = new object();

    public SharePlaySound mPlayLib;

    private bool _enableSound = true;

    public bool EnableSound
    {
        get { return _enableSound; }
        set
        {
            this._enableSound = value;
            EnableAllAudio(this._enableSound);
        }
    }

    public static AudioManager GetInstance()
    {
        return Instance;
    }
    protected override void Init()
    {
        InitAudioSource();
    }
    private void InitAudioSource()
    {
        mPlayLib = new SharePlaySound(this.gameObject);

        UnityEngine.SceneManagement.SceneManager.sceneLoaded += delegate(UnityEngine.SceneManagement.Scene arg0,
            UnityEngine.SceneManagement.LoadSceneMode arg1)
        {
            this.mPlayLib.StopAndClearAllAudio();
//            StopAllAudio();
        };
    }

    #region 音效管理
    //needRepeate 解决相同音效播放间隔过近导致的中断问题
    public void PlayEffectAudio(AudioClip clip, bool loop = false, float value = 1.0f,bool needSame = true)
    {
        if (!_enableSound || clip == null) return;

        value = this.AudioVolume(clip.name, value);

        mPlayLib.PlayFirst(clip,true,loop,value,0,0); //只有两个地方用到这个方法，不用考虑是否用旧的音效
//        if (needSame)
//        {
//            SoundUnitItem item = mPlayLib.FindSameAudioClip(clip.name);
//            if(item == null)
//            {
//                mPlayLib.PlayFirst(clip,true,loop,value,0,0);
//            }
//            else
//            {
//                //资源池play
//                mPlayLib.PlayUnitItem(item,item.mAudioSource.clip,true,loop,value,0,0);
//            }    
//        }
//        else
//        {
//            mPlayLib.PlayFirst(clip,true,loop,value,0,0);
//        }
        
    }

    public void StopEffectAudio(string name)
    {
        if (!_enableSound || string.IsNullOrEmpty(name)) return;

        this.mPlayLib.StopEffectAudio(name);
    }

    public void StopAllEffectAudio()
    {
        this.mPlayLib.StopAudioByType(1);
    }

    
    public void AsyncPlayEffectAudio(string effectAudioName, bool loop = false, float volume = 1.0f,bool needSame = true)
    {
        if (!_enableSound) return;
        SoundUnitItem item = mPlayLib.FindSameAudioClip(effectAudioName , needSame);
        volume = this.AudioVolume(effectAudioName, volume);
        if (item != null)
        {
           
            mPlayLib.PlayUnitItem(item,item.mAudioSource.clip,true,loop,volume,0,0);
        }

        else
        {
           
            AsyncLoadSound(effectAudioName,true,loop,volume,0,0);
        }
    }
    

    #endregion

    #region 音乐管理

    //播放音乐
    public void PlayMusicAudio(AudioClip clip, bool loop = true, float value = 1.0f, float delaytime = 0,
        float time = 0f)
    {
        if (clip == null) return;
        mPlayLib.PlayFirst(clip,false,true,value,delaytime,time);
    }

    //停止音乐
    public void StopMusicAudio(string name, float time = 0f)
    {
        if (string.IsNullOrEmpty(name)) return;
        mPlayLib.StopMusicAudio(name,time);
    }

    //暂停音乐
    public void PauseMusicAudio(string name, float time = 0, float value = 1f)
    {
        if (string.IsNullOrEmpty(name)) return;
        mPlayLib.PauseMusicAudio(name,time,value);
    }
    //设置音量
    public void SetMusicAudioVolum(string name, float time = 0, float value = 1f)
    {
        if (string.IsNullOrEmpty(name)) return;
        mPlayLib.SetMusicAudioVolume(name,time,value);
    }
    //续播音乐
    public void UnPauseMusicAudio(string name, bool loop = true, float value = 1f, float time = 0)
    {
        if (string.IsNullOrEmpty(name)) return;
        
        if (!_enableSound) return;
        
        value = this.AudioVolume(name, value);

        SoundUnitItem item = mPlayLib.FindSameAudioClip(name);
        if (item != null)
        {
            mPlayLib.UnPauseMusicAudio(item, loop, value, time);
        }
        else
        {
            this.AsyncLoadSound(name, false,loop, value, time);

        }
        

    }

   

    //停止全部音乐
    public void StopAllMusicAudio()
    {
        this.mPlayLib.StopAudioByType(2);
    }
    private static object  sound = new object();
    public void AsyncPlayMusicAudio(string audioName, bool loop = true, float value = 1.0f, float delaytime = 0f,
        float time = 0f)
    {
        if (!_enableSound) return;
        
        lock (sound)
        {
            SoundUnitItem item = mPlayLib.FindSameAudioClip(audioName);
            if(item ==null)
            {
                AsyncLoadSound(audioName,false,loop,value,delaytime,time);
            }
            else
            {
                //资源池play
                mPlayLib.PlayUnitItem(item,item.mAudioSource.clip,false,loop,value,delaytime,time);
            }
        }
        
    }
    #endregion

    public float AudioVolume(string name, float value)
    {
        if (AudioSlotsConfig.Instance == null) return value;
        return AudioSlotsConfig.Instance.Volume(name, value);
    }

    #region enable sound
    
    public void EnableAllAudio(bool enable)
    {
        EnableEffectAudio(enable);
        EnableMusicAudio(enable);
    }

    //TODO 测试
    public void EnableEffectAudio(bool enable)
    {
        if (!enable)
        {
            this.mPlayLib.StopAudioByType(1);
        }
    }

    public void EnableMusicAudio(bool enable)
    {
        if(mPlayLib==null) return;
        this.mPlayLib.EnableMusicAudio(enable);
    }

    #endregion
    public void StopAllAudio()
    {
        StopAllEffectAudio();
        StopAllMusicAudio();
    }
    
    #region load加载的方式播放
    
    
    private void AsyncLoadSound(string audioName,bool isEffect = false, bool loop = false, float value = 1.0f, float delaytime = 0f,
        float time = 0f)
    {
        if (!_enableSound) return;
        
        StartCoroutine(Libs.ResourceLoadManager.Instance.AsyncLoadResource<AudioClip>(audioPath + audioName,
            (path, clip) =>
            {
//                    Debug.LogError(audioName+"666666666");
//                    Debug.LogError(audioName);
                if (clip == null)
                {
                    return;
                }
                mPlayLib.PlayFirst(clip,isEffect,loop,value,delaytime,time);
            }));
        
    }

    #endregion

    public void PauseSound()
    {
        this.mPlayLib.PauseAllSound();
    }

    public void ResumeSound()
    {
        this.mPlayLib.ResumeAllSound();
    }

    
}
