using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace DGPersistentSystem
{
    public abstract class PersistentScriptableObject: ScriptableObject, IComparable
    {
        [SerializeField, HideInInspector]
        private string id;

        public string ID => id;

#if UNITY_EDITOR
        protected virtual void OnValidate()
        {
            GenerateGuidIfNeeded();
        }
        
        private void GenerateGuidIfNeeded()
        {
            if (Application.isPlaying) return;
            if (!string.IsNullOrEmpty(id)) return;
            id = Guid.NewGuid().ToString();

            EditorUtility.SetDirty(this);
            if (BuildPipeline.isBuildingPlayer)
            {
                AssetDatabase.SaveAssets();
            }
        }
#endif

        public int CompareTo(object obj)
        {
            if (obj is not PersistentScriptableObject otherPersistentObject) return -1;
            var fullName = otherPersistentObject.GetType().FullName;
            var value = GetType().FullName;
            if (value != null)
                return fullName != null && fullName.Equals(value)
                    ? string.Compare(id, otherPersistentObject.id, StringComparison.Ordinal)
                    : string.Compare(value, otherPersistentObject.GetType().FullName, StringComparison.Ordinal);
            return -1;
        }
    }
}
