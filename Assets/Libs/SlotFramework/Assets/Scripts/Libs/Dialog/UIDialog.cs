using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using Classic;

namespace Libs
{
	public class UIDialog : UIWindow 
	{
		public enum EnumDialogRootPath
		{
			None,
			TopBanner
		}

		/// <summary>
		/// 在竖版机器中，竖版弹窗弹出时，在有横版弹窗存在时，不旋转根节点，不改变相机尺寸，直接弹出
		/// 优点： 防止在横版弹窗弹出时，竖版弹窗弹出直接导致横版页面放大无法操作，只有重启
		/// 缺点：竖版弹窗会横向弹出，而且尺寸会变小
		/// </summary>
		public bool IgnoreRotateWithPortrait = false;
		
		/// <summary>
		/// 在竖版机器中，横版弹窗弹出，不旋转根节点，但是设置CameraOrthographicSize保持和横版一样大小, 参考LevelBoostLoadingDialog,只应用于loading弹窗
		/// </summary>
		public bool IgnoreRotateInPortraitWindow = false;
		
		public const string GAMEOBJECT_UNVISIBLE_LAYER = "UnVisible";
		public const string GAMEOBJECT_DIALOG_LAYER = "Dialog";
		public const string GAMEOBJECT_UI_LAYER = "UI";

		public System.Action OnDialogCloseCallBack;

		public bool bResponseBackButton = true;
 
		public bool isModel = true;
        public bool isPortrait = false;
        //		public bool isQueueDialog = true;  //是否放入队列中，前一个dialog关闭之后才显示这个dialog
        public bool EnableInspectorMaskAlpha = false;
		public float maskAlpha = 0.5f;
		public Button BtnClose = null;
		public bool isNeedMask = false; // 只针对非model，model的都已经带有mask
		protected bool closeMask = true;

        public EnumDialogRootPath DialogRootPath = EnumDialogRootPath.None;

        public bool IsTopMask = true;

		[Header("是否需要暂停背景游戏，默认都需要")]
		public bool IsBackGroundStop = true;//不需要有的包括vile左下角对话框、 LevelUpSmall 、 UnlockOneSlotDialog、betchange

		[Header("是否显示金币浮动框")] 
		public bool IsShowFlyCoinsPanel = false;

		public bool isCloseParticle = false;

		[HideInInspector]
		public int eId;//用于标识对话框的事件位置索引
		[HideInInspector]
		public bool skipCloseAll;
		[HideInInspector]
		public bool forceSkipCloseAll;
		[HideInInspector]
		public Action EndCallback;//禁止任何游戏逻辑使用此变量，此标记只用于关闭回调使用
		[HideInInspector]
		public bool isNew=false; //禁止任何游戏逻辑使用此变量，此标记只用于标示是否为新弹窗机制
		[HideInInspector]
		public string bundleName; //标识当前对话框所使用的bundle，便于后续释放
		public bool enableFadeMask = false;
		public float maskFadeIn = 0.3f;
		public float maskFadeOut = 0.3f;
		public float startAlpha = 0.2f;
		public bool NeedShowMask()
		{
			return isModel || isNeedMask;
		}
		private List<Transform> allTransforms= new List<Transform>();
        protected override void Awake (){

	        if (this is PaytablePanel)
	        {
		        isNeedMask = true;
	        }
	        if (DialogRootPath==EnumDialogRootPath.TopBanner)
	        {
		        DialogRootPath = EnumDialogRootPath.None;
		        PortraitAdapt();
		        isNeedMask = true;
		        IsTopMask=true;
	        }
			base.Awake ();
			if (BtnClose == null) {
				Transform t = transform.Find("BtnClose");
				if (t !=null) BtnClose = t.GetComponent<Button>();
			}

			if(BtnClose!=null) UGUIEventListener.Get(BtnClose.gameObject).onClick = this.BtnCloseClick;

			this.CompleteQuitCallBack = () => {
				try
				{
					if (OnDialogCloseCallBack!=null)
					{
						OnDialogCloseCallBack();
					}
				}
				catch (Exception e)
				{
					BaseGameConsole.ActiveGameConsole().SendCatchExceptionToServer(e);
				}
				UIManager.Instance.Close (this);
			};

            if (!IsBackGroundStop) { 
                ParticleSystem[] ps = FindObjectsOfType<ParticleSystem> ();
    			for (int i = 0; i < ps.Length; i++) {
                    ParticleSystem.MainModule mainModule = ps[i].main;
                    mainModule.useUnscaledTime = true;
                }
            }
           
        }
        
        protected virtual void BtnCloseClick(GameObject closeBtnObject){

			//Libs.SoundEntity.Instance.Click ();
            Libs.AudioEntity.Instance.PlayCloseClickEffect();
			this.ShowOut ();
        }

		public virtual void Close()
		{ 
			if(isCloseParticle)
			{
				ParticleSystem[] ps = FindObjectsOfType<ParticleSystem> ();
				for (int i = 0; i < ps.Length; i++) 
				{
					ps[i].Stop();
					ps[i].Clear();
                }
			}

			this.ShowOut ();
		}

		public void CloseIgnoreState()
		{
			Close();
		}

		/// <summary>
		/// 伴随着飞金币动画完成关闭
		/// </summary>
		/// <param name="buttons">禁用可交互按钮</param>
		/// <param name="starTrans">开始飞金币的位置</param>
		/// <param name="callBack">飞金币结束的回调方法</param>
		/// <param name="isPlayAudio">是否播放音乐</param>
		/// <param name="canCollectContinue">是否可以连续点击收集</param>
		public void CloseWithCoinsFly(List<Button> buttons,Transform starTrans, Action callBack = null, bool isPlayAudio = true)
		{
			
			if (callBack == null) callBack = Close;
			if (!FlyCoinsPanel.Instance.IsOpenCloseWithFly)
			{
				callBack();
				Messenger.Broadcast (SlotControllerConstants.OnBlanceChangeForDisPlay);
				if(isPlayAudio) Libs.AudioEntity.Instance.PlayCoinCollectionEffect();
				return;
			}
			if (buttons != null)
			{
				for (int i = 0; i < buttons.Count; i++)
				{
					//buttons[i].interactable = false;
					buttons[i].enabled = false;
					UGUIEventListener eventListener = buttons[i].GetComponent<UGUIEventListener>();
					if (eventListener != null)
					{
						DestroyImmediate(eventListener);
					}
				}
			}

			try
			{
				Messenger.Broadcast<Transform, Libs.CoinsBezier.BezierType, System.Action>(
					GameConstants.CollectBonusWithType, starTrans, Libs.CoinsBezier.BezierType.DailyBonus, callBack);
			}
			catch (Exception e)
			{
				callBack();
				Log.Error($"CloseWithCoinsFly has error :{e}");
			}
			Messenger.Broadcast (SlotControllerConstants.OnBlanceChangeForDisPlay);
			if(isPlayAudio) Libs.AudioEntity.Instance.PlayCoinCollectionEffect();
		}

		#region 对显示的粒子处理隐藏
		private List<ParticleSystem> m_ShowingParticleSystem= new List<ParticleSystem>();
		[HideInInspector]
		public bool HasHideParticleSystem = false;

		public virtual void HideShowingParticles()
		{
			if (HasHideParticleSystem) {
				return;
			}

			this.m_ShowingParticleSystem = Util.FindChildrenIn<ParticleSystem> (this.gameObject);

			SetParicleLayer (GAMEOBJECT_UNVISIBLE_LAYER);

			HasHideParticleSystem = true;
		}

		public virtual void ShowParticles()
		{
			if (!HasHideParticleSystem) {
				return;
			}
			
			if(DialogRootPath == EnumDialogRootPath.None) SetParicleLayer (GAMEOBJECT_DIALOG_LAYER);
			//if(DialogRootPath == EnumDialogRootPath.TopBanner) SetParicleLayer (GAMEOBJECT_UI_LAYER);

			this.m_ShowingParticleSystem.Clear ();

			HasHideParticleSystem = false;
		}

		public void SetParicleLayer(string layerName)
		{
			if (m_ShowingParticleSystem.Count <= 0) {
				return;
			}

			int layerInt = LayerMask.NameToLayer (layerName);
			for (int i = 0; i < this.m_ShowingParticleSystem.Count; i++) {
                ParticleSystem particleSystem = this.m_ShowingParticleSystem[i];
                if (particleSystem == null) continue;
                this.m_ShowingParticleSystem [i].gameObject.layer = layerInt;
			}
		}

		public void SetAllGameObjectLayer(string layerName)
		{
			
			if (allTransforms.Count <= 0) {
				return;
			}

			int layerInt = LayerMask.NameToLayer (layerName);
			for (int i = 0; i < this.allTransforms.Count; i++) {
				Transform transform = this.allTransforms[i];
				if (transform == null) continue;
				this.allTransforms[i].gameObject.layer = layerInt;
			}
		}

		#endregion
		public void PortraitAdapt()
		{
			#region 竖版机器适配
			//GameObject rootCanvas = GameObject.Find("DialogCamera/DialogCanvas");
			//GameObject topRootCanvas = topRoot.transform.parent.parent.parent.gameObject;
			GameObject topCanvas=GameObject.Find("Main Camera/BannerCanvas");
			if (BaseGameConsole.ActiveGameConsole().IsInSlotMachine() && SkySreenUtils.CurrentOrientation == ScreenOrientation.Portrait )
			{
				if (topCanvas!=null)
				{
					Vector3 defaultScale = this.transform.localScale;
					Vector3 verticalScale = topCanvas.transform.localScale;
					float v = 0.56f;
					Vector3 newScale=new Vector3(defaultScale.x*v ,defaultScale.y*v,defaultScale.z*v);

					if (CommonMobileDeviceAdapter.IsWideScreen())
					{
						float mult = ((float)Screen.width / (float)Screen.height) / (1080f / 1920f);
						this.transform.localScale = newScale * mult;

					}
					else if (SkySreenUtils.GetScreenSizeType() == SkySreenUtils.ScreenSizeType.Size_4_3)
					{
						this.transform.localScale =newScale*1.2f;
					}
					else
					{
						this.transform.localScale =newScale;
					}
				}
			}
			#endregion
		}
	}
}
