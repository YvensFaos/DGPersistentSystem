using System;

namespace DGPersistentSystem
{
    [AttributeUsage(AttributeTargets.Field)]
    public class PersistentFieldAttribute: Attribute
    {
        public string CustomKey { get; }
        public bool Compress { get; }

        private readonly bool _hasCustomKey;
    
        public PersistentFieldAttribute(string customKey = null, bool compress = false)
        {
            CustomKey = customKey;
            Compress = compress;
            _hasCustomKey = !string.IsNullOrEmpty(customKey);
        }

        public bool HasCustomKey() => _hasCustomKey;
    }
}