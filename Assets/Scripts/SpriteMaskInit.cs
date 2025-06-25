using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteMask))]
public class SpriteMaskInit : MonoBehaviour {
	public const string SPRITE_MASK_INIT_EVENT = "SPRITE_MASK_INIT_EVENT";
	SpriteMask mask ;
	// Use this for initialization
	void Awake()
	{
		mask = GetComponent<SpriteMask> ();
		Messenger.AddListener (GameConstants.OnSlotMachineSceneInit, maskInit);
		Messenger.AddListener (SlotControllerConstants.OnEnterFreespin, maskInit);
		Messenger.AddListener (SlotControllerConstants.OnQuitFreespin, maskInit);
		Messenger.AddListener (SPRITE_MASK_INIT_EVENT, maskInit);
	}

	void maskInit()
	{
		if(mask!=null)
		{
			mask.updateSprites ();
		}
	}

	void OnDestroy()
	{
		Messenger.RemoveListener (GameConstants.OnSlotMachineSceneInit, maskInit);
		Messenger.RemoveListener (SlotControllerConstants.OnEnterFreespin, maskInit);
		Messenger.RemoveListener (SlotControllerConstants.OnQuitFreespin, maskInit);
		Messenger.RemoveListener (SPRITE_MASK_INIT_EVENT, maskInit);
	}

}
