using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardEffects : MonoBehaviour
{
    public GameObject bigwinAnimation;
   void Awake(){
        Messenger.AddListener<bool>(SlotControllerConstants.PLAY_OUTER_BORDER_BIG_WIN_ANIMATION, PlayOuterBorderBigWinAnimation);
    }

    void OnDestroy(){
        Messenger.RemoveListener<bool>(SlotControllerConstants.PLAY_OUTER_BORDER_BIG_WIN_ANIMATION, PlayOuterBorderBigWinAnimation);
    }

    void PlayOuterBorderBigWinAnimation(bool enabled){
        if (bigwinAnimation!=null) {
            bigwinAnimation.SetActive(enabled);
        }
    }
}
