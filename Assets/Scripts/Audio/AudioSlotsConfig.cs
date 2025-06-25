using System;
using UnityEngine;
using System.Collections.Generic;


public class AudioSlotsConfig : MonoBehaviour
{
    private const string SPIN = "Spin";
    [Header("BaseGame背景音量")]
    [Range(0, 1)]
    public float base_volume = 1f;
    [Header("BaseGame背景压低音量")]
    [Range(0, 1)]
    public float base_low_volume = 0.3f;

    private const string SPECIAL = "Special";
    [Header("FreeGame背景音量")]
    [Range(0, 1)]
    public float free_volume = 1f;
    [Header("FreeGame背景压低音量")]
    [Range(0, 1)]
    public float free_low_volume = 0.3f;

    private const float low_volume = 0.3f;

    [Header("声音配置")]
    public List<AudioInfo> audioInfo;

    public static AudioSlotsConfig Instance;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        } 
    }

    //播放音量
    public float Volume(string name, float value)
    {
        if(name == SPIN) return base_volume;
        if(name == SPECIAL) return free_volume;

        foreach (var info in audioInfo)
        {
            if(info.name == name) return info.volume;
        }

        return value;
    }

    public float ReduceVolume(string name)
    {
        if(name == SPIN) return base_low_volume;
        if(name == SPECIAL) return free_low_volume;
        return low_volume;
    }

    [Serializable]
    public class AudioInfo
    {
        [Header("名称")]
        public string name = "";
        
        [Header("音量")]
        [Range(0, 1)]
        public float volume = 1f;
    }
}
