using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace DGPersistentSystem.Utils.Editor
{
    public static class AssetCollector
    {
        public static List<T> Get<T>()
        {
            var results = new List<T>();
            var assetPaths = Directory.GetFiles(Application.dataPath, "*.asset", SearchOption.AllDirectories)
                .Where(path => !path.Contains("/Editor/"))
                .Select(path => path[(Application.dataPath.Length - "Assets".Length)..])
                .ToArray();

            foreach (var assetPath in assetPaths)
            {
                var obj = AssetDatabase.LoadAssetAtPath<ScriptableObject>(assetPath);
                if (obj != null && obj is T objT)
                {
                    results.Add(objT);
                }
            }
            return results;
        }
    }
}