using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine;

public class NoDrawingRayCast : Graphic
{

	public override void SetMaterialDirty ()
	{

	}

	public override void SetVerticesDirty ()
	{

	}

	protected override void OnFillVBO (List<UIVertex> vbo)
	{

		vbo.Clear ();

	}
}

