using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class TestBoardItemRender : MonoBehaviour
{
    public Toggle m_Toggle;
    public Image m_Image;

    [HideInInspector]
    public int SymbolId = -2;
    public void SetData(int id, Sprite s)
    {
         SymbolId = id;
         this.m_Image.sprite = s;
         this.m_Image.gameObject.SetActive(true);
    }

    public bool IsSelect
    {
        get => m_Toggle.isOn;
        set => m_Toggle.isOn = value;
    }

    public void Reset()
    {
        m_Toggle.isOn = false;
        SymbolId = -2;
        m_Image.sprite = null;
        this.m_Image.gameObject.SetActive(false);
        m_Toggle.isOn = false;
    }
}
