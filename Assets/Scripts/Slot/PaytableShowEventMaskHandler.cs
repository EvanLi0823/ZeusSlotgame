using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PaytableShowEventMaskHandler : MonoBehaviour
{
    private Image MaskImg = null;
    private void Awake()
    {
        if (MaskImg == null)
        {
            MaskImg = GetComponent<Image>();
        }
        Messenger.AddListener<bool>(PaytableShowEventHandler.ENABLE_SHOW_SYMBOL_BOARD, EnableLevelMask);
    }

    private void OnDestroy()
    {
        Messenger.RemoveListener<bool>(PaytableShowEventHandler.ENABLE_SHOW_SYMBOL_BOARD, EnableLevelMask);
    }

    private void EnableLevelMask(bool enableMask)
    {
        if (enableMask)
        {
            MaskImg.color = Color.white;
        }
        else
        {
            MaskImg.color = Color.clear;
        }

    }


}
