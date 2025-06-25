using UnityEngine;
using System.Collections;

namespace Libs
{
	public class FitParticleSystem : MonoBehaviour
	{

		public float DefaultWidthRatio = 16f;
		public float DefaultHeightRatio = 9f;

		public ParticleSystem[] mParticleSystems = null;
		public bool enablePaticleScale = false;
		public Vector3 scaleXYZ = Vector3.one;
		void Awake ()
		{
            if (SkySreenUtils.GetScreenSizeType() == SkySreenUtils.ScreenSizeType.Size_16_9)
            {
                return;
            }
			float targetScale = Screen.height * DefaultWidthRatio / (Screen.width * DefaultHeightRatio);
			if (mParticleSystems == null || 
				(mParticleSystems != null && mParticleSystems.Length == 0)) {
				mParticleSystems = GetComponentsInChildren<ParticleSystem> ();
			}

			if (mParticleSystems != null) {
				foreach (ParticleSystem particle in mParticleSystems) {
					if (enablePaticleScale) {
                        particle.startSize = particle.startSize / targetScale;
						particle.transform.localScale = scaleXYZ;
					}
				}
			}
		}
	}
}


