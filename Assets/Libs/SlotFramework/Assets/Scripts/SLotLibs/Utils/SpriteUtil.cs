using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Classic
{
	public class SpriteUtil
	{
		private static SpriteUtil singletonInstance = null;
		private Dictionary<string, Sprite[]> textureCache = new Dictionary<string, Sprite[]>();

		private SpriteUtil()
		{
		
		}

		public static SpriteUtil GetInstance()
		{
			if (singletonInstance == null) 
				singletonInstance = new SpriteUtil ();
			return singletonInstance;
		}

		public Sprite SpriteByNameInAtlas(string spriteName, string atlasName)
		{
			if (textureCache.ContainsKey (atlasName)) 
			{
				Sprite[] sprites = textureCache[atlasName];
				return FindSpriteByNameInArray(sprites, spriteName);
			}
			return null;
		}

		public Sprite SpritByName(string strPath)
		{
			return Resources.Load<Sprite>(strPath);
		}

		public void AddSpriteAtlas(string strPath)
		{
			if (!textureCache.ContainsKey (strPath)) {
				Sprite[] sprites = Resources.LoadAll<Sprite> (strPath);
				textureCache.Add (strPath, sprites);
			}
		}

		public void RemoveSpriteAtlas(string strPath)
		{
			if (textureCache.ContainsKey (strPath)) 
			{
				textureCache.Remove(strPath);
			}
		}

		public void RemoveAllSpriteAtlas()
		{
			if(textureCache.Keys.Count > 0)
				textureCache.Clear ();
		}

		private Sprite FindSpriteByNameInArray(Sprite[] sprites, string spriteName)
		{
			foreach (Sprite sprite in sprites) 
			{
				if(spriteName == sprite.name)
					return sprite;
			}
			return null;
		}
	}


}

