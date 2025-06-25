using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FitStartSizeParticleSystem : MonoBehaviour {


	private ParticleSystem.MainModule mParticleMain;
	public Vector3 size3D = Vector3.one;
	void Awake ()
	{
		if (SkySreenUtils.GetScreenSizeType() == SkySreenUtils.ScreenSizeType.Size_16_9)
		{
			return;
		}

		mParticleMain = GetComponent<ParticleSystem> ().main;

		mParticleMain.startSizeX = new ParticleSystem.MinMaxCurve(size3D.x);
		mParticleMain.startSizeY = new ParticleSystem.MinMaxCurve (size3D.y);
		mParticleMain.startSizeZ = new ParticleSystem.MinMaxCurve (size3D.z);
	}
}
