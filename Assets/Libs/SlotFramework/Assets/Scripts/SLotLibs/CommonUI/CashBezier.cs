using UnityEngine;

namespace Libs
{
    public class CashBezier : CoinsBezier
    {
        private string BezierObjectPath = "Prefab/Shared/CashImage";
        private static string BezierPanelPath = "Prefab/Shared/CashBezierPanel";

        private static CashBezier _instance;
        private string PoolKey = "BezierCash";

        public static CashBezier Instance
        {
            get
            {
                if (_instance == null)
                {
                    // 尝试获取场景中已存在的，active的同类对象
                    _instance = GameObject.FindObjectOfType<CashBezier>();
                    if (_instance == null)
                    {
                        // 初始化
                        _instance = Instantiate(
                                Libs.ResourceLoadManager.Instance.LoadResource<GameObject>(BezierPanelPath))
                            .GetComponent<CashBezier>();
                        // 向场景中导入指定prefab，并从中获取对象
                        // 设置transform并移动至最上层
                        _instance.transform.SetParent(Libs.UIManager.Instance.Root.transform.parent);
                        (_instance.transform as RectTransform).localPosition = new Vector3(0, 0, 0);

                        _instance.transform.SetAsLastSibling();
                    }

                    _instance.BezierObject =
                        Libs.ResourceLoadManager.Instance.LoadResource<GameObject>(_instance.BezierObjectPath);
                    Canvas canvas = _instance.GetComponent<Canvas>();
                    if (canvas != null)
                    {
                        canvas.overrideSorting = true;
                    }

                    BezierMiddlePointSettings();
                    BezierMiddlePointOffsetSettings();
                    SizeScaleChangeSettings();
                    TimeScaleChangeSettings();
                    TimeDurationSettings();
                    PopDurationSettings();
                }

                if (SkySreenUtils.CurrentOrientation == ScreenOrientation.Portrait)
                {
                    if (IphoneXAdapter.IsIphoneX())
                    {
                        (_instance.transform as RectTransform).localScale =
                            new Vector3(iphoneXScale.x, iphoneXScale.y, 1);
                    }
                    else if (SkySreenUtils.GetScreenSizeType() == SkySreenUtils.ScreenSizeType.Size_16_9)
                    {
                        (_instance.transform as RectTransform).localScale =
                            new Vector3(iphoneScale.x, iphoneScale.y, 1);
                    }
                    else if (SkySreenUtils.GetScreenSizeType() == SkySreenUtils.ScreenSizeType.Size_4_3)
                    {
                        (_instance.transform as RectTransform).localScale = new Vector3(ipadScale.x, ipadScale.y, 1);
                    }
                }
                else
                {
                    (_instance.transform as RectTransform).localScale = new Vector3(1, 1, 1);
                }

                return _instance;
            }
        }
    }
}