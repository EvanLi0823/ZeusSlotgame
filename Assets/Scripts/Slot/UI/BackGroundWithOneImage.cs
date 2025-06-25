using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BackGroundWithOneImage : BackgroundPanel {
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
