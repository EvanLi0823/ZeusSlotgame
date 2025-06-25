using System;
using WildWest;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class WildWestGatherItem : MonoBehaviour
{    
    public int index {get; private set;}
    public GameObject Open; 
    public GameObject Close; 
    public GameObject Disable;
    public GameObject LockIn;
    public Button Click;
    public List<Text> pendant;
    
    private Text coin;
    private GameObject coinDouble;
    private GameObject bonus;

    private WildWestGatherPage page;

    void Awake()
    {
        Click.onClick.AddListener(OnClick);
        coin = Open.transform.GetChild(0).GetComponent<Text>();
        coinDouble = Open.transform.GetChild(1).gameObject;
        bonus = Open.transform.GetChild(2).gameObject;
    }

    public void InitItem(WildWestGatherPage _page, int _index)
    {
        page = _page;
        index = _index;
    }

    public void InitItemUI(FreatureInfo.ItemInfo info, bool unlock, bool disable, float pendantcount)
    {
        foreach (var item in pendant) item.text = Utils.Utilities.ThousandSeparatorNumber(pendantcount, false);

        if(info != null){
            if(info.name == "BONUS"){
            bonus.SetActive(true);
            }else if(info.name == "2X"){
                coinDouble.SetActive(true);
            }else if(info.name == "COIN"){
                coin.gameObject.SetActive(true);
                coin.text = this.UICoin((long)(info.coin));
            }
            Open.SetActive(true);
        }else if(!unlock){
            LockIn.SetActive(true);
        }else if(disable){
            Disable.SetActive(true);
        }else{
            Close.SetActive(true);
            Click.interactable = true;
        }
    }

    public void UpdateItemUI(bool disable)
    {
        if(disable)
        {
            Disable.SetActive(true);
            Close.SetActive(false);
        }else
        {
            Click.interactable = true;
        }
    }

    public void InitClickItem(FreatureInfo.ItemInfo info)
    {
        Close.SetActive(false);

        if(info.name == "BONUS"){
            bonus.SetActive(true);
            Libs.AudioEntity.Instance.PlayEffect("award_appear_bonus");
        }else if(info.name == "2X"){
            Libs.AudioEntity.Instance.PlayEffect("award_appear_2x");
            coinDouble.SetActive(true);
        }else if(info.name == "COIN"){
            Libs.AudioEntity.Instance.PlayEffect("award_appear_number");
            coin.gameObject.SetActive(true);
            if (info.indexDouble == -1)
            {
                coin.text = this.UICoin((long)(info.coin));
            }
            else
            {
                coin.text = this.UICoin((long) (info.coin / 2));
            }
        }
        Open.SetActive(true);
        Open.GetComponent<Animator>().SetTrigger("open");
    }

    public void InitDoubleClickItem(FreatureInfo.ItemInfo info)
    {
        coin.text = this.UICoin((long)info.coin);
    }

    public void OnClick()
    {
        page.OnClickItem(index);
    }

    public string UICoin(long _value)
    {
        // if(_value == "") return _value;
        //
        // double value = double.Parse(_value);
        //
        // if(value >= Math.Pow(10, 9)) return this.RetainOne((value/Math.Pow(10, 9)).ToString("#0.00")) + "B";
        //  
        // if(value >= Math.Pow(10, 6)) return this.RetainOne((value/Math.Pow(10, 6)).ToString("#0.00")) + "M";
        //
        // if(value >= Math.Pow(10, 3)) return this.RetainOne((value/Math.Pow(10, 3)).ToString("#0.00")) + "K";

        return Utils.Utilities.GetBigNumberShow(_value);
    }

    public string RetainOne(string _value)
    {
        float value = float.Parse(_value);
        float valueOne = float.Parse(value.ToString("#0"));
        if(Mathf.Approximately(value, valueOne))
        {
            return valueOne.ToString();
        }
        return value.ToString();
    }

    public void SetItemCanClick()
    {
        Disable.SetActive(false);
        Close.SetActive(true);
        Click.interactable = true;
    }
}
