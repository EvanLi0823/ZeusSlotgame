using UnityEngine;
using System.Collections;
using TMPro;
using DG.Tweening;

namespace Classic
{
    public class AnimatiorElement : BaseElementPanel
    {

        protected Animator animator;
        protected Tweener t;

        //public override void InitElement(BaseReel ReelContronller, int SymbolIndex)
        //{
        //    base.InitElement(ReelContronller, SymbolIndex);
        //}

		public override void PlayAnimation (int animationId,bool isLoop = true, System.Action VideoInitHandler=null, System.Action VideoCompleteHandler=null,float RepeatPlayStartTime =0f,bool isUseCache = true)
        {
            base.PlayAnimation (animationId);

            PlayStripBackgroundAnimation ();

            if (AnimationGO == null) {
				LoadAnimation (animationParent);
            }
			PlayAnimationGameObject (animationId);
        }

		protected virtual void PlayAnimationGameObject(int animationId, bool showAnimationFrame = true)
		{
			if (AnimationGO != null) {
                EnableSymbolImage (false);
				AnimationGO.SetActive (true);
                if(AnimationGO.transform.childCount != 0)
                {
                    animator = AnimationGO.transform.GetChild (0).GetComponent<Animator> ();
                    if (animator != null)
                    {
                        animator.SetInteger ("state", animationId);
                    }
                }
			} else {
				animator = null;
			}
			if (showAnimationFrame) {
//				if (this.animationParent == null) {
//					Debug.Log (3333);
//				}
//				Transform frameImageTransform = this.animationParent.Find ("BGImage");
//				if (frameImageTransform != null) {
//					frameImageTransform.gameObject.SetActive (true);
//				}
			}


			if (AwardNumber > 0) {
//				Transform winTextTransform = this.animationParent.Find ("WinText");
//				if (winTextTransform != null) {
//					winTextTransform.gameObject.SetActive (true);
//					winTextTransform.gameObject.GetComponent<TextMeshProUGUI> ().SetText (Utils.Utilities.ThousandSeparatorNumber(AwardNumber));
//					t = winTextTransform.DOScale (Vector3.zero, 0.5f).From ().SetEase (Ease.OutBack).OnComplete (() => {
//						winTextTransform.gameObject.SetActive (false);
//						winTextTransform.gameObject.SetActive (true);
//					}).OnUpdate (()=>{winTextTransform.gameObject.SetActive (false);
//						winTextTransform.gameObject.SetActive (true);});;
//				}
			}
		}


        public virtual void PlayStripBackgroundAnimation(){
            
        }

        public override void StopAnimation (bool showAnimationFrame = true)
        {
            Transform frameImageTransform = this.animationParent.Find ("BGImage");
            if (frameImageTransform != null) {
                frameImageTransform.gameObject.SetActive (false);
            }

            if (animator != null) {
                DestroyAnimation ();
                animator = null;
            }

            ShowStripBackgroundImage ();

            EnableSymbolImage (true);

            Transform winTextTransform = this.animationParent.Find ("WinText");
            if (winTextTransform != null) {
                winTextTransform.gameObject.SetActive (false);
            }
            if (t != null) {
                t.Kill ();
            }
			
            AwardNumber = 0;
        }
        public virtual void ShowStripBackgroundImage(){
            
        }
		public override void PauseAnimation (bool notChange = false)
        {
            Transform frameImageTransform = this.animationParent.Find ("BGImage");
            if (frameImageTransform != null) {
                frameImageTransform.gameObject.SetActive (false);
            }

            ShowStripBackgroundImage ();

            if (animator != null) {
                animator.SetInteger ("state", 0);
				if (notChange) {
					EnableSymbolImage (true);
					if (AnimationGO != null) {
						AnimationGO.SetActive (false);
					}
				}
            }

            Transform winTextTransform = this.animationParent.Find ("WinText");
            if (winTextTransform != null) {
                winTextTransform.gameObject.SetActive (false);
            }
            AwardNumber = 0;
        }

        public override void PlayNonSelectionAnimation(int animationId, bool showAnimationFrame = true){
            base.PlayAnimation (animationId);

            PlayStripBackgroundAnimation ();

            if (AnimationGO == null) {
				LoadAnimation (animationParent);
            }
            if (AnimationGO != null) {
                EnableSymbolImage (false);
                animator = AnimationGO.transform.GetChild (0).GetComponent<Animator> ();
                AnimationGO.SetActive (true);
                animator.SetInteger ("state", animationId);
            } else {
                animator = null;
            }
            if (showAnimationFrame) {
                Transform frameImageTransform = this.animationParent.Find ("BGImage");
                if (frameImageTransform != null) {
                    frameImageTransform.gameObject.SetActive (true);
                }
            }
        }
    }
}
