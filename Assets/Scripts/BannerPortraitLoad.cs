using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BannerPortraitLoad : MonoBehaviour
{
    private string local_path = "Prefab/Shared/BannerPrefab_Portrait";
    private GameObject banner;
	
    private void Awake()
	{
        
#if UNITY_EDITOR
        SkySreenUtils.SetScreenResolutions();
#endif

		if (transform.childCount == 0)
        {
            if(AdaptionTools.isUsePortraitPadBanner())
                local_path = "Prefab/Shared/BannerPrefab_Portrait_IPad";
            GameObject banner = Instantiate(Resources.Load<GameObject> (local_path) as GameObject, gameObject.transform, false);
            banner.name = "BannerPrefab";
        }

        if(banner == null)
        {
            banner = this.transform.GetChild(0).gameObject;
            banner.name = "BannerPrefab";
        } 

        if(IphoneXAdapter.IsIphoneX())
        {
            RectTransform bottom = banner.transform.Find("BottomPanel").GetComponent<RectTransform>();
            if(bottom != null) bottom.anchoredPosition = Vector3.zero;
            RectTransform top = banner.transform.Find("TopPanel").GetComponent<RectTransform>();
            if(top != null) top.anchoredPosition = Vector3.zero;
        } 
    }

}
