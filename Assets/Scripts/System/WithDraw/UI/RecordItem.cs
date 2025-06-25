
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class RecordItem: MonoBehaviour
{
    private TextMeshProUGUI cashTMP;
    private TextMeshProUGUI progressTMP;
    private Image progressBar;
    public RecordItemData itemData;
    private Image paltformImg;

    private void Awake()
    {
        cashTMP = Utils.Utilities.RealFindObj<TextMeshProUGUI>(transform, "cashTMP");
        progressTMP = Utils.Utilities.RealFindObj<TextMeshProUGUI>(transform, "inProgress/bottom/progressTMP");
        progressBar = Utils.Utilities.RealFindObj<Image>(transform, "inProgress/bottom/checking/progressBar");
        paltformImg = Utils.Utilities.RealFindObj<Image>(transform, "platformIMG");
    }

    public void Refresh()
    {
        cashTMP.text = OnLineEarningMgr.Instance.GetMoneyStr(itemData.cash,needIcon:false);
        progressTMP.text = $"{itemData.curProgress}/{itemData.targetProgress}";
        if (progressBar != null)
        {
            if (itemData.targetProgress > 0)
            {
                progressBar.fillAmount = (float)itemData.curProgress / itemData.targetProgress;
            }
            else
            {
                progressBar.fillAmount = 0;
            }
        }
        Sprite sp = Resources.Load<Sprite>("Platform/"+itemData.platSpIndex);
        if (sp != null)
        {
            paltformImg.sprite = sp;
        }
    }
    
    public void UpdateData(int i, RecordItemData data)
    {
        itemData = data;
        Refresh();
    }
    
    public void OnDispose()
    {
        //旧数据解绑prefab
        if (itemData!=null)
        {
            itemData.UnBindUI();
        }
    }
    private void OnDestroy()
    {
        OnDispose();
    }
}
