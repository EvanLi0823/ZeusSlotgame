using System;
using UnityEngine;
using System.Collections;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;
public class UIIncreaseNumber : MonoBehaviour {
    [Header("用来连接TextMeshProUGUI组件")]
    public TextMeshProUGUI TxtNumber;
    [Header("用来连接Text组件")]
    public Text TxtNumber2;

    private Tween txtTween = null;
	//private System.Action<string> callback= null;

    private Tweener doTween;

    private string color;
    void Awake()
	{
		TxtNumber = GetComponent<TextMeshProUGUI> ();
        TxtNumber2 = GetComponent<Text>();
    }

	void OnDestroy()
	{
		if (txtTween != null) {
			txtTween.Kill (true);
		}
		this.StopAllCoroutines ();
	}

	public void Reset()
	{
		this.StopAllCoroutines ();
		if (this.TxtNumber != null)
		{
			this.TxtNumber.text = "";
		}

		if (this.TxtNumber2 != null)
		{
			this.TxtNumber2.text = "";
		}

		CurrentValue = 0;
	}
    public void SetTextColor(string _color)
    {
        color = _color;
    }

    public void SetTextColor(Color color)
    {
	    if (TxtNumber2!=null)
	    {
		    TxtNumber2.color = color;
	    }
    }

    public void SetNumber(double originNum, float _IncEverySeconds, double start_num = 0f, double end_num = 0f) //,System.Action<string> refreshCB = null)
	{
        		
		this.StopAllCoroutines ();
//		DOTween.Kill (this);
		this.IncreaseNumberEverySeconds = _IncEverySeconds;
		this.CurrentValue = originNum;
		this.StartValue = start_num;
		this.EndValue = end_num;
        //this.callback = refreshCB;
        if (doTween != null)
        {
	        doTween.Kill();
        }
        RefreshTxt();

        if (IncreaseNumberEverySeconds != 0) 
        {
            if(!this.gameObject.activeInHierarchy)
            {
                return;
            }
            StartCoroutine (BeginIncrease());
		}	
	}

    public void IncreaseTo(double result, float time=2f)
    {
        if (doTween != null)
        {
            DOTween.Kill(doTween, true);
        }

        #region JackPotSystem
        if (checkCdtCB != null && !checkCdtCB(result))
        {
	        return;
        }
        #endregion
       

        doTween = DOTween.To(() => this.CurrentValue, x => this.CurrentValue = x, result, time)
            .ChangeStartValue(this.CurrentValue)
            .OnUpdate(RefreshTxt)
            .OnComplete(() =>
            {
                RefreshTxt();
            });
    }

    #region JackPotSystem

    private Func<double,bool> checkCdtCB = null;
    public void SetCheckCondition(Func<double,bool> checkCB)
    {
	    this.checkCdtCB = checkCB;
    }

    #endregion
  
    protected virtual IEnumerator BeginIncrease()
	{
		if (this.TxtNumber == null&&this.TxtNumber2==null) {
			yield break;
		}

		while (true) {
			if (this==null||(this.TxtNumber == null&&this.TxtNumber2==null)) {
				yield break;
			}
            this.IncreaseTo(this.CurrentValue + IncreaseNumberEverySeconds * waitTime, waitTime);
            yield return waitSeconds;
		}
	}

    protected void RefreshTxt()
    {
	    if (EndValue > 0)
	    {
		    if (CurrentValue >= EndValue)
		    {
			    CurrentValue = StartValue;
		    }
	    }
        string numberTxt = Utils.Utilities.ThousandSeparator(this.CurrentValue);

        if (this.TxtNumber != null)
        {
            if(string.IsNullOrEmpty(this.color))
            {
                this.TxtNumber.text = numberTxt;
            }else
            {
                this.TxtNumber.text = string.Format("<#{0}>{1} </color>", this.color, numberTxt);
            }
        }

        if (this.TxtNumber2 != null)
        {
	        this.TxtNumber2.text = numberTxt;
        }
        //if (callback != null) callback(numberTxt);
    }

    public double GetCurrentValue()
    {
	    return CurrentValue;
    }
	private float waitTime = 1.0f ;
	WaitForSeconds waitSeconds = new WaitForSeconds (1.0f);

    protected double CurrentValue = 0;
    protected double StartValue = 0;
    protected double EndValue = 0;
    protected float IncreaseNumberEverySeconds;
//	private float ChangeDuration = 1f;
}
