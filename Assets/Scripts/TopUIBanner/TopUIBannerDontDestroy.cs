using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class TopUIBannerDontDestroy : MonoBehaviour
{
    private Canvas canvas;
    private GameObject ChildGO;
	public static TopUIBannerDontDestroy instance;
	public static void AddTopUIPrefab()
	{
		if (instance == null)
		{
			GameObject BannerObject =
				Instantiate(Resources.Load<GameObject>("Prefab/Shared/TopUIBanner") as GameObject);
			BannerObject.name = "TopUIBanner";
		}
	}

	void Awake () 
	{
		if (!instance) 
		{
			instance = this;
			DontDestroyOnLoad (gameObject);
			canvas = this.GetComponent<Canvas>();
			ChildGO = transform.GetChild(0).gameObject;
			SceneManager.sceneLoaded += this.SceneChangeHandler;
		}
		else 
		{
			Destroy(gameObject);
		}
	}


	void OnDestroy()
    {
		SceneManager.sceneLoaded -= this.SceneChangeHandler;
	}

    void SceneChangeHandler (Scene s, LoadSceneMode mode)
    {
	    RefreshUI();
    }

    private void RefreshUI()
    {
	    if (SkySreenUtils.CurrentOrientation== ScreenOrientation.Portrait && !SwapSceneManager.Instance.GetLogicSceneName().Equals("ClassicLobby") && !SwapSceneManager.Instance.GetLogicSceneName().Equals("ClubLobby")|| SwapSceneManager.Instance.GetLogicSceneName().Equals("EmptyScene"))
	    {
		    this.ChildGO.SetActive(false);
	    }else
	    {
		    if(!this.ChildGO.activeSelf)
		    {
			    this.ChildGO.SetActive(true);
		    }
		    if(canvas != null)
		    {
			    GameObject camera = GameObject.FindGameObjectWithTag("MainCamera"); 
			    if(camera != null)
			    {
				    canvas.worldCamera = camera.GetComponent<Camera>();
			    }

			    if (IphoneXAdapter.IsIphoneX())
			    {
				    CanvasScaler[] canvasScaler = camera.GetComponentsInChildren<CanvasScaler>();
				    for (int i = 0; i < canvasScaler.Length; i++)
				    {
					    if (canvasScaler[i].uiScaleMode != CanvasScaler.ScaleMode.ConstantPixelSize)
					    {
						    canvasScaler[i].uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
					    }
				    }
			    }
			    
		    }

		    if (this.ChildGO.activeSelf)
		    {
		    }
	    }
    }
}
