using UnityEngine;
using System.Collections;

namespace UI
{
	public interface IGPUConsumer
	{
		float RequiredGPUScore ();
		bool  ShouldDeactiveGameObjectOnLowGPU();
		bool  ShouldDisableComponentOnLowGPU();
		bool  DoApply();
		bool  Restore();
	}
}
