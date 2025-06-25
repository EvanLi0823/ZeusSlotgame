using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.UI.Extensions{
    public class CustomGridPaylineRenderer : UILineRenderer
    {
		public List<UIVertex[]> segments = new List<UIVertex[]>();
        protected override void OnPopulateMesh(VertexHelper vh)
        {
			segments.Clear ();
            relativeSize = true;
            UseMargins = true;
			LineCaps = false;
			LineJoins = JoinType.Miter;
            int ArraySize = 5;

            m_points = new Vector2[ArraySize];
            m_points[0] = new Vector2(0, 0);
            m_points[1] = new Vector2(0,1);
            m_points[2] = new Vector2(1,1);
            m_points[3] = new Vector2(1, 0);
            m_points[4] = new Vector2(0, 0);

			if (m_points == null)
				return;
			Vector2[] pointsToDraw = m_points;
			//If Bezier is desired, pick the implementation
			if (BezierMode != BezierType.None && m_points.Length > 3)
			{
				BezierPath bezierPath = new BezierPath();

				bezierPath.SetControlPoints(pointsToDraw);
				bezierPath.SegmentsPerCurve = BezierSegmentsPerCurve;
				List<Vector2> drawingPoints;
				switch (BezierMode)
				{
				case BezierType.Basic:
					drawingPoints = bezierPath.GetDrawingPoints0();
					break;
				case BezierType.Improved:
					drawingPoints = bezierPath.GetDrawingPoints1();
					break;
				default:
					drawingPoints = bezierPath.GetDrawingPoints2();
					break;
				}

				pointsToDraw = drawingPoints.ToArray();
			}

			var sizeX = rectTransform.rect.width;
			var sizeY = rectTransform.rect.height;
			var offsetX = -rectTransform.pivot.x * rectTransform.rect.width;
			var offsetY = -rectTransform.pivot.y * rectTransform.rect.height;

			// don't want to scale based on the size of the rect, so this is switchable now
			if (!relativeSize)
			{
				sizeX = 1;
				sizeY = 1;
			}

			if (UseMargins)
			{
				sizeX -= Margin.x;
				sizeY -= Margin.y;
				offsetX += Margin.x / 2f;
				offsetY += Margin.y / 2f;
			}

			vh.Clear();

			// Generate the quads that make up the wide line
//			var segments = new List<UIVertex[]>();
			if (LineList)
			{
				for (var i = 1; i < pointsToDraw.Length; i += 2)
				{
					var start = pointsToDraw[i - 1];
					var end = pointsToDraw[i];
					start = new Vector2(start.x * sizeX + offsetX, start.y * sizeY + offsetY);
					end = new Vector2(end.x * sizeX + offsetX, end.y * sizeY + offsetY);

					if (LineCaps)
					{
						segments.Add(CreateLineCap(start, end, SegmentType.Start));
					}

					segments.Add(CreateLineSegment(start, end, SegmentType.Middle));

					if (LineCaps)
					{
						segments.Add(CreateLineCap(start, end, SegmentType.End));
					}
				}
			}
			else
			{
				for (var i = 1; i < pointsToDraw.Length; i++)
				{
					var start = pointsToDraw[i - 1];
					var end = pointsToDraw[i];
					start = new Vector2(start.x * sizeX + offsetX, start.y * sizeY + offsetY);
					end = new Vector2(end.x * sizeX + offsetX, end.y * sizeY + offsetY);

					if (LineCaps && i == 1)
					{
						segments.Add(CreateLineCap(start, end, SegmentType.Start));
					}

					segments.Add(CreateLineSegment(start, end, SegmentType.Middle));

					if (LineCaps && i == pointsToDraw.Length - 1)
					{
						segments.Add(CreateLineCap(start, end, SegmentType.End));
					}
				}
			}

			/////////////////////////// 设置corner
			for (var i = 0; i < segments.Count; i++)
			{
				//////////÷
				if (!LineList && i < segments.Count - 1) {
					SetCorner (i, i + 1);
				}
			}
			SetCorner (segments.Count-1, 0);
			/// /////////

            for (var i = 0; i < segments.Count; i++)
            {
                vh.AddUIVertexQuad(segments[i]);
            }
        }

		private void SetCorner(int first, int second)
		{
			var vec1 = segments[first][1].position - segments[first][2].position;
			var vec2 = segments[second][2].position - segments[second][1].position;
			var angle = Vector2.Angle(vec1, vec2) * Mathf.Deg2Rad;

			// Positive sign means the line is turning in a 'clockwise' direction
			var sign = Mathf.Sign(Vector3.Cross(vec1.normalized, vec2.normalized).z);

			// Calculate the miter point
			var miterDistance = LineThickness / (2 * Mathf.Tan(angle / 2));
			var miterPointA = segments[first][2].position - vec1.normalized * miterDistance * sign;
			var miterPointB = segments[first][3].position + vec1.normalized * miterDistance * sign;

			var joinType = LineJoins;
			if (joinType == JoinType.Miter)
			{
				// Make sure we can make a miter join without too many artifacts.
				if (miterDistance < vec1.magnitude / 2 && miterDistance < vec2.magnitude / 2 && angle > MIN_MITER_JOIN)
				{
					segments[first][2].position = miterPointA;
					segments[first][3].position = miterPointB;
					segments[second][0].position = miterPointB;
					segments[second][1].position = miterPointA;

				}

			}

		}
    }
}

