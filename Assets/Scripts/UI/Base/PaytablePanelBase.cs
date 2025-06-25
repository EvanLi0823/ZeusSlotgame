using UnityEngine;
using UnityEngine.UI;
using Libs;

namespace Classic
{
	public class PaytablePanelBase : UIDialog
	{
		[HideInInspector]
		public UnityEngine.UI.Button CloseButton;
		[HideInInspector]
		public UnityEngine.UI.Button LeftButton;
		[HideInInspector]
		public UnityEngine.UI.Button RightButton;
		[HideInInspector]
		public UnityEngine.GameObject MiddlePanel;

		protected override void Awake ()
		{
			base.Awake ();
            if (isPortrait)
            {
                this.CloseButton = Util.FindObject<UnityEngine.UI.Button>(transform, "Anchor/CloseButton/");
                if (this.CloseButton==null)
                {
	                this.CloseButton = Util.FindObject<UnityEngine.UI.Button>(transform, "CloseButton/");
                }
                UGUIEventListener.Get(this.CloseButton.gameObject).onClick = this.OnButtonClickHandler;
                this.LeftButton = Util.FindObject<UnityEngine.UI.Button>(transform, "Anchor/LeftButton/");
                if ( this.LeftButton==null)
                {
	                this.LeftButton = Util.FindObject<UnityEngine.UI.Button>(transform, "LeftButton/");
                }
                if (this.LeftButton != null)
                {
                    UGUIEventListener.Get(this.LeftButton.gameObject).onClick = this.OnButtonClickHandler;
                }
                this.RightButton = Util.FindObject<UnityEngine.UI.Button>(transform, "Anchor/RightButton/");
                if (RightButton == null)
                {
	                this.RightButton = Util.FindObject<UnityEngine.UI.Button>(transform, "RightButton/");
                }
                if (RightButton != null)
                {
                    UGUIEventListener.Get(this.RightButton.gameObject).onClick = this.OnButtonClickHandler;
                }
                this.MiddlePanel = Util.FindObject<UnityEngine.GameObject>(transform, "Anchor/MiddlePanel/");
                if (MiddlePanel==null)
                {
	                this.MiddlePanel = Util.FindObject<UnityEngine.GameObject>(transform, "MiddlePanel/");
                }
            }
            else
            {
                this.CloseButton = Util.FindObject<UnityEngine.UI.Button>(transform, "CloseButton/");
                if (this.CloseButton == null)
                {
					this.CloseButton = Util.FindObject<UnityEngine.UI.Button>(transform, "Anchor/CloseButton/");    
                }
                UGUIEventListener.Get(this.CloseButton.gameObject).onClick = this.OnButtonClickHandler;
                
                this.LeftButton = Util.FindObject<UnityEngine.UI.Button>(transform, "LeftButton/");
                if (this.LeftButton == null)
                {
					this.LeftButton = Util.FindObject<UnityEngine.UI.Button>(transform, "Anchor/LeftButton/");    
                }
                if (this.LeftButton != null)
                {
                    UGUIEventListener.Get(this.LeftButton.gameObject).onClick = this.OnButtonClickHandler;
                }
                
                this.RightButton = Util.FindObject<UnityEngine.UI.Button>(transform, "RightButton/");
                if (this.RightButton == null)
                {
					this.RightButton = Util.FindObject<UnityEngine.UI.Button>(transform, "Anchor/RightButton/");    
                }
                if (RightButton != null)
                {
                    UGUIEventListener.Get(this.RightButton.gameObject).onClick = this.OnButtonClickHandler;
                }
                
                this.MiddlePanel = Util.FindObject<UnityEngine.GameObject>(transform, "MiddlePanel/");
                if (this.MiddlePanel == null)
                {
					this.MiddlePanel = Util.FindObject<UnityEngine.GameObject>(transform, "Anchor/MiddlePanel/");    
                }
            }
          
		}
	}
}
