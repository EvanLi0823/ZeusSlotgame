using UnityEngine;
using System.Collections;
using Libs;

namespace Classic
{
	public abstract class ResultState
	{
        public BaseActionNormal action = new BaseActionNormal();

		public ResultState(){
			Init ();
		}
		public  virtual void Run ()
		{
		}

		public virtual void Init(){
	
		}
		#region Restore Game
	

		//public virtual void Restore(){
		//}
		#endregion
	
	
		public  virtual void End ()
		{
            ResultStateManager.Instante.PlayNext ();
		}
	}
}
