/// <summary>
/// 
/// </summary>
public interface IPrivacyImplThird
{
    bool CanCollectData { get; }
    bool CanShowSettingItem { get; }

    bool IsGranted { get; set; }

    void OnGranted(bool isGranted);
}