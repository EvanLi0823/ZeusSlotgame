using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Core;
namespace Classic
{
    public class BaseReel : MonoBehaviour
    {
        protected float elementRelativelyHeight;
        protected float addWhenChange;
		
		
		protected virtual void Update() {
			BaseUpdate ();
		}

		protected virtual void BaseUpdate(){
		}

        public virtual bool IsReelRunningStateTerminated { get; protected set; }

        public virtual void TerminateReelRunningState() {
            this.IsReelRunningStateTerminated = true;
        }

        public virtual void TerminateReelRunningStateIfNeed(){
            if (ShouldStopAtOnce()) {
                TerminateReelRunningState ();
            }
        }
		protected float timeStep
		{
            get { return BaseSlotMachineController.Instance.averageFrame.averageTime; }
//			get { return Time.deltaTime; }
        }

        protected int PostionOfReelData = 0;

        public GameObject AnimationReel {
            get;
            set;
        }

        public virtual void InitElements (ReelManager GameContronller, GameConfigs elementConfigs, bool IsEndReel)
        {
            InitData (GameContronller,elementConfigs,IsEndReel);
            Layout (ElementNumber + 1);
        }

        protected virtual void InitData(ReelManager GameContronller, GameConfigs elementConfigs, bool IsEndReel){
            PostionOfReelData = 0;
            this.reelManager = GameContronller;
            Index = this.reelManager.Reels.IndexOf (this);

            positionMap = new Dictionary<int, BaseElementPanel> ();
            this.IsEndReel = IsEndReel;
            Elements = new List<BaseElementPanel> ();
            this.elementConfigs = elementConfigs;
            this.Result = new List<int> ();
            GameConfigs.ReelConfigs reelConfigs = this.elementConfigs.GetReelConfigs (this.Index);
            RunWithReelData = reelConfigs.RunWithReelData;
            if (this.reelManager.gameConfigs.createResultStrategy == ResultContent.CreateResultStrategy.CreateResultByweight) {
                PostionOfReelData = reelConfigs.StartInData;
            } else {
                RunWithReelData = false;
            }
            ElementNumber = reelConfigs.ElementNumbers;
            elementRelativelyHeight = reelConfigs.elementHeight / reelConfigs.PanelHeight;
                       
        }

        protected void Layout(int layoutElementNumber){
            if (elementConfigs.hasBlank) {
                addWhenChange = elementRelativelyHeight * (layoutElementNumber / 2.0f);
            } else {
                addWhenChange = elementRelativelyHeight * (layoutElementNumber);
            }

            List<BaseElementPanel> elements = new List<BaseElementPanel>();
            List<GameObject> animationElements = new List<GameObject>();

            for (int i = 0; i < layoutElementNumber; i++) {
                BaseElementPanel element = Instantiate(this.elementConfigs.GetReelConfigs(this.Index).elementPanel) as BaseElementPanel;
                element.gameObject.transform.SetParent(transform, false);
                elements.Add(element);

                GameObject animationElement = Instantiate(((ReelManager)reelManager).animationCanvas.animationElement) as GameObject;
                animationElement.transform.SetParent(this.AnimationReel.transform, false);
                animationElements.Add(animationElement);

                element.animationParent = animationElement.transform;
                Elements.Add(element);
            }

            float startY = 0;

			float endY = elementRelativelyHeight;


			if (elementConfigs.hasBlank) {
				startY = -0.5f*elementRelativelyHeight;
				endY = 0.5f*elementRelativelyHeight;
			}

			for (int j = 0; j < layoutElementNumber; j++) {
                BaseElementPanel element = elements[j];
                RectTransform r = element.gameObject.transform as RectTransform;
                r.anchorMax = new Vector2 (1, startY + elementRelativelyHeight);
                r.anchorMin = new Vector2 (0, startY);
				CreateXOffsetByY (r);

                GameObject animationElement = animationElements[j];
                RectTransform r2 = animationElement.transform as RectTransform;
                r2.anchorMax = new Vector2 (1, startY + elementRelativelyHeight);
                r2.anchorMin = new Vector2 (0, startY);
				CreateXOffsetByY (r2);

                element.InitElement (this, GenSymbol ());
                element.PositionId = j;

                if (RunWithReelData) {
					PostionOfReelData = (PostionOfReelData + 1) % ReelDataLenth();
                }
                if (elementConfigs.hasBlank) {
                    startY += elementRelativelyHeight / 2f;
                } else {
                    startY += elementRelativelyHeight;
                }

            }
        }

		protected void CreateXOffsetByY(RectTransform r){
			float offsetX = Mathf.Abs (((r.anchorMax.y + r.anchorMin.y) / 2f) - 0.5f)*elementConfigs.reelConfigs[Index].offsetX;
			r.anchorMax = new Vector2 (1+offsetX, r.anchorMax.y);
			r.anchorMin = new Vector2 (offsetX, r.anchorMin.y);
		}


		public virtual void StartRunning(){
			StartCoroutine (ReelRunning ());
		}

		protected virtual IEnumerator ReelRunning ()
        {
            this.IsReelRunningStateTerminated = false;
            yield return new WaitForSeconds (elementConfigs.GetReelConfigs (Index).StartDelayTime);
            if (this.IsEndReel) {
                reelManager.State = GameState.RUNNING;
                reelManager.StopRun ();
            }
        }

        public virtual bool ShouldStoppedByPreReel(){//LQ ReelSlowDown judge the strip stop not at once
            return !elementConfigs.StopAtOnce;
        }

        public virtual bool ShouldStopAtOnce(){//LQ ReelRunning Judgement at once
                                               //20190416
            return false; //this.reelManager.gameConfigs.StopAtOnce == true||this.reelManager.ShouldStopAtOnceIgnoreAnticipation() || Index == reelManager.GetCurrentRunningFirstReelIndex();
        }

        public virtual IEnumerator ReelSlowDown ()
        {
            if (!reelManager.fastStop) {
                yield return new WaitForSeconds (elementConfigs.GetReelConfigs (Index).StopDelay);
            }
            if (this.IsEndReel) {
                if (this.reelManager.OnStop != null) {
                    this.reelManager.OnStop ();
                }
            }
        }
            
        public virtual void ResultConvert (List<int> reelResult, int ResultStartIndex = 0)
        {
            Result.Clear ();
            Result.AddRange (reelResult);
        }

		protected virtual int GenSymbol ()
        {
            int symbolIndex = 0;
            List<ResultContent.WeightData> reelData=null;
            try
            {
                if (RunWithReelData)
                {
                    //改变带子可能导致带子长度变短，对其进行取模运算，此方法只是UI使用不会影响数据层。
                    reelData = this.reelManager.resultContent.ReelResults[Index].reelData;
                    symbolIndex = reelData[PostionOfReelData % reelData.Count].value;
                }
                else
                {
                    symbolIndex = this.elementConfigs.RandGenSymbol();
                }
            }
            catch (System.Exception ex)
            {
                string errorMsg = ("GenSymbol null check: reelManager:" + (this.reelManager == null) +
                                      " resultContent:" + (this.reelManager != null ? this.reelManager.resultContent == null : true) +
                                      " reelResults:" + (this.reelManager != null && this.reelManager.resultContent != null ? this.reelManager.resultContent.ReelResults == null : true) +
                                      " reelResults[Index]:" + (this.reelManager != null && this.reelManager.resultContent != null && this.reelManager.resultContent.ReelResults != null ? this.reelManager.resultContent.ReelResults[Index] == null : true) +
                                      " reelData:" + (this.reelManager != null && this.reelManager.resultContent != null && this.reelManager.resultContent.ReelResults != null && this.reelManager.resultContent.ReelResults[Index] != null ? this.reelManager.resultContent.ReelResults[Index].reelData == null : true) +
                                      " elementConfigs:" + (this.elementConfigs == null ? true : false) +
                                      " PostionOfReelData:" + PostionOfReelData +
                                      " reelData.Count:" + (reelData != null ? reelData.Count : 0));
                throw new System.Exception(ex.Message+" StackTrace:"+ex.StackTrace+" data:"+errorMsg);
            }

            return symbolIndex;
          
           
        }

		public virtual void PlaySlowAnimation ()
        {
            if (needToPlayAnticipationAnimation) {
				if (RenderLevelSwitchMgr.Instance.CheckRenderLevelIsOK(GameConstants.AnticipationAnimationLevel_Key)){
                elementConfigs.GetReelConfigs (Index).ShowAnimationGO.SetActive (true);
				if (elementConfigs.GetReelConfigs (Index).ShowAnimationGoMask!=null) {
					elementConfigs.GetReelConfigs (Index).ShowAnimationGoMask.SetActive (true);
				} }
                Libs.AudioEntity.Instance.PlayAnticipationSoundEffect ();
            }
        }

		public virtual void StopSlowAnimation ()
        {
            if (needToPlayAnticipationAnimation) {
                elementConfigs.GetReelConfigs (Index).ShowAnimationGO.SetActive (false);
				if (elementConfigs.GetReelConfigs (Index).ShowAnimationGoMask!=null) {
					elementConfigs.GetReelConfigs (Index).ShowAnimationGoMask.SetActive (false);
				}
                Libs.AudioEntity.Instance.StopAnticipationSoundEffect ();
            }
        }


        protected virtual void PlaySmartSound ()
        {
			bool hasPlayedSmartSound = false;

			int beginIndex = 0;
			int endIndex = ElementNumber;
			if (reelManager.gameConfigs.hasBlank) {
				beginIndex = 1;
				endIndex = ElementNumber - 1;
			}

			for (int i = beginIndex; i < endIndex; i++) {
				if (i<Result.Count) {
					SymbolMap.SymbolElementInfo info = this.reelManager.symbolMap.getSymbolInfo (Result[i]);
//					if (info !=null && info.isSmartSound ) {
                        //20190416
						//int playIndex = reelManager.smartSoundReelStripsController.CheckPlaySmartSoundCondition (reelManager, info, this.Index, i);
						//if (playIndex >= 0) {
						//	//Debug.Log ("PlaySmartSound MultiLine Result[i]:"+Result[i].ToString());
						//	if (!hasPlayedSmartSound) {
						//		hasPlayedSmartSound = true;
						//		PlaySmartSymbolSoundEffect (playIndex);
						//	}
						//	//Debug.Log ("Results.Count:  "+Result.Count+"   positionMap.Count:  "+positionMap.Count);
						//	PlaySmartSymbolAnimationEffect (playIndex,i);

						//}
//					}
				}
			}

		}
				

        public void PlaySmartSymbolSoundEffect(int playIndex){
            if (playIndex>=0) {
				
				Libs.AudioEntity.Instance.PlaySmartSoundEffect(playIndex);
            }
           
        }
		public const string TRIGGER_SMART_SOUND_SYMBOL_POSITION = "TriggerSmartSoundSymbolPosition";
		public void PlaySmartSymbolAnimationEffect(int playIndex,int elementPos){
			if (playIndex >= 0&& (!this.IsEndReel || reelManager.gameConfigs.EnableLastReelSmartSound) &&!this.reelManager.ShouldStopAtOnceIgnoreAnticipation()) {
                //20190416
    //            if (BaseSlotMachineController.Instance.reelManager.enableSmartSymbolAnimation) {
				//	if (elementConfigs.GetReelConfigs (Index).HasAnticipationSymbolAnimation) {
				//		if (RenderLevelSwitchMgr.Instance.CheckRenderLevelIsOK(GameConstants.SymbolBlinkAnimationLevel_Key))
				//			PlaySmartSymbolAnimation(elementPos);
				//		//Symbol Blink  hyccong
				//	}
				//	if (reelManager.gameConfigs.hasSmartSoundSymbolLoopAnimation) {
				//		if (reelManager.gameConfigs.bonusAnimationAfterBlink) {
				//			if (RenderLevelSwitchMgr.Instance.CheckRenderLevelIsOK(GameConstants.SymbolBonusAnimationLevel_Key))
				//				PlaySmartSymbolLoopAnimation(elementPos);
				//			//BonusAnimation  hyccong
				//		} 
				//		else {
				//			Messenger.Broadcast<int,int> (TRIGGER_SMART_SOUND_SYMBOL_POSITION,this.Index,elementPos);
				//		}
				//	}
				//	Messenger.Broadcast<int> (PLAY_ANTICIPATION_EFFECT,Index);
				//}
			}
		}
		public void PlaySmartSymbolAnimation(int elementPos){
			if (positionMap.Count>0) {
				positionMap[elementPos].PauseAnimation ();
				positionMap[elementPos].PlayAnimation ((int)BaseElementPanel.AnimationID.AnimationID_SmartSymbolReminder,false);
				float animationDuration = reelManager.gameConfigs.smartSoundSymbolAnimationDuration;
				Libs.DelayAction da = new Libs.DelayAction (animationDuration,null,()=>{
					if (positionMap.Count>0) {
						positionMap[elementPos].StopAnimation();
					}
				});
				da.Play ();
			}
           
        }
		private List<Libs.DelayAction> smartSymbolLoopAnimationEventList= new List<Libs.DelayAction>();
		private List<BaseElementPanel> elements = new List<BaseElementPanel>();
		public void StopSmartSymbolLoopAnimationEvent(){
			if (smartSymbolLoopAnimationEventList!=null&&smartSymbolLoopAnimationEventList.Count>0) {
				for (int i = 0; i < smartSymbolLoopAnimationEventList.Count; i++) {
					if (smartSymbolLoopAnimationEventList[i]!=null) {
						smartSymbolLoopAnimationEventList[i].Stop ();
					}
				}
				smartSymbolLoopAnimationEventList.Clear ();

			}
			if (elements!=null&&elements.Count>0) {
				foreach (BaseElementPanel item in elements) {
					item.StopAnimation ();
				}
				elements.Clear ();
			}
		}
		public void PlaySmartSymbolLoopAnimation(int elementPos){
			
			float animationDuration = reelManager.gameConfigs.smartSoundSymbolAnimationDuration;
			Libs.DelayAction smartSymbolLoopAnimationEvent = new Libs.DelayAction (animationDuration+0.1f,null,()=>{
				if (positionMap.Count>0) {
					positionMap[elementPos].PlayAnimation((int)BaseElementPanel.AnimationID.AnimationID_SmartSymbolLoopReminder,false);
					elements.Add (positionMap[elementPos]);
				}
				//Debug.Log("PlaySmartSymbolLoopAnimation:  "+positionMap[elementPos].SymbolIndex.ToString());
			});
			smartSymbolLoopAnimationEvent.Play ();
			smartSymbolLoopAnimationEventList.Add (smartSymbolLoopAnimationEvent);

		}

        private bool _needToPlayAnticipationAnimation;
        public bool needToPlayAnticipationAnimation {
            get {
				return _needToPlayAnticipationAnimation &&(elementConfigs.GetReelConfigs (Index).ShowAnimationGO != null);
            }

//            set{
//                _hasAnticipationAnimation = value && (!BaseSlotMachineController.Instance.isInFreespinGame) && (elementConfigs.GetReelConfigs (Index).ShowAnimationGO != null);
//            }
            set{
                _needToPlayAnticipationAnimation = value ;
            }
        }

		public int ReelDataLenth(){
			return this.reelManager.resultContent.ReelResults [Index].reelData.Count;
		}

		public void ChangeSymbolData(int startIndex = 1)
		{
			PostionOfReelData = startIndex;
			for (int i = 0; i < Elements.Count; i++) {
				positionMap [i].SymbolIndex = GenSymbol ();
				PostionOfReelData = (PostionOfReelData + 1) % ReelDataLenth ();
			}
		}
            
        public void DestroyReel ()
        {
            Destroy (AnimationReel);
            Destroy (gameObject);
        }

        public void HideAllElements() {
            for (int i = 0; i < Elements.Count; i++) {
                BaseElementPanel element = Elements[i];
                element.gameObject.SetActive(false);
            }
        }

        public void ShowAllElements() {
            for (int i = 0; i < Elements.Count; i++) {
                BaseElementPanel element = Elements[i];
                element.gameObject.SetActive(true);
            }
        }

		public T AddWholeAnimation<T>(T _wholeAnimation)where T:ReelWholeAnimation
		{
			T element = Instantiate(_wholeAnimation) as T;
			element.transform.SetParent(AnimationReel.transform, false);
			(element.transform as RectTransform).anchorMin = Vector2.zero;
			(element.transform as RectTransform).anchorMax = Vector2.one;

			return element;
		}

		public BaseElementPanel GetMiddleElementRender()
		{
			if (Elements.Count > 0) {
				return Elements[(Elements.Count - 1)/2];
			}
			return null;
		}

        public ReelState State {
            get;
            set;
        }

        public bool IsReelStateRunning() {
            return this.State == ReelState.RUNNING || this.State == ReelState.FIXRUNNING;
        }

        public bool IsEndReel {
            get;
            set;
        }

        public int Index {
            get;
            set;
        }

        public List<int> Result {
            get;
            set;
        }

        public int ResultStartIndex {
            get;
            set;
        }

        public int ElementNumber {
            get;
            set;
        }

        public GameConfigs elementConfigs {
            get;
            set;
        }

        public List<BaseElementPanel> Elements {
            get;
            set;
        }

        public void SortElements() {
            this.Elements.Sort(
                (x, y) => {
                    RectTransform xRectTransform = x.transform as RectTransform;
                    RectTransform yRectTransfrom = y.transform as RectTransform;
                    if (!elementConfigs.GetReelConfigs(Index).isReverse) {
                        return xRectTransform.anchorMin.y.CompareTo(yRectTransfrom.anchorMin.y);
                    } else {
                        return yRectTransfrom.anchorMin.y.CompareTo(xRectTransform.anchorMin.y);
                    }
                }
                );
        }

        public ReelManager reelManager {
            get;
            set;
        }

        public bool RunWithReelData {
            get;
            set;
        }
		[HideInInspector]
		public bool preReelHasStoped = false;
        public Dictionary<int,BaseElementPanel> positionMap {
            get;
            set;
        }


		[HideInInspector]
		public float CenterAnchorX;  //锚点中心x值
    }
}
 