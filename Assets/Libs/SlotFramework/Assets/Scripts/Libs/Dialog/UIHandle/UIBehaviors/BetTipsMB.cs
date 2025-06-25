using RealYou.Core.UI;
using Classic;
using UniRx.Async;

public class BetTipsMB : IMachineBehaviour
{
    #region TriggerMoment

    public int Priority => (int)Libs.UIMBPriority.BetTips; //要求最后弹出

    public UniTask OnPreEnterMachine(bool isRestore)
    {
        return UniTask.CompletedTask;
    }

    public UniTask OnLateEnterMachine(bool isRestore)
    {
        return UniTask.CompletedTask;
    }

    public UniTask OnSpinEnd()
    {
        DataManager.GetInstance().UpdateUserCurrentBet();
        UserManager.GetInstance().HandleBetTips();
        return UniTask.CompletedTask;
    }


    public UniTask OnExitMachine()
    {
        return UniTask.CompletedTask;
    }

    #endregion
}