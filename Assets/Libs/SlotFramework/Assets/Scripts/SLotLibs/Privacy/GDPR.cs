
/// <summary>
/// GDPR 需授权后才能收集信息
/// </summary>
public class GDPR : IPrivacyImplThird
{
    public bool CanCollectData
    {
        get { return IsGranted; }
    }

    public bool CanShowSettingItem
    {
        get { return IsGranted; }
    }

    public bool IsGranted { get; set; }

    public void OnGranted(bool isGranted)
    {
        Plugins.NativeAPIs.SetGdprGranted (isGranted);
    }
}