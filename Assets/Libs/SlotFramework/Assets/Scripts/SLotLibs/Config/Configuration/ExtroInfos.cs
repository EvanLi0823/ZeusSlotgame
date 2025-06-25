using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Utils;

namespace Classic
{
	public class ExtroInfos
	{
		public static readonly string EXTRA_INFOS = "ExtraInfos";
	    public	Dictionary<string,object> infos = new Dictionary<string, object> ();

		public ExtroInfos (Dictionary<string,object> infos)
		{
			if (infos != null) {
				this.infos = infos;
			}
		}


        public  Dictionary<string,object> GetSubInfos(string key){
            return Utilities.GetValueOrDefault<Dictionary<string,object>> (infos, key);
        }
    }
}
