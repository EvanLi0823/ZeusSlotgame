using UnityEngine;
using UnityEngine.UI;
using Libs;

namespace Classic
{
    public class JackPotWinDialogBase : UIDialog 
    {
    	[HideInInspector]
        public TMPro.TextMeshProUGUI YOUWINAND;
    	[HideInInspector]
        public TMPro.TextMeshProUGUI TxtNumber;
    	[HideInInspector]
        public UnityEngine.UI.Text TxtWinAward;
        [HideInInspector]
        public UnityEngine.UI.Text TxtWinAward1;
        [HideInInspector]
        public TMPro.TextMeshProUGUI TxtWinAward2;
        [HideInInspector]
        public UnityEngine.UI.Button BtnOk;

        protected override void Awake()
        {
            base.Awake();
            this.YOUWINAND = Util.FindObject<TMPro.TextMeshProUGUI>(transform,"YOUWINAND/");
           
            this.TxtNumber = Util.FindObject<TMPro.TextMeshProUGUI>(transform,"YOUWINAND/TxtNumber/");

    		this.TxtWinAward = Util.FindObject<UnityEngine.UI.Text>(transform,"WInImage/TxtWinAward/");

            this.TxtWinAward1 = Util.FindObject<UnityEngine.UI.Text>(transform, "WInImage/NewTxtWinAward/");
            this.TxtWinAward2 = Util.FindObject<TMPro.TextMeshProUGUI>(transform, "WInImage/TxtWinAward/");
            this.BtnOk = Util.FindObject<UnityEngine.UI.Button>(transform,"BtnOk/");
            ReFindObject();
            if(BtnOk != null) 
            {
                UGUIEventListener.Get(this.BtnOk.gameObject).onClick = this.OnButtonClickHandler;
            }
        }

        void ReFindObject(){
                if (this.YOUWINAND == null)
                {
                    this.YOUWINAND = Util.FindObject<TMPro.TextMeshProUGUI>(transform, "Panel/YOUWINAND/");
                }
                if (this.TxtNumber==null)
                {
                    this.TxtNumber = Util.FindObject<TMPro.TextMeshProUGUI>(transform, "Panel/YOUWINAND/TxtNumber/");
                }
            if (this.TxtWinAward==null  && this.TxtWinAward1==null&&this.TxtWinAward2==null)
                {
                    this.TxtWinAward = Util.FindObject<UnityEngine.UI.Text>(transform, "Panel/WInImage/TxtWinAward/");
                    if (TxtWinAward==null)
                    {
                        this.TxtWinAward2 = Util.FindObject<TMPro.TextMeshProUGUI>(transform, "Panel/WInImage/TxtWinAward/");
                    }
                }
                if (this.BtnOk==null)
                {
                    this.BtnOk = Util.FindObject<UnityEngine.UI.Button>(transform, "Panel/BtnOk/");
                }
            }
    }
}
