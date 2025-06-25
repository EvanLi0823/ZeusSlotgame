using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class VoteButton : MonoBehaviour {

    public Sprite normalSprite;
    public Sprite disabledSprite;

    public void EnableVoteButton(bool bEnable) {
        Image image = this.GetComponent<Image>();
        if (image != null) {
            image.sprite = bEnable ? normalSprite : disabledSprite;
        }
    }
}
