using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WesternTreasureJackpotGame : MonoBehaviour
{
    private List<Image> selectImageList = new List<Image>();
    private List<Image> noSelectImageList = new List<Image>();
    private List<Button> coinBtnList = new List<Button>();
    private List<WesternTreasureCoinItem> coinItemList = new List<WesternTreasureCoinItem>();
    private WesternTreasureReelManager westernTreasureReelManager;
    private GameObject GrandEffect;
    private GameObject MajorEffect;
    private GameObject MinorEffect;
    private GameObject MiniEffect;
    public double money;
    public SpinResultMachineJackpot machineJackpot=new SpinResultMachineJackpot();
    void Awake()
    {
        InitEffect();
        InitImage();
        InitCoinItem();
        InitButtonClick();
    }

    private void InitEffect()
    {
        MiniEffect = Util.FindObject<GameObject>(transform.parent, "JackPot/BG_Jackpot/Mini");
        MinorEffect = Util.FindObject<GameObject>(transform.parent, "JackPot/BG_Jackpot/Minor");
        MajorEffect = Util.FindObject<GameObject>(transform.parent, "JackPot/BG_Jackpot/Major");
        GrandEffect = Util.FindObject<GameObject>(transform.parent, "JackPot/BG_Jackpot/Grand");
    }

    private void InitImage()
    {
        selectImageList.Clear();
        noSelectImageList.Clear();
        for (int i = 2; i < 6; i++)
        {
            Image select = Util.FindObject<Image>(transform, "ImagePool/select_"+i.ToString());
            Image noselect = Util.FindObject<Image>(transform, "ImagePool/noSelect_" + i.ToString());
            selectImageList.Add(select);
            noSelectImageList.Add(noselect);
        }

    }
    private void InitCoinItem()
    {
        coinItemList.Clear();
        for (int i = 1; i <= 12; i++)
        {
            WesternTreasureCoinItem coinItem = Util.FindObject<WesternTreasureCoinItem>(transform, "coin" + i.ToString("00"));
            coinItem.Init();
            coinItemList.Add(coinItem);
        }
    }
    private void InitButtonClick()
    {
        coinBtnList.Clear();
        for (int i = 0; i < 12; i++)
        {
            Button coinButton = Util.FindObject<Button>(transform, "coin" + (i+1).ToString("00")+ "/coin");
            int uiIndex = int.Parse(coinButton.transform.parent.name.Substring(4,2))-1;
            coinButton.onClick.AddListener(delegate () { this.Choose(uiIndex,coinButton,1); });
            coinBtnList.Add(coinButton);
        }
       
    }
    public void InitJackpotGame(WesternTreasureReelManager manager)
    {
        westernTreasureReelManager = manager;
    }
    public void EnterGame()
    {
        machineJackpot=new SpinResultMachineJackpot();
        AudioManager.Instance.StopAllAudio();
        AudioManager.Instance.AsyncPlayEffectAudio("loading");
        AudioManager.Instance.AsyncPlayMusicAudio("Jackpot");
        RefreshUI();
        
    }
    private void RefreshUI()
    {
        List<int> clickIndex = westernTreasureReelManager.spinResult.clickDataList;
        for (int i = 0; i < clickIndex.Count; i++)
        {
            if (clickIndex[i]!=-1)
            {
                westernTreasureReelManager.spinResult.clickIndexList[clickIndex[i]] = false;
                Choose(clickIndex[i], coinBtnList[clickIndex[i]], 1);
            }
        }
    }


    public void Choose(int index,Button button,int animationId)
    {
        //Debug.LogError("   ----------  Choose      "+index);
        if (animationId!=2)
        {
            AudioManager.Instance.AsyncPlayEffectAudio("Bonus_pick");
        }

        int clickIndex = westernTreasureReelManager.spinResult.clickIndex;
      
        if (clickIndex>=12) {
            return;
        }
        int treeItem = westernTreasureReelManager.spinResult.jackpotList[clickIndex];
        //Debug.LogError(index + "  --------  [" + animationId + "]   --------     " + westernTreasureReelManager.spinResult.jackpotList[clickIndex].isClick);
        if (coinItemList.Count!=0 && !westernTreasureReelManager.spinResult.clickIndexList[index]) {
            button.enabled = false;
            if (animationId == 1)
            {
                westernTreasureReelManager.spinResult.clickIndexList[index] = true;
            }
            if (westernTreasureReelManager.spinResult.clickDataList.IndexOf(index)<0 && animationId==1)
            {
                westernTreasureReelManager.spinResult.clickDataList.Add(index);
               
            }
            westernTreasureReelManager.spinResult.clickIndex++;
            List<Image> selectImg = animationId == 1 ? selectImageList : noSelectImageList;
            coinItemList[index].Click(treeItem, selectImg, animationId);
        }
       
        //Debug.LogError(index + "  --------  [" + animationId + "]   ---#########-----     " + westernTreasureReelManager.spinResult.jackpotList[clickIndex].isClick);
        if (westernTreasureReelManager.spinResult.isPickOver()) {
            //Debug.LogError("   ---------------------------------  OverGame      ");
            SetAllButtonState(false);
            StartCoroutine(OverGame());
        }
    }

    private void PlayJackpotEffect(int winType)
    {
        GrandEffect.SetActive(winType==2);
        MajorEffect.SetActive(winType==3);
        MinorEffect.SetActive(winType==4);
        MiniEffect.SetActive(winType==5);
    }

    public void ClearGame()
    {
        for (int i = 0; i < coinItemList.Count; i++)
        {
            //coinItemList[i].PlayAnimation(0);
            coinItemList[i].ClearEffect();
        }
        SetAllButtonState(true);
        westernTreasureReelManager.spinResult.clickIndex = 0;
    }
    private void SetAllButtonState(bool canClick)
    {
        for (int i = 0; i < coinBtnList.Count; i++)
        {
            coinBtnList[i].enabled = canClick;
        }

    }

    private List<string> jackpotStrList = new List<string>();
    private IEnumerator OverGame()
    {
        //Debug.LogError("---------------结束pickgame游戏");
        yield return new WaitForSeconds(1f);
        this.ShowOtherCoin(); 
        yield return new WaitForSeconds(1f);
       
        AudioManager.Instance.AsyncPlayEffectAudio("Bonus_win");
        int winType = westernTreasureReelManager.spinResult.winType;
        for (int i = 0; i < coinItemList.Count; i++)
        {
            coinItemList[i].PlayWinEffect(winType);
        }
        PlayJackpotEffect(winType);
        yield return new WaitForSeconds(1.5f);
        westernTreasureReelManager.JackPotIncreaseValue += westernTreasureReelManager.jackPotData.GetJackPotIncreaseValue(winType);
        money=westernTreasureReelManager.jackPotData.GetJackPotAward(winType);
        westernTreasureReelManager.jackPotData.UpdataUI(true);
        westernTreasureReelManager.spinResult.winMoney=money;
        westernTreasureReelManager.spinResult.isOpenEndDialog = true;
        westernTreasureReelManager.OpenJackpotEndDialog(winType,money);
        machineJackpot.AddJackpotItem(new JackpotItem(money,5-winType,TriggerFeatureType.Symbol));
        if(machineJackpot?.GetJackpotCount()>0)
            SpinResultProduce.AddProvider(machineJackpot);
        jackpotStrList.Clear();
        for (int i = 0; i < westernTreasureReelManager.spinResult.jackpotList.Count; i++)
        {
            jackpotStrList.Add(westernTreasureReelManager.spinResult.jackpotList[i].ToString());
        }
        SpinResultProduce.AddProvider(new SpinResultPick(jackpotStrList, money));
        SpinResultProduce.InternalSend();
        
        yield return new WaitForSeconds(1f);
        ClearGame();
        PlayJackpotEffect(0);
    }
    private void ShowOtherCoin()
    {
        
        for (int i = 0; i < coinBtnList.Count; i++)
        {
            if(!westernTreasureReelManager.spinResult.clickIndexList[i])
                this.Choose(i, coinBtnList[i], 2);
        }
    }
    
}
