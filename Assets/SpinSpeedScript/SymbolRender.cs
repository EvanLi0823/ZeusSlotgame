using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

[Serializable]
public class SymbolRender : MonoBehaviour {
//	public Image m_Image;
	public SpriteRenderer m_SymbolSprite;
	public Image m_SymbolImage;

	[HideInInspector]
	public bool NeedMove = true;

	[HideInInspector]
	public Vector3 InitialPosition;  //初始化的位置,为了计算quickStop，因为急停的时候需要计算从初始位置的坐标
	protected Vector3 StartPosition,MovingTmpPosition; //每次spin的时候当前symbol的位置，以及spin的时候symbol的位置

	protected float MinScopeY, MaxScopeY;
	protected int PageIndex;

	protected IRunControllerAdapter runControllerAdapter;
	protected IReelAdapter reelAdapter;

    protected SymbolChangeState m_SymbolChangeState = SymbolChangeState.ReelStop; //symbol 改变的时候的状态，用来更改running和静止时的状态
	public SymbolChangeState SymbolChangeState
	{
		get => m_SymbolChangeState;
		set => this.m_SymbolChangeState = value;
	}
    
    public virtual void InitializePosition()
	{
		InitialPosition = this.transform.localPosition;
	}

	public virtual void RecordPosition(float minY, float maxY)
	{
		MovingTmpPosition = StartPosition = this.transform.localPosition;
		MinScopeY = minY;
		MaxScopeY = maxY;

		PageIndex = (int)( (StartPosition.y-maxY) / (maxY-minY));

        MoveUpPageIndex = (int)((StartPosition.y - minY) / (maxY - minY));

    }
	//如果有必要需要处理z坐标，向下移动symbol
	public virtual bool MoveDistance(float offsetAllY,ref int zIndex)
	{
		bool isChangeLoop = false;
		float y = StartPosition.y - offsetAllY;
		int currentPageIndex =(int)( (y-MaxScopeY) / (MaxScopeY - MinScopeY));

//		Debug.Log ("currentPageIndex:" + currentPageIndex);
		if (currentPageIndex != PageIndex) {
			isChangeLoop = true;
			PageIndex = currentPageIndex;
		}
		y = y - (MaxScopeY - MinScopeY) * currentPageIndex;
		//real position
		MovingTmpPosition.y = y;

		if (isChangeLoop) {
			MovingTmpPosition.z = zIndex;
		}

		if (this != null&&this.transform != null)
		{
			this.transform.localPosition = MovingTmpPosition;
		}

		return isChangeLoop;
	}

	/// <summary>
	/// 用于在
	/// </summary>
	/// <param name="offsetAllY"></param>
	public void ResetPageIndex(float offsetAllY)
	{
		float y = StartPosition.y - offsetAllY;
		PageIndex =(int)( (y-MaxScopeY) / (MaxScopeY - MinScopeY));
	}

    private int MoveUpPageIndex;
    //如果有必要需要处理z坐标，向下移动symbol
    public virtual bool MoveUpDistance(float offsetAllY, ref int zIndex)
    {
        bool isChangeLoop = false;
        float y = StartPosition.y + offsetAllY;
        int currentPageIndex = (int)((y - MinScopeY) / (MaxScopeY - MinScopeY));

        //      Debug.Log ("currentPageIndex:" + currentPageIndex);
        if (currentPageIndex != PageIndex)
        {
            isChangeLoop = true;
            PageIndex = currentPageIndex;
        }
        y = y - (MaxScopeY - MinScopeY) * currentPageIndex;
        //real position
        MovingTmpPosition.y = y;

        if (isChangeLoop)
        {
            MovingTmpPosition.z = zIndex;
        }

        this.transform.localPosition = MovingTmpPosition;

        return isChangeLoop;
    }

    public virtual void SetOrderZ(int zIndex)
	{
		if (this == null) return;
		if (this.transform == null) return;
		MovingTmpPosition = this.transform.localPosition;
		MovingTmpPosition.z = zIndex;
		this.transform.localPosition = MovingTmpPosition;
//		Debug.Log (zIndex);
	}

	public virtual void QuickStopMoveDistance(float offsetY)
	{
		float y = this.InitialPosition.y + offsetY;
		MovingTmpPosition.y = y;
		this.transform.localPosition = MovingTmpPosition;
	}

	//用来处理掉落模型
	public void MoveDownOffset(float offsetAllY)
	{
		MovingTmpPosition.y =  StartPosition.y - offsetAllY;;
		this.transform.localPosition = MovingTmpPosition;
	}
		
	public void ChangeSymbol(Sprite s)
	{
//		if (m_Image != null) {
//			m_Image.sprite = s;
//			m_Image.SetNativeSize ();
//		}
		if (m_SymbolSprite != null) {
			m_SymbolSprite.sprite = s;
//			m_Sprite.
//			m_Sprite.
		}
	}


	#region 兼顾slots
	public IRunControllerAdapter ControllerAdapter
	{
		set{
			runControllerAdapter = value;
		}
		get{
			return this.runControllerAdapter;
		}
	}

	public IReelAdapter ReelAdapter
	{
		set{
			reelAdapter = value;
		}
		get{
			return this.reelAdapter;
		}
	}
	#endregion
	#region 改变symbol的方法，子类中的设置方法SymbolIndex不再使用
	public int symbolIndex;
	protected SymbolMap.SymbolElementInfo elementInfo;
	public virtual void ChangeSymbol(int symbolIndex, SymbolChangeState state = SymbolChangeState.ReelStop)
	{
		SymbolMap.SymbolElementInfo  info = runControllerAdapter.GetSymbolInfoByIndex (symbolIndex);
		this.ChangeSymbol (info, state);
	}

	public void ChangeSymbol(SymbolMap.SymbolElementInfo elementInfo, SymbolChangeState _state = SymbolChangeState.ReelStop)
	{
        m_SymbolChangeState = _state;

        this.elementInfo = elementInfo;
        if (elementInfo == null) {
            this.symbolIndex = -1;
            if (this.m_SymbolSprite != null) {
                m_SymbolSprite.sprite = null;
            }
        }
        else
        {
            this.symbolIndex = elementInfo.Index;

            if (this.m_SymbolSprite != null)
            {
                this.m_SymbolSprite.sprite = runControllerAdapter.GetGameConfig().GetBySymbolIndex(symbolIndex);
                this.m_SymbolSprite.sortingOrder = elementInfo.ScoreLevel + 1;
            }
            if (m_SymbolImage != null)
            {
                m_SymbolImage.sprite = runControllerAdapter.GetGameConfig().GetBySymbolIndex(symbolIndex);
            }
        }
		SymbolChangeHandler ();
	}

	/// <summary>
	/// Symbols  changed handler.
	/// </summary>
	protected virtual void SymbolChangeHandler()
	{

	}

	public virtual void InitSymbolHandler(){

	}

    public virtual void StartSpinHandler()
    { 
    
    }

	//主要兼容老版本，设置里面的indexofDisplay
	public virtual void SetStaticPosition(int index)
	{

	}


	public virtual void SetAnimationParent(Transform o)
	{

	}
	#endregion

	#region reel跟轮子相关
	private int _positionId;

	public int PositionId {
		get {
			return _positionId;
		}

		set {
			_positionId = value;
		}
	}

	public int ReelIndex
	{
		get{
			return this.reelAdapter.GetReelIndex ();
		}
	}

	public int ReelRenderNumber
	{
		get{
			return this.reelAdapter.GetReelShowNumber ();
		}
	}

	public int BoardReelNum
	{
		get{
			return this.reelAdapter.GetBoardConfig ().ReelConfigs.Length;
		}
	}

	public Classic.GameConfigs GetGameConfig()
	{
		return runControllerAdapter.GetGameConfig ();
	}

	#endregion
}
