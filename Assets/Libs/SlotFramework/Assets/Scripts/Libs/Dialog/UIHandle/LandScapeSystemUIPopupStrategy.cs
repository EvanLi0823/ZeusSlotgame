using System.Collections;
using System.Collections.Generic;
using Libs;
using UnityEngine;


public class LandScapeSystemUIPopupStrategy : SystemUIPopupStrategy
{
    public override bool CanPopup()
    {
        return base.CanPopup() && SkySreenUtils.CurrentOrientation != ScreenOrientation.Portrait;
    }
}
