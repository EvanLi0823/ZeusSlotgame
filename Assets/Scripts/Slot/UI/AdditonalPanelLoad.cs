using System;
using System.Collections;
using Libs;
using UnityEngine;

public class AdditonalPanelLoad: MonoBehaviour
{
    private string local_path = "Prefab/Shared/AdditionalPanel_portrait.prefab";
    private GameObject banner;

    private void Start()
    {
        AddressableManager.Instance.LoadAsset<GameObject>(local_path, (asset) =>
        {
            if (asset != null)
            {
                banner= Instantiate(asset,gameObject.transform, false);
            }
        });
    }
}
