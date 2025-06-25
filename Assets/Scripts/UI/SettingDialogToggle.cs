using UnityEngine;
using System.Collections;
using Classic;
using TMPro;
using UnityEngine.UI;
public class SettingDialogToggle : MonoBehaviour {

    public TextMeshProUGUI isOffText;
    public TextMeshProUGUI isOnText;
    public void OnToggleChange(){
        if (GetComponent<Toggle>().isOn) {
            isOffText.gameObject.SetActive (false);
            isOnText.gameObject.SetActive (true);
        } else {
            isOffText.gameObject.SetActive (true);
            isOnText.gameObject.SetActive (false);
        }
    }
}
