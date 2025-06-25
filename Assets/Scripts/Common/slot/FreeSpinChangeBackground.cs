using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FreeSpinChangeBackground : BackgroundPanel {

	[SerializeField]
	private Image backgroundImage;

	[SerializeField]
	private Sprite normalBGSprite;

	[SerializeField]
	private Sprite freespinBGSprite;

	[SerializeField]
	private Image boardImage;

	[SerializeField]
	private Sprite normalBoardSprite;

	[SerializeField]
	private Sprite freespinBoardSprite;
	public override void OnAwakeBackgroundPanel(){
		base.OnAwakeBackgroundPanel ();
	}

	public override void OnDestroyBackgroundPanel(){
		base.OnDestroyBackgroundPanel ();
	}

	public override void OnEnterFreespin(){
		backgroundImage.sprite = freespinBGSprite;
		if (boardImage!=null&&freespinBoardSprite!=null)
		{
			boardImage.sprite = freespinBoardSprite;
		}
	}

	public override void OnQuitFreespin(){
		backgroundImage.sprite = normalBGSprite;
		if (boardImage!=null&&normalBoardSprite!=null)
		{
			boardImage.sprite = normalBoardSprite;
		}
	}
}
