using System.Collections.Generic;
using UnityEngine;

namespace CardSystem
{
    public class ADCondition:BaseWeightCondition
    {
        public override void Execute()
        {
            Debug.Log("ADCondition Execute");
            bool rewardADIsReady = ADManager.Instance.RewardAdIsOk(ADEntrances.REWARD_VIDEO_ENTRANCE_CARDLOTTERY);
            //广告未加载好
            if (!rewardADIsReady)
            {
                //展示未加载好广告的提示，直接给看广告成功的奖励
                ADManager.Instance.ShowLoadingADsUI(endCallBack: () =>
                {
                    Messenger.Broadcast<int>(ADConstants.PlayCardLotteryAD,0);
                });
            }
            else
            {
                ADManager.Instance.PlayRewardVideo(ADEntrances.REWARD_VIDEO_ENTRANCE_CARDLOTTERY);
            }
        }
    }
}