//看广告条件

using System.Collections.Generic;

public class ADCondition
{
   private Dictionary<string, object> data = new Dictionary<string, object>();
   public ADCondition(Dictionary<string,object> data)
   {
      
   }
   //条件满足
   public bool isMeetCondition()
   {
      return true;
   }
   //条件重置
   public virtual void ResetCondition()
   {
     
   }
   
   //条件重置
   public virtual void UpdateCondition()
   {
      
   }
}
