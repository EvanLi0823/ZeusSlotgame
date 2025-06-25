using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AutoChangeFontPos : MonoBehaviour
{
    [Header("限制文字个数的阈值")][Tooltip("如果条件(大于或者小于)达到这个值就会进行文字的偏移")]
    public int _maxFontLength = 0;
    [Header("文字的初始大小")][Tooltip("控制Scale")]
    public float _scale;
    [Header("文字的偏移量")][Tooltip("控制Transform的x,y偏移量")]
    public Vector2 _changeFontPos = new Vector2();
    [Header("是否反转条件(默认条件是文字个数大于MaxFontLengt)")][Tooltip("条件只有大于和小于没有等于")]
    public bool isChangeContation = false;
    
    private Text _text;

    private int _targetLength;

    private float _oPosX;
    private float _oPosY;

    private float _hPos;
    private float _vPos;

    private RectTransform _rect;

    private bool isChanged = false;
    private void Awake()
    {
        _rect = transform as RectTransform;
        _text = GetComponent<Text>();
        _targetLength = _maxFontLength;
        _oPosX = _rect.anchoredPosition.x;
        _oPosY = _rect.anchoredPosition.y;
        
        _hPos = _oPosX;
        _vPos = _oPosY;
    }

    private void Start()
    {
        InitSelfPosition();
    }

    public void ChangeFontSize()
    {
        if(CheckSelf()) ChangeSelf();
        else ReSetSelf();
    }

    void ReSetSelf()
    {
        if(!isChanged) return;
        _hPos = _oPosX;
        _vPos = _oPosY;
        _targetLength = _maxFontLength;
        _rect.anchoredPosition = new Vector2(_hPos,_vPos);
    }
    bool CheckSelf()
    {
        if (_text == null) return false;
        if (ContationCheck(isChangeContation)) return true;
        else return false;
    }

    void ChangeSelf()
    {
        isChanged = true;
        if (_text.text.Length > _targetLength)// && _text.text.Length - _targetLength>3)
        {
            _hPos += _changeFontPos.x;
            _vPos += _changeFontPos.y;
        }else if (_text.text.Length < _targetLength)// && _targetLength-_text.text.Length>3)
        {
            _hPos -= _changeFontPos.x;
            _vPos -= _changeFontPos.y;
        }
        _targetLength = _text.text.Length;
        _rect.anchoredPosition = new Vector2(_hPos,_vPos);
    }

    void InitSelfPosition()
    {
        int changeNum = 0;
        //if (isChangeContation) changeNum = -changeNum;
        if (!isChangeContation && _text.text.Length > _targetLength)// && _text.text.Length - _targetLength>3)
        {
            changeNum = _text.text.Length - _maxFontLength;
                        _hPos += _changeFontPos.x * changeNum;
            _vPos += _changeFontPos.y  * changeNum;
        }else if (isChangeContation && _text.text.Length < _targetLength)// && _targetLength-_text.text.Length>3)
        {
            changeNum = _maxFontLength -_text.text.Length;
            _hPos -= _changeFontPos.x  * changeNum;
            _vPos -= _changeFontPos.y  * changeNum;
        }
        _targetLength = _text.text.Length;
        _rect.anchoredPosition = new Vector2(_hPos,_vPos);
    }
    
    bool ContationCheck(bool canChange)
    {
        if (_text == null)
        {
            Debug.LogError("Auto Change Font Pos Has Exception!");
            return false;
        }
        if (canChange) return _text.text.Length < _maxFontLength;
        else return _text.text.Length > _maxFontLength;
    }
}
