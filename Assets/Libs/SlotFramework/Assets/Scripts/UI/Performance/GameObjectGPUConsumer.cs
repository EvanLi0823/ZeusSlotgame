using UnityEngine;
using System.Collections;

namespace UI.Performance
{
	public class GameObjectGPUConsumer : MonoBehaviour, IGPUConsumer
	{
		public GameObject GameObject;

		public float ExpectedGPUScore;
	

		#region LifeCycle
		
		void Start()
		{
			PerformanceManager.Instance.Register (this);
			DoApply ();
		}

		#endregion




		#region Public API

		public virtual float RequiredGPUScore ()
		{
			return ExpectedGPUScore;
		}
	
		public virtual bool  ShouldDeactiveGameObjectOnLowGPU ()
		{
			return false;
		}
	
		public virtual  bool  ShouldDisableComponentOnLowGPU ()
		{
			return true;
		}
	
		public virtual bool DoApply ()
		{
			bool hasApplied = false;
		
			if (GameObject != null && PerformanceManager.Instance.GPUScore () < RequiredGPUScore ()) {
				GameObject.SetActive(false);
				hasApplied = true;
			}
		
			return hasApplied;
		}

		public virtual bool Restore()
		{
			bool hasRestored = false;
			
			if (GameObject != null && !GameObject.activeSelf) {
				GameObject.SetActive( true);
				hasRestored = true;
			}
			
			return hasRestored;
		}

		#endregion

	}
}
