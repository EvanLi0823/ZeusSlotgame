using System;

namespace Libs
{
    public class ResourceCacheEntry<T> where T : class
    {
        public T Asset { get; }
        public DateTime LastAccessTime { get; set; }

        public ResourceCacheEntry(T asset)
        {
            Asset = asset;
            LastAccessTime = DateTime.Now;
        }
    }
}