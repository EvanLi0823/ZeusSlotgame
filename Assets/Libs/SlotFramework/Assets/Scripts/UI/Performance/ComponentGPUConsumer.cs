using UnityEngine;
using System.Collections;

namespace UI.Performance
{
	public class ComponentGPUConsumer : MonoBehaviour,IGPUConsumer
	{
		public MonoBehaviour Component;

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

		public virtual bool  ShouldDeactiveGameObjectOnLowGPU()
		{
			return false;
		}

		public virtual  bool  ShouldDisableComponentOnLowGPU()
		{
			return true;
		}

		public virtual bool DoApply()
		{
			bool hasApplied = false;

			if (Component != null && PerformanceManager.Instance.GPUScore () < RequiredGPUScore()) {
				Component.enabled = false;
				hasApplied = true;
			}

			return hasApplied;
		}

		public virtual bool Restore()
		{
			bool hasRestored = false;

			if (Component != null && !Component.enabled) {
				Component.enabled = true;
				hasRestored = true;
			}

			return hasRestored;
		}

		#endregion

	}
}
