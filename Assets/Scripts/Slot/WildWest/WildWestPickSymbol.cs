using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System.Collections.Generic;
public class WildWestPickSymbol : MonoBehaviour
{
    public List<GameObject> pickList = new List<GameObject>();
    public Text pendant;
    public Image icon;
    public Button pickBtn;
    public Animator animator;

    private WildWestFreeSpin freespin;

    private bool IsStart = false;

    private int btnIndex = 0;
    private int count = 0;
    private int freetype = 0;

    public void InitData(Sprite sprite, int index, int _count, int _btnIndx,  int _freetype, WildWestFreeSpin _freespin)
    {
        icon.sprite = sprite;
        pickBtn.GetComponent<Image>().sprite = sprite;
        pickList[index].SetActive(true);
        freespin = _freespin;
        count = _count;
        btnIndex = _btnIndx;
        freetype = _freetype;
        pendant.text = count.ToString();
        pickBtn.onClick.AddListener(OnClick);
    }

    public void OnClick()
    {
        IsStart = true;
        pendant.text = "";
        freespin.PickInitFreeSpin(this, count, btnIndex);
        this.PlayClick();

        new Libs.DelayAction(0.5f, null, ()=>
        {
            this.RollPendantUI();
        }).Play();
    }

    public void PlayClick()
    {
        animator.SetTrigger("click");
    }

    private long pendantcount = 0;

    private void RollPendantUI()
    {
        Libs.AudioEntity.Instance.PlayEffect("pick_num_rise", true);
        Utils.Utilities.AnimationTo (this.pendantcount, count, 1f, SetPendantUI, null, ()=>
        {
            Libs.AudioEntity.Instance.StopEffectAudio("pick_num_rise");
            Libs.AudioEntity.Instance.PlayEffect("pick_num_stop");
            freespin.PandentAddEnd();
            animator.SetTrigger("close");
            new Libs.DelayAction(2.5f, null, ()=>
            {
                this.StartFreeSpin();
            }).Play();
        }).Play();
    }

    private void SetPendantUI(long count)
    {
        this.pendantcount = count;
        pendant.text =  this.pendantcount.ToString();;
    }

    private void StartFreeSpin()
    {
        freespin.StartFreeSpin(freetype);
    }
    
}
