using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Libs
{
    /// <summary>
    /// Dialog orientation adapter.
    ///1.SceneExchange 单独处理
    ///2.竖版机器专用对话框处理(bonus框 Jackpot框、freespin专有弹框)
    ///3.竖版通用对话框处理(except sceneExchange 升级框 解锁机器框、n kind of X框、bigwin框)
    ///4.横版通用对话框处理(except sceneExchange)
    /// 
    /// </summary>
    public class DialogOrientationAdapter : MonoBehaviour
    {
        public const string ADJUST_SCREEN_ORIENTATION_SETTINGS = "AdjustScreenOrientationSettings";
        UIBase uiBase;//m_Data 属性
        GameObject mask;//此遮罩是用于当切换横竖屏时，打开，切换完毕再将其关闭，否则图像会跳，同时此遮罩预制体内必须为false，App启动时，不会对其处理
        private void Start()
        {
            uiBase = transform.GetComponent<UIBase>();
            if (!(uiBase is UIDialog))
            {
                Destroy(this);
                return;
            }
            if (GetComponent<SceneExchange>() != null )
            {
                (uiBase as UIDialog).isPortrait = true;
                mask = Util.FindObject<GameObject>(transform, "Mask/");
                if (mask != null) mask.SetActive(true);
            }
            if (GetComponent<FirstToLobbyDialog>() != null)
            {
                (uiBase as UIDialog).isPortrait = true;
                mask = Util.FindObject<GameObject>(transform, "Mask/");
                if (mask != null) mask.SetActive(false);
            }
            
            StartCoroutine(AdjustScreenOrientationSettings());
        }

        IEnumerator AdjustScreenOrientationSettings()
        {
            if (GetComponent<SceneExchange>() != null || GetComponent<FirstToLobbyDialog>() != null)
            {
                yield return new WaitForEndOfFrame();
                yield return new WaitForEndOfFrame();
                yield return GameConstants.TwoIn10SecondWait;
                if (mask != null) mask.SetActive(false);
            }
        }
    }
}

