using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Libs;

namespace Classic
{
	public class BetChangeDialog : UIDialog 
{
		public float jackpotX = -237f;
     	protected override void Awake()
    	{
	        base.Awake();
	        this.AutoQuit = true;
	        this.DisplayTime = 2f;
	        Messenger.AddListener(GameConstants.DO_SPIN, Close);

        }

		protected override void Start()
		{
			base.Start();
			Classic.UserManager.GetInstance().UserProfile().HasShowBetChangeTips = false;
		}

		protected override void OnDestroy()
		{
			Messenger.RemoveListener(GameConstants.DO_SPIN, Close);
			base.OnDestroy();
		}
	}
}
