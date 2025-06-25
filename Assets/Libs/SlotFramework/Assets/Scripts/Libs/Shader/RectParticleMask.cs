using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class RectParticleMask : Mask
{
    private float lastMinX;
    private float lastMinY;
    private float lastMaxX;
    private float lastMaxY;
    
    protected override void Start()
    {
        base.Start();
        lastMinX = 0.0f;
        lastMinY = 0.0f;
        lastMaxX = 0.0f;
        lastMaxY = 0.0f;
        SendRectToShader();
    }

    protected void SendRectToShader()
    {
        //后去rect裁剪区域
        Vector3[] corners = new Vector3[4];
        RectTransform rectTransform = transform as RectTransform;
        if(rectTransform == null)
            return;
        rectTransform.GetWorldCorners(corners);
        float minX, minY, maxX, maxY;
        
        minX = corners[0].x;
        minY = corners[0].y;
        maxX = corners[2].x;
        maxY = corners[2].y;

        if (minX == lastMinX && minY == lastMinY && maxX == lastMaxX && maxY == lastMaxY)
        {
            return;
        }
        lastMinX = minX;
        lastMaxX = maxX;
        lastMinY = minY;
        lastMaxY = maxY;
        
        //把rect区域传递给shader
        ParticleSystem[] particlesSystems = transform.GetComponentsInChildren<ParticleSystem>();
        
        foreach (ParticleSystem particleSystem in particlesSystems)
        {
            if(particleSystem == null )
                continue;
            Renderer rend = particleSystem.GetComponent<Renderer>();
            if (rend == null)
                continue;
            var tempMaterial = rend.sharedMaterial;
            if(tempMaterial == null)
                continue;
            tempMaterial.SetFloat("_MinX", minX);
            tempMaterial.SetFloat("_MinY", minY);
            tempMaterial.SetFloat("_MaxX", maxX);
            tempMaterial.SetFloat("_MaxY", maxY);
            rend.material = tempMaterial;
        }
    }

    private void Update()
    {
        SendRectToShader();
    }
}