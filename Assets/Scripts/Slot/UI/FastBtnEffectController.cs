using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Classic;
public class FastBtnEffectController : MonoBehaviour
{

    private Image fastImage;
    [SerializeField]
    private Sprite fastBtnSprite;
    [SerializeField]
    private Sprite fastBtnClickedSprite;
    [SerializeField]
    private Sprite fastBtnDisableSprite;
    [SerializeField]
    private Sprite normalBtnSprite;
    [SerializeField]
    private Sprite normalBtnClickedSprite;
    [SerializeField]
    private Sprite normalBtnDisableSprite;

    //private EventTrigger fastSpinBtnEvent;
    private Button fastBtn;
    private void Awake()
    {
        fastImage = GetComponent<Image>();
        fastBtn = GetComponent<Button>();
        Messenger.AddListener(GameConstants.OnSlotMachineSceneInit, SetFastBtnState);
        Messenger.AddListener(GameConstants.PressedFastButton,SetFastBtnState);
        //fastSpinBtnEvent = gameObject.AddComponent<EventTrigger>();
        //EventTrigger.Entry entry = new EventTrigger.Entry();
        //entry.eventID = EventTriggerType.PointerUp;
        //entry.callback.AddListener((data)=> {
        //    SetFastBtnState();});
        //fastSpinBtnEvent.triggers.Add(entry);
    }
  

    private void OnDestroy()
    {
        Messenger.RemoveListener(GameConstants.OnSlotMachineSceneInit, SetFastBtnState);
        Messenger.RemoveListener(GameConstants.PressedFastButton, SetFastBtnState);
    }
    public void SetFastBtnState()
    {
        if (BaseSlotMachineController.Instance == null || BaseSlotMachineController.Instance.reelManager == null) return;
        ReelManager reelManager = BaseSlotMachineController.Instance.reelManager;
        if (reelManager.GetFastMode())
        {
            fastImage.sprite = fastBtnSprite;
            SpriteState spriteState = new SpriteState();
//            spriteState = fastBtn.spriteState;
            spriteState.pressedSprite = fastBtnClickedSprite;
            spriteState.highlightedSprite = fastBtnSprite;
            spriteState.disabledSprite = fastBtnDisableSprite;
            fastBtn.spriteState = spriteState;
        }
        else
        {
            fastImage.sprite = normalBtnSprite;
            SpriteState spriteState = new SpriteState();
//            spriteState = fastBtn.spriteState;
            spriteState.pressedSprite = normalBtnClickedSprite;
            spriteState.highlightedSprite = normalBtnSprite;
            spriteState.disabledSprite = normalBtnDisableSprite;
            fastBtn.spriteState = spriteState;
        }

    }
}
