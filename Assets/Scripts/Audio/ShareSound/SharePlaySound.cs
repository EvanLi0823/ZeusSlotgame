using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Libs
{
    public class SoundUnitItem
    {
        public AudioSource mAudioSource;

        public bool IsALive
        {
            get => EligibleAudioSource && mAudioSource.isPlaying;
        }

        public bool EligibleAudioSource
        {
            get { return mAudioSource != null && mAudioSource.clip != null; }
        }

        public bool SameAudio(string name)
        {
            return EligibleAudioSource && mAudioSource.clip.name.Equals(name);
        }

        public bool IsAudioPlaying
        {
            get => EligibleAudioSource && mAudioSource.isPlaying;
        }

        public bool IsIdleClip
        {
            get => mAudioSource.clip == null || !mAudioSource.isPlaying;
        }

        public SoundUnitItem(AudioSource mAudio)
        {
            this.mAudioSource = mAudio;
        }

        public bool IsEffect { set; get; } = false;

        //处理是否在pause中的music，只对music有用。play和stop的时候也都要设置
        public bool IsPausing { set; get; } = false;

        public string ToString()
        {
            if (mAudioSource.clip != null)
                return mAudioSource.name + "   isPlaying: " + mAudioSource.isPlaying + "    IsPausing:" + IsPausing;
            return "null";
        }
    }

    public class SharePlaySound
    {
        public List<SoundUnitItem> AudioQueue { private set; get; }

        public SharePlaySound(GameObject go, int maxLength = 15 )
        {
            AudioQueue = new List<SoundUnitItem>();
            for (int i = 0; i < maxLength; i++)
            {
                AudioSource _audioSource = go.AddComponent<AudioSource>();
                _audioSource.volume = 1f;
                _audioSource.loop = false;
                _audioSource.playOnAwake = false; //这个根据需要进行区分是否music还是effect

                SoundUnitItem unitItem = new SoundUnitItem(_audioSource);

                AudioQueue.Add(unitItem);
            }

            //TODO: 场景切换的时候停止音效
//            UnityEngine.SceneManagement.SceneManager.sceneLoaded += delegate(UnityEngine.SceneManagement.Scene arg0,
//                UnityEngine.SceneManagement.LoadSceneMode arg1)
//            {
//                StopAllAudio();
//            };
        }
        
        /// <summary>
        /// 将当前播放的放到最前面
        /// </summary>
        public void SortUnitItems(SoundUnitItem _item )
        {
            AudioQueue.Remove(_item);
            AudioQueue.Add(_item);
        }

        /// <summary>
        /// 对象池中相同名字的clip，确保是有值
        /// </summary>
        /// <param name="name"></param>
        /// <param name="isIdentical">是否是唯一的,音乐的唯一，音效看需求; 非唯一且如果没播放则返回true </param>
        /// <returns></returns>
        public SoundUnitItem FindSameAudioClip(string name,bool isIdentical = true)
        {
            SoundUnitItem ret = this.AudioQueue.Find(delegate(SoundUnitItem item)
            {
                if (item.SameAudio(name))
                {
                    if(isIdentical)
                    {
                        return true;
                    }
                    else if(!item.mAudioSource.isPlaying)
                    {
                        return true;
                    }
                }

                return false;
            });

            return ret;
        }

        public SoundUnitItem FindUsableAudioSource(string name)
        {
            SoundUnitItem ret = null;

            ret = this.AudioQueue.Find((item) => item.IsIdleClip);

            return ret;
        }

        #region 音乐

        /// <summary>
        /// 有clip 的直接播放在没播放的audioSource上
        /// </summary>
        /// <param name="clip"></param>
        /// <param name="isEffect">是否是音效还是bg音乐</param>
        /// <param name="loop"></param>
        /// <param name="value"></param>
        /// <param name="delaytime"></param>
        /// <param name="time"></param>
        public void PlayFirst(AudioClip clip,bool isEffect = false, bool loop = false, float value = 1.0f, float delaytime = 0,
            float time = 0f)
        {
            if (clip == null)
            {
                Debug.LogError("null........");
            }
            SoundUnitItem item = this.FindUsableAudioSource(clip.name);
            this.PlayUnitItem(item,clip,isEffect,loop,value,delaytime,time);
        }
        
        public float AudioVolume(string name, float value)
        {
            if (AudioSlotsConfig.Instance == null) return value;
            return AudioSlotsConfig.Instance.Volume(name, value);
        }
        
        /// <summary>
        /// 找出共享可用的或空闲的item进行播放
        /// </summary>
        /// <param name="item"></param>
        /// <param name="clip"></param>
        /// <param name="isEffect">是否是音效还是bg音乐</param>
        /// <param name="loop"></param>
        /// <param name="value"></param>
        /// <param name="delaytime"></param>
        /// <param name="time"></param>
        public void PlayUnitItem(SoundUnitItem item,AudioClip clip,bool isEffect = false, bool loop = false, float value = 1.0f, float delaytime = 0, float time = 0f)
        {
            if (item == null)
                return;
            AudioSource _source = item.mAudioSource;
            if (_source == null)
            {
                return;
            }

            if (_source.clip != null && _source.isPlaying)
            {
                _source.Stop();
            }
            _source.DOKill();
            
            _source.clip = clip;
            _source.loop = loop;
            if (time > 0)
            {
                _source.volume = 0f;
                _source.DOFade(value, time).SetDelay(delaytime).OnStart(() => { _source.Play(); }).SetUpdate(true);
            }
            else if (delaytime > 0)
            {
                _source.DOFade(value, time).SetDelay(delaytime).OnStart(() => { _source.Play(); }).SetUpdate(true);
            }
            else
            {
                _source.volume = value;
                _source.Play();
            }

            SortUnitItems(item);
            item.IsPausing = false;
            item.IsEffect = isEffect;
        }

        /// <summary>
        /// 从对象池中查找对象，尝试找到了则play，没有则返回false
        /// </summary>
        /// <param name="name"></param>
        /// <param name="loop"></param>
        /// <param name="value"></param>
        /// <param name="time"></param>
        /// <returns>对象池中是否存在可播放的内容</returns>
        public void UnPauseMusicAudio(SoundUnitItem item , bool loop = true, float value = 1f, float time = 0)
        {
            AudioSource _source = item.mAudioSource;
//            if (_source == null || _source.clip == null || _source.clip.name != name)
//                return false;

            _source.DOKill();
            _source.loop = loop;
            if (time > 0)
            {
                _source.volume = 0f;
                _source.DOFade(value, time).OnComplete(() =>
                {
                    _source.DOKill();
                    _source.UnPause();
                    if (_source.isPlaying)
                    {
                        _source.Play();
                    }
                }).SetUpdate(true);
            }
            else
            {
                _source.volume = value;
                _source.UnPause();
                if (!_source.isPlaying)
                {
                    _source.Play();
                }
            }

            this.SortUnitItems(item); 
            item.IsPausing = false;
        }

        //停止音乐
        public void StopMusicAudio(string name, float time = 0f)
        {
            if (string.IsNullOrEmpty(name)) return;

            foreach (SoundUnitItem item in AudioQueue)
            {
                AudioSource music = item.mAudioSource;
                if (music.clip != null && music.clip.name == name)
                {
                    music.DOKill();
                    if (time > 0)
                    {
                        music.DOFade(0f, time).OnComplete(() =>
                        {
                            music.DOKill();
                            music.Stop();
                        }).SetUpdate(true);
                    }
                    else
                    {
                        music.volume = 0;
                        music.Stop();
                    }
                }
                item.IsPausing = false;
            }
        }

        //暂停音乐
        public void PauseMusicAudio(string name, float time = 0, float value = 1f)
        {
            if (string.IsNullOrEmpty(name)) return;

            foreach (SoundUnitItem item in AudioQueue)
            {
                AudioSource music = item.mAudioSource;
                if (music.clip != null && music.clip.name == name && music.isPlaying)
                {
                    music.DOKill();
                    if (music.volume >= value) music.volume = value;
                    if (time > 0)
                    {
                        music.loop = false;
                        music.DOFade(0, time).OnComplete(() =>
                        {
                            music.DOKill();
                            music.Pause();
                        }).SetUpdate(true);
                    }
                    else
                    {
                        music.Pause();
                    }

                    item.IsPausing = false;

                    break;
                }
            }
        }

        public void SetMusicAudioVolume(string name, float time = 0, float value = 1f)
        {
            if (string.IsNullOrEmpty(name)) return;

            foreach (SoundUnitItem item in AudioQueue)
            {
                AudioSource music = item.mAudioSource;
                if (music.clip != null && music.clip.name == name && music.isPlaying)
                {
                    music.DOKill();
                    if (time > 0)
                    {
                        music.DOFade(value, time).OnComplete(() =>
                        {
                            music.DOKill();
                        }).SetUpdate(true);
                    }
                    else
                    {
                        music.volume = value;
                    }

                    item.IsPausing = false;

                    break;
                }
            }
        }

        #region enable开关

        public void EnableMusicAudio(bool enable)
        {
            foreach (SoundUnitItem item in AudioQueue)
            {
                AudioSource music = item.mAudioSource;
//                if (music != null && music.enabled != enable)
//                {
//                    music.enabled = enable;
                if(music !=null){
                    if (music.clip != null)
                    {
                        music.DOKill();
                        if (enable)
                        {
                            if (music.loop && !item.IsEffect)
                            {
                                music.Play();
                            }
                        }
                        else
                        {
                            music.Stop();
                        }
                    }
                }
                item.IsPausing = false;
            }
        }

        #endregion

        #endregion

        #region 音效

        public void StopEffectAudio(string name)
        {
            if (string.IsNullOrEmpty(name)) return;

            foreach (SoundUnitItem item in AudioQueue)
            {
                AudioSource music = item.mAudioSource;
                if (music.clip != null && music.clip.name == name)
                {
                    music.DOKill();
                    {
                        music.Stop();
                    }
                }
                item.IsPausing = false;
            }
        }

        #endregion

        #region 停止音乐、音效
        /// <summary>
        /// 0停止所有
        /// 1为effect
        /// 2为music
        /// </summary>
        /// <param name="_type"></param>
        public void StopAudioByType(int _type = 0)
        {
            foreach (SoundUnitItem item in AudioQueue)
            {
                AudioSource music = item.mAudioSource;
                if (music.enabled && music.clip != null)
                {
                    //要停止的类型跟自身的类型不一致的时候才停止
                    bool isDiffType = (_type == 1 && !item.IsEffect) || (_type == 2 && item.IsEffect);
                    if(!isDiffType)
                    {
                        music.DOKill();
                        music.Stop();
                        music.loop = false;
                    }
                }
                
                item.IsPausing = false;
            }
        }

        public void StopAndClearAllAudio()
        {
            if (AudioQueue == null)
            {
                return;
            }
            foreach (SoundUnitItem item in AudioQueue)
            {
                AudioSource music = item.mAudioSource;
                if (music == null)
                {
                    continue;
                }
                if (music.enabled && music.clip != null)
                {
                    music.DOKill();
                    music.Stop();
                    music.clip = null;
                }
                
                item.IsPausing = false;
            }
        }
        #endregion
        
        

        #region pause unpause

        public void PauseAllSound()
        {
            //stop all Effect,不需要再播放effect音效
            
            foreach (SoundUnitItem item in AudioQueue)
            {
                if (item == null) continue;
                AudioSource music = item.mAudioSource;
                if (music == null) continue;

                if (music.clip != null && music.isPlaying)
                {
                    music.DOKill();
                    music.Pause();
                    item.IsPausing = true;
                }
            }
        }

        public void ResumeAllSound()
        {
            foreach (SoundUnitItem item in AudioQueue)
            {
                if (item == null) continue;
                AudioSource music = item.mAudioSource;
                if (music == null) continue;

                if (music.enabled &&  music.clip != null && !music.isPlaying)
                {
                    music.UnPause();
                    item.IsPausing = false;
                }
            }
        }

        #endregion

    }

//    public class SoundMusic : SharePlaySound
//    {
//        public override void Play(string name, bool loop = true, float value = 1f, float time = 0)
//        {
//        }
//
//        public void Pause()
//        {
//        }
//
//        public void Stop()
//        {
//        }
//    }
}