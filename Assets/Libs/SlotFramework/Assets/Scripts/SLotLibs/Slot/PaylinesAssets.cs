using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaylinesAssets : MonoBehaviour {
	[SerializeField]
	[Header("所有payline线的图片")]
	public List<Sprite> m_PayLineImageAssets = new List<Sprite>();

	public static PaylinesAssets Instance = null;
	private const string DICT_KEY = "PayLinePoints";

	//所有payline的顶点坐标
	private List< List<Vector2> > m_LinePointList = new List<List<Vector2>>();
	//TODO: 分析plist信息 获取图片
	private void ParsePlist()
	{
		if (BaseSlotMachineController.Instance == null) {
			return;
		}

		SlotMachineConfig slotConfig = BaseSlotMachineController.Instance.slotMachineConfig;
		if (!slotConfig.extroInfos.infos.ContainsKey (DICT_KEY)) {
			return;
		}

		m_LinePointList.Clear ();
		List<object> children = slotConfig.extroInfos.infos [DICT_KEY] as List<object>;
		for (int i = 0; i < children.Count; i++) {
			List<Vector2> lineVector = new List<Vector2> ();
			List<object> child = children [i] as List<object>;
			for (int j = 0; j < child.Count; j++) {
				List<object> v = child [j] as List<object> ;
				float v1 = Utils.Utilities.CastValueFloat (v [0]);
				float v2 = Utils.Utilities.CastValueFloat (v [1]);
				Vector2 vector = new Vector2 (v1, v2);
				lineVector.Add (vector);
			}
			m_LinePointList.Add (lineVector);
		}
	}

	public Sprite GetPayLineSprite(int payLineId)
	{
		if (payLineId >= m_PayLineImageAssets.Count) {
			return m_PayLineImageAssets[0];
		}
		return m_PayLineImageAssets [payLineId];
	}

	public List<Vector2> GetPayLinePoints(int payLineId)
	{
		//test
		if (payLineId >= m_LinePointList.Count) {
			Debug.LogError ( payLineId + "划线的点索引大于最大的值了");
			return m_LinePointList[0];
		}

		return m_LinePointList [payLineId];
	}

	void Awake()
	{
		Instance = this;
		Messenger.AddListener (GameConstants.OnSlotMachineSceneInit, ParsePlist);
	}

	void OnDestroy()
	{
		Instance = null;
		Messenger.RemoveListener (GameConstants.OnSlotMachineSceneInit, ParsePlist);
	}
}
