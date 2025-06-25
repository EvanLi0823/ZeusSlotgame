using UnityEngine;
using System.Collections;

public class CollectBallBehaviour : MonoBehaviour
{
	public int Index;

	protected Transform mTransfm;

	public void Init()
	{
		mTransfm = transform;
	}

	public virtual void DoBehaviour(){
    }

    public virtual void EndBehaviour(){
    }
}

