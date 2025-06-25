using System;
using System.Collections;
using System.Collections.Generic;
namespace Libs{
	public class PoolMgr
	{
		private static PoolMgr mDefaultPools=null;
		public static PoolMgr DefaultPools
		{
			get
			{
				if(null == mDefaultPools)
				{
					mDefaultPools = new PoolMgr();
				}
				return mDefaultPools;
			}
		}

		private Dictionary<string,Pool> pools;

		public PoolMgr()
		{
			this.pools = new Dictionary<string, Pool>();
		}

		public bool HasPool(string id)
		{
			return pools.ContainsKey(id);
		}

		public Pool GetOrCreatePool(string id)
		{
			Pool p = GetPool(id);
			if (p == null)
			{
				p = CreatePool(id);
			}
			return p;
		}

		protected Pool CreatePool(string id)
		{
			Pool p = new Pool();
			pools.Add(id,p);
			return p;
		}

		public Pool GetPool(string id)
		{
			if (pools.ContainsKey(id))
			{
				return pools[id];
			}
			return null;
		}


		public void RemovePool(string id)
		{
			if (!pools.ContainsKey(id))
				return;

			Pool p = pools[id];
			p.Clear();

			pools.Remove(id);
		}

		public void RemoveAll()
		{
			foreach (Pool v in pools.Values)
			{
				v.Clear();
			}
			pools.Clear();
		}
	}
}