using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UI.Utils;

public class VoteListItem : MonoBehaviour {

    public Image voteItemContentImage;
    public string voteItemContentImageURL;

    public Image voteButtonImage;
    public Sprite voteButtonSelectedSprite;
    public Sprite voteButtonUnselectedSprite;

    public Image selectedBackgroundImage;
    public Sprite selectedBackgroundSprite;

    public const string VoteListItemClicked = "VoteListItemClicked";

    public Dictionary<string, object> _VoteDetailDict = null;
    public Dictionary<string, object> VoteDetailsDict { get {
            return _VoteDetailDict;
        }
        set {
            if (_VoteDetailDict != value) {
                _VoteDetailDict = value;

                if (_VoteDetailDict.ContainsKey("ImageSprite")) {
                    Sprite imageSprite = _VoteDetailDict["ImageSprite"] as Sprite;
                    SetItemContentImage(imageSprite);
                }
            }
        }
    }

    public enum VoteListItemStatus {
        None, 
        Selected,
        Unselected
    }

    public VoteListItemStatus _ItemStatus = VoteListItemStatus.None;
    public VoteListItemStatus ItemStatus {
        get {
            return _ItemStatus;
        }
        set {
            if (_ItemStatus != value) {
                _ItemStatus = value;

                switch(_ItemStatus) {
                    case VoteListItemStatus.None:
                    case VoteListItemStatus.Unselected: {
                            selectedBackgroundImage.sprite = null;
                            selectedBackgroundImage.color = Color.clear;
                            voteButtonImage.sprite = voteButtonUnselectedSprite;
                        }
                        break;

                    case VoteListItemStatus.Selected: {
                            selectedBackgroundImage.sprite = selectedBackgroundSprite;
                            selectedBackgroundImage.color = Color.white;
                            voteButtonImage.sprite = voteButtonSelectedSprite;
                        }
                        break;

                    default:
                        break;
                }
            }
        }
    }

    void Awake() {
        ItemStatus = VoteListItemStatus.Unselected;
        Messenger.AddListener<VoteListItem>(VoteListItemClicked, OnVoteListItemClicked);
    }
    
    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnDestroy() {
        Messenger.RemoveListener<VoteListItem>(VoteListItemClicked, OnVoteListItemClicked);
    }

    public void OnVoteListItemClick() {
        if (ItemStatus == VoteListItemStatus.Selected) {
            ItemStatus = VoteListItemStatus.Unselected;
        } else if (ItemStatus == VoteListItemStatus.None || 
            ItemStatus == VoteListItemStatus.Unselected) {
            ItemStatus = VoteListItemStatus.Selected;
            Messenger.Broadcast<VoteListItem>(VoteListItemClicked, this);
        }
    }

    public void OnVoteListItemClicked(VoteListItem item) {
        if (item != this) {
            this.ItemStatus = VoteListItemStatus.Unselected;
        }
    }

    public void SetItemContentImage(Sprite sprite) {
        if (voteItemContentImage != null) {
            voteItemContentImage.sprite = sprite;
        }
    }
}
