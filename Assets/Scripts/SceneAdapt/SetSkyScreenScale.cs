using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetSkyScreenScale : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        float scale = SkySreenUtils.GetDlgScale();
        transform.localScale = new Vector3(scale,scale,1);
    }
}
