using UnityEngine;
using System.Collections.Generic;
namespace Libs{

	public class Pool
	{
		public int size = 10;
		private Stack<Object> stack;
		public Pool()
		{
			stack = new Stack<Object>();
		}

		public Object CreateObject(Object v)
		{
			if (stack.Count > 0)
			{			
				return stack.Pop();
			}
			else
			{
				Object newObj = Object.Instantiate(v);
				return newObj;
			}
		}

		public void DestoryObject(GameObject v)
		{
			if (stack.Count < size || size < 0)
			{
				if (!stack.Contains (v)) {
					stack.Push (v);
					v.SetActive(false);
				}
			}
			else
			{
				Object.Destroy(v);
			}
		}

		public void Clear()
		{
			foreach (Object v in stack)
			{
				Object.Destroy(v);
			}
			stack.Clear();
		}
	}
}