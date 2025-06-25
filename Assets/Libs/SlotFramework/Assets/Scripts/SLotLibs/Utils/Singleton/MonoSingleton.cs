using UnityEngine;

public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
{
    private static T _Instance = null;

    public static bool applicationIsQuitting = false;
    public static T Instance
    {
        get
        {
            if (_Instance == null)
            {
                if (applicationIsQuitting)
                {
                    return _Instance;
                }
                _Instance = GameObject.FindObjectOfType(typeof(T)) as T;
                if (_Instance == null)
                {
                    GameObject go = new GameObject(typeof(T).Name);
                    _Instance = go.AddComponent<T>();
                }
            }

            return _Instance;
        }
    }

    /// <summary>
    /// Awake：设置为不销毁，初始化
    /// </summary>
    /// <param></param>
    /// <returns></returns>
    private void Awake()
    {
        if (_Instance == null)
        {
            _Instance = this as T;
        }

        DontDestroyOnLoad(gameObject);
        Init();
    }

    /// <summary>
    /// 析构：清理工作，释放引用，销毁对象
    /// </summary>
    /// <param></param>
    /// <returns></returns>
    private void OnDestroy()
    {
        applicationIsQuitting = true;
        Dispose();
        MonoSingleton<T>._Instance = null;
        UnityEngine.Object.Destroy(gameObject);
    }

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param></param>
    /// <returns></returns>
    protected virtual void Init()
    {

    }

    /// <summary>
    /// 清理
    /// </summary>
    /// <param></param>
    /// <returns></returns>
    public virtual void Dispose()
    {

    }


}