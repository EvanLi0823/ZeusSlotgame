using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using DG;
using UnityEngine.UI;
using TMPro;
namespace Libs {
public class ScrollContainer : MonoBehaviour {

	public Scrollbar MyScrollBar;
		[HideInInspector]
	public bool  Scrolling = false;
	public TextMeshProUGUI NumberTxt;
    public int NodeNumber = 3;

	void Awake()
	{

	}
	// Use this for initialization
//	void Start () {
//		if(EnableScroll){
//			StartCoroutine(StartMove());
//		}
//	}
        //LQ 先设置节点关系，再进行移动设定
        public void setEnableScroll(bool isScroll,bool onlyShowTime = false)
		{
			if (!this.gameObject.activeSelf) {
				return;
			}
			if (isScroll){
				if(this.Scrolling)
				{
					return;
				}
				else {
					//StartCoroutine (StartMove ());//禁用滚动
				 
					this.Scrolling = true;
					this.NumberTxt.transform.SetAsLastSibling();

				}
			}
			else {
                StopAllCoroutines();
				this.Scrolling = false;
				this.MyScrollBar.value = 1f;
				//this.NumberTxt.transform.SetAsFirstSibling();
                //LQ Add 第四次获取之后只能显示时间，前四次则显示奖励
                //if (onlyShowTime) {
                    this.NumberTxt.transform.SetAsLastSibling ();
                //} else {
                //   this.NumberTxt.transform.SetAsFirstSibling ();
                //}
               
			}
		}

	IEnumerator StartMove()
	{
		yield return new WaitForSeconds(2f);
            //LQ 检测第一个节点是否是激活节点，后续将其设为最后一个兄弟节点,只有激活节点有效
            RectTransform transformFirst = null;
            for (int i = 0; i < NodeNumber; i++) {
                 transformFirst = this.transform.GetChild(i) as RectTransform;
                if (transformFirst.gameObject.activeSelf) {
                    break;
                }
            }
		
		DOTween.To(()=>MyScrollBar.value,x=>MyScrollBar.value=x,0f,0.5f).SetEase(Ease.Linear);//LQ modify
		yield return new WaitForSeconds(1.5f);
		transformFirst.SetAsLastSibling();
		MyScrollBar.value = 1f;
        yield return new WaitForSeconds(1f);
		StartCoroutine(StartMove());
	}
}

}