using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class SpineGraphic : Graphic
{    
    public List<float> vertices = new List<float>();

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();

        for (int i = 0; i < vertices.Count; i++)
        {
            if(i > vertices.Count - 1) break;
            vh.AddVert(new Vector3(vertices[i], vertices[i+1]), Color.white, Vector2.zero);
            i++;
        }
        
        for (int i = 0; i < vertices.Count/2 - 2; i++)
        {
            vh.AddTriangle(0, i + 1, i + 2);
        }
    }
}
