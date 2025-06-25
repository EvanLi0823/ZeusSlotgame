using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

namespace Classic
{
    public class RollIndependentElement : AnimatiorElement
    {
        protected float timeStep {
            get { return BaseSlotMachineController.Instance.averageFrame.averageTime; }
        }

        public List<Image> rollImages;
        public Image staticImage1;
        public Image staticImage2;

        public bool hasAnticipationAnimation;
        public GameObject anticipationAnimationGO;
        //		public bool needAddAnticipationTime;

        public float elementRelativelyHeight = 1;
        public float addWhenChange = 2;

        public override void InitElement (BaseReel ReelContronller, int SymbolIndex)
        {
            base.InitElement (ReelContronller, SymbolIndex);
            rollImages.Add (staticImage1);
            rollImages.Add (staticImage2);
            staticImage1.sprite = CreateRandomSprite ();
            staticImage2.sprite = CreateRandomSprite ();
        }

        public virtual void ChangePosition (Image element, float offset)
        {
            RectTransform r = element.gameObject.transform as RectTransform;
            Vector2 max = r.anchorMax;
            Vector2 min = r.anchorMin;
            float maxY = max.y - offset;
            float minY = min.y - offset;
            if (minY < -elementRelativelyHeight) {
                minY += addWhenChange;
                maxY += addWhenChange;
                element.sprite = CreateRandomSprite ();
                if (symbolAnimationState == SymbolAnimatioState.FIXRUNNING) {
                    symbolAnimationState = SymbolAnimatioState.SLOWDOWN;
                    m_SymbolImage = element;
                    SymbolIndex = ReelContronller.reelManager.resultContent.ReelResults [ReelIndex].SymbolResults [PositionId];
//					StartCoroutine (SymbolSlowDown (1f));
                }
            }
            r.anchorMax = new Vector2 (max.x, maxY);
            r.anchorMin = new Vector2 (min.x, minY);
        }

        public Sprite CreateRandomSprite ()
        {
            List<ResultContent.WeightData> reelResult = this.ReelContronller.reelManager.resultContent.ReelResults [this.ReelIndex].reelData; 

            int randomIndex = UnityEngine.Random.Range (0, reelResult.Count);
            return ReelContronller.reelManager.gameConfigs.GetBySymbolIndex (reelResult [randomIndex].value);
        }


        public void StartRunning (float runV, float runningTime)
        {
            StartCoroutine (SymbolRunning (runV, runningTime));
        }

        protected virtual IEnumerator SymbolRunning (float runV, float runningTime)
        {
            float offset = 0;

            symbolAnimationState = SymbolAnimatioState.RUNNING;
            float startTime = Time.realtimeSinceStartup;//LQ  不要使用每帧时间累加的方式，累加方式不准确
            while (symbolAnimationState == SymbolAnimatioState.RUNNING || symbolAnimationState == SymbolAnimatioState.FIXRUNNING) {
                offset = timeStep * runV;
                foreach (Image image in rollImages) {
                    ChangePosition (image, offset);
                }
                if (Time.realtimeSinceStartup -startTime > runningTime) {
                    if (symbolAnimationState == SymbolAnimatioState.RUNNING) {
                        symbolAnimationState = SymbolAnimatioState.FIXRUNNING;
                    }
                }

                yield return new WaitForSeconds (timeStep);
            }
            float moveDistant = 1;
            float currentV = GetGameConfig().reelConfigs [ReelIndex].StopV;
            offset = timeStep * currentV;
            float totalMove = 0;
            while (totalMove < moveDistant - 2 * offset) {
                offset = timeStep * currentV;
                foreach (Image image in rollImages) {
                    ChangePosition (image, offset);
                }
                totalMove += offset;
                yield return new WaitForSeconds (timeStep);
            }
            foreach (Image image in rollImages) {
                SetPosition (image);
            }
            //LQ 在此处判定Symbol停止，根据reelPanel的条件判定当前如果为smartSoundElement是否可以播放声效 IndexOfDisplay 的值从上到下依次为 3、2、1、0
            ReelManager reelManager = ReelContronller.reelManager;
            GameConfigs elementConfigs = GetGameConfig();
            bool isSmartSoundSymbol = false; //reelManager.symbolMap.getSymbolInfo (SymbolIndex).isSmartSound; //20191120。取消使用

            if (isSmartSoundSymbol) {
                //20190416
                //reelManager.smartSoundReelStripsController.CheckIndependentSmartSoundElement (reelManager, this.ReelIndex, SymbolIndex, PositionId);
            }

            if (anticipationAnimationGO != null) {
                anticipationAnimationGO.SetActive (false);
                Libs.AudioEntity.Instance.StopAnticipationSoundEffect ();
            }

            if (ReelContronller.IsEndReel) {
                if (PositionId == 0) {

                    if (ReelContronller.reelManager.OnStop != null) {
                        ReelContronller.reelManager.OnStop ();
                    }
                    ReelContronller.reelManager.State = GameState.READY;
                }
            } else {
                if (PositionId != 0) {
                    RollIndependentElement next = (RollIndependentElement)ReelContronller.Elements [PositionId - 1];
                    if (next.anticipationAnimationGO != null && next.hasAnticipationAnimation) {
                        next.anticipationAnimationGO.SetActive (true);
                        Libs.AudioEntity.Instance.PlayAnticipationSoundEffect ();
                    }
                }
            }
          
        }

        protected  void SetPosition (Image element)
        {
            RectTransform r = element.gameObject.transform as RectTransform;
            Vector2 max = r.anchorMax;
            Vector2 min = r.anchorMin;
            float maxY = max.y;
            float minY = min.y;
            if (m_SymbolImage == element) {
                r.anchorMax = new Vector2 (max.x, 1);
                r.anchorMin = new Vector2 (min.x, 0);
            } else {
                r.anchorMax = new Vector2 (max.x, 2);
                r.anchorMin = new Vector2 (min.x, 1);
            }
        }

        public SymbolAnimatioState symbolAnimationState = SymbolAnimatioState.RUNNING;

        public enum SymbolAnimatioState
        {
            RUNNING,
            FIXRUNNING,
            SLOWDOWN,
            SLOWDOWNOVER,
            STOPPING
        }
	
    }
}
