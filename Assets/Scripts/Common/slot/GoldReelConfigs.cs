using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class GoldReelConfig
{
    [Header("带子转动的时间")]
    public float RotationTime;

    [Header("带子匀速时的速度")]
    public float ConstantSpeed;

    [Header("带子停止间隔时间")]
    public float StopDelayTime;

    [Header("带子停下时的速度")]
    public float StopSpeed;

    [Header("停止减速频率(越小改变越快)")]
    public float StopFrequency;

    [Header("向下摆动结束时的速度")]
    public float DownSpeed;

    [Header("向下摆动减速频率(越小改变越快)")]
    public float DownFrequency;

    [Header("向上摆动结束时的速度")]
    public float UpSpeed;

    [Header("向上摆动减速频率(越小改变越快)")]
    public float UpFrequency;
}
