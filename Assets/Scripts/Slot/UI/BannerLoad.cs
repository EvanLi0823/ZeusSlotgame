using UnityEngine;
using System.Collections;
using Libs;
//[ExecuteInEditMode]
public class BannerLoad : MonoBehaviour 
{
	private GameObject BannerObject ;
	void Awake()
	{
        if (transform.childCount == 0) 
        {
			BannerObject = Instantiate (Resources.Load<GameObject> ("Prefab/Shared/BannerPrefab") as GameObject, gameObject.transform, false);
            BannerObject.name = "BannerPrefab";
            CommonPhoneCanvasAdapter canvasAdapter = gameObject.GetComponent<CommonPhoneCanvasAdapter>();
            if(canvasAdapter == null)
            {
                gameObject.AddComponent<CommonPhoneCanvasAdapter>();
            }
            TopUIBannerDontDestroy.AddTopUIPrefab();
        }
        Messenger.Broadcast (SlotControllerConstants.OnBlanceChangeForDisPlay);
    }
}
