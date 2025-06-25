using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Sprites;
public class MaskableImage : Image
 {
    protected Mask mask;
    float minX, minY, maxX, maxY;
    protected override void Start(){
        base.Start();

        //获取父对象身上的Mask
        mask = this.transform.parent.GetComponent<Mask>();
        if (mask!=null)
        {
            RectTransform rectTrans = mask.rectTransform;
            Vector3[] corners = new Vector3[4];
            rectTrans.GetWorldCorners(corners);
            minX = corners[0].x;
            minY = corners[0].y;
            maxX = corners[2].x;
            maxY = corners[2].y;

            Material m = material;
            m.SetFloat("_MinX", minX);
            m.SetFloat("_MinY", minY);
            m.SetFloat("_MaxX", maxX);
            m.SetFloat("_MaxY", maxY);
        }
    }
 }