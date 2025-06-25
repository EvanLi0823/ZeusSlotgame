using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent (typeof(AutoChangeFontPos))]
public class AutoFitCustomFontTextPro : Text
{
    private RectTransform _rect;
    private float WidthScope;
    private float HeightScope;
    float widthScale = 1f;
    float heightScale = 1f;
    float useScale = 1f;
    private float _oRectScale;

    private AutoChangeFontPos _changeFontPos;

    private float _proportion = 1f;
    private void Awake()
    {
        _rect = transform as RectTransform;
        _changeFontPos = GetComponent<AutoChangeFontPos>();
        _oRectScale = _changeFontPos._scale;
        _proportion = (1f / _oRectScale);
    }
    
    

    protected override void OnEnable (){
        base.OnEnable ();//必须调用父类方法，否则无法添加默认材质
        _rect = this.transform as RectTransform;
        alignByGeometry = true;
        horizontalOverflow = HorizontalWrapMode.Overflow;
        verticalOverflow = VerticalWrapMode.Overflow;
        //alignment = TextAnchor.MiddleCenter;
        supportRichText = false;
    }
			
    protected override void UpdateGeometry(){
        AdjustTextComponent();
        base.UpdateGeometry ();
        _changeFontPos.ChangeFontSize();
    }

    void AdjustTextComponent(){
        WidthScope = _rect.sizeDelta.x;
        HeightScope = _rect.sizeDelta.y;
        if (Mathf.Approximately(WidthScope,0f)||Mathf.Approximately(HeightScope,0f))
        {
            WidthScope = _rect.rect.width;
            HeightScope = _rect.rect.height;
        }
        widthScale = _oRectScale;
        heightScale = _oRectScale;
        useScale = 1f;
        if (preferredWidth > WidthScope)
        {
            widthScale = WidthScope / (preferredWidth*_proportion);
        }

        if (preferredHeight > HeightScope) {
            heightScale = HeightScope / preferredHeight;
        }

        useScale = Mathf.Min (widthScale, heightScale);
        _rect.localScale = new Vector3 (useScale,useScale,1f);
    }
}