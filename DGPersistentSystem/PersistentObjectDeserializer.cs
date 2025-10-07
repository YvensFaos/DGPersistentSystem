using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace DGPersistentSystem
{
    public static class PersistentObjectDeserializer
    {
        public static ScriptableObject DeserializeObjectToJson(string json)
        {
            if (string.IsNullOrEmpty(json)) return null;
            var serializedObject = JsonUtility.FromJson<SerializedObject>(json);
            var found = DataRegistry.GetInstance().persistentObjects.Find(p => p.ID.Equals(serializedObject.id));
            if (found != null)
            {
                return found;
            }

            //Construct ScriptableObject
            var type = serializedObject.type;
            var scriptableObjectType = Type.GetType(type);
            if (!typeof(PersistentScriptableObject).IsAssignableFrom(scriptableObjectType)) return null;
            var newScriptable = ScriptableObject.CreateInstance(scriptableObjectType);
            var fields = serializedObject.fields;
            ApplyFields(fields, newScriptable);
            return newScriptable;
        }

        public static List<ScriptableObject> DeserializeToJson(string json)
        {
            if (string.IsNullOrEmpty(json)) return null;
            var results = new List<ScriptableObject>();
            var serializedObjects = JsonUtility.FromJson<SerializedObjects>(json);
            serializedObjects.objects.ForEach(serializedObject =>
            {
                var deserializedObject = JsonUtility.FromJson<SerializedObject>(serializedObject);
                var found = DataRegistry.GetInstance().persistentObjects.Find(p => p.ID.Equals(deserializedObject.id));
                if (found != null)
                {
                    results.Add(found);
                }
                else
                {
                    //Construct ScriptableObject
                    var type = deserializedObject.type;
                    var scriptableObjectType = Type.GetType(type);
                    if (!typeof(PersistentScriptableObject).IsAssignableFrom(scriptableObjectType)) return;
                    var newScriptable = ScriptableObject.CreateInstance(scriptableObjectType);
                    var fields = deserializedObject.fields;
                    ApplyFields(fields, newScriptable);
                    results.Add(newScriptable);
                }
            });
            Debug.Log("6");
            return results;
        }

        private static void ApplyFields(List<SerializedFieldValue> fields, ScriptableObject target)
        {
            var targetType = target.GetType();

            foreach (var field in fields)
            {
                var valueToConvert = field.compressed ? DecompressString(field.value) : field.value;
                if (field.custom)
                {
                    var spriteName = typeof(Sprite).AssemblyQualifiedName;
                    if (field.originalType.Equals(spriteName))
                    {
                        //TODO
                    }
                    else
                    {
                        Debug.LogError($"No parsing found for custom type: {field.originalType}");
                    }
                }
                else
                {
                    var targetField = FindField(targetType, field.type);

                    var fieldType = targetField.GetType();
                    if (fieldType == typeof(int))
                    {
                        targetField.SetValue(target, int.Parse(valueToConvert));
                    }

                    if (fieldType == typeof(float))
                    {
                        targetField.SetValue(target, float.Parse(valueToConvert));
                    }

                    if (fieldType == typeof(Vector2))
                    {
                        var parts = valueToConvert.Split(',').Select(float.Parse).ToArray();
                        targetField.SetValue(target, new Vector2(parts[0], parts[1]));
                    }

                    if (targetType == typeof(Vector3))
                    {
                        var parts = valueToConvert.Split(',').Select(float.Parse).ToArray();
                        targetField.SetValue(target, new Vector3(parts[0], parts[1], parts[2]));
                    }

                    if (targetType == typeof(Color))
                    {
                        targetField.SetValue(target,
                            ColorUtility.TryParseHtmlString("#" + valueToConvert, out var color)
                                ? color
                                : Color.magenta);
                    }

                    if (targetType == typeof(Quaternion))
                    {
                        var parts = valueToConvert.Split(',').Select(float.Parse).ToArray();
                        targetField.SetValue(target, new Quaternion(parts[0], parts[1], parts[2], parts[3]));
                    }
                }
            }
        }

        private static FieldInfo FindField(Type type, string fieldName)
        {
            var currentType = type;
            while (currentType != null)
            {
                var field = currentType.GetField(fieldName,
                    BindingFlags.Public |
                    BindingFlags.NonPublic |
                    BindingFlags.Instance);

                if (field != null) return field;
                currentType = currentType.BaseType;
            }

            return null;
        }

        private static string DecompressString(string compressedString)
        {
            if (string.IsNullOrEmpty(compressedString)) return string.Empty;
            var compressedBytes = Convert.FromBase64String(compressedString);
            using var inputStream = new MemoryStream(compressedBytes);
            using var gzipStream = new GZipStream(inputStream, CompressionMode.Decompress);
            using var outputStream = new MemoryStream();
            gzipStream.CopyTo(outputStream);
            return Encoding.UTF8.GetString(outputStream.ToArray());
        }
    }
}