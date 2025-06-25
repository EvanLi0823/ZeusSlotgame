using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;
public class CustomPayLineRenderer : UILineRenderer {
    protected override void OnPopulateMesh(VertexHelper vh){
        OnPopulateCustomMesh(vh);
    }
    #region CustomMeshPayLineHandle
    public Dictionary<int, List<Vector2>> dict = new Dictionary<int, List<Vector2>>();
    List<int> needCreateJoinSegmentIndexList = new List<int>();
    protected void OnPopulateCustomMesh(VertexHelper vh)
    {
        if (m_points == null)
            return;
        Vector2[] pointsToDraw = m_points;

        var sizeX = rectTransform.rect.width;
        var sizeY = rectTransform.rect.height;
        var offsetX = -rectTransform.pivot.x * rectTransform.rect.width;
        var offsetY = -rectTransform.pivot.y * rectTransform.rect.height;
        //SetTestData();
        
//        Debug.Log("1.OnPopulateCustomMesh sizeX:"+sizeX+" sizeY:"+sizeY+" offsetX:"+offsetX+" offsetY:"+offsetY);
//        if (UseMargins)
//        {
//            sizeX -= Margin.x;
//            sizeY -= Margin.y;
//            offsetX += Margin.x / 2f;
//            offsetY += Margin.y / 2f;
//        }
//        Debug.Log("2.OnPopulateCustomMesh sizeX:" + sizeX + " sizeY:" + sizeY + " offsetX:" + offsetX + " offsetY:" + offsetY);

        
        vh.Clear();
        //Debug.Log("OnPopulateMesh Length:" + pointsToDraw.Length);
        // Generate the quads that make up the wide line
        var segments = new List<UIVertex[]>();
        needCreateJoinSegmentIndexList.Clear();
        for (var i = 1; i < pointsToDraw.Length; i++)
        {
            var start = pointsToDraw[i - 1];
            var end = pointsToDraw[i];
            //第一个点和最后一个点不添加帽子，同时，中间的替换点不添加帽子线段
            bool startCap = true;
            bool endCap = true;
            if (i == 1) startCap = false;
            if (i == pointsToDraw.Length - 1) endCap = false;
            if (dict.ContainsKey(i - 1))
            {
                if (dict[i - 1].Count > 1) start = dict[i - 1][1];
                else Debug.LogError("gameconfig panelWidth and reelWidth not config or not right reelConfigs.Length reelConfigs[reelIndex].ElementNumbers not right!");
                startCap = false;
            }
            if (dict.ContainsKey(i)) { 
                if(dict[i].Count>0) end = dict[i][0];
                else Debug.LogError("gameconfig panelWidth and reelWidth not config or not right! reelConfigs.Length reelConfigs[reelIndex].ElementNumbers not right!");
                endCap = false; 
            }

            start = new Vector2(start.x * sizeX + offsetX, start.y * sizeY + offsetY);
            end = new Vector2(end.x * sizeX + offsetX, end.y * sizeY + offsetY);
           
            if (LineCaps&&startCap)
            {
                //segments.Add(CreateLineCap(start, end, SegmentType.Start));
            }

            segments.Add(CreateLineSegment(start, end, SegmentType.Middle));

            if (LineCaps&&endCap)
            {
                needCreateJoinSegmentIndexList.Add(segments.Count-1);
                //segments.Add(CreateLineCap(start, end, SegmentType.End));
            }

        }
       
        //平滑交点，对两个相连线段进行平滑处理
        // Add the line segments to the vertex helper, creating any joins as needed
        for (var i = 0; i < segments.Count; i++)
        {
            if (i < segments.Count - 1&&needCreateJoinSegmentIndexList.Contains(i))
            {
                var vec1 = segments[i][1].position - segments[i][2].position;
                var vec2 = segments[i + 1][2].position - segments[i + 1][1].position;
                var angle = Vector2.Angle(vec1, vec2) * Mathf.Deg2Rad;

                // Positive sign means the line is turning in a 'clockwise' direction
                var sign = Mathf.Sign(Vector3.Cross(vec1.normalized, vec2.normalized).z);

                // Calculate the miter point
                var miterDistance = LineThickness / (2 * Mathf.Tan(angle / 2));
                var miterPointA = segments[i][2].position - vec1.normalized * miterDistance * sign;
                var miterPointB = segments[i][3].position + vec1.normalized * miterDistance * sign;

                var joinType = LineJoins;
                if (joinType == JoinType.Miter)
                {
                    // Make sure we can make a miter join without too many artifacts.
                    if (miterDistance < vec1.magnitude / 2 && miterDistance < vec2.magnitude / 2 && angle > MIN_MITER_JOIN)
                    {
                        segments[i][2].position = miterPointA;
                        segments[i][3].position = miterPointB;
                        segments[i + 1][0].position = miterPointB;
                        segments[i + 1][1].position = miterPointA;
                    }
                    else
                    {
                        joinType = JoinType.Bevel;
                    }
                }

                if (joinType == JoinType.Bevel)
                {
                    if (miterDistance < vec1.magnitude / 2 && miterDistance < vec2.magnitude / 2 && angle > MIN_BEVEL_NICE_JOIN)
                    {
                        if (sign < 0)
                        {
                            segments[i][2].position = miterPointA;
                            segments[i + 1][1].position = miterPointA;
                        }
                        else
                        {
                            segments[i][3].position = miterPointB;
                            segments[i + 1][0].position = miterPointB;
                        }
                    }

                    var join = new UIVertex[] { segments[i][2], segments[i][3], segments[i + 1][0], segments[i + 1][1] };
                    vh.AddUIVertexQuad(join);
                }
            }
            vh.AddUIVertexQuad(segments[i]);
        }
    }
   
    private void SetTestData()
    {
        dict.Clear();
        List<Vector2> symbol1 = new List<Vector2>();
        symbol1.Add(new Vector2(0.01f, 0.1666f));
        symbol1.Add(new Vector2(0.19f, 0.3166f));
        dict.Add(1, symbol1);

        List<Vector2> symbol2 = new List<Vector2>();
        symbol2.Add(new Vector2(0.21f, 0.35f));
        symbol2.Add(new Vector2(0.39f, 0.5f));
        dict.Add(2, symbol2);

        List<Vector2> symbol3 = new List<Vector2>();
        symbol3.Add(new Vector2(0.41f, 0.5f));
        symbol3.Add(new Vector2(0.59f, 0.5f));
        dict.Add(3, symbol3);

        List<Vector2> symbol4 = new List<Vector2>();
        symbol4.Add(new Vector2(0.61f, 0.5f));
        symbol4.Add(new Vector2(0.79f, 0.65f));
        dict.Add(4, symbol4);

        List<Vector2> symbol5 = new List<Vector2>();
        symbol5.Add(new Vector2(0.81f, 0.6833f));
        symbol5.Add(new Vector2(0.99f, 0.8333f));
        dict.Add(5, symbol5);
    }

    #endregion
}
