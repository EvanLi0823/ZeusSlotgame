using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ReelConstant  
{
	public static readonly string  REEL_STOP_RUN_HANDLER ="REEL_STOP_RUN_HANDLER";
}
[Serializable]
public class ReelGlidingData
{
    public bool IsGliding;
    public List<int> ShowSymbolData;
    public int GlidingLength;
    public bool IsUp;

}

public enum SymbolChangeState
{
    ReelStop,
    Running,
    //Initial
}
