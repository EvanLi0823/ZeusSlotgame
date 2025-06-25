using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System.Collections.Generic;
public class WildWestBackgroudPanel : BackgroundPanel
{
    [SerializeField]
    private GameObject basegame;
    
    [SerializeField]
    private GameObject freegame;
    [SerializeField]
    private GameObject title_none;
    [SerializeField]
    private GameObject title_all;
    
    [SerializeField]
    private Text count;
    [SerializeField]
    private Animator leftAni;
    
    [SerializeField]
    private Text pendant;
    [SerializeField]
    private Animator pendantAni;
    
    [SerializeField]
    private Animator titleAni;

    [SerializeField]
    private Text symbolcount;

    [SerializeField]
    private Image symbolImage;

    [SerializeField]
    private List<Animator> symbolItem;
    
    [SerializeField]
    private List<Sprite> symbolSprite;


    private long pendantcount;

    private WildWestReelManager bison;

    private int level = 0;

    
    public override void OnEnterFreespin()
    {
        level = 0;
        count.text = "0";
        this.FadeOut(freegame);
        this.FadeIn(basegame);

        title_none.SetActive(true);
        title_all.SetActive(false);

        foreach (var item in symbolItem) item.gameObject.SetActive(false);

        Messenger.Broadcast<bool>(SlotControllerConstants.DisableSpinButton, false);

        this.InitFreeSpin();
        
    }

    public void InitFreeSpin()
    {
        level = bison.spinresult.freeLevel;

        if(level == -1)
        {
            foreach (var item in symbolItem)
            {
                item.gameObject.SetActive(true);
                item.SetTrigger("finish");
            }
            title_none.SetActive(false);
            title_all.SetActive(true);
        }else
        {
            count.text = bison.spinresult.featureItemNum().ToString();
            symbolcount.text = bison.spinresult.GetLeftCount().ToString();
            symbolImage.sprite = symbolSprite[level];
            for (int i = 0; i < level; i++)
            {
                symbolItem[i].gameObject.SetActive(true);
                symbolItem[i].SetTrigger("finish");
            }

            symbolItem[level].gameObject.SetActive(true);
            symbolItem[level].SetTrigger("enter");
        }
    }

    public override void OnQuitFreespin()
    {
        this.FadeOut(basegame);
        this.FadeIn(freegame);
        foreach (var item in symbolItem) item.gameObject.SetActive(false);
    }

    public void InitBisonUI(WildWestReelManager _bison)
    {
        bison = _bison;
        this.TextUI(bison.spinresult.pendantNum);
    }

    public void SetFreeSpinUI()
    {
        count.text = bison.spinresult.featureItemNum().ToString();

        if(level == -1) return;
        
        count.text = bison.spinresult.featureItemNum().ToString();
        leftAni.SetTrigger("open");

        symbolcount.text = bison.spinresult.GetLeftCount().ToString();

        if(level != bison.spinresult.freeLevel)
        {
            titleAni.SetTrigger("open");
            Libs.AudioEntity.Instance.PlayEffect("mission_change");
            symbolItem[level].SetTrigger("over");
            level =  bison.spinresult.freeLevel;

            if(level == -1)
            {
                title_none.SetActive(false);
                title_all.SetActive(true);
                return ;
            }

            symbolImage.sprite = symbolSprite[level];
            symbolItem[level].gameObject.SetActive(true);
            symbolItem[level].SetTrigger("enter");
        }

    }

    public void SetPendantUI(bool ani = true)
    {
        if(ani)
        {
            pendantAni.SetTrigger("open");
            Libs.AudioEntity.Instance.PlayEffect("fly_to_meter_response");
        } 
        this.PendantUI();
    }

    private void PendantUI()
    {
        Utils.Utilities.AnimationTo (this.pendantcount, bison.spinresult.pendantNum, 0.5f, TextUI).Play();
    }

    private void TextUI(long count)
    {
        this.pendantcount = count;
        pendant.text =  Utils.Utilities.ThousandSeparatorNumber(this.pendantcount, false);
    }

    public void SetFreeB04UI(int value)
    {
        leftAni.SetTrigger("open");
        count.text = value.ToString();
    }
    
    private void FadeOut(GameObject fadeout)
    {
        fadeout.SetActive(true);
        Graphic[] graphic = fadeout.GetComponentsInChildren<Graphic>();
        foreach (var gra in graphic)
        {
            gra.color = new Color(1, 1, 1, 0);
            gra.DOFade(1, 0.5f).SetEase(Ease.Linear); 
        }
    }

    private void FadeIn(GameObject fadeIn)
    {
        Graphic[] graphic = fadeIn.GetComponentsInChildren<Graphic>();
        foreach (var gra in graphic)
        {
            gra.color = Color.white;
            gra.DOFade(0, 0.5f).SetEase(Ease.Linear).OnComplete(()=>
            {
                fadeIn.SetActive(false);
            }); 
        }
    }
}
