using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CommonBackgroundPanel : BackgroundPanel {

	[SerializeField]
	private Image backgroundImage;

	[SerializeField]
	private Sprite normalBGSprite;

	[SerializeField]
	private Sprite freespinBGSprite;

	public override void OnAwakeBackgroundPanel(){
		base.OnAwakeBackgroundPanel ();
	}

	public override void OnDestroyBackgroundPanel(){
		base.OnDestroyBackgroundPanel ();
	}

	public override void OnEnterFreespin(){
		backgroundImage.sprite = freespinBGSprite;
	}

	public override void OnQuitFreespin(){
		backgroundImage.sprite = normalBGSprite;
	}
}
