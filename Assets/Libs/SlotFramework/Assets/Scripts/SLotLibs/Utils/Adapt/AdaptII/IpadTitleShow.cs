using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IpadTitleShow : MonoBehaviour
{
    void Start()
    {
        if (SkySreenUtils.IsRealPad())
        {
            if (!gameObject.activeSelf)
                gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);

        }
    }
}
