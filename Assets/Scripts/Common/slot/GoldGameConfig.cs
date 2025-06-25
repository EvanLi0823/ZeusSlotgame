using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
[Serializable]
public class GoldGameConfig : MonoBehaviour {
	[Header("类似于埃及艳后，所有的paylines播放完，播一遍单条线payline。然后循环播放所有paylines等等")]
	public bool IsAllPaylinesRound = false;

	[Header("类似与3tigers、弗兰肯斯坦等，所有的中奖symbol都播动画,然后用粒子来循环播放粒子做的payline")]
	public bool IsParticlePayline = false;

	[Header("是否关卡自定义适配，用于classic移植过来的关卡")]
	public bool IsSelfAdapter = false;

	[Header("IphoneX适配比例（0-1）用于需要自定义IphoneX适配的关卡")]
	public float iphoneXScaleFactor = -1;

	[Header("Ipad适配比例（0-1）用于需要自定义Ipad适配的关卡")]
	public float ipadScaleFactor = -1;
    [Header("Iphone适配比例（0-1）用于需要自定义IPhone适配的关卡")]
    public float iphoneScaleFactor = -1;
    [Header("AutoSpin和FreeSpin时，Spin结束触发BigWin,MegaWin,EpicWin时，下次Spin等待时间")]
	public float isBigWinWaitTime = 1;

	[Header("AutoSpin和FreeSpin时，Spin结束有中奖时，下次Spin等待时间")]
	public float isAwardedWaitTime = 1;

	[Header("是否重置boardConfig，一般用在freespin时需要切换界面")]
	public bool NeedResetBoard;
//	public GoldReelConfig[] GoldReelConfigs;


}
