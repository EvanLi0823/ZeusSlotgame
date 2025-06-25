using Classic;

/// <summary>
/// 隐私政策相关接口，
/// 封装了GDPR和CCPA的相关操作.
/// </summary>
public class Privacy
{
    private static PrivacyImpl _privacyImpl;

    private static IPrivacyImplThird _privacyImplThird;

    public static bool HasSelectedGDPR
    {
        get { return _privacyImpl.HasSelectedGDPR; }
        set { _privacyImpl.HasSelectedGDPR = value; }
    }

    public static void Init()
    {
        _privacyImpl.Init(Privacy.isGranted());
    }

    public static void OnGranted(bool isGranted)
    {
        _privacyImpl.OnGranted(isGranted);
        BaseGameConsole.ActiveGameConsole().LogBaseEvent(Analytics.PrivacyStatus, "isGranted", isGranted, "privacyType",
            _privacyImpl.GetPrivacyType().ToString());
    }

    public static bool CanCollectData()
    {
        return _privacyImpl.CanCollectData();
    }
    
    public static bool CanShowSettingItem()
    {
        return _privacyImpl.CanShowSettingItem();
    }


    public static bool isGranted()
    {
        return _privacyImpl.isGranted();
    }

    public static bool IsOpenTerms_EUDialog()
    {
        return _privacyImpl.IsOpenTerms_EUDialog();
    }

    public static PrivacyType GetPrivacyType()
    {
        return _privacyImpl.GetPrivacyType();
    }

    static Privacy()
    {
        _privacyImpl = new PrivacyImpl();
    }
}