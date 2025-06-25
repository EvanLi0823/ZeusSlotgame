using System;
using TMPro;
using UnityEngine;

public class CoinsPanelItem : MonoBehaviour
{
    public TextMeshProUGUI coinText;

    private void Awake()
    {
        if (coinText == null)
        {
            FindTransform(transform, "TextCoins");
        }
    }
    
    private Transform FindTransform(Transform trans, string name)
    {
        Transform target = trans.Find(name);
        if (target) return target;
        int childNum = trans.childCount;
        if (childNum == 0) return null;
        for (int i = 0; i < childNum; i++)
        {
            target = FindTransform(trans.GetChild(i), name);
            if (target) return target;
        }

        return null;
    }

    public void Init(string text)
    {
        if (coinText == null) return;
        coinText.text = text;
    }
}