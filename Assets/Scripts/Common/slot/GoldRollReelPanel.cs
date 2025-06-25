using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using DG.Tweening;
using UnityEngine.UI;
using Classic;

public class GoldRollReelPanel : RollReelPanel
{
	private float currentSpeed = 0.0f;

	private GoldsReelManager goldReelManager;

	public override void InitElements(ReelManager GameContronller, GameConfigs elementConfigs, bool IsEndReel)
	{
		base.InitElements(GameContronller, elementConfigs, IsEndReel);
		goldReelManager = this.reelManager as GoldsReelManager;
		if(goldReelManager == null) Utils.Utilities.LogError ("this theme not is  GoldsReelManager");
	}

	protected override void Update() 
	{

	}

	private void FixedUpdate() 
	{
		base.BaseUpdate();
	}

	protected override bool ReelRuningOffset(float StartTime)
	{
		bool IsToNextState = IsReelRunningStateTerminated;

		if (Time.realtimeSinceStartup -StartTime > this.goldReelManager.GetGoldReelConfig(Index).RotationTime && ShouldStopAtOnce ()) 
		{
			IsToNextState = true;
			currentSpeed = this.goldReelManager.GetGoldReelConfig(Index).ConstantSpeed;
		}

		float offset = MoveOffset(this.goldReelManager.GetGoldReelConfig(Index).ConstantSpeed);
		ChangeStripPropertyDataWhenRunning (offset);
		ChangeElementsPosition (offset);

		return IsToNextState;
	}

	protected override bool IsRunAndSlowDown(bool hasStop, float startTime, bool fastStop)
	{
		return (hasStop || Time.realtimeSinceStartup - startTime > this.goldReelManager.GetGoldReelConfig(Index).StopDelayTime || fastStop);
	}

	protected override void BeforeStopRunOffset()
	{
		ChangeElementsPosition(MoveOffset(this.goldReelManager.GetGoldReelConfig(Index).ConstantSpeed));
	}

	protected override bool ReelDownOffset()
	{
		float offset = MoveOffset(currentSpeed);
		ChangeStripPropertyDataWhenSlowDown (offset);
		ChangeElementsPosition(offset);
		this.SubtractSpeed(this.goldReelManager.GetGoldReelConfig(Index).DownSpeed, this.goldReelManager.GetGoldReelConfig(Index).StopFrequency);
		if (currentSpeed < this.goldReelManager.GetGoldReelConfig(Index).StopSpeed) 
		{
			currentSpeed = this.goldReelManager.GetGoldReelConfig(Index).StopSpeed;
			return true;
		}
		return false;
	}

	protected override void ReelSlowDownoverOffset()
	{
		ChangeElementsPosition (MoveOffset(currentSpeed));
	}

	protected override float MoveOffsetStoping()
	{
		return MoveOffset(currentSpeed);
	}

	protected override float RunBackDownOffset(float totalM, float moveDis)
	{
		this.SubtractSpeed(this.goldReelManager.GetGoldReelConfig(Index).DownSpeed, this.goldReelManager.GetGoldReelConfig(Index).DownFrequency);
		float downOffset = MoveOffset(currentSpeed);
		if (totalM + downOffset > moveDis) {
			downOffset = moveDis - totalM;
			currentSpeed = this.goldReelManager.GetGoldReelConfig(Index).DownSpeed;
		}
		return downOffset;
	}

	protected override float RunBackUpOffset()
	{
		this.SubtractSpeed(this.goldReelManager.GetGoldReelConfig(Index).UpSpeed, this.goldReelManager.GetGoldReelConfig(Index).UpFrequency);
		float upOffset = MoveOffset(currentSpeed);
		return upOffset;
	}

	private float MoveOffset(float speed)
	{
		return (Time.fixedDeltaTime * speed)/elementConfigs.GetReelConfigs (Index).PanelHeight;
	}

	private void SubtractSpeed(float speed, float frequency)
	{
		float velocity = 0.0f;
		currentSpeed = Mathf.SmoothDamp(currentSpeed, speed, ref velocity, frequency);
	}
}

