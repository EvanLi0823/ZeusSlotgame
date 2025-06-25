using System.Collections;
using System.Collections.Generic;
using Libs;
using UnityEngine;

public class PortraitSystemUIPopupStrategy : SystemUIPopupStrategy
{
    public override bool CanPopup()
    {
        return base.CanPopup() && SkySreenUtils.CurrentOrientation == ScreenOrientation.Portrait;
    }
}
