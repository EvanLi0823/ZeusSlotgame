using System;
using Spine.Unity;
using UnityEngine;
using UnityEngine.Video;
using System.Collections;
using System.Collections.Generic;

namespace Classic
{
	public class GameConfigs : MonoBehaviour
	{
		public bool hasBlank = false;
        public float PanelWidth = 1f;
		public float ReelPanelWidth = 1f;
		public float ReelsSpace = 0f;
        public float TileAnimationDuration = 1.4f;
        public float MulTileDurationForAutoRunDelay = 4.0f;
		public bool ShowAllLinesAnimation = false;
		public float smartSoundSymbolAnimationDuration = 0.333f;
		public bool hasSmartSoundSymbolLoopAnimation = false;
		public bool bonusAnimationAfterBlink = true;
		public bool StopAtOnce = false;
		public bool StopAtOnceIgnoreAnticipation = false;
		public bool readDataFromPlist = true;
		public bool EnableSpinBtnInFreeSpin = false;
        public ResultContent.CreateResultStrategy  createResultStrategy = ResultContent.CreateResultStrategy.CreateResultByweight;
		public float ScalFactor = 1f;
		public int AnimationTypeTag = 20;
//		public bool HasBigAnimation = false;
		
		// [HideInInspector]
		public int BigWinTag = 5;
		
		// [HideInInspector]
		public int MegaWinTag = 10;
		
		// [HideInInspector]
		public int EpicWinTag = 15;
		public bool ShowAllAnimation = false;
		public float ShowAllAnimationTag = 3f;
		public bool HasFreespinGame=false;
        public int FreeSpinTimes = 7;
		public int FreeSpinNeedNum = 3;

        public bool HasBonusGame = false;
		public BonusGame bonusGame;
		public int BonusGameNeedNum = 3;
        public bool NeedBonusSymbolContinue = false;
        public bool BonusNeedToSatisfyLineTable = false;
		public bool BonusNeedCheckResult = true;

		public bool HasOnceMore = false;
		public float GenRate = 0.3f;

		public bool HasCircleSymbol = false;
		public int WinCirCleNum = 3;
		[Tooltip("启用从右到左旋转带子")]
		public bool isReverseRollReel = false;
		//16:9的时候tour的移动距离
//		public float TournamentMoveX = 122f;

		//1.1.9添加payline
		public bool EnableAnimationPayLine = false;
        //2.4.0
        public PayLineConfig payLineConfig=new PayLineConfig();

        public enum PayLineType{
            None,
            OnlyLine,//仅画线 （非全线）
            LineAndFrame,//线框 （非全线）
			BoxParticle,//粒子框 （非全线或者全线）
			OnlyFrame, //仅画框 （非全线）
	        AllLineFrame,//仅画框 （全线）
        }
		public enum LineType {
            None,
			Fade,//小丑用的这个 为了贴近竞品划线的节奏
		}
        [Serializable]
        public class PayLineConfig{
            public PayLineType payLineType = PayLineType.OnlyLine;
            public PayLineType SpaghettiPayLineType = PayLineType.OnlyLine;
			public LineType lineType = LineType.None;
            public int OnlyLineThickness = 16;
            public int LineAndFrameThickness = 16;
			[Header("用贴图的形式画线，贴图挂在PaylinesAssets下，坐标点从plist读取")]
			public bool IsUseTexture = false;
            
			public GameObject boxParticle;
        }
		//1.2.7
		[Header("是否必须播放payline动画")]
		[Tooltip("必须要先播放所有的payline，快速spin也不能打断")]
		public bool ShowSpaghetti = false;
        [Header("只有一个中奖线时，不进行重绘")]
        public bool SingleLineReBuild = true;
		//1.2.3是否最后一个轮子不播放smartSound

		[Tooltip("是否最后一个轮子播放smartSound,默认不播放")]
		public bool EnableLastReelSmartSound = false;

		//1.2.3
		[Tooltip("最后一个轮子停止后的等待时间")]
		public float ReelEndWaitDuration = 0f;

		[Tooltip("freespin的txt显示方式，0为normal，1为带有倍率，2为util停止")]
		public int FreeSpinType = 0;

		[HideInInspector]
		public Animator ShineAnimator;
		public ShineAnimation shine;

		public Sprite ElementBg1;
		public Sprite ElementBg2;

		public ElementResource[] elementResources;
		public ReelConfigs[] reelConfigs;
        [Header("spinBGM音量(0-1)")]
        public float spin_BGM_volume = 1.0f;
        [Header("中奖音效音量(0-1)")]
        public float win_sound_volume = 1.0f;
		public Sprite RandGenSprite ()
		{
			return elementResources [UnityEngine.Random.Range (0, elementResources.Length)].staticSprite;
		}

		public Sprite GetBySymbolIndex (int symbolIndex)
		{
			if (symbolIndex < elementResources.Length && symbolIndex >-1) {
				return elementResources [symbolIndex].staticSprite;
			}
			return null;
		}

		public Sprite GetFastBySymbolIndex (int symbolIndex)
		{
			if (symbolIndex < elementResources.Length && symbolIndex >-1) {
				return elementResources [symbolIndex].fastStaticSprite;
			}
			return null;
		}

		public GameObject GetAnimation(int symbolIndex){
			if (symbolIndex < elementResources.Length && symbolIndex >-1 ) {
				return  elementResources [symbolIndex].animationGO;
			}
			return null;
		}

		public Sprite GetBackground(int symbolIndex){
			if (symbolIndex < elementResources.Length && symbolIndex >-1) {
				return elementResources [symbolIndex].BackGround;
			}
			return null;
		}

		public int GetReelNumber(){

			return reelConfigs.Length;
		}

		public ReelConfigs GetReelConfigs(int index){
	
			return reelConfigs [index];
		}

		public int RandGenSymbol(){
			return UnityEngine.Random.Range (0, elementResources.Length);
		}

		public void ReplaceReelManagerData(ReelManagerDataConfig reelManagerDataConfig){

			if (readDataFromPlist && reelManagerDataConfig != null) {
				if (reelManagerDataConfig.baseDataConfig != null) {
					ScalFactor = reelManagerDataConfig.baseDataConfig.scalFactor;
					AnimationTypeTag =(int) (reelManagerDataConfig.baseDataConfig.lineAnimationTag*ScalFactor);
//					HasBigAnimation = reelManagerDataConfig.baseDataConfig.hasBigAnimation;
					//BigAnimationTag = reelManagerDataConfig.baseDataConfig.bigAnimationTag;
				}

				if (reelManagerDataConfig.showAllSymbolAnimationConfig != null) {
					ShowAllAnimation = reelManagerDataConfig.showAllSymbolAnimationConfig.showAllAnimation;
					ShowAllAnimationTag = reelManagerDataConfig.showAllSymbolAnimationConfig.showAllAnimationTag;
				}

				if (reelManagerDataConfig.freeSpinAwardConfig != null) {
					HasFreespinGame=reelManagerDataConfig.freeSpinAwardConfig.hasFreespinGame;
					FreeSpinTimes = reelManagerDataConfig.freeSpinAwardConfig.freeSpinTimes;
					FreeSpinNeedNum = reelManagerDataConfig.freeSpinAwardConfig.awardSymbolCount;
				}

				if (reelManagerDataConfig.bonusGameAwardConfig != null) {
					HasBonusGame = reelManagerDataConfig.bonusGameAwardConfig.hasBonusGame;
					BonusGameNeedNum = reelManagerDataConfig.bonusGameAwardConfig.awardSymbolCount;
					BonusNeedCheckResult = reelManagerDataConfig.bonusGameAwardConfig.bonusNeedCheckResult;
				}

				if (reelManagerDataConfig.onceMoreGameConfig != null) {
					HasOnceMore = reelManagerDataConfig.onceMoreGameConfig.hasOnceMore;
					GenRate = reelManagerDataConfig.onceMoreGameConfig.genRate;
				}
			}
		}

		[Serializable]
		public class ReelConfigs{
			public int StartInData = 0;
			public bool RunWithReelData = false;
            public BaseReel reelPanel;
			public BaseElementPanel elementPanel;
            public GameObject reelStopAnimation;
            public bool isReverse = false;
			public int ElementNumbers= 1;
			public int resultLenth = 5;
			public float PanelHeight = 1f;
			public float elementHeight = 1f;
			public float relativeHeight = 1f;
			public float Runtime = 5f;
			public float StartDelayTime = 1f;
            public float ElementAtomDelayTime = 0.5f; // 元素间相对延迟，实际使用中对某个元素使用时需要累加之前所有元素的DelayTime
			public float StopDelay = 0f;
			public float StartRunA = 0f;
			public float RunV = 3f;
			public float StopV = 2f;
			public float StopA = 15f;
			public float BackDistace = 1f;
			public float RunBackV1 = 4f;
			public float RunBackV2 = 2f;
            public float backA1 = 1000f;
            public float backA2 = 1000f;
			public float SlowV = 2f;
			public float SlowA = 20f;
            public float SlowFastRunV = 2f;
            public float SlowFastRunTime = 1f;
			public float SlowRunBackV2 = 1f;
			public float GlidingPreWaitTime = 0.5f;
			public float GlidingSpeed = 3f;
			public float GlidingAfterWaitTime = 0.5f;
			public GameObject ShowAnimationGO;
			public GameObject ShowAnimationGoMask;
			public bool IsShowBonusAnticipation = true;  //当前轮子是否需要播放bonus相关的anticipation动画
			public bool HasAnticipationSymbolAnimation = false;
			public float offsetX = 0;
			public float reelOffsetX = 0;
		}

		[Serializable]
		public class ElementResource
		{
			public Sprite staticSprite;
			public Sprite fastStaticSprite;
			public GameObject animationGO;
			public Sprite BackGround;

			[Header("webm视频动画video，老的格式动画请不要设置")]
			public List<VideoData> VideoAnimations= new List<VideoData>();
		}

		[Serializable]
		public class VideoData
		{
			[Header("老的动画id，gold的大小normal动画都为1")]
			public int AnimationId = 1;
			[Header("动画video")]
			public VideoClip m_VideoAnimation;
		}

		void Awake()
		{
			if (shine != null) {
				if (RenderLevelSwitchMgr.Instance.CheckRenderLevelIsOK(GameConstants.ShineAnimationLevel_Key))
					ShineAnimator = shine.GetCurrentUsedShineAnimation ();
				else
					ShineAnimator = null;
            }
            else
            {
                ShineAnimator = null;
            }
		}
	}
}
