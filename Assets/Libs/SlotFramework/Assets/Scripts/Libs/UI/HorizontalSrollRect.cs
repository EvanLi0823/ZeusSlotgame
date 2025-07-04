﻿using UnityEngine;
using System.Collections;

namespace UnityEngine.UI
{
	public class HorizontalSrollRect : ScrollRectBase
	{
//		public float RenderWidthOrHeight = 800;
		protected override float GetSize (RectTransform item)
		{
			return LayoutUtility.GetPreferredWidth (item) + contentSpacing;
		}

		protected override float GetDimension (Vector2 vector)
		{
			return vector.x;
		}

		protected override Vector2 GetVector (float value)
		{
			return new Vector2 (-value, 0);
		}

		protected override void Awake ()
		{
			base.Awake ();
			directionSign = 1;

			GridLayoutGroup layout = content.GetComponent<GridLayoutGroup> ();
			if (layout != null && layout.constraint != GridLayoutGroup.Constraint.FixedRowCount) {
				Debug.LogError ("[LoopHorizontalScrollRect] unsupported GridLayoutGroup constraint");
			}
		}

		protected override bool UpdateItems (Bounds viewBounds, Bounds contentBounds)
		{
			bool changed = false;
			if (viewBounds.max.x > contentBounds.max.x - 500) {
				float size = NewItemAtEnd();
				if (size > 0) {
					if (threshold < size) {
						// Preventing new and delete repeatly...
						threshold = size ;//* 1.1f;
					}
					changed = true;
				}
			} else if (viewBounds.max.x < contentBounds.max.x - threshold) {
				float size = DeleteItemAtEnd ();
				if (size > 0) {
					changed = true;
				}
			}

			if (viewBounds.min.x < contentBounds.min.x) {
				float size = NewItemAtStart();
				if (size > 0) {
					if (threshold < size) {
						threshold = size;//* 1.1f;
					}
					changed = true;
				}
			} else if (viewBounds.min.x > contentBounds.min.x + threshold) {
				float size = DeleteItemAtStart ();
				if (size > 0) {
					changed = true;
				}
			}
			return changed;
		}
	}
}