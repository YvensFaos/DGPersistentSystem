using System.Collections.Generic;
using UnityEngine;

namespace DGPersistentSystem
{
    [CreateAssetMenu(fileName = "Data Registry", menuName = "Persistency/Data Registry", order = 0)]
    public class DataRegistry: ScriptableObject
    {
        private static DataRegistry _instance;

        public static DataRegistry GetInstance()
        {
            return _instance;
        }
        
        [SerializeField] 
        public List<PersistentScriptableObject> persistentObjects;

        private void Awake()
        {
            if (_instance != null) return;
            Debug.Log($"Set DataRegistry {name} as singleton [Awake].");
            _instance = this;
        }

        private void OnValidate()
        {
            if (_instance != null) return;
            Debug.Log($"Set DataRegistry {name} as singleton [OnValidate].");
            _instance = this;
        }
    }
}