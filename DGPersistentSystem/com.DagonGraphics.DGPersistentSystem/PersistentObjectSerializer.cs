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
    public static class PersistentObjectSerializer
    {
        public static string SerializeToJson(PersistentScriptableObject so)
        {
            if (so == null) return string.Empty;
            var serialized = new SerializedObject(so.ID, so.name, so.GetType().Name);
            var fields = GetAllFields(so.GetType())
                .Where(f => f.GetCustomAttribute<PersistentFieldAttribute>() != null)
                .ToList();
            fields.ForEach(field =>
            {
                var attribute = field.GetCustomAttribute<PersistentFieldAttribute>();
                var value = GetFieldValue(field, attribute, so);

                serialized.fields.Add(attribute.HasCustomKey()
                    ? new SerializedFieldValue(attribute.CustomKey, field.FieldType.Name, value, field.Name, true, attribute.Compress)
                    : new SerializedFieldValue(field.Name, field.FieldType.Name, value,"", false, attribute.Compress));
            });
            return JsonUtility.ToJson(serialized, true);
        }

        private static IEnumerable<FieldInfo> GetAllFields(Type type)
        {
            var fields = new List<FieldInfo>();
            var currentType = type;
            while (currentType != null && currentType != typeof(object))
            {
                fields.AddRange(currentType.GetFields(
                    BindingFlags.Instance | 
                    BindingFlags.Public | 
                    BindingFlags.NonPublic | 
                    BindingFlags.DeclaredOnly));
                currentType = currentType.BaseType;
            }
            return fields;
        }

        private static string GetFieldValue(FieldInfo field, PersistentFieldAttribute attribute, object so)
        {
            var value = field.GetValue(so);
            var strValue = value switch
            {
                null => string.Empty,
                int intValue => $"{intValue}",
                float floatValue => $"{floatValue}",
                Vector2 vector2 => $"{vector2.x},{vector2.y}",
                Vector3 vector3 => $"{vector3.x},{vector3.y},{vector3.z}",
                Color color => ColorUtility.ToHtmlStringRGBA(color),
                Quaternion quaternion => $"{quaternion.x},{quaternion.y},{quaternion.z},{quaternion.w}",
                Sprite sprite => $"{sprite.name}",
                string => value.ToString(),
                _ => ""
            };
            return attribute.Compress ? CompressString(strValue) : strValue;
        }

        private static string CompressString(string input)
        {
            if (string.IsNullOrEmpty(input)) return string.Empty;
            var inputBytes = Encoding.UTF8.GetBytes(input);
            using var outputStream = new MemoryStream();
            using (var gzipStream = new GZipStream(outputStream, CompressionMode.Compress))
            {
                gzipStream.Write(inputBytes, 0, inputBytes.Length);
            }
            return Convert.ToBase64String(outputStream.ToArray());
        }
    }
}