using UnityEngine;
using System.Collections;
using Libs;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
namespace Classic
{

	public class CircleSymbolRender : AnimatiorElement {
		private readonly string SCATTER_WHEEL_WIN = "WheelWin";

		private Circling currentCircle;
//		private bool isLastRender = false; 
		private SuperWheelReelCircleSymbols  circleSymbols;
		private System.Action callBack;

		public override void PlayAnimation (int animationId,bool isLoop = true, System.Action VideoInitHandler=null, System.Action VideoCompleteHandler=null,float RepeatPlayStartTime =0f,bool isUseCache = true)
		{
			base.PlayAnimation (animationId ,true, null, null,0f);
		}

		public void PlayOriginAnimation(int animationId)
		{
			base.PlayAnimation (animationId);
		}

		public void PlayCircleAnimation(System.Action _callback)
		{
//			this.isLastRender = isLastRender;
			this.callBack = _callback;
            int reelIndex = this.ReelIndex;
			PlayAnimation (reelIndex+1);
			circleSymbols = animationParent.transform.GetComponentInChildren<SuperWheelReelCircleSymbols> ();
			currentCircle = circleSymbols.circleSymbols [reelIndex];	
			currentCircle.gameObject.SetActive (true);
 
			StartCoroutine( UI.Utils.UIUtil.DelayAction(2.4f,delegate(){currentCircle.StartRoll(CircleNumIndex ,CircleRollEnd,true,false);}));
		}


		private void CircleRollEnd(Circling circle)
		{
//			Image scaleImage = currentCircle.AllNumImages[this.awardNumIndex];
//			scaleImage.transform.DOScale (3f, 2f).SetEase(Ease.OutBack).OnComplete(()=>{
//				if(this.isLastRender)
//				{
//					ResultStateManager.Instante.AddNormal();
//					ResultStateManager.Instante.Play ();
//				}
//
//			});

			GameObject temp =circleSymbols.transform.Find ("WinText").gameObject;
			if (temp == null) {
//				if (this.isLastRender) {
					if (this.callBack != null) {
						callBack ();
					}
//				}
				return;
			}
			 
			Libs.AudioEntity.Instance.PlayEffect (SCATTER_WHEEL_WIN);

			temp.SetActive (true);
			temp.transform.localScale = Vector3.one;
			temp.GetComponent<TextMeshProUGUI> ().SetText (CircleAwardNum.ToString ());
			temp.transform.DOScale (Vector3.zero, 1f).From ().SetEase (Ease.OutBack).OnComplete (() => {
				
//				StartCoroutine(UI.Utils.UIUtil.DelayAction(DelayTime,()=>{
 
						if(this.callBack!=null){
							callBack();
						}
 
//					}));
				}
			);
		}

		public void EndCircle()
		{
			if (circleSymbols != null) {
				GameObject temp = circleSymbols.transform.Find ("WinText").gameObject;
				if (temp != null) {
					temp.SetActive (false);
				}
			}
			if (currentCircle != null) {
				currentCircle.HideBlackMask ();
			}
			StopAnimation();
			Destroy(AnimationGO);
		}

		public void HideMask()
		{
			if (circleSymbols != null) {
				GameObject temp = circleSymbols.transform.Find ("WinText").gameObject;
				if (temp != null) {
					temp.SetActive (false);
				}
			}
			currentCircle.HideBlackMask ();
		}
	}
}