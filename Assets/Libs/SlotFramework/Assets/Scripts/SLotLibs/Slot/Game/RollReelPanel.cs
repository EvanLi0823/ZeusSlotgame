using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using DG.Tweening;
using UnityEngine.UI;

namespace Classic
{	
    public class RollReelPanel : BaseReel
    {
		static int m_allRunningReelNum = 0;
        protected BaseElementPanel LocomotiveElement {
            get;
            set;
        }
			
		float stateStartTime;

		float lastTime = 0f;
		protected float timeStepRollReel;
		protected bool NetworkSpinResponse;
		void Awake()
		{
			m_allRunningReelNum = 0;
			lastTime  = Time.realtimeSinceStartup;
		}
		protected override void Update() 
		{
			base.Update();
		}
		protected override void BaseUpdate(){

			timeStepRollReel = (Time.realtimeSinceStartup - lastTime);
			lastTime = Time.realtimeSinceStartup;

			switch (this.State) {
			case ReelState.READY:
				break;
			case ReelState.DelayBeforeRunning:
				DelayBeforeRunning ();
				break;
			case ReelState.SpeedUpBeforeRunning:
				SpeedUpBeforeRunning ();
				break;
			case ReelState.RUNNING:
			case ReelState.FIXRUNNING:
				ReelRunning ();
				break;
			case ReelState.DelayBeforeStop:
				DelayBeforeStop ();
				break;
			case ReelState.SLOWDOWNFASTRUN:
				FastRunBeforeSlowDown ();
				break;
			case ReelState.SLOWDOWN:
				ReelSlowDown ();
				break;
			case ReelState.SLOWDOWNOVER:
				ReelSlowDownover ();
				break;
			case ReelState.STOPPING:
				ReelStopingInUpdate ();
				break;
			case ReelState.RUNBACKDOWN:
				ReelRunBackDown ();
				break;
			case ReelState.RUNBACKUP:
				ReelRunBackUp ();
				break;
			case ReelState.RUNSHAKE:
                ReelRunShake();
				break;
			default:
				break;
			}
		}

		public override void StartRunning(){
			NetworkSpinResponse = !BaseSlotMachineController.Instance.reelManager.SpinUseNetwork;
			m_allRunningReelNum++;
			jumpToStoping = false;
			IsReelRunningStateTerminated = false;

			if (elementConfigs.GetReelConfigs (Index).StartDelayTime > 0) {
				PrepareDelayBeforeRunning ();
			} else {
				//PrepareRunning ();
				PrepareSpeedUpBeforeRunning();
			}
		}

		static Vector3 VEC_UP_OFFSET = new Vector3(0,17,0);
		public override void InitElements (ReelManager GameContronller, GameConfigs elementConfigs, bool IsEndReel)
		{
			InitData (GameContronller,elementConfigs,IsEndReel);
			Layout (ElementNumber + 1);

			normalPos = transform.localPosition;
			upPos = normalPos + VEC_UP_OFFSET;
		}

		void PrepareDelayBeforeRunning(){
			this.State = ReelState.DelayBeforeRunning;
			stateStartTime = Time.realtimeSinceStartup;
		}

		void DelayBeforeRunning(){
			if (Time.realtimeSinceStartup - stateStartTime > elementConfigs.GetReelConfigs (Index).StartDelayTime) {
				//PrepareRunning ();
				PrepareSpeedUpBeforeRunning();
			}
		}
		void PrepareSpeedUpBeforeRunning(){
			if (Mathf.Approximately(0,elementConfigs.GetReelConfigs(Index).StartRunA)) {
				PrepareRunning ();//当启动加速度为0时，按照原始方式执行
			} 
			else {//启动加速度不为0时，启动有加速过程
				InitBeforeStartRunning ();
				stateStartTime = Time.realtimeSinceStartup;
				this.State = ReelState.SpeedUpBeforeRunning;
				if (this.IsEndReel) {
					reelManager.State = GameState.RUNNING;
				}
			}
		}

		void SpeedUpBeforeRunning(){
			float step = timeStepRollReel;
			float StartV = elementConfigs.GetReelConfigs (Index).StartRunA * (Time.realtimeSinceStartup - stateStartTime);
			float offset = step * StartV;
			ChangeStripPropertyDataWhenRunning (offset);
			ChangeElementsPosition (offset);
			bool needJumpToNextState = IsReelRunningStateTerminated;
			if (needJumpToNextState||StartV >= elementConfigs.GetReelConfigs(Index).RunV) {
				PrepareRunningAfterSpeedUp ();
			}
		}

		void InitBeforeStartRunning(){
			for (int j = 0; j < this.Elements.Count; j++) {
				this.Elements [j].ChangeColor ();
				this.Elements [j].StopAnimation ();
			}

			ChangeStripPropertyDataBeforeRunning ();
			positionMap.Clear ();
			LocomotiveElement = null;

		}
		void PrepareRunningAfterSpeedUp(){
			this.State = ReelState.RUNNING;
			stateStartTime = Time.realtimeSinceStartup;
		}
		void PrepareRunning(){
			this.State = ReelState.RUNNING;
			for (int j = 0; j < this.Elements.Count; j++) {
				this.Elements [j].ChangeColor ();
				this.Elements [j].StopAnimation ();
			}
			if (this.IsEndReel) {
				reelManager.State = GameState.RUNNING;
			}
			ChangeStripPropertyDataBeforeRunning ();
			positionMap.Clear ();
			LocomotiveElement = null;
			stateStartTime = Time.realtimeSinceStartup;
			//elementConfigs.GetReelConfigs (Index).isReverse = false;
		}

		protected void ReelRunning(){
		
			if (this.ReelRuningOffset(stateStartTime)) {
				
				this.reelManager.StopRun();
				if (ShouldFastStop ()) {

					PrepareSlowDown ();
				} else {
					PrepareDelayBeforeStop ();
				}
			}
		}

		//todo-xiaogang
		protected virtual bool ReelRuningOffset(float StartTime)
		{
			float step = timeStepRollReel;
			bool needJumpToNextState = IsReelRunningStateTerminated;

			if (Time.realtimeSinceStartup -stateStartTime > elementConfigs.GetReelConfigs (Index).Runtime && ShouldStopAtOnce ()) 
			{
				needJumpToNextState = true;
			}

			float offset = step * elementConfigs.GetReelConfigs (Index).RunV;
			ChangeStripPropertyDataWhenRunning (offset);
			ChangeElementsPosition (offset);

			return needJumpToNextState;
		}


		public override bool ShouldStopAtOnce(){
            //20190416
            return false;// (Index ==this.reelManager.GetCurrentRunningFirstReelIndex())||this.reelManager.ShouldStopAtOnceIgnoreAnticipation() || (this.reelManager.gameConfigs.StopAtOnce&&!IsAfterAncitipationReel ());
            
        }

        protected bool IsAfterAncitipationReel(){
			if (reelManager.gameConfigs.isReverseRollReel) {
				for (int i = reelManager.Reels.Count-1; i >this.Index ; i--) {
					if(reelManager.Reels[i].needToPlayAnticipationAnimation){
						return true;
					}
				}
			} else {
				for (int i = 0; i < this.Index; i++) {
					if(reelManager.Reels[i].needToPlayAnticipationAnimation){
						return true;
					}
				}
			}
           
            return false;
        }

        public virtual bool NeedFastRunBeforeSlowDown() {
            return needToPlayAnticipationAnimation;
        }

		void PrepareDelayBeforeStop(){
			this.State = ReelState.DelayBeforeStop;
			stateStartTime = Time.realtimeSinceStartup;
		}

		void DelayBeforeStop(){
			if (this.IsRunAndSlowDown(preReelHasStoped&&NeedFastRunBeforeSlowDown (), stateStartTime, StopAtOnceWhenTriggerFastStop())) {
				PrepareFastRunAndSlowDown ();
				stateStartTime = Time.realtimeSinceStartup; 
			} else { 
				this.BeforeStopRunOffset();
			}
		}

		//todo-xiaogang
		protected virtual bool IsRunAndSlowDown(bool hasStop, float startTime, bool fastStop)
		{
			return (hasStop || Time.realtimeSinceStartup - startTime > elementConfigs.GetReelConfigs (Index).StopDelay || fastStop);
		}

		//todo-xiaogang
		protected virtual void BeforeStopRunOffset()
		{
			float step = timeStepRollReel;
			float offset = step * elementConfigs.GetReelConfigs(Index).RunV;
			ChangeElementsPosition(offset);
		}

		void PrepareFastRunAndSlowDown(){ 
			if (preReelHasStoped && NeedFastRunBeforeSlowDown ()) {
				PrepareFastRunBeforeSlowDown ();
			} else {
				PrepareSlowDown ();
			}

		}

		protected virtual void PrepareFastRunBeforeSlowDown(){
			this.State = ReelState.SLOWDOWNFASTRUN;
			Messenger.Broadcast<int> (REEL_FAST_RUN_BEFORE_SLOW_DOWN,this.Index);
			stateStartTime = Time.realtimeSinceStartup;
			PlaySlowAnimation ();
		}


		public virtual  void FastRunBeforeSlowDown() {
			GameConfigs.ReelConfigs reelConfigs = this.elementConfigs.GetReelConfigs(this.Index);
			if (Time.realtimeSinceStartup - stateStartTime < reelConfigs.SlowFastRunTime&&!StopAtOnceWhenTriggerFastStop()) {
				float offset = timeStepRollReel * reelConfigs.SlowFastRunV;
				ChangeElementsPosition (offset);
			} else {
				PrepareSlowDown ();
			}
        }
			
		protected bool StopAtOnceWhenTriggerFastStop(){
			return reelManager.ShouldStopAtOnceIgnoreAnticipation ();
		}
		protected virtual bool ShouldFastStop(){
			return (reelManager.fastStop ||  (ShouldStoppedByPreReel () && (!needToPlayAnticipationAnimation)&&Mathf.Abs(elementConfigs.GetReelConfigs (Index).StopDelay)<=0.001f));
        }

	

		float currentV;
		float currentA;
		float stopV;

		void PrepareSlowDown(){
			this.State = ReelState.SLOWDOWN;
			 currentV = elementConfigs.GetReelConfigs (Index).RunV;

			if (needToPlayAnticipationAnimation) {
				currentA = elementConfigs.GetReelConfigs (Index).SlowA;
			} else {
				currentA = elementConfigs.GetReelConfigs (Index).StopA;
			}
			if (needToPlayAnticipationAnimation) {
				stopV = elementConfigs.GetReelConfigs (Index).SlowV;
			} else {
				stopV = elementConfigs.GetReelConfigs (Index).StopV;
			}
			ChangeStripPropertyDataBeforeSlowDown ();
			stateStartTime = Time.realtimeSinceStartup;
		}

		void ReelSlowDown() 
		{
			if (StopAtOnceWhenTriggerFastStop()&&currentA!=elementConfigs.GetReelConfigs (Index).StopA) {
				currentA = elementConfigs.GetReelConfigs (Index).StopA;
				stopV = elementConfigs.GetReelConfigs (Index).StopV;
			}
			if (this.ReelDownOffset())  PrepareSlowDownOver ();

		}

		//todo-xiaogang
		protected virtual bool ReelDownOffset()
		{
			float step = timeStepRollReel;
			float offset = step * currentV;
			ChangeStripPropertyDataWhenSlowDown (offset);
			ChangeElementsPosition(offset);

			currentV -= step * currentA;
			if (currentV < stopV) 
			{
				currentV = stopV;
				return true;
			}
			return false;
		}
			
		bool jumpToStoping = false;

		void PrepareSlowDownOver(){
			this.State = ReelState.SLOWDOWNOVER;
			if (RunWithReelData) {
				if (elementConfigs.hasBlank && (Result [(Result.Count - 1) / 2] != -1)) {
					PostionOfReelData = (this.ResultStartIndex + ReelDataLenth() - BaseSlotMachineController.Instance.ReelMoveBeforeStopCount()) % ReelDataLenth();
				} else {
					PostionOfReelData = (this.ResultStartIndex + ReelDataLenth()) % ReelDataLenth();
				}
			}
			stateStartTime = Time.realtimeSinceStartup;
		}

		void ReelSlowDownover(){
			if (jumpToStoping && LocomotiveElement != null) {
				PrePareStoping ();
			} else {
				this.ReelSlowDownoverOffset();
			}
		}

		//todo-xiaogang
		protected virtual void ReelSlowDownoverOffset()
		{
			ChangeElementsPosition (timeStepRollReel * currentV);
		}


        protected virtual void ChangeStripPropertyDataBeforeRunning(){
            //目前只供wildstrip使用 goldslots16关
        }
        protected virtual void ChangeStripPropertyDataWhenRunning(float offset){
            //目前只供wildstrip使用 goldslots16关
        }
        protected virtual void ChangeStripPropertyDataBeforeSlowDown(){
            //目前只供wildstrip使用 goldslots16关
        }
        protected virtual void ChangeStripPropertyDataWhenSlowDown(float offset){
            //目前只供wildstrip使用 goldslots16关
        }
        protected virtual float LocomotiveElementStartMinY() {
            if (elementConfigs.GetReelConfigs(Index).isReverse) {
                return -elementRelativelyHeight;
            } else {
                return -elementRelativelyHeight + addWhenChange;
            }
        }

        protected virtual float LocomotiveElementFinalMinY() {
            if (elementConfigs.GetReelConfigs(Index).isReverse) {
                if (elementConfigs.hasBlank) {
					return -elementRelativelyHeight * 0.5f;//addWhenChange - elementRelativelyHeight - elementRelativelyHeight * 0.5f;
                } else {
					return 0;//addWhenChange - elementRelativelyHeight - elementRelativelyHeight;
                }
            } else {
                if (elementConfigs.hasBlank) {
                    return -elementRelativelyHeight * 0.5f;
                } else {
                    return 0;
                }
            }
        }

		float totalMove = 0;

		void PrePareStoping(){
			this.State = ReelState.STOPPING;
			if (needToPlayAnticipationAnimation) {
				currentV = elementConfigs.GetReelConfigs (Index).SlowV;
			} else {
				currentV = elementConfigs.GetReelConfigs (Index).StopV;
			}
			if (LocomotiveElement != null) {
				if (elementConfigs.GetReelConfigs(Index).isReverse) {
					totalMove = (LocomotiveElement.transform as RectTransform).anchorMin.y - LocomotiveElementStartMinY();
				} else {
					totalMove = LocomotiveElementStartMinY() - (LocomotiveElement.transform as RectTransform).anchorMin.y;
				}
			}
		}
			
		protected void ReelStopingInUpdate ()
		{
			if (StopAtOnceWhenTriggerFastStop()&&currentV!=elementConfigs.GetReelConfigs (Index).StopV) {
				currentV = elementConfigs.GetReelConfigs (Index).StopV;
			}
			float moveDistance =  StopMoveDistance();
			float offset = this.MoveOffsetStoping();
			if (totalMove + offset <= moveDistance) {
				ChangeElementsPosition (offset);
				totalMove += offset;
			} else {
				ChangeElementsPosition (moveDistance - totalMove);
				for (int i = 0; i < Elements.Count; i++) {
					BaseElementPanel element = Elements[i];
					SetPosition(element);
				}
				if (IsShakeStop)
				{
					PrepareReelRunShake();
				}
				else
				{
					PrepareReelRunBackDown();
				}
			}
		}

		//todo-xiaogang
		protected virtual float MoveOffsetStoping()
		{
			return timeStepRollReel * currentV;
		}

		public bool IsShakeStop{ get; set;}

		protected virtual void BeforeReelRunBack() {
			return;
		}
		protected virtual void AfterReelRunBack() {
			return;
		}

		float backDonwV = 0;
		float backDownA;

		protected virtual void PlayRollEndSound()
		{
			this.reelManager.PlayRollEndSound (this.Index);
//			Libs.AudioEntity.Instance.PlayReelStopEffect(this.Index);
		}

		void PrepareReelRunBackDown(){
			
			BeforeReelRunBack();
			this.State = ReelState.RUNBACKDOWN;
			PlayRollEndSound ();
			PlaySmartSound ();//LQ adjust position to response the smartSymbol in time

			if (needToPlayAnticipationAnimation) {
				currentV = elementConfigs.GetReelConfigs (Index).SlowV;
			} else {
				currentV = elementConfigs.GetReelConfigs (Index).RunBackV1;
			}
			if (LocomotiveElement != null) {
				if (elementConfigs.GetReelConfigs(Index).isReverse) {
					totalMove = (LocomotiveElement.transform as RectTransform).anchorMin.y - LocomotiveElementFinalMinY();
				} else {
					totalMove = LocomotiveElementFinalMinY() - (LocomotiveElement.transform as RectTransform).anchorMin.y;
				}
			}
				
			if (needToPlayAnticipationAnimation) {
				backDonwV = elementConfigs.GetReelConfigs (Index).SlowV;
			} else {
				backDonwV = elementConfigs.GetReelConfigs (Index).StopV;
			}
			backDownA = elementConfigs.GetReelConfigs (Index).backA1;
		}

		void ReelRunBackDown(){
			float moveDistance = elementRelativelyHeight * elementConfigs.GetReelConfigs (Index).BackDistace;
 
			float offset = RunBackDownOffset(totalMove, moveDistance);

			if ((totalMove+offset) >= moveDistance) {
				for (int i = 0; i < Elements.Count; i++) {
					BaseElementPanel element = Elements[i];
					ChangePositionForBack (element, moveDistance-totalMove);
				}
				PrepareReelRunBackUp ();
			} else {
				totalMove += offset;
				for (int i = 0; i < Elements.Count; i++) {
					BaseElementPanel element = Elements[i];
					ChangePositionForBack (element, offset);
				}
			}
        }

		//todo-xiaogang
		protected virtual float RunBackDownOffset(float totalM, float moveDis)
		{
			float step = timeStepRollReel;

			if (backDonwV > currentV) {
				backDonwV -= step* backDownA;
				if (backDonwV<currentV) {
					backDonwV = currentV;
				}
			}

			float offset = step * backDonwV;
			
			if (totalMove + offset > moveDis) {
				offset = moveDis - totalMove;
				step = offset / backDonwV;
			}

			return offset;
		}

		float backUpV = 0;
		float backUpA;
		void PrepareReelRunBackUp(){
			this.State = ReelState.RUNBACKUP;
			float moveDistance = elementRelativelyHeight * elementConfigs.GetReelConfigs (Index).BackDistace;
			totalMove = 0;
			if (LocomotiveElement != null) {
				if (elementConfigs.GetReelConfigs(Index).isReverse) {
					totalMove = (LocomotiveElementFinalMinY() + moveDistance) - (LocomotiveElement.transform as RectTransform).anchorMin.y;
				} else {
					totalMove = (LocomotiveElement.transform as RectTransform).anchorMin.y - (LocomotiveElementFinalMinY() - moveDistance);
				}
			}

			if (needToPlayAnticipationAnimation) {
				currentV = elementConfigs.GetReelConfigs (Index).SlowRunBackV2;
			} else {
				currentV = elementConfigs.GetReelConfigs (Index).RunBackV2;
			}


			if (needToPlayAnticipationAnimation) {
				backUpV = elementConfigs.GetReelConfigs (Index).SlowV;
			} else {
				backUpV = elementConfigs.GetReelConfigs (Index).RunBackV1;
			}
			backUpA = elementConfigs.GetReelConfigs (Index).backA2;

		}

		void ReelRunBackUp(){
			float moveDistance = elementRelativelyHeight * elementConfigs.GetReelConfigs (Index).BackDistace;
			float offset = RunBackUpOffset();
			if ((totalMove+offset) < moveDistance) {
				for (int i = 0; i < Elements.Count; i++) {
					BaseElementPanel element = Elements [i];
					ChangePositionForBack (element, -offset);
				}
				totalMove += offset;
			} else {
				positionMap.Clear ();
				for (int i = 0; i < Elements.Count; i++) {
					BaseElementPanel element = Elements[i];
					SetPosition(element);
				}
				SortElements();
				ReelAnimationEnd ();
			}
		}

		//todo-xiaogang
		protected virtual float RunBackUpOffset()
		{
			float step = timeStepRollReel;
			if (backUpV > currentV) {
				backUpV -= step * backUpA;
				if (backUpV < currentV) {
					backUpV = currentV;
				}
			}
			float offset = step * backUpV;

			return offset;
		}

		public const string REEL_ANIMATION_END = "ReelAniamtionEnd";
		public const string REEL_FAST_RUN_BEFORE_SLOW_DOWN = "ReelFastRunBeforeSlowDown";
		protected virtual void ReelAnimationEnd(){
			this.StopSlowAnimation ();
			Messenger.Broadcast<int>(REEL_ANIMATION_END,this.Index);
			if (reelManager.EachReelStopHandler != null) {
				reelManager.EachReelStopHandler (this.Index);
			}

			m_allRunningReelNum--;

			if (m_allRunningReelNum == 0) {
				reelManager.State = GameState.READY;
				if (this.reelManager.OnStop != null) {
					this.reelManager.OnStop();
				}
			} else {
				if (reelManager.gameConfigs.isReverseRollReel) {
					if (reelManager.Reels [Mathf.Max(this.Index - 1,0)].ShouldStoppedByPreReel()) {
						reelManager.Reels[Mathf.Max(this.Index - 1,0)].TerminateReelRunningState();
					}
				} 
				else if(Index < reelManager.Reels.Count-1){
					if (reelManager.Reels [this.Index + 1].ShouldStoppedByPreReel()) {
						reelManager.Reels[this.Index + 1].TerminateReelRunningState();
					}
				}

				//(reelManager.Reels[this.Index + 1] as RollReelPanel).PlaySlowAnimation();
			}
			reelManager.ChangeCurrentFirstRunningReelIndex (Index);
			this.State = ReelState.END;
			AfterReelRunBack();
		}


        public override bool ShouldStoppedByPreReel(){
             return base.ShouldStoppedByPreReel () || (reelManager.gameConfigs.StopAtOnce && IsAfterAncitipationReel ());
        }
        protected bool ApproximateDistance(float distance1, float distance2) {
            return Mathf.Abs(distance1 - distance2) <= 0.001f;
        }

		public int testCount =0;

        protected virtual void ChangeElementsPosition(float offset) {
			testCount =0;
            bool elementJumped = false;
            for (int i = 0; i < Elements.Count; i++) {
                BaseElementPanel element = Elements[i];
				elementJumped |= ChangePosition(element, offset);
            }

            if (elementJumped) {
                SortElements();
            }

			RectTransform r = Elements[0].transform as RectTransform;
			if (!elementConfigs.GetReelConfigs (Index).isReverse) {
				if (-elementRelativelyHeight - r.anchorMin.y > 0) {

					for (int i = 0; i < Elements.Count; i++) {
						BaseElementPanel element = Elements [i];
						RectTransform tempR = element.transform as RectTransform;
						Vector2 max = tempR.anchorMax;
						Vector2 min = tempR.anchorMin;
						tempR.anchorMax = new Vector2 (max.x, i * elementRelativelyHeight);
						tempR.anchorMin = new Vector2 (min.x, (i - 1) * elementRelativelyHeight);
						CreateXOffsetByY (tempR);
					}
				}
			} else {
				if (r.anchorMax.y - addWhenChange > 0) {
					for (int i = 0; i < Elements.Count; i++) {
						BaseElementPanel element = Elements [i];
						RectTransform tempR = element.transform as RectTransform;
						Vector2 max = tempR.anchorMax;
						Vector2 min = tempR.anchorMin;
						tempR.anchorMax = new Vector2 (max.x, addWhenChange - i * elementRelativelyHeight);
						tempR.anchorMin = new Vector2 (min.x, addWhenChange- (i + 1) * elementRelativelyHeight);
						CreateXOffsetByY (tempR);
					}
				}
			}


        }

        protected int ResultIndex = 0;

        protected virtual bool ChangePosition (BaseElementPanel element, float offset)
        {
            RectTransform r = element.gameObject.transform as RectTransform;
            Vector2 max = r.anchorMax;
            Vector2 min = r.anchorMin;
            float maxY = 0;
            float minY = 0;
			float maxX = 1;
			float minX = 0;
            bool elementJumped = false;

            UnityAction setLocomotiveElementAction = () => {
                if (LocomotiveElement == null) {
                    LocomotiveElement = element;
                }
            };

            UnityAction moveElementAction = () => {
				r.anchorMax = new Vector2(1, maxY);
				r.anchorMin = new Vector2(0, minY);
				CreateXOffsetByY (r);
            };

            if (elementConfigs.GetReelConfigs (Index).isReverse) {
                maxY = max.y + offset;
                minY = min.y + offset;
                if (maxY >= addWhenChange || ApproximateDistance(maxY, addWhenChange)) {
                    minY -= addWhenChange;
                    maxY -= addWhenChange;
                    elementJumped = true;
                    if (this.State == ReelState.SLOWDOWNOVER || this.State == ReelState.STOPPING) {
                        if (RunWithReelData) {
                            if (this.State == ReelState.SLOWDOWNOVER && PostionOfReelData == this.ResultStartIndex) {
								jumpToStoping = true;
                                ResultIndex = Result.Count - 1;
                                element.SymbolIndex = Result [ResultIndex];
                                element.PositionId = ResultIndex;
                                ResultIndex--;
                                setLocomotiveElementAction();
                            } else if (this.State == ReelState.STOPPING) {
                                if (ResultIndex >= 0) {
                                    element.SymbolIndex = Result [ResultIndex];
                                    element.PositionId = ResultIndex;
                                    ResultIndex--;
                                }
                            } else {
                                element.SymbolIndex = GenSymbol ();
                            }
                        } else {
                            if (this.State == ReelState.SLOWDOWNOVER) {
                                ResultIndex = Result.Count - 1;
								jumpToStoping = true;
                                element.SymbolIndex = Result[ResultIndex];
                                element.PositionId = ResultIndex;
                                ResultIndex--;
                                setLocomotiveElementAction();
                            } else if (this.State == ReelState.STOPPING) {
                                if (ResultIndex >= 0) {
                                    element.SymbolIndex = Result[ResultIndex];
                                    element.PositionId = ResultIndex;
                                    ResultIndex--;
                                }
                            }
                        }
                    } else {
                        element.SymbolIndex = GenSymbol ();
                    }
                    if (RunWithReelData) {
						PostionOfReelData = (PostionOfReelData + 1) % ReelDataLenth();
                    }
                }
            } else {
                maxY = max.y - offset;
                minY = min.y - offset;
                if (minY <= -elementRelativelyHeight || ApproximateDistance(minY, -elementRelativelyHeight)) {
					testCount++;
                    minY += addWhenChange;
                    maxY += addWhenChange;
                    elementJumped = true;
                    if (this.State == ReelState.SLOWDOWNOVER || this.State == ReelState.STOPPING) {
                        if (RunWithReelData) {
                            if (this.State == ReelState.SLOWDOWNOVER && PostionOfReelData == this.ResultStartIndex) {
								jumpToStoping = true;
                                ResultIndex = 0;
                                element.SymbolIndex = Result[ResultIndex];
                                element.PositionId = ResultIndex;
                                ResultIndex++;
                                setLocomotiveElementAction();
                            } else if (this.State == ReelState.STOPPING) {
                                if (ResultIndex < Result.Count) {
                                    element.SymbolIndex = Result[ResultIndex];
                                    element.PositionId = ResultIndex;
                                    ResultIndex++;
                                }
                            } else {
                                element.SymbolIndex = GenSymbol();
                            }
                        } else {
                            if (this.State == ReelState.SLOWDOWNOVER) {
                                ResultIndex = 0;
								jumpToStoping = true;
                                element.SymbolIndex = Result[ResultIndex];
                                element.PositionId = ResultIndex;
                                ResultIndex++;
                                setLocomotiveElementAction();
                            } else if (this.State == ReelState.STOPPING) {
                                if (ResultIndex < Result.Count) {
                                    element.SymbolIndex = Result[ResultIndex];
                                    element.PositionId = ResultIndex;
                                    ResultIndex++;
                                }
                            }
                        }
                    } else {
                        element.SymbolIndex = GenSymbol ();
                    }
                    if (RunWithReelData) {
						PostionOfReelData = (PostionOfReelData + 1) % ReelDataLenth();
                    }
                }
            }

            moveElementAction();
            element.ChangeBackGroundInfo (r);
            return elementJumped;
        }

        protected virtual void ChangePositionForBack (BaseElementPanel element, float offset)
        {
            RectTransform r = element.gameObject.transform as RectTransform;
            Vector2 max = r.anchorMax;
            Vector2 min = r.anchorMin;
            if (elementConfigs.GetReelConfigs (Index).isReverse) {
                float maxY = max.y + offset;
                float minY = min.y + offset;
                r.anchorMax = new Vector2 (max.x, maxY);
                r.anchorMin = new Vector2 (min.x, minY);
            } else {
                float maxY = max.y - offset;
                float minY = min.y - offset;
                r.anchorMax = new Vector2 (max.x, maxY);
                r.anchorMin = new Vector2 (min.x, minY);
            }
			CreateXOffsetByY (r);
            element.ChangeBackGroundInfo (r);
        }

        protected virtual void SetPosition (BaseElementPanel element)
        {
            RectTransform r = element.gameObject.transform as RectTransform;
            Vector2 max = r.anchorMax;
            Vector2 min = r.anchorMin;

			element.PositionId = this.Elements.IndexOf (element);

            if (elementConfigs.hasBlank) {
                r.anchorMax = new Vector2(max.x, (element.PositionId * 0.5f + 0.5f) * elementRelativelyHeight);
                r.anchorMin = new Vector2(min.x, (element.PositionId * 0.5f - 0.5f) * elementRelativelyHeight);
            } else {
                r.anchorMax = new Vector2(max.x, (element.PositionId + 1) * elementRelativelyHeight);
                r.anchorMin = new Vector2(min.x, (element.PositionId) * elementRelativelyHeight);
            }
			CreateXOffsetByY (r);
            if (elementConfigs.GetReelConfigs(Index).isReverse) {
                element.SymbolIndex = Result[(1 + element.PositionId) % Result.Count];
            } else {
                element.SymbolIndex = Result[element.PositionId];
            }
            element.ChangeBackGroundInfo(r);
        }

        protected virtual float StopMoveDistance ()
        {
            if (elementConfigs.hasBlank) {
                return addWhenChange - elementRelativelyHeight * 0.5f;
            } else {
                return  addWhenChange - elementRelativelyHeight;
            }
        }

        public override void ResultConvert (List<int> reelResult, int ResultStartIndex = 0)
        {
            Result.Clear ();
            if (elementConfigs.GetReelConfigs (Index).isReverse) {
                if (RunWithReelData) {
                    this.ResultStartIndex = ResultStartIndex;
                  
					Result.Add (this.reelManager.resultContent.ReelResults [Index].reelData [(ResultStartIndex + reelResult.Count) % ReelDataLenth()].value);
                  
                    for (int i = 0; i < reelResult.Count; i++) {
                        Result.Add (reelResult [i]);
                    }
                } else {
                    if (elementConfigs.hasBlank) {
                        Result.Add (-1);
                    } else {
                        Result.Add (elementConfigs.RandGenSymbol ());
                    }
                    for (int i = 0; i < reelResult.Count; i++) {
                        Result.Add (reelResult [i]);
                    }
                }
            } else {
                if (RunWithReelData) {
                    this.ResultStartIndex = ResultStartIndex;
                    for (int i = 0; i < reelResult.Count; i++) {
                        Result.Add (reelResult [i]);
                    }
					Result.Add (this.reelManager.resultContent.ReelResults [Index].reelData [(ResultStartIndex + reelResult.Count) % ReelDataLenth()].value);
                } else {
                    for (int i = 0; i < reelResult.Count; i++) {
                        Result.Add (reelResult [i]);
                    }
                    if (elementConfigs.hasBlank)
                        Result.Add (-1);
                    else
                        Result.Add (elementConfigs.RandGenSymbol ());
                }
            }
        }
        public const string GLIDING_TRIGGER_AUDIO = "GlidingTrigger";

        public IEnumerator ReelGilding (bool isUp,int length = 1,bool enableReplaceSymbol = false)
        {
			this.State = ReelState.END;
            yield return new WaitForSeconds (elementConfigs.GetReelConfigs (Index).GlidingPreWaitTime);
            float totalDistance,changeElementDistance;
            if (elementConfigs.hasBlank) {
				totalDistance = elementRelativelyHeight / 2 *length;
                changeElementDistance = elementRelativelyHeight / 2;
            } else {
				totalDistance = elementRelativelyHeight *length;
                changeElementDistance = elementRelativelyHeight;
            }
            float frameOffset;
			frameOffset = timeStepRollReel * elementConfigs.GetReelConfigs (Index).GlidingSpeed;
            float currentHasMove = 0;

            int replaceIndex = 0;
            int startPos = 0;
            List<int> replaceElements = new List<int>();
            if (enableReplaceSymbol)
            {
                replaceElements = GetGlidingStripSymbolIndexs(isUp, length);
                BaseElementPanel hideElement = GetHideResultElement();
                hideElement.SymbolIndex = replaceElements[0];
                replaceElements.RemoveAt(0);
            }

            Libs.AudioEntity.Instance.PlayEffect(GLIDING_TRIGGER_AUDIO);
            while (currentHasMove < totalDistance) {
				yield return new WaitForSeconds (timeStepRollReel);

                bool elementJumped = false;
                bool elementChange = false;
                for (int i = 0; i < Elements.Count; i++)
                {
                    BaseElementPanel element = Elements[i];
                    elementChange = false;
                    if (currentHasMove + frameOffset >= totalDistance)
                    {
                        frameOffset = totalDistance - currentHasMove;
                    }
                    elementChange = ChangePositionForGliding(element, isUp ? -frameOffset : frameOffset, isUp);

                    if (elementChange && enableReplaceSymbol&&replaceIndex < replaceElements.Count) 
                    {
                        if (isUp&& startPos != 0) { element.SymbolIndex = replaceElements[replaceIndex]; replaceIndex++;}
                        if(!isUp) { element.SymbolIndex = replaceElements[replaceIndex]; replaceIndex++; }
                        startPos++;
                    }

                    elementJumped |= elementChange;
                }
                if (elementJumped) {                   
                    SortElements();
                }
                currentHasMove += frameOffset;
            }

            if (this.IsEndReel) {
                yield return new WaitForSeconds (elementConfigs.GetReelConfigs (Index).GlidingAfterWaitTime);
            } 
        }

        private BaseElementPanel GetHideResultElement(){
            for (int i = 0; i < Elements.Count; i++){
                RectTransform rt = Elements[i].transform as RectTransform;
                Vector2 max = rt.anchorMax;
                Vector2 min = rt.anchorMin;
                float maxY = max.y;
                float minY = min.y;
                if ((minY <= -elementRelativelyHeight || ApproximateDistance(minY, -elementRelativelyHeight)))
                {
                    return Elements[i];
                }
                else if ((maxY >= 1 + elementRelativelyHeight || ApproximateDistance(maxY, 1 + elementRelativelyHeight)))
                {
                    return Elements[i];
                }
            }
            return null;
        } 
        private List<int> GetGlidingStripSymbolIndexs(bool isUp,int num){
            reelManager.resultContent.ReelResults[Index].UpdateReelPosition(Result);
            List<int> symbolIndexs = new List<int>();
            for (int i = 0; i < num; i++)
            {
                if (isUp) symbolIndexs.Add(this.reelManager.resultContent.ReelResults[Index].PreSymbolIndex(i+1));
                else symbolIndexs.Add(this.reelManager.resultContent.ReelResults[Index].AfterSymbolIndex(i+1));
            }
            return symbolIndexs;
        }
        private void UpdateResultReelPosition(){
            
        }

        public bool ChangePositionForGliding (BaseElementPanel element, float offset, bool isUp)
        {
            bool elementJumped = false;
            RectTransform r = element.gameObject.transform as RectTransform;
            Vector2 max = r.anchorMax;
            Vector2 min = r.anchorMin;
            float maxY = max.y - offset;
            float minY = min.y - offset;
            if ((minY <= -elementRelativelyHeight || ApproximateDistance(minY, -elementRelativelyHeight)) && !isUp) {
                minY += addWhenChange;
                maxY += addWhenChange;
                elementJumped = true;
            } else if ((maxY >= 1 + elementRelativelyHeight || ApproximateDistance(maxY, 1 + elementRelativelyHeight)) && isUp) {
                minY -= addWhenChange;
                maxY -= addWhenChange;
                elementJumped = true;
            }

            r.anchorMax = new Vector2 (max.x, maxY);
            r.anchorMin = new Vector2 (min.x, minY);
			CreateXOffsetByY (r);
            element.ChangeBackGroundInfo (r);
            return elementJumped;
        }

        public void ChangePositionMap (bool isUp)
        {
            positionMap.Clear ();
            for (int i = 0; i < this.Elements.Count; i++) {
                if (isUp) {
                    if (this.Elements [i].PositionId == this.Elements.Count - 1) {
                        this.Elements [i].PositionId = 0;
                    } else {
                        this.Elements [i].PositionId++;
                    }
                } else {
                    if (this.Elements [i].PositionId == 0) {
                        this.Elements [i].PositionId = this.Elements.Count - 1;
                    } else {
                        this.Elements [i].PositionId--;
                    }
                }
            }
        }

		float shakeStartTime;
		void PrepareReelRunShake()
		{
			BeforeReelRunBack();
			this.State = ReelState.RUNSHAKE;
			PlayRollEndSound();
			PlaySmartSound();

			shakeStartTime = Time.timeSinceLevelLoad;
			if (LocomotiveElement != null)
			{
				if (elementConfigs.GetReelConfigs(Index).isReverse)
				{
					totalMove = (LocomotiveElement.transform as RectTransform).anchorMin.y - LocomotiveElementFinalMinY();
				}
				else
				{
					totalMove = LocomotiveElementFinalMinY() - (LocomotiveElement.transform as RectTransform).anchorMin.y;
				}
			}
		}

		float shakeDistance = 0.01f;
		float[] m_Distance_Array = {0.025f,0.015f,0.0050f,0f};
		private const float TWO_PI = Mathf.PI * 2;
		private const float FOUR_PI = Mathf.PI * 4;
		private const float SIX_PI = Mathf.PI * 6;
		void ReelRunShake()
		{
			//此处TempFValue表示 t     y = sin(90t)
			float TempFValue = 90 * (Time.timeSinceLevelLoad - shakeStartTime);

			if (TempFValue <= Mathf.PI){
				shakeDistance = m_Distance_Array[0];
			} else if (TempFValue <= TWO_PI){
				shakeDistance = m_Distance_Array[1];
			} else if (TempFValue <= FOUR_PI){
				shakeDistance = m_Distance_Array[2];
			} else if (TempFValue <= SIX_PI){
				shakeDistance = m_Distance_Array[3];
			} else{
				positionMap.Clear ();

				for (int i = 0; i < Elements.Count; i++) {
					BaseElementPanel element = Elements[i];
					SetPosition(element);
				}
				SortElements();
				ReelAnimationEnd();
				return;
			}

			//此处TempFValue表示当前时间的振幅
			TempFValue = Mathf.Sin(TempFValue) * shakeDistance;

			for (int i = 0; i < Elements.Count; i++){
				BaseElementPanel element = Elements[i];
				SetPosition_Shake(element, TempFValue);
			}
		}

		protected virtual void SetPosition_Shake (BaseElementPanel element, float offset)
		{
			RectTransform r = element.gameObject.transform as RectTransform;
			Vector2 max = r.anchorMax;
			Vector2 min = r.anchorMin;

			element.PositionId = this.Elements.IndexOf (element);

			if (elementConfigs.hasBlank) {
				r.anchorMax = new Vector2(max.x, (element.PositionId * 0.5f + 0.5f) * elementRelativelyHeight);
				r.anchorMin = new Vector2(min.x, (element.PositionId * 0.5f - 0.5f) * elementRelativelyHeight);
			} else {
				r.anchorMax = new Vector2(max.x, (element.PositionId + 1) * elementRelativelyHeight);
				r.anchorMin = new Vector2(min.x, (element.PositionId) * elementRelativelyHeight);
			}

			if (elementConfigs.GetReelConfigs(Index).isReverse) {
				element.SymbolIndex = Result[(1 + element.PositionId) % Result.Count];
				float maxY = r.anchorMax.y + offset;
				float minY = r.anchorMin.y + offset;
				r.anchorMax = new Vector2 (max.x, maxY);
				r.anchorMin = new Vector2 (min.x, minY);
			} else {
				element.SymbolIndex = Result[element.PositionId];
				float maxY = r.anchorMax.y - offset;
				float minY = r.anchorMin.y - offset;
				r.anchorMax = new Vector2 (max.x, maxY);
				r.anchorMin = new Vector2 (min.x, minY);
			}

			element.ChangeBackGroundInfo(r);
		}

		Vector3 upPos;
		Vector3 normalPos;

		public void ReelUpBeforeSpin()
		{
			Vector3 vecPos = transform.localPosition;
			DOTween.To(() => transform.localPosition, x => vecPos = x,upPos, 0.15f).OnUpdate(() =>{
				transform.localPosition = vecPos;
			});
		}

		public void ReelDownBeforeSpin()
		{
			Vector3 vecPos = transform.localPosition;
			DOTween.To(() => transform.localPosition, x => vecPos = x,normalPos, 0.2f).OnUpdate(() =>{
				transform.localPosition = vecPos;
			});
		}
	}
}
