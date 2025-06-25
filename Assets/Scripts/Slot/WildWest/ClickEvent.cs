using UnityEngine;

namespace WildWest{
    public class ClickEvent : MonoBehaviour
    {
        private WildWestGatherDialog dialog;

        public void InitEvent(WildWestGatherDialog _dialog)
        {
            dialog = _dialog;
        }

        public void OnClick()
        {
            dialog.ClickFinish();
        }
    }
}
