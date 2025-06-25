using Libs;
using Classic;
using WildWest;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System.Collections.Generic;


public class WildWestGatherDialog : UIDialog
{
    public Transform symbolInfo;
    public List<WildWestGatherPage> page;
    public List<GameObject> pageInfo;

    public Button LeftBtn;
    public Button RightBtn;
    public Button CloseBtn;
    public Animator clickEffect;
    private ClickEvent Event;

    public GameObject moveEffect;
    public GameObject stopEffect;

    public Text pendant;

    private int index;
    private string clickName = "";
    private WildWestReelManager WildWest;
    private WildWestGatherPage onPage;
    private bool IsStartSuper = false;

    private const float distance = 616f;
    
    public void OnStart(WildWestReelManager _WildWest, int _index, bool unlock = false)
    {
        Libs.AudioEntity.Instance.PlayMuisc("click_collection", false);
        
        WildWest = _WildWest;
        index = _index;

        Event = clickEffect.GetComponent<ClickEvent>();
        Event.InitEvent(this);

        foreach (var item in page) item.gameObject.SetActive(true);

        foreach (var item in WildWest.featureTable)
        {
            int index = WildWest.featureTable.IndexOf(item);
            page[index].InitPage(WildWest, this, item);
        }

        pendant.text = Utils.Utilities.ThousandSeparatorNumber(WildWest.spinresult.pendantNum, false);

        this.SetPageNumber();


        if(unlock)
        {
            new Libs.DelayAction(2, null, ()=>
            {
                OnClickRight();
                LeftBtn.onClick.AddListener(OnClickLeft);
                RightBtn.onClick.AddListener(OnClickRight);
                UGUIEventListener.Get(this.CloseBtn.gameObject).onClick = this.OnButtonClickHandler;
            }).Play();
        }else
        {
            LeftBtn.onClick.AddListener(OnClickLeft);
            RightBtn.onClick.AddListener(OnClickRight);
            UGUIEventListener.Get(this.CloseBtn.gameObject).onClick = this.OnButtonClickHandler;
        }
    }

    public void OnClick(WildWestGatherPage _onPage, FreatureInfo.ItemInfo info, FreatureInfo featureInfo)
    {
        SpinResultProduce.AddProvider(new SpinResultBonusTrigger(TriggerFeatureType.Collect,TriggerFeatureType.Collect,SpinResultProduce.InternalGetSymbolsList()));
        onPage = _onPage;

        LeftBtn.interactable = false;
        RightBtn.interactable = false;

        pendant.text = Utils.Utilities.ThousandSeparatorNumber(WildWest.spinresult.pendantNum, false);
        WildWest.bisonUI.SetPendantUI(false);

        clickName = "";

        if(info.name == "COIN")
        {
            clickName = "pick";
            SpinResultProduce.AddProvider(new SpinResultMiniGame(info.coin));
            UserManager.GetInstance ().IncreaseBalanceAndSendMessage((long)info.coin);
            if (BaseGameConsole.ActiveGameConsole().IsInSlotMachine() && info.coin > 0)
            {
                BaseSlotMachineController slot = BaseGameConsole.ActiveGameConsole().SlotMachineController;
                slot.statisticsManager.WinCoinsNum += info.coin;
            }
            if(info.indexDouble != -1) clickName = "double";
        }else if(info.name == "2X")
        {
            clickName = "pick";
        }else if(info.name == "BONUS")
        {
            clickName = "bonus";
            WildWest.InitFreeSpinData(info.name, featureInfo);
        } 
        
        if(featureInfo.ISACHIEVE)
        {
            IsStartSuper = true;
            WildWest.InitFreeSpinData("SUPERFREE", featureInfo);
        }

        SpinResultProduce.InternalSend();
    }

    public void ClickFinish()
    {
        if(IsStartSuper && clickName != "double")
        {
            this.CloseDialog();
            WildWest.gatherIndex = index;
            IsStartSuper = false;
            return;
        }

        if(clickName == "pick")
        {
            LeftBtn.interactable = true;
            RightBtn.interactable = true;
            onPage.OnClickFinish();
        }
        if(clickName == "double") onPage.OnDoubleClick();
        if(clickName == "bonus")
        {
            WildWest.gatherIndex = index;
            this.CloseDialog();
        } 
    }

    public void CloseDialog()
    {
        LeftBtn.interactable = false;
        RightBtn.interactable = false;
        new Libs.DelayAction(1f, null, ()=>{
            this.Close();
        }).Play();
    }

    public void StartSuperFree()
    {
        if(IsStartSuper)
        {
            this.CloseDialog();
            WildWest.gatherIndex = index;
            IsStartSuper = false;
        }else{
            LeftBtn.interactable = true;
            RightBtn.interactable = true;
            onPage.OnClickFinish();
        }
    }

    private void SetPageNumber()
    {
        if(index == 0) LeftBtn.gameObject.SetActive(false);
        if(index == 4) RightBtn.gameObject.SetActive(false);
        symbolInfo.localPosition = new Vector3(-index * distance,0,0);
        page[index].gameObject.SetActive(true);
        pageInfo[index].gameObject.SetActive(true);
        page[index].GetComponent<Animator>().SetTrigger("open");
    }

    public override void OnButtonClickHandler(GameObject go)
    {
        base.OnButtonClickHandler (go);
        CloseBtn.interactable = false;
        WildWest.gatherIndex = index;
        this.Close();
        Libs.AudioEntity.Instance.PlayEffect("click_button_close");
    }

    public void OnClickLeft()
    {
        if(index == 0) return;
        Libs.AudioEntity.Instance.PlayEffect("click_button_left_right");
        LeftBtn.interactable = false;
        RightBtn.interactable = false;
        page[index-1].GetComponent<Animator>().SetTrigger("open");
        symbolInfo.DOLocalMoveX(symbolInfo.localPosition.x+distance, 0.3f).SetEase((Ease)Ease.InOutCirc).OnComplete(()=>
        {
            LeftBtn.interactable = true;
            RightBtn.interactable = true;
            index--;
            foreach (var item in pageInfo) item.SetActive(false);
            pageInfo[index].SetActive(true);
            if(index == 0) LeftBtn.gameObject.SetActive(false);
            if(index == 3) RightBtn.gameObject.SetActive(true);
        });
    }

    public void OnClickRight()
    {
        if(index == 4) return;
        Libs.AudioEntity.Instance.PlayEffect("click_button_left_right");
        LeftBtn.interactable = false;
        RightBtn.interactable = false;
        page[index+1].GetComponent<Animator>().SetTrigger("open");
        symbolInfo.DOLocalMoveX(symbolInfo.localPosition.x-distance, 0.3f).SetEase((Ease)Ease.InOutCirc).OnComplete(()=>
        {
            LeftBtn.interactable = true;
            RightBtn.interactable = true;
            index++;
            foreach (var item in pageInfo) item.SetActive(false);
            pageInfo[index].SetActive(true);
            if(index == 1) LeftBtn.gameObject.SetActive(true);
            if(index == 4) RightBtn.gameObject.SetActive(false);
        });
    }

}
