using Classic;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
public class SpineSymbolRender : AnimatiorElement 
{
	protected List<SkeletonDataAsset> skeletonDataAsset = new List<SkeletonDataAsset>();
	protected string animationName = "";
	protected string animationSkin = "default";
	protected bool animationClip = false;
	[HideInInspector]
	public SpineAnimation mainSpine; //主spine动画
	[HideInInspector]
	public List<SpineAnimation> otherSpine = new List<SpineAnimation>(); //附属spine动画, 由于spine动画制作限制, 一个Symbol动画需要做多段spine动画
	public int curAnimationId = 0;
	
	public float m_spineScale = 1.0f;
	protected int loopCount = 0;
	private const string SPINE_ANIMATION = "Prefab/Shared/SpineAnimation";

	protected virtual GameObject LoadSpineAnimationObj()
	{
		GameObject spineAnimation = Resources.Load<GameObject>(UnityEngine.SceneManagement.SceneManager.GetActiveScene ().name + "/SpineAnimation") as GameObject;
		if(spineAnimation == null)
		{
			spineAnimation = Resources.Load<GameObject>(SPINE_ANIMATION) as GameObject;
		}
		return Instantiate(spineAnimation, this.animationParent);
	}
	public override void InitElement (BaseReel ReelContronller, int SymbolIndex)
	{
		base.InitElement (ReelContronller, SymbolIndex);
		GameObject o = LoadSpineAnimationObj();
		if(o != null)
		{
			mainSpine = o.GetComponent<SpineAnimation>();
		}
	}

	public override void PlayAnimation (int animationId, bool isLoop = true, System.Action InitHandler = null, 
		System.Action CompleteHandler = null, float RepeatPlayStartTime = 0, bool isUseCache = true)
	{
		if(animationId == 2) 
		{
			animationId = 1;
		}

		if(curAnimationId == 3)
		{
			return;
		}

		curAnimationId = animationId;

		this.DestroyOtherSpine();
		this.SymbolSkeletonDataAsset(animationId);

		if(skeletonDataAsset.Count == 0)
		{
			base.PlayAnimation(animationId, true, null, null, 0);
			return;
		}

		for (int i = 0; i < this.skeletonDataAsset.Count; i++)
		{
			if(i == 0)
			{
				this.PlayMainSpine(skeletonDataAsset[i], animationName, isLoop, InitHandler, CompleteHandler, RepeatPlayStartTime);
			}else
			{
				this.PlayOtherSpine(skeletonDataAsset[i], animationName, isLoop, RepeatPlayStartTime);
			}
		}

		this.EnableSymbolImage(false);
	}

	//播放主Spine动画
	public virtual void PlayMainSpine (SkeletonDataAsset skeletonDataAsset, string animationName, bool isLoop = true, 
		System.Action InitHandler = null, System.Action CompleteHandler = null, float RepeatPlayStartTime = 0f)
	{
		
		if(animationParent != null&&this.animationParent.childCount != 0)
		{
			mainSpine = this.animationParent.GetChild(0).gameObject.GetComponent<SpineAnimation>();
		}
		if(mainSpine == null)
		{
			GameObject o = LoadSpineAnimationObj();
			if (o != null)
			{
				o.transform.localScale = new Vector3(m_spineScale, m_spineScale, m_spineScale);
				mainSpine = o.GetComponent<SpineAnimation>();
			}
		}
		
		if(mainSpine != null)
		{
			mainSpine.transform.localScale = new Vector3(m_spineScale, m_spineScale, m_spineScale);
			bool isPartical = ((ReelManager) runControllerAdapter).gameConfigs.payLineConfig.payLineType == GameConfigs.PayLineType.BoxParticle;
			mainSpine.Play(skeletonDataAsset, animationName, isLoop,RepeatPlayStartTime, loopCount, 
				InitHandler, CompleteHandler, animationClip, animationSkin,isParticalLineType:isPartical);
		} 
	}

	//播放附属Spine动画
	public virtual void PlayOtherSpine (SkeletonDataAsset skeletonDataAsset, string animationName, bool isLoop = true, 
		float RepeatPlayStartTime = 0)
	{
		GameObject o = LoadSpineAnimationObj();
		if(o != null)
		{
			o.transform.localScale = new Vector3(m_spineScale, m_spineScale, m_spineScale);
			SpineAnimation spine = o.GetComponent<SpineAnimation>();
			if(spine != null)
			{
				bool isPartical = ((ReelManager) runControllerAdapter).gameConfigs.payLineConfig.payLineType == GameConfigs.PayLineType.BoxParticle;
				spine.Play(skeletonDataAsset, animationName, isLoop, RepeatPlayStartTime, loopCount, null,
					null, animationClip, animationSkin,isParticalLineType:isPartical);
				otherSpine.Add(spine);
			} 
		}
	}

	public virtual void SymbolSkeletonDataAsset (int animationId)
	{
		skeletonDataAsset.Clear();
		animationName = "";
		
		if (BaseSlotMachineController.Instance == null) 
		{
			return;
		}

		if (BaseSlotMachineController.Instance.slotMachineConfig == null) 
		{
			return;
		}

		if(!BaseSlotMachineController.Instance.slotMachineConfig.SpineAsset.ContainsKey(this.SymbolIndex))
		{
			return;
		}
		SpineResourceAsset resourceAsset = BaseSlotMachineController.Instance.slotMachineConfig.SpineAsset[this.SymbolIndex].Find (delegate(SpineResourceAsset obj) 
		{
			return obj.animationId == animationId;
		});
		
		if(resourceAsset == null)
		{
			return;
		}

		animationName = resourceAsset.animationName;
		animationSkin = resourceAsset.animationSkin;;
		animationClip = resourceAsset.animationClip;
		foreach (var item in resourceAsset.skeletonDataAsset)
		{
			skeletonDataAsset.Add(item);
		}
	}

	public override void PauseAnimation (bool notChange = false)
	{
		if(mainSpine != null)
		{
			mainSpine.Pause();
		}
		
		foreach (var spine in otherSpine)
		{
			spine.Pause();
		}

		if (animator != null) {
			animator.SetInteger ("state", 0);
			if (notChange) {
				EnableSymbolImage (true);
				if (AnimationGO != null) {
					AnimationGO.SetActive (false);
				}
			}
		}
		
		this.EnableSymbolImage (true);
	}

	public override void StopAnimation (bool showAnimationFrame = true)
	{
		curAnimationId = 0;
		
		if(mainSpine != null)
		{
			mainSpine.Stop();
			mainSpine = null;
		}

		this.DestroyOtherSpine ();

		if (animator != null || AnimationGO!=null) {
			DestroyAnimation ();
			animator = null;

			AnimationGO = null;
		}
		
		this.EnableSymbolImage (true);
	}

	public void DestroyOtherSpine ()
    {
        for (int i = otherSpine.Count - 1; i >= 0 ; i--)
		{
			if(otherSpine[i] != null && otherSpine[i].gameObject != null)
				Destroy(otherSpine[i].gameObject);
			otherSpine.RemoveAt(i);
		}

		otherSpine.Clear();
    }

	public void SetLoopCount (int count)
	{
		loopCount = count;
	}

	//由于同SpriteRenderer同层级时先激活的低于后激活的会导致层级错乱
	public override void EnableSymbolImage (bool state)
	{
		if (m_SymbolSprite != null) 
		{
			m_SymbolSprite.color = state ? Color.white : Color.clear;
		}

		Graphic[] graphics = this.transform.GetComponentsInChildren<Graphic>();
		if(graphics == null) return;
		foreach (var item in graphics)
		{
			item.color = state ? Color.white : Color.clear;
		}
	}

	public void SetAnimationPosOffset(Vector3 localPos)
	{
		if (mainSpine!=null)
		{
			mainSpine.transform.localPosition = localPos;
		}

		if (otherSpine!=null)
		{
			for (int i = 0; i < otherSpine.Count; i++)
			{
				if(otherSpine[i]==null) continue;
				otherSpine[i].transform.localPosition = localPos;
			}
		}
	}
}
