using UnityEngine;
using UnityEngine.UI;
using Libs;

namespace Classic
{
public class JackPotWinDialog_Jackpot777Base : UIDialog 
{
		[HideInInspector]
    public UnityEngine.GameObject MINI;
		[HideInInspector]
    public UnityEngine.GameObject MINOR;
		[HideInInspector]
    public UnityEngine.GameObject MAJOR;
		[HideInInspector]
    public UnityEngine.GameObject MEGA;
		[HideInInspector]
    public UnityEngine.UI.Image MEGA_TEXTTURE;
		[HideInInspector]
    public UnityEngine.UI.Image MAJOR_TEXTTURE;
		[HideInInspector]
    public UnityEngine.UI.Image MINOR_TEXTTURE;
		[HideInInspector]
    public UnityEngine.UI.Image MINI_TEXTTURE;
		[HideInInspector]
    public UnityEngine.UI.Text TxtWinAward;
		[HideInInspector]
    public UnityEngine.UI.Button BtnOk;

    protected override void Awake()
    {
        base.Awake();
        this.MINI = Util.FindObject<UnityEngine.GameObject>(transform,"BGImage/Title/MINI/");
        this.MINOR = Util.FindObject<UnityEngine.GameObject>(transform,"BGImage/Title/MINOR/");
        this.MAJOR = Util.FindObject<UnityEngine.GameObject>(transform,"BGImage/Title/MAJOR/");
        this.MEGA = Util.FindObject<UnityEngine.GameObject>(transform,"BGImage/Title/MEGA/");
        this.MEGA_TEXTTURE = Util.FindObject<UnityEngine.UI.Image>(transform,"YouWon/MEGA_TEXTTURE/");
        this.MAJOR_TEXTTURE = Util.FindObject<UnityEngine.UI.Image>(transform,"YouWon/MAJOR_TEXTTURE/");
        this.MINOR_TEXTTURE = Util.FindObject<UnityEngine.UI.Image>(transform,"YouWon/MINOR_TEXTTURE/");
        this.MINI_TEXTTURE = Util.FindObject<UnityEngine.UI.Image>(transform,"YouWon/MINI_TEXTTURE/");
        this.TxtWinAward = Util.FindObject<UnityEngine.UI.Text>(transform,"WInImage/TxtWinAward/");
        this.BtnOk = Util.FindObject<UnityEngine.UI.Button>(transform,"BtnOk/");
        UGUIEventListener.Get(this.BtnOk.gameObject).onClick = this.OnButtonClickHandler;
    }
}}
