using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;
using Classic;
using Utils;

public class LeftMessagePanel : MonoBehaviour {

	[SerializeField]
	private  Image bgImage;
	[SerializeField]
	private  TextMeshProUGUI lineTitle;
	[SerializeField]
	private  TextMeshProUGUI lineIndex;
    [SerializeField]
    private  TextMeshProUGUI lineWin;
	[SerializeField]
	private  TextMeshProUGUI  payTitle;
	[SerializeField]
	private  TextMeshProUGUI payCoins;

	void Start () {
		ClearMassgae ();
		Messenger.AddListener<AwardResult.AwardPayLine> (SlotControllerConstants.ShowAwardAnimation, ShowWinTips);
		Messenger.AddListener (SlotControllerConstants.OnSpinStart,ClearMassgae);
	}
	void OnDestroy() 
	{
		Messenger.RemoveListener<AwardResult.AwardPayLine> (SlotControllerConstants.ShowAwardAnimation, ShowWinTips);
		Messenger.RemoveListener (SlotControllerConstants.OnSpinStart,ClearMassgae);
	}

	void ShowWinTips(AwardResult.AwardPayLine awardPayLine){
		bgImage.gameObject.SetActive (true);
		if (awardPayLine.LineIndex >= 0) {
			lineTitle.text = "LINE #";
			lineIndex.text = (awardPayLine.LineIndex+1).ToString ();
            lineWin.text = "WIN";
			payTitle.text = "PAYS $";
            payCoins.text = Utilities.CastToIntStringWithThousandSeparator(awardPayLine.awardValue * AwardResult.CurrentResultBet);

			if (BaseSlotMachineController.Instance.reelManager.isFreespinBonus) {
				payCoins.text += " * " + BaseSlotMachineController.Instance.freespinGame.multiplier;
			}
		} else {
			if (awardPayLine.LineIndex == -2) {
				lineTitle.text = "BONUS WIN";
			} else if (awardPayLine.LineIndex == -3) {
				lineTitle.text = "SCATTER WIN";
			} else {
				lineTitle.text = "MULTIWAY WIN";
			}
            lineWin.text = "";
			lineIndex.text = "";
			payTitle.text = "PAYS $";
            payCoins.text = Utilities.ThousandSeparatorNumber(awardPayLine.awardValue * AwardResult.CurrentResultBet);
		}
	}

	void ClearMassgae(){
		bgImage.gameObject.SetActive (false);
		lineTitle.text = "";
		lineIndex.text = "";
        lineWin.text = "";
		payCoins.text = "";
		payTitle.text = "";
	}
}
