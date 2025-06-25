using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class SingleNumber : MonoBehaviour
{
    public Text text0;
    public Text text1;


    private Tween tween;

    private string curNumber = "";

    private float moveY = 0;

    void Awake()
    {
        moveY = this.GetComponent<RectTransform>().rect.height;
    }

    public void SetSingleScale(Vector2 pos)
    {
        text0.transform.parent.localScale = pos;
        text1.transform.parent.localScale = pos;
    }

    public void SetText(string singleNumber)
    {
        if(tween != null)
        {
            tween.Kill(true);
            tween = null;
        }
        curNumber = singleNumber;
        text0.text = curNumber;
        this.transform.localPosition = new Vector2(this.transform.localPosition.x, 0);;
    }

    public void SetSingleText(string singleNumber, float time = 0.2f)
    {
        if(singleNumber == curNumber)
        {
            return;
        }else
        {
            curNumber = singleNumber;
            text1.text = curNumber;
            tween = this.transform.DOLocalMoveY(moveY, time).SetEase ((Ease)Ease.Linear).OnComplete(()=>
            {
                text0.text = singleNumber;
                this.transform.localPosition = new Vector2(this.transform.localPosition.x, 0);;
            });
        }
    }
}
