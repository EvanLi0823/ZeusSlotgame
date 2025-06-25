using WildWest;
using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

public class WildWestGatherPage : MonoBehaviour
{
    private FreatureInfo featureInfo;
    private WildWestReelManager bison;
    private FreatureInfo.ItemInfo info;
    private WildWestGatherDialog dialog;
    private List<WildWestGatherItem> itemUI = new List<WildWestGatherItem>();

    public void InitPage(WildWestReelManager _bison, WildWestGatherDialog _dialog, FreatureInfo _featureInfo)
    {
        bison = _bison;
        dialog = _dialog;
        featureInfo = _featureInfo;

        for (int i = 0; i < this.transform.childCount; i++)
        {
            WildWestGatherItem item = this.transform.GetChild(i).GetComponent<WildWestGatherItem>();
            item.InitItem(this, i);
            itemUI.Add(item);
        }

        foreach (var item in itemUI)
        {
            FreatureInfo.ItemInfo info = this.GetItemUIInfo(item.index);
            item.InitItemUI(info, featureInfo.ISUNLOCK, featureInfo.pendant > bison.spinresult.pendantNum, featureInfo.pendant);
        }

        if(featureInfo.infoIndex >= featureInfo.itemInfo.Count || featureInfo.infoIndex == 0) return;

        if(featureInfo.itemInfo[featureInfo.infoIndex-1].index != -1 && featureInfo.itemInfo[featureInfo.infoIndex-1].name == "2X")
        {
            foreach (var item in itemUI)
            {
                if (!this.IsOpenItem(item.index))
                {
                    item.GetComponent<Animator>().SetTrigger("close");
                    item.SetItemCanClick();
                }
            }
        }
    }

    public FreatureInfo.ItemInfo GetItemUIInfo(int index)
    {
        foreach (var item in featureInfo.itemInfo)
        {
            if(item.index == index) return item;
        }
        return null;
    }

    public void OnClickItem(int index)
    {
        Libs.AudioEntity.Instance.PlayEffect("click_card");
        this.DisabeButton();
        info = featureInfo.GetItemInfo(index);
        if(info.indexDouble == -1) bison.spinresult.SetPendantNum(-featureInfo.pendant);
        itemUI[index].InitClickItem(info);
        dialog.clickEffect.transform.position = itemUI[index].transform.position;
        dialog.clickEffect.SetTrigger("click");
        if(info.name == "2X") dialog.stopEffect.gameObject.SetActive(false);
        if(info.indexDouble != -1) dialog.stopEffect.transform.position = itemUI[index].transform.position;
        dialog.OnClick(this, info, featureInfo);
    }

    public void OnClickFinish()
    {

        if(info.name == "2X")
        {
            Libs.AudioEntity.Instance.PlayEffect("number_disapper");
            foreach (var item in itemUI)
            {
                if(!this.IsOpenItem(item.index))
                {
                    item.GetComponent<Animator>().SetTrigger("close");
                    item.SetItemCanClick();
                    // item.UpdateItemUI(featureInfo.pendant > bison.spinresult.pendantNum);
                } 
            }
            return;
        }

        foreach (var item in itemUI)
        {
            if(!this.IsOpenItem(item.index)) item.UpdateItemUI(featureInfo.pendant > bison.spinresult.pendantNum);
        }
    }

    public void OnDoubleClick()
    {
        Libs.AudioEntity.Instance.PlayEffect("pick_again");
        GameObject moveObject = null;
        if(info.indexDouble != -1)
        {
            moveObject = Instantiate(dialog.moveEffect, this.transform);
            moveObject.transform.localScale = Vector3.one;
            moveObject.transform.position = itemUI[info.indexDouble].transform.position;
        }
        if(moveObject == null) return;

        moveObject.transform.DOMove(itemUI[info.index].transform.position, 0.8f).OnStart(()=>{
            moveObject.SetActive(true);
        }).OnComplete(()=>{
            Libs.AudioEntity.Instance.PlayEffect("award_response");
            Destroy(moveObject);
            dialog.stopEffect.gameObject.SetActive(true);
            itemUI[info.index].InitDoubleClickItem(info);
            foreach (var item in itemUI) if(!this.IsOpenItem(item.index)) item.GetComponent<Animator>().SetTrigger("open");
            dialog.StartSuperFree();
        });
    }

    public bool IsOpenItem(int index)
    {
        foreach (var item in featureInfo.itemInfo)
        {
            if(item.index == index) return true;
        }
        return false;
    }

    public void DisabeButton()
    {
        foreach (var item in itemUI) item.Click.interactable = false;
    }

}