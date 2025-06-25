using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

public class ResourceCache: MonoSingleton<ResourceCache>
{
    private bool loadSuccess = false;

    private Coroutine preLoad;
    private Coroutine backLoad;
    public bool LoadSuccess()
    {
        return loadSuccess;
    }
    private Dictionary<string, Object> cache = new Dictionary<string, Object>();
    protected override void Init()
    {
        base.Init();
        preLoad =  StartCoroutine(PreLoadResources());
        //需要等带预加载完成才能进入游戏的资源
        Messenger.AddListener(GameConstants.OnSceneInit,OnSceneInit);
    }

    void OnSceneInit()
    {
        backLoad =  StartCoroutine(BackLoadResources());
        Messenger.RemoveListener(GameConstants.OnSceneInit,OnSceneInit);
    }

    public void LoadingSceneStart(Action callBack)
    {
        
    }
    //预加载的资源，需要加载完成后才能进入游戏
    IEnumerator PreLoadResources()
    {
        List<string> resourcePaths = new List<string>()
        {
            "SpinWithDrawStartDialog_p",
            "Prefab/UI/KindOfSymbolDialog_p",
            "Prefab/UI/WithDrawDialog_p",
            "Prefab/UI/LuckyCashDialog_p",
            "CardSystemLotteryDialog_p",
            "CardSystemGetCardDialog_p",
            "CardSystemCollectionDialog_p",
        };
        foreach (string path in resourcePaths)
        {
            string[] parts = path.Split('/');
            string name = parts[parts.Length - 1];
            if (cache.ContainsKey(name))
            {
                continue;
            }
            // 开始异步加载
            ResourceRequest request = Resources.LoadAsync(path);
            // 等待加载完成
            while (!request.isDone)
            {
                yield return null;
            }
            // 确保资源加载成功
            if (request.asset != null)
            {
                cache.Add(name,request.asset);
                Debug.Log("PreLoadResources LoadResource success====="+path);
            }
            else
            {
                Debug.LogError("PreLoadResources LoadResource error====="+path);
            }
            yield return GameConstants.FrameTime;
        }
        Debug.Log("PreLoadResources All Resources is Loaded");
        loadSuccess = true;
    }

    //后台加载的资源
    IEnumerator BackLoadResources()
    {
        string[] resourcePaths = new string[]
        {
            "SpinWithDrawEndDialog_p",
            "Prefab/UI/AccountDialog_p",
            "Prefab/UI/SettingDialog_p",
            "Prefab/UI/AccountEnsureDialog_p",
            "WesternTreasure/FreeGameStartDialog",
            "WesternTreasure/FreeGameEndDialog",
            "WesternTreasure/FreeGameRetriggerDialog",
            "WesternTreasure/WesternTreasureMiniDialog",
            "WesternTreasure/WesternTreasureManorDialog",
            "WesternTreasure/WesternTreasureMajorDialog",
            "WesternTreasure/WesternTreasureGrandDialog",
            "WesternTreasure/PaytablePanel",
        };
        foreach (string path in resourcePaths)
        {
            // 开始异步加载
            ResourceRequest request = Resources.LoadAsync(path);
            // 等待加载完成
            while (!request.isDone)
            {
                yield return null;
            }
            // 确保资源加载成功
            if (request.asset != null)
            {
                string[] parts = path.Split('/');
                string name = parts[parts.Length - 1];
                cache.Add(name,request.asset);
                Debug.Log("[ResourceCache][BackLoadResources] success====="+path);
            }
            else
            {
                Debug.LogError("[ResourceCache][BackLoadResources] error====="+path);
            }
            yield return null;
        }
        Debug.Log("[ResourceCache][BackLoadResources] All Resources is Loaded");
       
    }
    
    public void CacheResource(string path, Object resource)
    {
        if (!Instance.cache.ContainsKey(path))
        {
            Instance.cache.Add(path, resource);
        }
    }
    
    public T GetResource<T>(string path) where T : Object
    {
        if (Instance.cache.TryGetValue(path, out Object resource))
        {
            return (T)resource;
        }
        return null;
    }
    
    public bool HasResource(string path)
    {
        return Instance.cache.ContainsKey(path);
    }

    public override void Dispose()
    {
        base.Dispose();
        ClearCache();
        if(backLoad!=null) 
            StopCoroutine(backLoad);
            backLoad = null;
        if(preLoad!=null) 
            StopCoroutine(preLoad);
            preLoad = null;    
    }

    public void ClearCache()
    {
        Instance.cache.Clear();
        Resources.UnloadUnusedAssets();
    }
}
