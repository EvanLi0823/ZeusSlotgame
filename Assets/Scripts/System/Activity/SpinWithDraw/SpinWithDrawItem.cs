using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Activity
{
    public class SpinWithDrawItem:BaseIcon
    {
        private Button clickButton;
        private Image bar;
        private void Awake()
        {
            clickButton = GetComponent<Button>();
            bar = Util.FindObject<Image>(transform, "bg/bar");
            if (clickButton!=null)
            {
                clickButton.onClick.AddListener(OnButtonClick);
            }
        }

        protected override void AddListener()
        {
            
        }

        public override void RefreshProgress(float f,string info=null)
        {
            bar.fillAmount = f;
        }
        
        void OnButtonClick()
        {
            ActivityManager.Instance.OnClickIcon(activityId);
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            Debug.Log("SpinWithDrawItem Destroy GameObject");
        }
    }
}