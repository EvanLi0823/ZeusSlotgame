using UnityEngine;
using System.Collections;

namespace Classic
{
  public delegate void GameCallback ();
  public delegate void WheelCallback(float angleDistance);

	public delegate void ReelStopCallback(int reelIndex);
}
