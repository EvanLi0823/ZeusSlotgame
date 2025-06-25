using UnityEngine;
using System.Collections;


namespace Classic
{
public class ChangePatricalSize : MonoBehaviour {

		public float devHeight = 9f;
		public float devWidth = 16f;

		void Awake ()
		{

			resetSize ();
		}
		
		private void resetSize ()
		{
            if (SkySreenUtils.GetScreenSizeType() != SkySreenUtils.ScreenSizeType.Size_16_9){
                ParticleSystem[] ps = gameObject.GetComponentsInChildren<ParticleSystem>();
                float targetScale = FixedFull.Scaler;
                foreach (ParticleSystem p in ps)
                {
                    p.startSize = p.startSize * targetScale;
                }
            }
			
		}
}

}
