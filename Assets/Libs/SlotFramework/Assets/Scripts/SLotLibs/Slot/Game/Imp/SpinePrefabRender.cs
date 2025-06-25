using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SpinePrefabRender : SpineSymbolRender
{
    protected GameObject PrefabAniObj;
    public override void PlayAnimation(int animationId, bool isLoop = true, Action VideoInitHandler = null, Action VideoCompleteHandler = null, float RepeatPlayStartTime = 0, bool isUseCache = true)
    {

        GameObject go = this.GetGameConfig().GetAnimation(symbolIndex);
        if (go != null && PrefabAniObj == null)
        {
            PrefabAniObj = Instantiate(go) as GameObject;
            PrefabAniObj.transform.SetParent(animationParent, false);
            PrefabAniObj.transform.SetSiblingIndex(1);
            Util.SetChildrenLayer(this.PrefabAniObj.transform, LayerMask.NameToLayer("UnVisible"));
        }

        base.PlayAnimation(animationId, isLoop, delegate {
            if (PrefabAniObj != null && PrefabAniObj.GetComponent<Animator>() != null)
            {
                PrefabAniObj.GetComponent<Animator>().SetInteger("state", animationId);
                PrefabAniObj.transform.SetAsLastSibling();
                Util.SetChildrenLayer(this.PrefabAniObj.transform, LayerMask.NameToLayer("UI"));
            }
        }, VideoCompleteHandler, RepeatPlayStartTime, isUseCache);

    }

    public void PlayPrefabAnimator(int id)
    {
        if (PrefabAniObj != null && PrefabAniObj.GetComponent<Animator>() != null)
        {
            PrefabAniObj.GetComponent<Animator>().SetInteger("state", id);
        }
    }

    public override void PauseAnimation(bool notChange = false)
    {
        base.PauseAnimation(notChange);
        if(PrefabAniObj !=null)
        {
            Destroy(PrefabAniObj);
        }
        PrefabAniObj = null;
    }

    public override void StopAnimation(bool showAnimationFrame = true)
    {
        base.StopAnimation(showAnimationFrame);
        if (PrefabAniObj != null)
        {
            Destroy(PrefabAniObj);
        }
        PrefabAniObj = null;
    }
}
