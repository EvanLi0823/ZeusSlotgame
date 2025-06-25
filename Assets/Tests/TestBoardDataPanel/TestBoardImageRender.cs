using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class TestBoardImageRender : MonoBehaviour
{
    private int SymbolId;
    private TestBoardDataPanel DataPanel;
    private Image m_Image;

    private void Awake()
    {
        m_Image = GetComponent<Image>();
    }

    public void ClickBtn()
    {
        DataPanel.ChooseItem(SymbolId,m_Image.sprite);
    }

    public void Init(TestBoardDataPanel _board,int  _SymbolId, Sprite _sprite)
    {
        DataPanel = _board;
        this.SymbolId = _SymbolId;
        this.m_Image.sprite = _sprite;
    }
}
