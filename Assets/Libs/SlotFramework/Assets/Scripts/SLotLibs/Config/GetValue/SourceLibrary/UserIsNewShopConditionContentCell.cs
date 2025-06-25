using System.Collections;
using System.Collections.Generic;
using Classic;
using UnityEngine;

public class UserIsNewShopConditionContentCell : ConditionContentCell
{
    public bool isNewShop = false;
    public UserIsNewShopConditionContentCell(object obj)
    {
        try
        {
            isNewShop = Utils.Utilities.CastValueBool(obj);
        }
        catch (System.Exception ex)
        {
            Utils.Utilities.LogPlistError("UserIsNewShopConditionContentCell:" + obj.ToString() + ex.Message);
        }
    }
    public override bool ConditionIsOK()
    {
		Dictionary<string, object> tempDic = Plugins.Configuration.GetInstance().GetValueWithPath<Dictionary<string, object>>(GameConstants.ShopItems_KEY, null);
        bool isNewShopType = false;
        if (tempDic != null)
        {
			if (tempDic.ContainsKey(GameConstants.ShopIsNew_Key))
            {
				isNewShopType = Utils.Utilities.GetBool(tempDic, GameConstants.ShopIsNew_Key, false);
            }
        }

        return isNewShopType == isNewShop;
    }
}
