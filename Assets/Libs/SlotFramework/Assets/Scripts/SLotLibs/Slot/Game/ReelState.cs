using UnityEngine;
using System.Collections;

namespace Classic
{
	public enum ReelState
	{
		READY,
		DelayBeforeRunning,
		SpeedUpBeforeRunning,
		RUNNING,
		FIXRUNNING,
		DelayBeforeStop,
		SLOWDOWN,
        SLOWDOWNFASTRUN, 
        SLOWDOWNOVER,
		STOPPING,
		RUNBACKDOWN,
		RUNBACKUP,
		RUNSHAKE,
		END
	}

}
