using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class UIChangeNumber : MonoBehaviour {
	public float ShowTime = 3f;
	public bool IsNeedMultiple = false;
    public bool EnableRollingUpAudio = false;
	private TextMeshProUGUI TxtNumber;
	private UnityEngine.UI.Text TxtNumber2;
	private long initNum = 0;
	private Tweener tweener;
//先乘以系数，然后再变化，最终Utils.Utilities.ThousandSeparatorNumber就不需要再相乘了
	void Awake()
	{
        if (TxtNumber==null) TxtNumber = GetComponent<TextMeshProUGUI>();
        if (TxtNumber2==null) TxtNumber2 = GetComponent<UnityEngine.UI.Text>();
	}

	public void SetInitNumber(long _initNumber = 0L)
	{
		if (this.IsNeedMultiple)
		{
			_initNumber *= Core.ApplicationConfig.GetInstance().ShowCoinsMultiplier;
		}
		this.initNum = _initNumber;
		CaculateTxt (initNum);
	}

    public void SetNumber(long number){ 
		this.SetNumber (number, this.ShowTime, this.IsNeedMultiple);
	}
    public void SetNumber(long number,float showtime,bool needMultiple = true)
    {
	    this.IsNeedMultiple = needMultiple;
	    if (this.IsNeedMultiple)
	    {
		    number *= Core.ApplicationConfig.GetInstance().ShowCoinsMultiplier;
	    }
	    
        this.ShowTime = showtime;
        if (tweener != null)
        {
            tweener.Complete();
        }

        if (initNum != number)
        {
			tweener = Utils.Utilities.AnimationTo (initNum,number,ShowTime,CaculateTxt,StartCallBack,EndCallBack).SetUpdate(true);
        }
        else
        {
            this.initNum = number;
			this.CaculateTxt(initNum);
        }
    }
	private void CaculateTxt(long num)
	{ 
		initNum = num;
		if (TxtNumber != null) {
			TxtNumber.text = Utils.Utilities.ThousandSeparatorNumber (num, false); //string.Format("{0:0,0}",initNum);
		} else if(TxtNumber2!=null){
			TxtNumber2.text = Utils.Utilities.ThousandSeparatorNumber (num, false); 
		}
	}
    private void StartCallBack(){
        if (EnableRollingUpAudio)
        {
            Libs.AudioEntity.Instance.PlayRollUpEffect();
        }
    }
    private void EndCallBack()
    {
        if (EnableRollingUpAudio)
        {
            Libs.AudioEntity.Instance.StopRollingUpEffect();
            Libs.AudioEntity.Instance.PlayRollEndEffect();
        }
    }
}
