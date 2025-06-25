using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonMask : MonoBehaviour
{
    public Image mask;
    // Start is called before the first frame update
    void OnEnable()
    {
        // Messenger.AddListener<bool>(GameConstants.ShowButtonMask,ShowMask);
        // Messenger.AddListener<int>(GameConstants.ChangeMaskOrder,ChangeOrder);
    }

    private void OnDisable()
    {
        // Messenger.RemoveListener<bool>(GameConstants.ShowButtonMask,ShowMask);
        // Messenger.RemoveListener<int>(GameConstants.ChangeMaskOrder,ChangeOrder);
    }

    private void ChangeOrder(int order)
    {
        this.gameObject.GetComponent<Canvas>().sortingOrder = order;
    }
    
    void ShowMask(bool state)
    {
        mask.gameObject.SetActive(state);
    }
}
