
public enum PrivacyType
{
    Unknow,
    GDPR,
    CCPA
}

public class PrivacyImpl
{
   
	
    /// <summary>
    /// 用户是否做出了选择.
    /// </summary>
    private const string HasSelectedKey = "GDPRSelected";
	
    private bool grantedExecuted = false;

    private PrivacyType _privacyType;

    private IPrivacyImplThird _privacyImplThird;

    public bool HasSelectedGDPR
    {
        get {
            return SharedPlayerPrefs.GetPlayerBoolValue (HasSelectedKey, false);
        }
        set {
            SharedPlayerPrefs.SetPlayerPrefsBoolValue (HasSelectedKey, value);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="isGranted"></param>
    public void Init(bool isGranted)
    {
        //现在虽然是只调用了OnGranted，似乎是可以优化掉这个init函数的，
        //但是为了区分init和由于click导致ongranted，所以需要保留init。
        OnGranted(isGranted);
    }

    public void OnGranted(bool isGranted)
    {
        if (!grantedExecuted)
        {
            //ccpa可以收集用户数据.
            if (isGranted || _privacyType == PrivacyType.CCPA)
            {
                grantedExecuted = true;
            }
        }

        Plugins.Configuration.GetInstance().SetValueWithPath<bool>("isGdprGranted", isGranted);
        if (_privacyImplThird != null)
        {
            SharedPlayerPrefs.SetPlayerPrefsBoolValue("isGdprGranted", isGranted);
            _privacyImplThird.IsGranted = isGranted;
            _privacyImplThird.OnGranted(isGranted);
        }
    }

    public bool isGranted()
    {
        return _privacyImplThird == null || _privacyImplThird.IsGranted;
    }

    public bool CanCollectData()
    {
        return _privacyImplThird == null || _privacyImplThird.CanCollectData;
    }
    
    public bool CanShowSettingItem()
    {
        return _privacyImplThird == null || _privacyImplThird.CanShowSettingItem;
    }

    public bool isGdprUser(){
        return Plugins.Configuration.GetInstance ().GetValueWithPath<bool> ("isGdprUser", false);
    }

    public bool isCCPAUser()
    {
        //由于library库还没有写好CCPA的接口，默认所有用户都是CCPA的，欧洲用户是GDPR
        return true;
    }

    public bool IsOpenTerms_EUDialog()
    {
        return _privacyType != PrivacyType.Unknow && !HasSelectedGDPR;
    }

    public PrivacyType GetPrivacyType()
    {
        return _privacyType;
    }

    public PrivacyImpl()
    {
        if(isGdprUser())
        {
            _privacyType = PrivacyType.GDPR;
            _privacyImplThird = new GDPR();
        }
        else if (isCCPAUser())
        {
            _privacyType = PrivacyType.CCPA;
            _privacyImplThird = new CCPA();
        }
        else
        {
            _privacyType = PrivacyType.Unknow;
        }

        if (_privacyImplThird != null)
        {
            _privacyImplThird.IsGranted = SharedPlayerPrefs.GetPlayerBoolValue("isGdprGranted",
                Plugins.Configuration.GetInstance().GetValueWithPath<bool>("isGdprGranted", false));
        }
    } 
}