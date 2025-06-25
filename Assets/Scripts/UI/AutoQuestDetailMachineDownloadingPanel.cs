using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class AutoQuestDetailMachineDownloadingPanel : MonoBehaviour {

    public Slider machineDownloadProgressSlider;
    public TextMeshProUGUI machineDownloadProgressText;
    
    public string MachineName { get; set; }

    // Use this for initialization
    void Start () {
	    
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void Refresh(string machineName) {
        MachineName = machineName;
    }

    public void SetDownloadingProgress(float progress) {
        if (machineDownloadProgressSlider != null) {
            machineDownloadProgressSlider.value = progress;   
        }

        if (machineDownloadProgressText != null) {
            machineDownloadProgressText.text = Mathf.Round(progress * 100) + "%";
        }
    }
}
