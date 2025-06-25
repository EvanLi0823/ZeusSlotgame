using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Libs;
using System;


public class SlotsAssets:MonoBehaviour  {
	public static string CARD_LOADED_Handler = "CardLoadHandler";
	public static string CARD_COLLECT_Handler = "CardCollectHandler";

	public static SlotsAssets Instance
	{
		get{
//			if (_instance == null) {
//				_instance = new SlotsAssets ();
//			}
			return _instance;
		}
	}

	static SlotsAssets()
	{
		if (_instance == null) {
			GameObject go = new GameObject ("CardsAssets");

			DontDestroyOnLoad (go);
			_instance = go.AddComponent<SlotsAssets> ();
		}
	}

//	public Dictionary<string,GameObject>
	public  void PushGameObject(string name,GameObject o, bool isInLoading = false)
	{
		o.gameObject.SetActive (false);
		if (isInLoading) {
			o.transform.SetParent (Instance.transform, false);
		} else {
			o.transform.SetParent (null, false);
		}
		cardGameObjectDictionary [name] = o;

	}

	public GameObject PopGameObject(string name)
	{
		if (!cardGameObjectDictionary.ContainsKey (name) || cardGameObjectDictionary[name]==null) {
			return null;
		}
		cardGameObjectDictionary [name].SetActive (true);
		return cardGameObjectDictionary[name];
	}


	private void LoadSingleCard(string cardName)
	{
		CoroutineUtil.Instance.StartCoroutine (ResourceLoadManager.Instance.AsyncLoadResource<GameObject> (string.Format (localCardPath, cardName), delegate(string arg1, GameObject arg2) {
			if (arg2 != null) { 	

				GameObject createPrefab = GameObject.Instantiate (arg2); 
				createPrefab.SetActive(false);
				cardGameObjectDictionary [arg1] = createPrefab;
				Messenger.Broadcast<CardData> (CARD_LOADED_Handler, new CardData (cardName, null));
			}
		}));
	}
	//TODO:多个同时加载
	public IEnumerator LoadAllCards ( Action<string,float> loadingCallback=null,float currentProgress=0f,float totalStep =3)
	{
		List<SlotMachineConfig> slotMachineConfigs = BaseGameConsole.ActiveGameConsole ().SlotMachines ();
		for (int i = 0; i < slotMachineConfigs.Count; i++) {

			string cardLoadName = slotMachineConfigs [i].GetCardName ();
			if (!cardGameObjectDictionary.ContainsKey (cardLoadName) || cardGameObjectDictionary [cardLoadName] == null) {
				#if UNITY_IOS
//				if (slotMachineConfigs [i].SlotIndex >= GameConstants.LobbyCardStayNumber) {	
				if(!slotMachineConfigs [i].IsCardInPackage){
				} 
				else
				{
					LoadCardFromLocalContent( cardLoadName);
				}

				if(loadingCallback!=null){
					loadingCallback("",currentProgress + (float)i/slotMachineConfigs.Count*(float)1/totalStep);
				}
				#elif UNITY_ANDROID
				cardLoadName = string.Format("Prefab/Cards/{0}",cardLoadName);
				yield return CoroutineUtil.Instance.StartCoroutine (ResourceLoadManager.Instance.AsyncLoadResource<GameObject> (cardLoadName, delegate(string arg1, GameObject arg2) {
					if (arg2 != null) {
						GameObject createPrefab = GameObject.Instantiate (arg2); 
						PushGameObject(arg1,createPrefab,true); 
						if(loadingCallback!=null){
							loadingCallback("",currentProgress + (float)i/slotMachineConfigs.Count*(float)1/totalStep);
						}
						Messenger.Broadcast<CardData> (CARD_LOADED_Handler, new CardData (cardLoadName, null));
					}
				}));
				#endif
			}
			yield return new WaitForEndOfFrame();
		}
		yield break;
	}

	/// <summary>
	/// 代表正在显示中，需要立马创建出来
	/// Containses the card in queue.
	/// </summary>
	/// <returns><c>true</c>, if card in queue was containsed, <c>false</c> otherwise.</returns>
	/// <param name="cardName">Card name.</param>
	public bool ContainsCardInQueue(string cardName)
	{
		return cardLoadQueue.Contains (cardName);
	}

	public void RemoveCard(string cardName)
	{
		if (cardLoadQueue.Contains (cardName)) {
			cardLoadQueue.Remove (cardName);
		}
	}

	public void ClearAllCard()
	{
//		#if UNITY_IOS
		for(int i = 0; i <Instance.transform.childCount;i++)
		{
			GameObject o = transform.GetChild(i).gameObject;
			Destroy(o);
		}
		cardGameObjectDictionary.Clear ();
//		#endif
   		this.cardLoadQueue.Clear ();
	}
	public IEnumerator LoadCardFromLocalContent(string cardLoadName )
	{
		cardLoadName = string.Format("Prefab/Cards/{0}",cardLoadName);
		yield return CoroutineUtil.Instance.StartCoroutine (ResourceLoadManager.Instance.AsyncLoadResource<GameObject> (cardLoadName, delegate(string arg1, GameObject arg2) {
			if (arg2 != null) {
				GameObject createPrefab = GameObject.Instantiate (arg2); 
				PushGameObject(arg1,createPrefab,true); 
 
				Messenger.Broadcast<CardData> (CARD_LOADED_Handler, new CardData (cardLoadName, null));
			}
		}));
	}

	private List<string> cardLoadQueue = new List<string>();
	private static Dictionary<string,GameObject> cardGameObjectDictionary = new Dictionary<string,GameObject> ();
	private const string localCardPath = "Prefab/Cards/{0}";
	private static SlotsAssets _instance;
}
 

public class CardData
{
	public GameObject cardGameObject;
	public string slotCardName;

	public CardData(string _slotCardName,GameObject _cardGameObject=null)
	{
		this.cardGameObject = _cardGameObject;
		this.slotCardName = _slotCardName;
	}
}
