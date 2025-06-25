using System;
using System.Collections.Generic;
using UnityEngine;

public class LinkReel : Reel
{
    public List<AnimationCurve> LinkCurves = new List<AnimationCurve>();
    public int AntiExtraSymbolNum;
    public List<float> NormalSpinTime = new List<float>();
    public List<float> TotalLength = new List<float>();
    protected List<float> m_SpinRunningTimes = new List<float>();
    protected List<int> StopedSymbolIndex= new List<int>();
    protected List<int> BouncedSymbolIndex= new List<int>();
    protected List<float> SymbolsBounceTime = new List<float>();

    public override void StartSpin(ReelRunData _data)
    {
	    base.StartSpin(_data);
	    m_SpinRunningTimes.Clear();
	    for (int i = 0; i < SymbolRenders.Count; i++)
	    {
		    m_SpinRunningTimes.Add(0f);
	    }
	    SetLinkCurves();
    }

    protected virtual void SetLinkCurves()
    {
	    NormalSpinTime.Clear();
	    TotalLength.Clear();
	    LinkCurves.Clear();
	    StopedSymbolIndex.Clear();
	    BouncedSymbolIndex.Clear();
	    SymbolsBounceTime.Clear();
	    for (int i = 0; i < ReelShowNum; i++)
	    {
		    AnimationCurve curve = new AnimationCurve( m_Curve.keys); //new AnimationCurve (this.m_NormalRunReelConfig.NormalRunReelCurve [index].keys);

		    Keyframe[] frames = curve.keys;
		    
		    float deltaY = (float)m_boardConfig.SymbolHeight / 1000 * AntiExtraSymbolNum*(ReelShowNum-i-1);
		    float v = CaculateKeyFrameTangent (frames [1], frames [2]);

		    float deltaX = deltaY / v; //时间➗2
		
		    for (int j = frames.Length - 1; j >= 2; j--) {
			    frames [j].value += deltaY;
			    frames [j].time += deltaX;

			    curve.MoveKey (j, frames [j]);
		    }
		    //倒数第二的斜率
		    // Keyframe lastSecFrame = frames[2];
		    // lastSecFrame.outTangent = CaculateKeyFrameTangent (frames [2], lastFrame);
		    NormalSpinTime.Add(curve.keys [curve.keys.Length - 1].time);
		    TotalLength.Add(curve.keys [curve.keys.Length - 1].value * 1000f);
		    LinkCurves.Add(curve);
		    SymbolsBounceTime.Add(curve.keys[curve.keys.Length-2].time);
	    }
    }

    protected override void DeltaMoveWheel()
    {
	    Debug.Log("LinkReel DeltaMoveWheel");
        m_FinalBoardIdsOneFrame.Clear();
	    float x = 0f;
	    float s = 0f;
        
        bool isStopAll = true;
        
	    for (int i = 0; i < SymbolRenders.Count; i++)
	    {
		    bool isCurrentRun = CheckLinkAnimationCurve(i,out x,out s);

		    if (isCurrentRun)
		    {
			    isStopAll = false;
			    if (SymbolRenders[i].MoveDistance(s, ref LayerOrderZ))
			    {
				    //小small的respin形式
				    if (this.m_boardConfig.IsSmallMode)
				    {
					    //最后一个symbol位置要停止即可
					    if (this.TotalLength[i] - s > this.m_boardConfig.SymbolHeight)
					    {
						    SymbolRenders[i].SymbolChangeState = SymbolChangeState.Running;
					    }
					    else
					    {
						    if (SymbolRenders[i].SymbolChangeState != SymbolChangeState.ReelStop)
						    {
							    SymbolRenders[i].ChangeSymbol(m_RunData.ResultSymbols[SymbolRenders[i].PositionId],
								    SymbolChangeState.ReelStop);
						    }
					    }
				    }
			    }

			    for (int j = 0; j < SymbolsBounceTime.Count; j++)
				{
					if (m_SpinRunningTimes[j] >= this.SymbolsBounceTime[j] && !BouncedSymbolIndex.Contains(j)) {
						BouncedSymbolIndex.Add(j);
						SymbolBounceCallback(ReelIndex,j);
					}
				}
			    
			 
			    FinalBoardSetRenders();
		    }
		    else
		    {
			    if (!StopedSymbolIndex.Contains(i))
			    {
				    SymbolStopCallback(ReelIndex,i);
				    StopedSymbolIndex.Add(i);
			    }
		    }
	    }

	    if (isStopAll)
	    {
		    VerifyStopSymbolRender();
	        
		    if (m_RunData.IsAnticipation) {
			    //anti的停止状态跟回弹时一样的逻辑
			
			    this.m_ReelController.ReelBounceBackHandler(this.ReelIndex);
		    }

		    ReelStopHandler (false);
	    }
    }
    
    protected virtual bool CheckLinkAnimationCurve(int index,out float x,out float s)
    {
	    x = 0f;
		s = 0f;
	    // Log.LogYellowColor(NormalSpinTime.Count+"====="+index);
	    bool canRun = m_SpinRunningTimes[index] < NormalSpinTime[index];
	    if (canRun)
	    {
		    m_SpinRunningTimes[index] += Time.deltaTime * m_RunData.RunTimeScale; // fast的时候会变快，时间乘以大于1的数
		    // if (m_ReelController.EnableNetWork && !m_ReelController.CanNetStop && m_SpinRunningTime>=m_ReelController.NetStopTime)
		    // {
			   //  m_SpinRunningTime -= m_ReelController.OneReelOffsetTime;
			   //  float offset = LinkCurves[index].Evaluate(m_SpinRunningTime);
			   //  for (int i = 0; i < this.SymbolRenders.Count; i++)
			   //  {
				  //   this.SymbolRenders[i].ResetPageIndex(offset*1000);
			   //  }
		    // }
		    x = m_SpinRunningTimes[index]; // / RunTime;
		    s = LinkCurves[index].Evaluate(x) * 1000f; //* RealLength * RunMultiple;
	    }

	    return canRun;
    }

    protected virtual void SymbolStopCallback(int ReelIndex,int PositionId)
    {
	    
    }
    
    protected virtual void SymbolBounceCallback(int ReelIndex,int PositionId)
    {
    }

    public float CaculateKeyFrameTangent(Keyframe k1, Keyframe k2)
    {
	    return (float)(((double)k2.value - (double)k1.value)/((double)k2.time - (double)k1.time) );
    }
    
}
