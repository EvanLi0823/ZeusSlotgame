using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Classic;

public class LockElementPanel : MonoBehaviour {

	public Transform mTransfm { get; protected set;}

	public LockReel ReelContronller {
		get;
		set;
	}
	public int IndexOfList {
		get;
		set;
	} 


	// 兼容sprite，image，两种方式都可实现
	public SpriteRenderer m_SymbolSprite;
	//		public Sprite m_SymbolSprite;
	public Image staticImage;
	public Image bgImage {get;set;}
	private int _indexOfDisplay;

	public int IndexOfDisplay {
		get {
			return _indexOfDisplay;
		}

		set {
			_indexOfDisplay = value;

			if (ReelContronller.positionMap.ContainsKey (_indexOfDisplay) == false) {
				ReelContronller.positionMap.Add(_indexOfDisplay, this);
			}

			if(bgImage == null){
				transform.SetSiblingIndex(ReelContronller.ElementNumber -_indexOfDisplay);
			}else{
				transform.SetSiblingIndex((2*ReelContronller.ElementNumber+1 -_indexOfDisplay));
			}


			if(m_SymbolSprite!=null){
				transform.localPosition = new Vector3(transform.localPosition.x,transform.localPosition.y,ReelContronller.ElementNumber -_indexOfDisplay);
			}
		}
	}
	private int _symbolIndex;

	public int SymbolIndex {
		get {
			return _symbolIndex;
		}
		set {
			_symbolIndex = value;
			ChangeElementPropertyDataWhenSymbolChange ();
		}
	}

	public bool IsBlankSymbol {
		set;
		get;
	}

	public virtual void ChangeElementPropertyDataWhenSymbolChange(){
		
			SetStaticSprite(this.ReelContronller.elementConfigs.GetBySymbolIndex(_symbolIndex));
			SetBackGround(this.ReelContronller.elementConfigs.GetBackground(_symbolIndex));
	}

	protected Vector3 ScaleFacter = new Vector3(1f,1f,1f);

	public virtual void SetStaticSprite (Sprite sprite)
	{
		if (sprite == null) {
			IsBlankSymbol = true;
			if (staticImage != null) {
				if (staticImage.sprite == null) {
					staticImage.sprite = this.ReelContronller.elementConfigs.GetBySymbolIndex (0);
				}
				staticImage.color = new Color (1, 1, 1, 0);
			}
			if(m_SymbolSprite !=null){
				m_SymbolSprite.sprite = null;//this.ReelContronller.elementConfigs.GetBySymbolIndex (0);
				m_SymbolSprite.color = new Color (1, 1, 1, 0);
			}
		} else {
			IsBlankSymbol = false;
			ScaleFacter = new Vector3 (sprite.rect.width / ReelContronller.elementConfigs.ReelPanelWidth, sprite.rect.height / ReelContronller.elementConfigs.reelConfigs [ReelContronller.Index].elementHeight, 1);
			if (staticImage != null) {
				staticImage.sprite = sprite;
				staticImage.transform.localScale = ScaleFacter;
				if (staticImage.color.a == 0) {
					staticImage.color = new Color (1, 1, 1, 1);
				}
			}
			if (m_SymbolSprite != null) {
				m_SymbolSprite.sprite = sprite;
				//显示的权重
				int scoreLv = this.ReelContronller.lockController.maskLayer;
				this.m_SymbolSprite.sortingOrder = 1 + scoreLv;
				m_SymbolSprite.color = Color.white;
			}
		}
	}

	public virtual void SetBackGround (Sprite sprite)
	{
		if (bgImage != null) {
			if (sprite == null) {
				bgImage.sprite = sprite;
				bgImage.gameObject.SetActive (false);
			} else {
				bgImage.gameObject.SetActive (true);
				bgImage.sprite = sprite;
			}
		}
	}
	public virtual void InitElement (LockReel ReelContronller, int SymbolIndex)
	{
		mTransfm = transform;
		this.ReelContronller = ReelContronller;
		IndexOfList = ReelContronller.Elements.IndexOf (this);
		_indexOfDisplay = IndexOfList;
		this.SymbolIndex = SymbolIndex;	
	}

	public virtual void ChangeColor (float r=1f, float g = 1f, float b = 1f, float a = 1f)
	{
		if (IsBlankSymbol) {
			if (staticImage != null) {
				staticImage.color = new Color (r, g, b, 0);
			} 
			if(m_SymbolSprite != null){
				m_SymbolSprite.color = new Color (r, g, b, 0);
			}
		} else {
			if (staticImage != null) {
				staticImage.color = new Color (r, g, b, a);
			}

			if (m_SymbolSprite != null) {
				m_SymbolSprite.color = new Color (r, g, b, a);
			}
		}
	}

	public virtual void EnableSymbolImage(bool enable){
		if (staticImage != null) {
			staticImage.gameObject.SetActive (enable);
		}
		if (m_SymbolSprite != null) {
			m_SymbolSprite.gameObject.SetActive (enable);
		}
	}

	public virtual void ChangeBackGroundInfo (RectTransform trans)
	{
		if (bgImage != null) {
			bgImage.rectTransform.anchorMax = trans.anchorMax;
			bgImage.rectTransform.anchorMin = trans.anchorMin;
		}	
	}

	public void SetOrderZ(int index)
	{
		Vector3 pos = this.transform.localPosition;
		pos.z = index;
		this.transform.localPosition = pos;
	}
}
