using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 解决 一个Coins图标 + 金币数量 组成的组合控件，水平居中的问题
/// 图标在左边时，金币数量左对齐
/// 图标在右边时，金币数量右对齐
/// </summary>
public class CoinsAutoCenter : Text
{
    private Image M_Icon;
    private bool isInit = false;
    private float initXPos =0f;
    private float initWidth = 0f;
    
    public override string text
    {
        get
        {
            return this.m_Text;
        }
        set
        {
            if (!isInit)
            {
                Init();
            }
            if (string.IsNullOrEmpty(value))
            {
                if (string.IsNullOrEmpty(this.m_Text))
                    return;
                this.m_Text = "";
                this.SetVerticesDirty();
            }
            else
            {
                if (!(this.m_Text != value))
                    return;
                this.m_Text = value;
                this.SetVerticesDirty();
                this.SetLayoutDirty();
            }
        }
    }

    private void Init()
    {
        isInit = true;
        M_Icon = this.GetComponentInChildren<Image>();
        initXPos = this.transform.localPosition.x;
        if (M_Icon != null)
            initWidth = preferredWidth + M_Icon.preferredWidth;
        else
            initWidth = preferredWidth;
    }
    protected override void UpdateGeometry(){
        AdjustTextComponent();
        base.UpdateGeometry ();
    }

    void AdjustTextComponent()
    {
        // 只在运行模式下才自动适配
        if (Application.IsPlaying(gameObject))
        {

            float totalWidth = preferredWidth + M_Icon.preferredWidth;
            float newPosX = initXPos + (initWidth - totalWidth) / 2;
            this.transform.localPosition = new Vector3(newPosX, transform.localPosition.y, 0);
        }
    }
}
