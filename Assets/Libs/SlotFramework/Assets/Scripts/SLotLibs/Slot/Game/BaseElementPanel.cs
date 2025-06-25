using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

namespace Classic
{
	public class BaseElementPanel : SymbolRender
	{
        public enum AnimationID
        {
            AnimationID_None                    = 0,
            AnimationID_NormalAward             = 1,	//low
            AnimationID_BackFromNormalAward     = 2, 	//high 
            AnimationID_BonusTriggered          = 3,	//bonus
            AnimationID_NoSelectionAwardLine    = 4,	//other award line 
            AnimationID_SmartSymbolReminder     = 5,	//smart sound
            AnimationID_ToggleToWildSymbol      = 6,	//变成wild
			AnimationID_SmartSymbolLoopReminder = 7,	//轮子停止前一直做动画的symbol
        }
		// 兼容sprite，image，两种方式都可实现
//		public SpriteRenderer m_SymbolSprite;
//		public Sprite m_SymbolSprite;

		public Image bgImage {get;set;}
		public Transform animationParent {get;set;}


		public virtual void InitElement (BaseReel ReelContronller, int SymbolIndex)
		{
			this.ReelContronller = ReelContronller;
			IndexOfList = ReelContronller.Elements.IndexOf (this);
//			_positionId = IndexOfList;
			this.SymbolIndex = SymbolIndex;	
			AnimationGO = null;
		}

        protected Vector3 ScaleFacter = new Vector3(1f,1f,1f);

		public virtual void SetStaticSprite (Sprite sprite)
		{
			if (sprite == null) {
				IsBlankSymbol = true;
				if (m_SymbolImage != null) {
					if (m_SymbolImage.sprite == null) {
						m_SymbolImage.sprite = this.GetGameConfig().GetBySymbolIndex (0);
					}
					m_SymbolImage.color = new Color (1, 1, 1, 0);
				}
				if(m_SymbolSprite !=null){
					m_SymbolSprite.sprite = null;//this.GetGameConfig().GetBySymbolIndex (0);
					m_SymbolSprite.color = new Color (1, 1, 1, 0);
				}
            } else {
				IsBlankSymbol = false;
				IsBlankSymbol = false;
				ScaleFacter = new Vector3 (sprite.rect.width / GetGameConfig().ReelPanelWidth, sprite.rect.height / GetGameConfig().reelConfigs [ReelIndex].elementHeight, 1);
				if (m_SymbolImage != null) {
					m_SymbolImage.sprite = sprite;
					m_SymbolImage.transform.localScale = ScaleFacter;
					if (m_SymbolImage.color.a == 0) {
						m_SymbolImage.color = new Color (1, 1, 1, 1);
					}
				}
				if (m_SymbolSprite != null) {
					m_SymbolSprite.sprite = sprite;
					//显示的权重
					int scoreLv = this.ReelContronller.reelManager.symbolMap.getSymbolInfo(this.SymbolIndex).ScoreLevel;
					this.m_SymbolSprite.sortingOrder = 1 + scoreLv;
					m_SymbolSprite.color = Color.white;
				}
			}
		}
		//从第二个往后的参数只适用于webm的动画 ,RepeatPlayStartTime特指播放第一遍动画之后再次播放的起始时间，可参照埃及艳后的wild变身动画
		//isUseCache：是否使用cache缓存的videoplayer，如true则用棋盘上存在的videoPlayer，否则用自己的
		/// <summary>
		/// Plaies the animation.
		/// </summary>
		/// <param name="animationId">Animation identifier.</param>
		/// <param name="isLoop">If set to <c>true</c> is loop.</param>
		/// <param name="VideoInitHandler">Video init handler.</param>
		/// <param name="VideoCompleteHandler">Video complete handler.</param>
		/// <param name="RepeatPlayStartTime"特指播放第一遍动画之后再次播放的起始时间，可参照埃及艳后的wild变身动画</param>
		/// <param name="isUseCache">是否使用cache缓存的videoplayer，如true则用棋盘上存在的videoPlayer，否则用自己的 <c>true</c> is use cache.</param>
		public virtual void PlayAnimation (int animationId,bool isLoop = true, System.Action VideoInitHandler=null, System.Action VideoCompleteHandler=null,float RepeatPlayStartTime =0f,bool isUseCache = true)
		{
			
		}

		public virtual void StopAnimation (bool showAnimationFrame = true)
		{
		}

		public virtual void PauseAnimation (bool notChange = false)
		{
		}

        public virtual void PlayTipAnimation(int animationId){
            
        }

        public virtual void PauseTipAnimation(){

        }

        public virtual void StopTipAnimation(){

        }

        public virtual void PlayNonSelectionAnimation(int animationId, bool showAnimationFrame = true){
            
        }
		public virtual void LoadAnimation (Transform parentTransform)
		{
			GameObject go = this.GetGameConfig().GetAnimation (symbolIndex);

			if (go != null) {
				DestroyAnimation ();
				AnimationGO = Instantiate (go) as GameObject;
				AnimationGO.transform.SetParent (parentTransform, false);
//				AnimationGO.transform.localScale = this.ScaleFacter;
				AnimationGO.transform.SetSiblingIndex(1);
                ChangeAnimationGOPropertyData ();


				//unity5.5.2 的bugfix
				Canvas[] canvasChilds = AnimationGO.GetComponentsInChildren<Canvas>();
				if (canvasChilds.Length > 0) {
					for (int i = 0; i < canvasChilds.Length; i++) {
						Canvas c = canvasChilds[i];
						if (c != null) {
							(c.gameObject.transform as RectTransform).anchorMin = new Vector2 (0, 0);
							(c.gameObject.transform as RectTransform).anchorMax = new Vector2 (1, 1);
							(c.gameObject.transform as RectTransform).offsetMin = new Vector2 (0, 0);
							(c.gameObject.transform as RectTransform).offsetMax = new Vector2 (0, 0);
						
						}
					}
				}

				List<Canvas> canvas = Util.FindChildrenIn<Canvas> (AnimationGO);
				for (int j = 0; j < canvas.Count; j++) {
					canvas[j].sortingOrder += reelAdapter.GetReelShowNumber() -PositionId;
				}

			} else {
				DestroyAnimation ();
			}
		}

        protected virtual void ChangeAnimationGOPropertyData(){

        }

		public void DestroyAnimation ()
		{
			if (AnimationGO != null) {
                Destroy(AnimationGO);
                AnimationGO = null;
			}
		}

		public virtual void ChangeColor (float r=1f, float g = 1f, float b = 1f, float a = 1f)
		{
			if (IsBlankSymbol) {
				if (m_SymbolImage != null) {
					m_SymbolImage.color = new Color (r, g, b, 0);
				} 
				if(m_SymbolSprite != null){
					m_SymbolSprite.color = new Color (r, g, b, 0);
				}
			} else {
				if (m_SymbolImage != null) {
					m_SymbolImage.color = new Color (r, g, b, a);
				}

				if (m_SymbolSprite != null) {
					m_SymbolSprite.color = new Color (r, g, b, a);
				}
			}
		}

		public virtual void EnableSymbolImage(bool enable){
			if (m_SymbolImage != null) {
				m_SymbolImage.gameObject.SetActive (enable);
			}
			if (m_SymbolSprite != null) {
				m_SymbolSprite.gameObject.SetActive (enable);
			}
		}

		public virtual void SetBackGround (Sprite sprite)
		{
			if (bgImage != null) {
				if (sprite == null) {
					bgImage.sprite = sprite;
					bgImage.gameObject.SetActive (false);
				} else {
					bgImage.gameObject.SetActive (true);
					bgImage.sprite = sprite;
				}
			}
		}
		[Obsolete]
		public virtual void ChangeBackGroundInfo (RectTransform trans)
		{
			if (bgImage != null) {
				bgImage.rectTransform.anchorMax = trans.anchorMax;
				bgImage.rectTransform.anchorMin = trans.anchorMin;
			}

			if (animationParent != null) {
				RectTransform r = animationParent as RectTransform;
				r.anchorMax = trans.anchorMax;
				r.anchorMin = trans.anchorMin;
			}
		}

		public virtual void ChangeGrey(bool symbolGray = true){
			if (symbolGray) ChangeColor (0.588F, 0.588F, 0.588F);
		}



		public int SymbolIndex {
			get {
				return symbolIndex;
			}
			set{
				symbolIndex = value;

				Debug.LogError ("通过设置SymbolIndex来改变symbol的方式已经改变，用ChangeSymbol方法代替");
				ChangeElementPropertyDataWhenSymbolChange ();
			}
		}
		public  void ChangeElementPropertyDataWhenSymbolChange(){
			if (this.ReelContronller.State == ReelState.SpeedUpBeforeRunning|| this.ReelContronller.State == ReelState.RUNNING || this.ReelContronller.State == ReelState.FIXRUNNING ||this.ReelContronller.State == ReelState.DelayBeforeStop
                ||this.ReelContronller.State == ReelState.SLOWDOWNFASTRUN||this.ReelContronller.State == ReelState.SLOWDOWN) {
				SetStaticSprite(this.GetGameConfig().GetBySymbolIndex(symbolIndex));
				SetBackGround(this.GetGameConfig().GetBackground(symbolIndex));
            } else {
				SetStaticSprite(this.GetGameConfig().GetBySymbolIndex(symbolIndex));
				SetBackGround(this.GetGameConfig().GetBackground(symbolIndex));
            }
        }

//		public void OnlySetIndexOfDisplay(int index)
//		{
//			this._positionId = index;
//		}

		public bool IsBlankSymbol {
			set;
			get;
		}
		[Obsolete]
		public int IndexOfList {
			get;
			set;
		} 
		//之后 会舍弃此属性
		[Obsolete]
		public BaseReel ReelContronller {
			get;
			set;
		}

		public GameObject AnimationGO {
			get;
			set;
		}

		
		public long AwardNumber {
			get;
			set;
		}

		public int CircleNumIndex {
			get;
			set;
		}

		public float CircleAwardNum {
			get;
			set;
		}

        #region Element Tracking

        public static bool EnableElementTracking {
            get;
            set;
        }

        private bool isTracking;
        public bool isElementTracking {
            set {
                isTracking = value;
            }

            get {
                return EnableElementTracking && isTracking;
            }
        }

        public int ElementTrakcingID {
            get;
            set;
        }

        #endregion

		public float CenterAnchorY
		{
			get{
				RectTransform localTransform = transform as RectTransform;

				float v = (localTransform.anchorMax.y + localTransform.anchorMin.y) / 2;
				return v;
			}
		}

		#region new speed version

		public override void SetStaticPosition(int index)
		{
			this.PositionId = index;
		}

		public override void SetAnimationParent(Transform o)
		{
			animationParent = o;
		}
		#endregion
    }
}
