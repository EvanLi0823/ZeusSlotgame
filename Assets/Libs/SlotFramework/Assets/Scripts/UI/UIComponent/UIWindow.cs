using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace UI.UIComponent
{
    public class UIWindow : MonoBehaviour
    {
        // Use this for initialization
        void Start()
        {
        }

        protected virtual void InitUI()
        {
        }

        public bool isShow()
        {
            return gameObject != null && gameObject.activeSelf;
        }

        public bool isHidden()
        {
            return gameObject != null && !gameObject.activeSelf;
        }

        public virtual void Close(Button source = null)
        {
            transform.gameObject.SetActive(false);
        }

        public virtual bool HasClosed()
        {
            return !transform.gameObject.activeSelf;
        }

        public virtual void Dispose()
        {
            if (!HasClosed())
            {
                Close();
            }

            transform.SetParent(null, false);
            Destroy(gameObject);
        }

        public Text GeText(string name)
        {
            return GameObject.Find(name).GetComponent<Text>();
        }
    }
}