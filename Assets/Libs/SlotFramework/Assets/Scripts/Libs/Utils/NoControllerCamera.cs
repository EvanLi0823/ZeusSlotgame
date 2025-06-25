using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoControllerCamera : MonoBehaviour
{
    static NoControllerCamera instance;
    private void Awake()
    {
    
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            DestroyObject(gameObject);
        }
    }
}
