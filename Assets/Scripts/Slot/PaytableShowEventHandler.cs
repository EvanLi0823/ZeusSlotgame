using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaytableShowEventHandler : MonoBehaviour
{
    public const string ENABLE_SHOW_SYMBOL_BOARD = "EnableShowSymbolBoard";
    private SpriteMask spriteMask= null;
    [HideInInspector]
    public Vector2 maskSize = Vector2.zero;
    private void Awake() {
        if (spriteMask==null)
        {
            spriteMask = GetComponent<SpriteMask>();
            maskSize = spriteMask.size;
        }
        Messenger.AddListener<bool>(ENABLE_SHOW_SYMBOL_BOARD, EnableBoardSpriteMask);
    }

    private void OnDestroy()
    {
        Messenger.RemoveListener<bool>(ENABLE_SHOW_SYMBOL_BOARD, EnableBoardSpriteMask);
    }

    private void EnableBoardSpriteMask(bool enableMask) {
        if (enableMask)
        {
            spriteMask.size = maskSize;
        }
        else
        {
            spriteMask.size = Vector2.zero;
        }

    }

}
