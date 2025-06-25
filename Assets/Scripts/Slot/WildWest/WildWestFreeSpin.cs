using Classic;
using UnityEngine;
using System.Collections.Generic;
using WildWest;

public class WildWestFreeSpin : FreespinGame
{
    public GameObject pick;
    public GameObject pickmask;
    
    [SerializeField]
    private WildWestReelManager bison;


    private int index = 0;
    private int freetimes = 0;

    private System.Action freestart = null;

    private List<WildWestPickSymbol> pickTable = new List<WildWestPickSymbol>();
    private List<WildWestSymbolRender> pickSymbolRender = new List<WildWestSymbolRender>();

    public bool Ispick = true;

    public override void OnEnterGame(ReelManager reelManager)
    {
        if(bison.spinresult.freeType == BisonFree.SUPER || bison.spinresult.freeType == BisonFree.BONUS)
        {
            reelManager.IsCollectGame = true;
        }
        base.OnEnterGame(reelManager);
    }
    
    public override void OnQuitGame(ReelManager reelManager)
    {

        if(bison.spinresult.freeType != BisonFree.SPIN)
        {
            Messenger.Broadcast<bool>(SlotControllerConstants.OnAverageBet, false);
            //BaseSlotMachineController.Instance.currentBettingTemp = BaseSlotMachineController.Instance.currentBetting;
        } 

        if(bison.spinresult.freeType == BisonFree.SUPER && bison.nextIndex != 0)
        {
            int index = bison.nextIndex;
            bison.nextIndex = 0;
            this.WildWestUnlockedDialog(index);
        } 
         
        base.OnQuitGame(reelManager);
    }

    private void WildWestUnlockedDialog(int index)
    {        
        Libs.UIManager.Instance.OpenMachineDialog<WildWestUnlockedDialog>( (dialog)=>
        {
            dialog.OnStart(index);
        }, ()=>
        {
            bison.WildWestGatherDialog(true);
        }, inAnimation:Libs.UIAnimation.Scale, outAnimation:Libs.UIAnimation.Scale, maskAlpha:0.8f);
    }

    public override void ChangeDataOnQuitGame(ReelManager reelManager)
    {
        bison.spinresult.ReSetFreeSpinData();
        base.ChangeDataOnQuitGame(reelManager);
    }

    public override void showFirstWinFreespin (int times, System.Action callback)
    {
        if(bison.spinresult.freeType == BisonFree.BONUS)
        {
            SpinResultProduce.AddProvider(new SpinResultFreeTrigger(TriggerFeatureType.Pick,times,null,false,false));
            this.ShowFreeBonusDialog(times, callback);
            return;
        }

        if(bison.spinresult.freeType == BisonFree.SUPER)
        {
            SpinResultProduce.AddProvider(new SpinResultFreeTrigger(TriggerFeatureType.Pick,times,null,false,true));
            this.ShowSuperFreeDialog(times, callback);
            return;
        }
        
        freetimes = times;
        freestart = ()=>
        {
            callback();
            Ispick = true;
            bison.SetFreePendant();
        };

        if(!Ispick)
        {
            base.showFirstWinFreespin(times, freestart);
            return;
        } 

        this.InitFreeSpinPanel(0);
    }

    public override void showNextWinFreespin (int times, System.Action callback)
    {

        freetimes = times;
        freestart = ()=>
        {
            callback();
            Ispick = true;
            bison.SetFreePendant();
        };

        if(!Ispick)
        {
            base.showNextWinFreespin(times, freestart);
            return;
        } 

        this.InitFreeSpinPanel(1);
    }

    public void ShowFreeBonusDialog (int times, System.Action callback)
    {
        PlayEnterAudio ();
        Libs.DelayAction delayAction = new Libs.DelayAction (1f, null, BaseSlotMachineController.Instance.StopAllAnimation);
        delayAction.Play ();
        if (BaseGameConsole.singletonInstance.IsInLobby ()) return;
		Libs.UIManager.Instance.OpenMachineDialog<WildWestFreeBonusStartDialog> ((dialog)=>{
			dialog.OnStart(times, bison.spinresult.featuremul);
		}, callback, inAnimation:Libs.UIAnimation.Scale, outAnimation:Libs.UIAnimation.Scale, maskAlpha:0.8f);
    }

    public void ShowSuperFreeDialog (int times, System.Action callback)
    {
        PlayEnterAudio ();
        Libs.DelayAction delayAction = new Libs.DelayAction (1f, null, BaseSlotMachineController.Instance.StopAllAnimation);
        delayAction.Play ();
        if (BaseGameConsole.singletonInstance.IsInLobby ()) return;
		Libs.UIManager.Instance.OpenMachineDialog<WildWestSuperFreeStartDialog> ((dialog)=>{
			dialog.OnStart(bison.gatherIndex);
		}, callback, inAnimation:Libs.UIAnimation.Scale, outAnimation:Libs.UIAnimation.Scale, maskAlpha:0.8f);
    }
    
    protected override void ShowWinDialogHandler(float delayTime,long winCoins,System.Action callback,int cash)
    {
        bison.spinresult.freespinPendant = bison.spinresult.infreePendant;
        base.ShowWinDialogHandler(delayTime, winCoins, callback,cash);
    }
    
    public void InitFreeSpinPanel(int value)
    {
        this.pickmask.SetActive(true);
        this.pickTable.Clear();
        BaseSlotMachineController.Instance.StopAllAnimation();
        List<int[]> table = new List<int[]>(bison.spinresult.freespinTable.Keys);
        foreach (var item in table)
        {
            GameObject freepick = Instantiate(pick, this.pickmask.transform.parent);
            WildWestSymbolRender render =  bison.GetSymbolRender(item[0], item[1]) as WildWestSymbolRender;
            render.gameObject.SetActive(false);
            pickSymbolRender.Add(render);
            freepick.transform.localScale = Vector3.one;
            freepick.transform.position = render.transform.position;
            WildWestPickSymbol pickItem = freepick.GetComponent<WildWestPickSymbol>();
            pickTable.Add(pickItem);
            pickItem.InitData(render.m_SymbolSprite.sprite, item[1]>1?1:0,  bison.spinresult.freespinTable[item], table.IndexOf(item), value, this);
        }
    }

    public void PickInitFreeSpin(WildWestPickSymbol item, int count, int _index)
    {
        Libs.AudioEntity.Instance.PlayEffect("pick_symbol");
        Ispick = false;
        bison.spinresult.freespinPendant = count;
        index = _index;
        foreach (var btn in pickTable) btn.pickBtn.interactable = false;
    }

    public void PandentAddEnd()
    {
        for (int i = 0; i < pickTable.Count; i++)
        {
            if(i != index)
            {
                pickTable[i].animator.SetTrigger("click");
            }
        }
    }

    public void StartFreeSpin(int value)
    {
        foreach (var item in pickSymbolRender) item.gameObject.SetActive(true);

        for (int i = pickTable.Count-1; i >= 0; i--)
        {
            Destroy(pickTable[i].gameObject);
            pickTable.RemoveAt(i);
        }

        pickmask.SetActive(false);

        if(value == 0) base.showFirstWinFreespin(freetimes, freestart);
        if(value == 1) base.showNextWinFreespin(freetimes, freestart);
    }


    public override void PlayWinFreeSpinAudio()
    {
        if(bison.spinresult.freeType != BisonFree.SPIN && !bison.isFreespinBonus) return;
        Libs.AudioEntity.Instance.PlayBonusTriggerEffect ();
    }
}
