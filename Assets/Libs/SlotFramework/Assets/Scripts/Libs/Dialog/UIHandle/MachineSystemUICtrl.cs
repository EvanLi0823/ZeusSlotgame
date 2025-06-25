using System;
using UnityEngine;
using DG.Tweening;


namespace Libs
{
    /// <summary>
    /// 必须在进入机器的Awake之后，调用才可以
    /// </summary>
    public class MachineSystemUICtrl : MonoBehaviour
    {
        [SerializeField] private GameObject jackpotTips;
        [SerializeField] private GameObject jackpotTextPanel;
        [SerializeField] private GameObject textPanel;
        [SerializeField] private GameObject BottomPanel;
        [SerializeField] private GameObject ProcessUIAnchor;
        [SerializeField] private GameObject LeftUIAnchor;
        //[SerializeField] private GameObject DebugTest;
        [SerializeField] private GameObject RewardSpin;
        [SerializeField] private GameObject TopBannerNew;
        
        private RectTransform mTrans;
        private CanvasGroup jpTipsCG;
        private CanvasGroup jpTextCG;
        private CanvasGroup textCG;
        private CanvasGroup bottomCG;
        private CanvasGroup processUICG;
        private CanvasGroup leftUICG;
        //private CanvasGroup debugTestCG;
        private CanvasGroup rewardSpinCG;
        private CanvasGroup topBannerCG;
        private void Awake()
        {
            mTrans = transform as RectTransform;
            InitComponents();
            Messenger.AddListener<ShowUIType,float>(GameConstants.SHOW_MACHINE_SYSTEM_UI_KEY,ShowMachineSystemUI);
            Messenger.AddListener<GameObject>(GameConstants.LEFT_UI_CREATE_SUCCEED,RefreshDynamicObject);
            Messenger.AddListener<GameObject>(GameConstants.CORNER_UI_CREATE_SUCCEED,RefreshDynamicObject);

        }

        #if UNITY_EDITOR
//        private void Start()
//        {
//            Invoke("TestHideUI",10f);
//            Invoke("TestShowUI",20);
//        }
//
//        void TestHideUI()
//        {
//            Messenger.Broadcast<ShowUIType,float>(GameConstants.SHOW_MACHINE_SYSTEM_UI_KEY,ShowUIType.HideAll,5f);
//        }
//
//        void TestShowUI()
//        {
//            Messenger.Broadcast<ShowUIType,float>(GameConstants.SHOW_MACHINE_SYSTEM_UI_KEY,ShowUIType.ShowAll,5f);
//        }
        #endif
        private void OnDestroy()
        {
            StopDoTween();
            Messenger.RemoveListener<ShowUIType,float>(GameConstants.SHOW_MACHINE_SYSTEM_UI_KEY,ShowMachineSystemUI);
            Messenger.RemoveListener<GameObject>(GameConstants.LEFT_UI_CREATE_SUCCEED,RefreshDynamicObject);
            Messenger.RemoveListener<GameObject>(GameConstants.CORNER_UI_CREATE_SUCCEED,RefreshDynamicObject);

        }

        private void RefreshDynamicObject(GameObject go)
        {
            if (currentShow ||go==null) return;
            ParticleSystem[] list = go.GetComponentsInChildren<ParticleSystem>();
            if (list != null)
            {
                foreach (ParticleSystem ps in list)
                {
                    Renderer renderer =  ps.GetComponent<Renderer>();
                    if(renderer==null) continue;
                    renderer.enabled = false;
                }
            }
            SpriteRenderer[] srs = go.GetComponentsInChildren<SpriteRenderer>();
            if (srs != null)
            {
                foreach (SpriteRenderer ps in srs)
                {
                    ps.DOFade( 0, 0);
                }
            }
        }
        private bool currentShow = true;
        void ShowMachineSystemUI(ShowUIType type,float fadeTime = 0f)
        {
            switch (type)
            {
                case ShowUIType.None:
                    break;
                case ShowUIType.ShowHorizonal:
                    ShowHorizonalSystemUI(true, fadeTime);
                    break;
                case ShowUIType.ShowVertical:
                    ShowVerticalSystemUI(true, fadeTime);
                    
                    break;
                case ShowUIType.ShowAll:
                    ShowHorizonalSystemUI(true, fadeTime);
                    ShowVerticalSystemUI(true, fadeTime);
                    break;
                case ShowUIType.HideHorizonal:
                    ShowHorizonalSystemUI(false, fadeTime);
                    break;
                case ShowUIType.HideVertial:
                    ShowVerticalSystemUI(false, fadeTime);
                    break;
                case ShowUIType.HideAll:
                    ShowHorizonalSystemUI(false, fadeTime);
                    ShowVerticalSystemUI(false, fadeTime);
                    break;
            }
        }

        void ShowHorizonalSystemUI(bool show,float fadeTime = 0f)
        {
            SetShowState(jpTipsCG, show, fadeTime);
            SetShowState(jpTextCG, show, fadeTime);
            SetShowState(textCG, show, fadeTime);
            SetShowState(bottomCG, show, fadeTime);
            SetShowState(processUICG, show, fadeTime);
            SetShowState(rewardSpinCG, show, fadeTime);
            SetShowState(topBannerCG, show, fadeTime);
        }

        private void SetShowState(CanvasGroup go,bool show,float fadeDuration)
        {

            if (go == null) return;
            Action psCB = () =>
            {
                ParticleSystem[] list = go.GetComponentsInChildren<ParticleSystem>();
                if (list != null)
                {
                    foreach (ParticleSystem ps in list)
                    {
                        Renderer renderer =  ps.GetComponent<Renderer>();
                        if(renderer==null) continue;
                        renderer.enabled = show;
                    }
                }
            };
            go.DOFade(show?1:0,fadeDuration)
                .OnStart(() =>
                {
                    if(!show) psCB();
                    if (!show)
                    {
                        go.interactable = false;
                        go.blocksRaycasts = false;
                    }
                        
                })
                .OnComplete(() =>
                {
                    if (show) { psCB(); }

                    if (show)
                    {
                        go.interactable = true;
                        go.blocksRaycasts = true;
                    }
                });
           

            SpriteRenderer[] srs = go.GetComponentsInChildren<SpriteRenderer>();
            if (srs != null)
            {
                foreach (SpriteRenderer ps in srs)
                {
                    ps.DOFade(show ? 1 : 0, fadeDuration);
                }
            }
        }
        
        void ShowVerticalSystemUI(bool show,float fadeTime = 0f)
        {
            currentShow = show;
            SetShowState(leftUICG, show, fadeTime);
        }
        

        void StopDoTween()
        {
            KillAnimation(jpTipsCG);
            KillAnimation(jpTextCG);
            KillAnimation(textCG);
            KillAnimation(bottomCG);
            KillAnimation(processUICG);
            KillAnimation(leftUICG);
            //KillAnimation(debugTestCG);
            KillAnimation(rewardSpinCG);
            KillAnimation(topBannerCG);
        }

        void KillAnimation(CanvasGroup cg)
        {
            if (cg == null) return;
            cg.DOKill();
            SpriteRenderer[] srs = cg.GetComponentsInChildren<SpriteRenderer>();
            if (srs != null)
            {
                foreach (SpriteRenderer ps in srs)
                {
                    ps.DOFade(1 ,0);
                }
            }
        }

        void InitComponents()
        {
            if (mTrans.childCount == 0) return;
            Transform mChild = mTrans.GetChild(0);
            if (mChild == null) return;
            int number = mChild.childCount;
            if (number > 9)
            {
                Debug.LogError("machine system ui show and hide not right!");
                return;
            }

            if (jackpotTips == null) jackpotTips = Util.FindObjectEx<GameObject>(mTrans, "Anchor/JackpotTips/");
            if (jackpotTextPanel == null) jackpotTextPanel = Util.FindObjectEx<GameObject>(mTrans, "Anchor/JackpotTextPanel/");
            if (textPanel == null) textPanel = Util.FindObjectEx<GameObject>(mTrans, "Anchor/TextPanel/");
            if (BottomPanel == null) BottomPanel = Util.FindObjectEx<GameObject>(mTrans, "Anchor/BottomPanel/");

            if (ProcessUIAnchor == null) ProcessUIAnchor = Util.FindObjectEx<GameObject>(mTrans, "Anchor/ProcessUIAnchor/");
            if (LeftUIAnchor == null) LeftUIAnchor = Util.FindObjectEx<GameObject>(mTrans, "Anchor/LeftUIAnchor/");
            //if (DebugTest == null) DebugTest = Util.FindObjectEx<GameObject>(mTrans, "Anchor/DebugTest/");
            if (RewardSpin == null) RewardSpin = Util.FindObjectEx<GameObject>(mTrans, "Anchor/RewardSpin/");

            if (TopBannerNew == null) TopBannerNew = Util.FindObjectEx<GameObject>(mTrans, "Anchor/TopBannerNew/");
         
            if (jackpotTips == null)
            {
                Debug.LogError($"machine system ui show and hide not right jackpotTips!");
                return;
            }
            if (jackpotTextPanel == null)
            {
                Debug.LogError("machine system ui show and hide not right jackpotTextPanel!");
                return;
            }
            if (textPanel == null)
            {
                Debug.LogError("machine system ui show and hide not right textPanel!");
                return;
            }
            if (BottomPanel == null)
            {
                Debug.LogError("machine system ui show and hide not right BottomPanel!");
                return;
            }
            if (ProcessUIAnchor == null)
            {
                Debug.LogError("machine system ui show and hide not right ProcessUIAnchor!");
                return;
            }
            if (LeftUIAnchor == null)
            {
                Debug.LogError("machine system ui show and hide not right LeftUIAnchor!");
                return;
            }
            //if (DebugTest == null)
            //{
            //    Debug.LogError("machine system ui show and hide not right DebugTest!");
            //    return;
            //}
            if (RewardSpin == null)
            {
                Debug.LogError("machine system ui show and hide not right RewardSpin!");
                return;
            }
            if (TopBannerNew == null)
            {
                Debug.LogError("machine system ui show and hide not right TopBannerNew!");
                return;
            }

            jpTipsCG= jackpotTips.GetComponent<CanvasGroup>();
            if (jpTipsCG == null) jpTipsCG = jackpotTips.AddComponent<CanvasGroup>();
            
            jpTextCG= jackpotTextPanel.GetComponent<CanvasGroup>();
            if (jpTextCG == null) jpTextCG = jackpotTextPanel.AddComponent<CanvasGroup>();

            textCG= textPanel.GetComponent<CanvasGroup>();
            if (textCG == null) textCG = textPanel.AddComponent<CanvasGroup>();

            bottomCG= BottomPanel.GetComponent<CanvasGroup>();
            if (bottomCG == null) bottomCG = BottomPanel.AddComponent<CanvasGroup>();

            processUICG= ProcessUIAnchor.GetComponent<CanvasGroup>();
            if (processUICG == null) processUICG = ProcessUIAnchor.AddComponent<CanvasGroup>();

            leftUICG= LeftUIAnchor.GetComponent<CanvasGroup>();
            if (leftUICG == null) leftUICG = LeftUIAnchor.AddComponent<CanvasGroup>();

            //debugTestCG= DebugTest.GetComponent<CanvasGroup>();
            //if (debugTestCG == null) debugTestCG = DebugTest.AddComponent<CanvasGroup>();

            rewardSpinCG= RewardSpin.GetComponent<CanvasGroup>();
            if (rewardSpinCG == null) rewardSpinCG = RewardSpin.AddComponent<CanvasGroup>();

            topBannerCG= TopBannerNew.GetComponent<CanvasGroup>();
            if (topBannerCG == null) topBannerCG = TopBannerNew.AddComponent<CanvasGroup>();
            ShowHorizonalSystemUI(true);
            ShowVerticalSystemUI(true);
        }
        
    }

    public enum ShowUIType
    {
        None = 0,
        ShowHorizonal = 1,
        HideHorizonal = 2,
        ShowVertical = 4,
        HideVertial = 8,
        ShowAll = 5,
        HideAll = 10
        
    }
}