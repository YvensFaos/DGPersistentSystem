using DGPersistentSystem.Utils.Editor;
using UnityEditor;
using UnityEngine;

namespace DGPersistentSystem.Editor
{
    [CustomEditor(typeof(DataRegistry))]
    public class DataRegistryEditor : UnityEditor.Editor
    {
        private string _jsonInputString =
            "{\n    \"id\": \"ab235a09-999d-46c5-b2ad-160d497d29ba\",\n    \"name\": \"ItemExample\",\n    \"type\": \"ItemSO\",\n    \"fields\": [\n        {\n            \"field\": \"itemSpriteString\",\n            \"type\": \"Sprite\",\n            \"value\": \"Background\",\n            \"originalType\": \"itemSprite\",\n            \"custom\": true,\n            \"compressed\": false\n        },\n        {\n            \"field\": \"itemName\",\n            \"type\": \"String\",\n            \"value\": \"Potion\",\n            \"originalType\": \"\",\n            \"custom\": false,\n            \"compressed\": false\n        },\n        {\n            \"field\": \"itemCount\",\n            \"type\": \"Int32\",\n            \"value\": \"100\",\n            \"originalType\": \"\",\n            \"custom\": false,\n            \"compressed\": false\n        }\n    ]\n}";

        private ScriptableObject _scriptableObj;

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            var dataRegistry = (DataRegistry)target;

            var persistentObjects = dataRegistry.persistentObjects;
            if (GUILayout.Button("Print Persistent Objects JSON"))
            {
                persistentObjects.ForEach(persistentScriptableObject =>
                {
                    var json = PersistentObjectSerializer.SerializeToJson(persistentScriptableObject);
                    Debug.Log(json);
                });
            }

            if (GUILayout.Button("Sort Registry"))
            {
                dataRegistry.persistentObjects.Sort();
                EditorUtility.SetDirty(dataRegistry);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }

            if (GUILayout.Button("Refresh Database"))
            {
                dataRegistry.persistentObjects = AssetCollector.Get<PersistentScriptableObject>();
                dataRegistry.persistentObjects.Sort();
                EditorUtility.SetDirty(dataRegistry);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }

            _scriptableObj = (ScriptableObject)EditorGUILayout.ObjectField("Debug SO Reference", _scriptableObj,
                typeof(ScriptableObject), false);
            _jsonInputString = EditorGUILayout.TextArea(_jsonInputString, EditorStyles.textField);
            
            if (GUILayout.Button("Try to Deserialize Test"))
            {
                var result = PersistentObjectDeserializer.DeserializeObjectToJson(_jsonInputString);
                _scriptableObj = result;
                Debug.Log(result);
            }
        }
    }
}