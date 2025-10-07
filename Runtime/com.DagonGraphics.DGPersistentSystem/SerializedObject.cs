using System;
using System.Collections.Generic;

namespace DGPersistentSystem
{
    [Serializable]
    public struct SerializedObjects
    {
        public List<string> objects;
        public SerializedObjects(List<string> objects)
        {
            this.objects = objects;
        }
    }
    
    [Serializable]
    public struct SerializedObject
    {
        public string id;
        public string name;
        public string type;
        public List<SerializedFieldValue> fields;
        public SerializedObject(string id, string name, string type)
        {
            this.id = id;
            this.name = name;
            this.type = type;
            fields = new List<SerializedFieldValue>();
        }
    }
    
    [Serializable]
    public struct SerializedFieldValue
    {
        public string field;
        public string type;
        public string value;
        public string originalType;
        public bool custom;
        public bool compressed;

        public SerializedFieldValue(string field, string type, string value, string originalType, bool custom, bool compressed)
        {
            this.field = field;
            this.type = type;
            this.value = value;
            this.originalType = originalType;
            this.custom = custom;
            this.compressed = compressed;
        }
    }
}