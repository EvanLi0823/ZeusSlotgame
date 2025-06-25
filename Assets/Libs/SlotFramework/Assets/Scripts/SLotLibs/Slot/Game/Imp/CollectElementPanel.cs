using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Classic;
using UnityEngine.UI;
public class CollectElementPanel : WebmSymbolRender 
{
	public UIText text;

	protected override void SymbolChangeHandler()
	{
		base.SymbolChangeHandler ();
		if (text == null) return;
		text.gameObject.SetActive (false);

		if (SymbolIndex == GetSymbolIndex()) 
		{
			text.gameObject.SetActive (true);
			text.SetText (GetTextContent());
		}
	}

	public virtual int GetSymbolIndex()
	{
		return -100;
	}

	public virtual string GetTextContent()
	{
		return "";
	}

	public override void ChangeGrey(bool symbolGray = true)
	{
		base.ChangeGrey (symbolGray);
		if (text != null) text.gameObject.SetActive (false);
	}
}
