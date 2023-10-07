using System.Collections.Generic;

namespace SecureLibrary.Core
{
    public class AttachedResourceCollection : Dictionary<string, AttachedResource>
    {
        public AttachedResourceCollection() { }

        public T GetOrNew<T>(string key) where T : AttachedResource, new()
        {
            if (TryGetValue(key, out var res) && res is T resT)
            {
                return resT;
            }
            else
            {
                T newRes = new();
                this[key] = newRes;
                return newRes;
            }
        }
    }
}
