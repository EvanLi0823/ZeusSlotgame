using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Libs;

public class UpdateCollectSprite : MonoBehaviour 
{
    public Sprite defaultSprite;
    public List<Image> ImageList = new List<Image>();
    public List<Sprite> NodeSpriteList = new List<Sprite>();
    Dictionary<string,Sprite> nameSpriteDict = new Dictionary<string, Sprite>();
	// Use this for initialization
	void Awake () {
        for (int i = 0; i < NodeSpriteList.Count; i++){
            Sprite sprite = NodeSpriteList[i];
            if (sprite == null) return;
            nameSpriteDict[sprite.name] = sprite;
        }
	}
	
    void OnDestroy () {
        NodeSpriteList.Clear();
        nameSpriteDict.Clear();
	}
   
}
