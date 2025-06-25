using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class ImageSlider : MonoBehaviour
{
    public Image fillImage;
    public float particleBaseOffset; 

    public Transform particleBase;
    public TextMeshProUGUI Text;
    float fillImageWith;

    void Start(){
        if(fillImage != null) fillImageWith = (fillImage.transform as RectTransform).sizeDelta.x;
    }

    Tween mTween;
    float TgtValue = -1;
    public float minDuration = 0.13f;
    public List<GameObject> myParticleObjList = new List<GameObject>();
    public void SetValue(float value)
    {
        if (TgtValue == value) return;
		if (fillImage == null) return;
        TgtValue = value;

        float duration = Mathf.Abs(value - fillImage.fillAmount);
        if (duration < minDuration) duration = minDuration;
        if (TgtValue == 0) duration = 0;

        float TemValue = fillImage.fillAmount;
        if(mTween != null) mTween.Kill();
       
        mTween = DOTween.To(() => fillImage.fillAmount, x => TemValue = x,value, duration).OnUpdate(() =>{
            if(Text != null) SetText(TemValue);
			fillImage.fillAmount = TemValue;
			if (particleBase == null) return;
			particleBase.localPosition = (fillImageWith * TemValue + particleBaseOffset) * Vector3.right;
        }).OnComplete(()=>{
			for(int i = 0;i< myParticleObjList.Count;i++){
				GameObject particleObj = myParticleObjList[i];
				if(particleObj == null) continue;
                particleObj.SetActive(TgtValue != 1);
            }
        });
    }


    void OnDestroy(){
        if(mTween != null) mTween.Kill();
    }

    void SetText(float value){
        if(value <= 0) Text.text = GameConstants.StringFormat_0_Per;
        else if(value < 0.1f) Text.text = value.ToString(GameConstants.StringFormat_0_Per);
        else if(value <= 1) Text.text = value.ToString(GameConstants.StringFormat_00_Per);
        else Text.text = GameConstants.StringFormat_100_Per;
    }
}
