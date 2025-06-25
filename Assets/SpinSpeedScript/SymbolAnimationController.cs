using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Classic;
//必须放在animationCanvas下面
[RequireComponent(typeof( AnimationCanvas))]
public class SymbolAnimationController : MonoBehaviour {

	private AnimationCanvas animationCanvas;

	//所有的动画render
	private Dictionary<int,Dictionary<int,GameObject>> animationRenders = new Dictionary<int, Dictionary<int, GameObject>> ();
	
	//所有的动画render的Y坐标
	private Dictionary<int,Dictionary<int,float>> animationRendersPosY = new Dictionary<int, Dictionary<int, float>> ();

	private Dictionary<int,GameObject> animationReels = new Dictionary<int, GameObject> ();
	// Use this for initialization
	void Awake () {
		animationCanvas = GetComponent<AnimationCanvas> ();
	}

	public void DestroyAnimation()
	{
		animationReels.Clear ();
		animationRenders.Clear ();
		Util.DestroyChildren(this.transform);
	}

	public void AddAnimationReel(int reelIndex,Vector3 v,Vector2 sizeV)
	{
        if(animationReels.ContainsKey(reelIndex))
        {
            return;
        }
        GameObject animationReel = Instantiate (animationCanvas.animationReel) as GameObject;
		animationReel.transform.SetParent (animationCanvas.transform, false);
		animationReel.transform.localPosition = v;
		(animationReel.transform as RectTransform).sizeDelta = sizeV;
		this.animationReels [reelIndex] = animationReel;
		animationReel.name = "animationReel" + reelIndex;
	}

	public void AddAnimationRender(int reelIndex, int positionId,Vector3 v,Vector2 sizeV,bool isUpOverDown)
	{
		if (this.animationReels [reelIndex] == null) {
			Debug.LogError (reelIndex+"的动画reel为空，初始化失败");
		}
        if (animationRenders.ContainsKey(reelIndex) && animationRenders[reelIndex].ContainsKey(positionId))
        {
            return;
        }
        GameObject animationRender = Instantiate (animationCanvas.animationElement, this.animationReels [reelIndex].transform, false);
		// animationRender.transform.SetParent(this.animationReels [reelIndex].transform, false);
		animationRender.transform.localPosition = v;
		animationRender.name = "animationRenders" + positionId;
		(animationRender.transform as RectTransform).sizeDelta = sizeV;

		if (isUpOverDown) {
			animationRender.transform.SetAsLastSibling ();
		} else {
			animationRender.transform.SetAsFirstSibling ();
		}

		if (!animationRenders.ContainsKey (reelIndex)) {
			animationRenders [reelIndex] = new Dictionary<int, GameObject> ();
		}
		if (!animationRendersPosY.ContainsKey (reelIndex)) {
			animationRendersPosY [reelIndex] = new Dictionary<int, float> ();
		}

		animationRenders [reelIndex] [positionId] = animationRender;
		animationRendersPosY[reelIndex][positionId] = animationRender.transform.localPosition.y;
	}

	public GameObject GetAnimationRender(int reelIndex, int positionId)
	{
		if (animationRenders.ContainsKey (reelIndex)) 
		{
			if (animationRenders[reelIndex] != null && animationRenders[reelIndex].ContainsKey(positionId))
			{
				return animationRenders [reelIndex] [positionId];
			}
		}
		return null;
	}

	public void ReSetAnimationRender(int reelIndex)
	{
		if(animationRenders.ContainsKey(reelIndex) && animationRendersPosY.ContainsKey (reelIndex)) 
		{
			foreach (var positionId in animationRenders[reelIndex].Keys)
			{
				animationRenders[reelIndex][positionId].transform.localPosition = new Vector2(0, animationRendersPosY[reelIndex][positionId]);
			}
		}
	}

    public void ExchangeAnimationRender(int fromReel, int fromPosition, int toReel, int toPosition)
    {
        GameObject fromGameObject = animationRenders[fromReel][fromPosition];
        GameObject toGameObject = animationRenders[toReel][toPosition];

        animationRenders[fromReel][fromPosition] = toGameObject;
        animationRenders[toReel][toPosition] = fromGameObject;
    }
    public Transform GetAnimationReel(int reelIndex)
    {
        if (animationReels.ContainsKey(reelIndex))
        {
            return animationReels[reelIndex].transform;
        }
        return null;
    }
}
