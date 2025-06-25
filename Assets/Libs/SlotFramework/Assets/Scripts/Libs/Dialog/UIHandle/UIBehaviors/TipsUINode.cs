using System;
using Classic;
using UnityEngine;

namespace Libs
{
    public class TipsUINode : MonoBehaviour
    {
        private GameObject go;

        private void Awake()
        {
            //
            if (go == null)
            {
                go = new GameObject("TipsPanel");
                go.transform.SetParent(transform, false);
                RectTransform mTrans = go.AddComponent<RectTransform>();
                mTrans.anchorMin = Vector2.zero;
                mTrans.anchorMax = Vector2.one;
                mTrans.offsetMin = Vector2.zero;
                mTrans.offsetMax = Vector2.zero;
                mTrans.localPosition = Vector3.zero;
                mTrans.localScale = Vector3.one;
                go.layer = LayerMask.NameToLayer("UI");
                // go.AddComponent<FixedFullY>();
            }

            UIManager.Instance.SetTipParentNode(go);
        }
    }
}