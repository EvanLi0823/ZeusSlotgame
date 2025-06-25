using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LuaFramework;

namespace Classic{
	public class ConditionContentCell : IConditionContent
	{
		public BaseCondition baseCondition;
		public int min=0;
		public int max=0;
		public ConditionContentCell(){
			
		}
		public ConditionContentCell(int min,int max){
			this.min = min;
			this.max = max;
		}
		public virtual bool ConditionIsOK(){
			return false;
		}

		public bool CheckCondition()
		{
			return ConditionIsOK();
		}

		public void SetExtraData(Dictionary<string, object> info)
		{
			if (info == null || info.Count == 0) return;	
			baseCondition = new EventCondition(info);
		}
	}
}
