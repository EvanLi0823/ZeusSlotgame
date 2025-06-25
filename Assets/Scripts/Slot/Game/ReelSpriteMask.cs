using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Classic;
public class ReelSpriteMask : MonoBehaviour
{
    private SpriteMask mask;
    private PaytableShowEventHandler eventHandler;
    private RectTransform mTrans;
    void Awake()
    {
        if (mask == null)
        {
            mask = GetComponent<SpriteMask>();
        }
        mTrans = transform as RectTransform;
        if (eventHandler == null)
        {
            eventHandler = GetComponent<PaytableShowEventHandler>();
        }
        Messenger.AddListener(GameConstants.OnSlotMachineSceneInit, InitReels);
        Messenger.AddListener<ReelManager>(GameConstants.ChangeGameConfigsMsg, ResetReels);
    }
    void OnDestroy()
    {
        Messenger.RemoveListener(GameConstants.OnSlotMachineSceneInit, InitReels);
        Messenger.RemoveListener<ReelManager>(GameConstants.ChangeGameConfigsMsg, ResetReels);
    }
    void InitReels()
    {
        if (mask != null)
        {
            mask.size = new Vector2(mTrans.rect.width, mTrans.rect.height);
            if (eventHandler != null) eventHandler.maskSize = mask.size;
        }
    }
    void ResetReels(ReelManager reelManager)
    {
        InitReels();
    }
}
