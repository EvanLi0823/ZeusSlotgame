/// <summary>
/// CCPA收集信息无需授权
/// 售卖信息需要授权，目前Flurry有售卖的情况，并且提供了关闭售卖的接口
/// </summary>
public class CCPA : IPrivacyImplThird
{
    public bool CanCollectData
    {
        get { return true; }
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