using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DGPersistentSystem
{
    public class SaveRegistry
    {
        private List<PersistentScriptableObject> _savedPersistentObjects;

        public virtual string SaveRegistryToJson()
        {
            var serializedObjects = new SerializedObjects(_savedPersistentObjects.Select(PersistentObjectSerializer.SerializeToJson).ToList());
            return JsonUtility.ToJson(serializedObjects, true);
        }
    }
}