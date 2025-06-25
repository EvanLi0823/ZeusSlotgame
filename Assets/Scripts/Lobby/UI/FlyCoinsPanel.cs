using System;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;
using Libs;

public class FlyCoinsPanel : MonoBehaviour
{
    #region coins panel属性

    public CoinsPanelItem h_CoinsPanel;//横版coins panel
    public CoinsPanelItem v_CoinsPanel;//竖版非pad coins panel
    public CoinsPanelItem v_CoinsPanel_iPad;//竖版 pad coins panel

    private CoinsPanelItem currentCoinsPanel;//当前使用的coins panel

    private string h_CoinsPanel_Path = "Adaption/CoinsPanel_Horizontal";
    private string v_CoinsPanel_Path = "Adaption/CoinsPanel_Vertical";
    private string v_CoinsPanel_iPad_Path = "Adaption/CoinsPanel_Vertical_iPad";

    private bool changeLobbyOver = true;
    public bool ChangeLobbyOver => changeLobbyOver;

    #endregion

    #region 适配属性

    private Vector3 target_Trans; //Top CoinsPanel显示位置
    
    private GameObject bannerC = null;//banner机器场景的根物体
    
    public Transform adaptionObj;//适配顶部
    [HideInInspector]
    public Transform topPanelParentScale = null;//top panel父物体 transform
    [HideInInspector]
    public RectTransform topPanelRect = null;//top panel rect transform

    private RectTransform selfRect = null;//自身rect transform
    
    private float heightProportion = 1f;//屏幕高度比
    private float curOrthographicSize = 1f;//当前相机 OrthographicSize 值
    private float canvasScaler = 1f;//bannerC canvasScaler
    
    private float HeightProportion => heightProportion * canvasScaler;//竖版机器特殊处理高度比

    #endregion

    #region 动画配置

    private float tweenerDelay = 0f;
    private float tweenerDuration = 3f;
    private long initNum = 0;

    #endregion

    #region 动画

    private DelayAction tweenAction; //动画事件
    private Tweener tweener; //DOTween动画

    #endregion

    private bool isOpen = true;
    private bool isOpenCloseWithFly = true;
    public bool IsOpenCloseWithFly => isOpenCloseWithFly;

    //判断是否为竖版
    private bool IsPortrait
    {
        get
        {
            if (BaseGameConsole.singletonInstance.IsInLobby()) return false;
            SlotMachineConfig config = BaseGameConsole.ActiveGameConsole().SlotMachineConfig(SwapSceneManager.Instance.GetLogicSceneName());
            if (config != null && config.IsPortrait)
            {
                return true;
            }
        
            return false;
        }
    }

    private bool IsCanShowInSpecialDialog
    {
        get
        {
            UIDialog dialog = UIManager.Instance.GetNowActiveDialog();
            if (dialog == null) return true;
            if (dialog.IsShowFlyCoinsPanel) return false;
            return true;
        }
    }

    private static FlyCoinsPanel _instance;

    public static FlyCoinsPanel Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.Instantiate(ResourceLoadManager.Instance.LoadResource<FlyCoinsPanel>("Prefab/Shared/FlyCoinsPanel"),UIManager.Instance.Root.transform.parent);
                if (_instance == null)
                {
                    Log.Error("FlyCoinsPanel Is Missing");
                    return null;
                }
                
                _instance.transform.localPosition = Vector3.zero;
                _instance.transform.localScale = Vector3.one;

                Canvas canvas = _instance.GetComponent<Canvas>();
                if (canvas == null)
                {
                    canvas = _instance.gameObject.AddComponent<Canvas>();
                    canvas.overrideSorting = true;
                    canvas.sortingOrder = 200;
                }
            }

            return _instance;
        }
    }

    void Awake()
    {
        isOpen = Plugins.Configuration.GetInstance ().GetValueWithPath<bool> ("ApplicationConfig/FlyCoinsPanelOpen", true);
        isOpenCloseWithFly = Plugins.Configuration.GetInstance ().GetValueWithPath<bool> ("ApplicationConfig/OpenDialogCloseWithFly", true);
        Messenger.AddListener<float, float>(GameConstants.SET_COINSPANEL_TWEENER_PARAM, SetTweenerParam);
        Messenger.AddListener(GameConstants.SWAP_SCENE_NEED_DO,HideSelf);
        Messenger.AddListener(GameConstants.SLOT_CONTROLLER_START_OVER,ChangeLobbyOverRight);

        if (h_CoinsPanel == null) h_CoinsPanel = Util.FindObject<CoinsPanelItem>(transform, h_CoinsPanel_Path);
        if (v_CoinsPanel == null) v_CoinsPanel = Util.FindObject<CoinsPanelItem>(transform, v_CoinsPanel_Path);
        if (v_CoinsPanel_iPad == null) v_CoinsPanel_iPad = Util.FindObject<CoinsPanelItem>(transform, v_CoinsPanel_iPad_Path);
        if (currentCoinsPanel == null) currentCoinsPanel = h_CoinsPanel;
        if (currentCoinsPanel == null)
        {
            Log.Error("FlyCoinsPanel Property currentCoinsPanel is null, please check prefab!");
        }
        selfRect = transform as RectTransform;
        GetDialogCameraWH();
    }

    void OnDestroy()
    {
        Messenger.RemoveListener<float, float>(GameConstants.SET_COINSPANEL_TWEENER_PARAM, SetTweenerParam);
        Messenger.RemoveListener(GameConstants.SWAP_SCENE_NEED_DO,HideSelf);
        Messenger.RemoveListener(GameConstants.SLOT_CONTROLLER_START_OVER,ChangeLobbyOverRight);
    }

    /// <summary>
    /// 设置动画设置
    /// </summary>
    /// <param name="delay"></param>
    /// <param name="duration"></param>
    void SetTweenerParam(float delay = 0f, float duration = 3f)
    {
        tweenerDelay = delay;
        tweenerDuration = duration;
    }

    /// <summary>
    /// 适配和显示
    /// </summary>
    private void  Show()
    {
        if (currentCoinsPanel == null) return;
        Messenger.Broadcast(GameConstants.GetTopPanelScaleAdaption);
        GetDialogCameraWH();
        ChangeTopPanelHeight();
        if(IsPortrait) AdaptionCoinsPanel();
        currentCoinsPanel.gameObject.SetActive(true);
        gameObject.SetActive(true);
    }

    /// <summary>
    /// 重置和隐藏
    /// </summary>
    private void HideSelf()
    {
        curOrthographicSize = 1f;
        canvasScaler = 1f;
        heightProportion = 1f;
        changeLobbyOver = false;

        gameObject.SetActive(false);
    }

    /// <summary>
    /// 获取当前coins panel 相关Transform信息
    /// </summary>
    /// <param name="coinsPanelTrans"></param>
    public void GetCurrentCoinsPanelTrans(Transform coinsPanelTrans)
    {
        if (coinsPanelTrans == null) return;
        RectTransform rect = coinsPanelTrans as RectTransform;
        if (rect != null)
        {
            target_Trans = rect.anchoredPosition;
        }
        if (IsPortrait)
        {
            if (SkySreenUtils.GetScreenSizeType() == SkySreenUtils.ScreenSizeType.Size_4_3)
            {
                currentCoinsPanel = v_CoinsPanel_iPad;
            }
            else
            {
                currentCoinsPanel = v_CoinsPanel;
            }
        }
        else
        {
            currentCoinsPanel = h_CoinsPanel;
        }
        currentCoinsPanel.GetComponent<RectTransform>().anchoredPosition = target_Trans;
    }

    /// <summary>
    /// 设置Coins值，播放动画
    /// </summary>
    /// <param name="number"></param>
    public void SetCoinsNumber(long number)
    {
        if (tweenAction != null && tweenAction.IsPlaying)
        {
            tweenAction.Stop(true);
        }

        if (tweener != null)
        {
            tweener.Kill(true);
        }

        if (number > initNum)
        {
            Show();
            tweenAction = new DelayAction(tweenerDelay, null, () =>
            {
                tweener = DOTween.To(() => this.initNum, x => this.initNum = x, number, tweenerDuration)
                    .OnUpdate(()=>CaculateTxt(currentCoinsPanel.coinText))
                    .OnComplete(() =>
                    {
                        tweenerDelay = 0f;
                        tweenerDuration = 3f;
                        CompleteShow(number);
                        currentCoinsPanel.gameObject.SetActive(false);
                        currentCoinsPanel.transform.localScale = Vector3.one;
                        gameObject.SetActive(false);
                    }).SetUpdate(true);
            });
            tweenAction.Play();
        }
        else
        {
            CompleteShow(number);
        }
    }

    /// <summary>
    /// 展示完成
    /// </summary>
    /// <param name="coins"></param>
    private void CompleteShow(long coins)
    {
        this.initNum = coins;
        this.CaculateTxt(currentCoinsPanel.coinText);
    }

    /// <summary>
    /// 文本赋值
    /// </summary>
    /// <param name="coinText"></param>
    private void CaculateTxt(TextMeshProUGUI coinText)
    {
        if (coinText == null)
        {
            if (tweener != null)
            {
                tweener.Kill();
            }

            return;
        }

        if (initNum == 0)
        {
            coinText.text = "0";
        }
        else
        {
            coinText.text = Utils.Utilities.ThousandSeparatorNumber(initNum); //string.Format("{0:0,0}",initNum);
        }
    }

    /// <summary>
    /// 获取Dialog Camera计算出得宽高
    /// </summary>
    private void GetDialogCameraWH()
    {
        curOrthographicSize = GetFloatNumWithLastThree(UIManager.Instance.UICamera.orthographicSize);
        float pixelsPer = GetComponent<Canvas>().referencePixelsPerUnit;
        float height =  curOrthographicSize * pixelsPer * 2;
        heightProportion = height / Screen.height;
        float screenAspect = Camera.main.aspect;
        float width = height * screenAspect;
        Vector2 rectDelta = new Vector2(width,height);
        selfRect.sizeDelta = rectDelta;
    }

    /// <summary>
    /// 修改Top panel 高度，横版修改scale缩放
    /// </summary>
    private void ChangeTopPanelHeight()
    {
        if (topPanelRect == null) return;
        if (IsPortrait)
        {
            if(bannerC == null) bannerC = GameObject.Find("Main Camera/BannerCanvas");
            if (bannerC != null)
            {
                CanvasScaler scaler = bannerC.GetComponent<CanvasScaler>();
                canvasScaler = scaler.scaleFactor;
            }
        }
        RectTransform adaptionRect = (adaptionObj as RectTransform);
        adaptionRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,topPanelRect.sizeDelta.y * (IsPortrait ? HeightProportion : 1f));
        if (topPanelRect.anchoredPosition.y != 0)
        {
            adaptionRect.anchoredPosition = new Vector2(0,topPanelRect.anchoredPosition.y * HeightProportion);
        }
        else
        {
            adaptionRect.anchoredPosition = Vector2.zero;
        }
        if(topPanelParentScale.localScale.x > 1)
        {
            adaptionRect.localScale = topPanelParentScale.localScale;
        }
    }

    /// <summary>
    /// 当前使用的coins panel进行适配 
    /// </summary>
    private void AdaptionCoinsPanel()
    {
        (currentCoinsPanel.transform as RectTransform).anchoredPosition *= HeightProportion;
        currentCoinsPanel.transform.localScale = Vector3.one * HeightProportion;
    }

    /// <summary>
    /// 保留后三位小数
    /// </summary>
    /// <param name="num"></param>
    /// <returns></returns>
    private float GetFloatNumWithLastThree(float num)
    {
        float m_Num = num * 1000f;
        int t_Num = Mathf.CeilToInt(m_Num);
        return t_Num/1000f;
    }

    public void InitNum(long num)
    {
        this.initNum = num;
        string initNumStr = Utils.Utilities.ThousandSeparatorNumber(initNum);
        h_CoinsPanel.Init(initNumStr);
        v_CoinsPanel.Init(initNumStr);
        v_CoinsPanel_iPad.Init(initNumStr);
    }

    public void ShowWithCoinsFly(long coinsNum)
    {
        if (!LimitOpenCondition()) return;
        SetCoinsNumber(coinsNum);
    }

    private void ChangeLobbyOverRight()
    {
        changeLobbyOver = true;
    }

    private bool LimitOpenCondition()
    {
        if (!isOpen) return false;
        if (!changeLobbyOver) return false;
        if (!IsCanShowInSpecialDialog) return false;
        if (IsPortrait) return false;
        return true;
    }
}