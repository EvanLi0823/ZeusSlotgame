using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Libs;

namespace Classic
{
public class PaytablePanel : PaytablePanelBase 
{
		public bool isOpenModel = true;
		public bool IsNewPayTable = false;
        private GameObject[] m_Renders;
        [HideInInspector]
		public int currentPage = 0;
     	protected override void Awake()
    	{
       		base.Awake();
			if (MiddlePanel != null) {
				int count = this.MiddlePanel.transform.childCount;
				m_Renders = new GameObject[count];
				for (int i = 0; i < count; i++) {
					m_Renders [i] = this.MiddlePanel.transform.GetChild (i).gameObject;
				}
				RefreshPage ();
			}
    	}

        protected override void Start()
        {
            base.Start();
            //Messenger.Broadcast<bool>(PaytableShowEventHandler.ENABLE_SHOW_SYMBOL_BOARD, false);
            Messenger.Broadcast<bool>(SlotControllerConstants.PLAY_BACKGROUND_MUSIC,false);
        }
        public void SetOpenMask()
		{
			if(!isOpenModel) this.isModel = false;
        }
		
		public void RefreshPage()
		{
			if (m_Renders == null) {
				return;
			}
			for (int i = 0; i < m_Renders.Length; i++) {
				m_Renders [i].SetActive (i == currentPage);
			}
		}

   		public override void OnButtonClickHandler(GameObject go)
    	{
            //    	    base.OnButtonClickHandler(go);
            if (IsNewPayTable) {
				if (this.CloseButton.gameObject == go) {
                    AudioEntity.Instance.PlayEffect("paytable_open");
                    this.Close ();
				} else if (this.LeftButton.gameObject == go) {
					currentPage = (currentPage - 1 + m_Renders.Length) % m_Renders.Length;
                    AudioEntity.Instance.PlayEffect("paytable_scroll");
                    RefreshPage();
				} else if (this.RightButton.gameObject == go) {
					currentPage = (currentPage + 1 + m_Renders.Length) % m_Renders.Length;
                    AudioEntity.Instance.PlayEffect("paytable_scroll");
                    RefreshPage();
				}
			} else {
                AudioEntity.Instance.PlayClickEffect();
                this.Close ();
			}
    	}

        protected override void OnDestroy()
        {
            //Messenger.Broadcast<bool>(PaytableShowEventHandler.ENABLE_SHOW_SYMBOL_BOARD, true);
            Messenger.Broadcast<bool>(SlotControllerConstants.PLAY_BACKGROUND_MUSIC,true);
            base.OnDestroy();
        }

    }
}
