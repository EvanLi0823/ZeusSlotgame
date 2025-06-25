using System.Collections.Generic;

public interface ICommandEvent
{
	void OnEventComplete(bool isAccept = true);
	void DisableAcceptBtn();
	void PlayCollectCoinsAnimation();
}