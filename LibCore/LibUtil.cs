using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UIElements;

namespace CagrsLib.LibCore
{
    public class LibUtil
    {
        #region Object

        public static string GetObjectName<TC>(Expression<Func<TC>> memberExpression)
        {
            MemberExpression expressionBody = (MemberExpression)memberExpression.Body;
            return expressionBody.Member.Name;
        }
        
        public static bool IsTagExist(string tag)
        {
            string[] tags = InternalEditorUtility.tags;
            foreach (var varTag in tags)
            {
                if (varTag.Equals(tag)) return true;
            }
            return false;
        }

        public static void CreateTag(string tagName)
        {
            if (! IsTagExist(tagName)) 
                InternalEditorUtility.AddTag(tagName);
        }
        
        public static Transform FindNearest(Transform player, Transform[] objects)
        {
            if (objects.Length < 1) return null;
            
            Transform nearGameObject = objects[0];
            float firstLength = Vector3.Distance(player.position, objects[0].position);
            for (int i = 1; i < objects.Length; i++)
            { 
                float length = Vector3.Distance(player.position, objects[i].position);
                if (firstLength > length)
                {
                    firstLength = length;
                    nearGameObject = objects[i];
                }
            }
            return nearGameObject;
        }

        #endregion
        
        #region Math

        public static int InRange(int n, int min, int max)
        {
            if (n < min) return min;
            if (n > max) return max;
            return n;
        }
        
        public static float InRange(float n, float min, float max)
        {
            if (n < min) return min;
            if (n > max) return max;
            return n;
        }

        public static float To(float a, float b, float j)
        {
            if (a > b) return a - j;
            if (a < b) return a + j;
            return a;
        }
        
        public static int To(int a, int b, int j)
        {
            if (a > b) return a - j;
            if (a < b) return a + j;
            return a;
        }
        
        public static float StrictTo(float a, float b, float j)
        {
            if (a > b) return InRange(a - j, b, a);
            if (a < b) return InRange(a + j, a, b);
            return a;
        }
        
        public static int StrictTo(int a, int b, int j)
        {
            if (a > b) return InRange(a - j, b, a);
            if (a < b) return InRange(a + j, a, b);
            return a;
        }

        public static double V3DistanceToZero(Vector3 p)
        {
            return Math.Sqrt(Math.Pow(p.x, 2) + Math.Pow(p.y, 2) + Math.Pow(p.z, 2));
        }
        
        public static double V2DistanceToZero(Vector2 p)
        {
            return Math.Sqrt(Math.Pow(p.x, 2) + Math.Pow(p.y, 2));
        }

        #endregion
        
        #region File Operation

        public static string ReadFile(FileInfo fileInfo)
        {
            string outputString = "";
    
            try
            {
                using (StreamReader streamReader = new StreamReader(fileInfo.FullName))
                {
                    string append;
    
                    while ((append = streamReader.ReadLine()) != null)
                    {
                        outputString += append + "\n";
                    }
                }
            }
            catch (FileNotFoundException exception)
            {
                Debug.LogError($"Can't found file \":{fileInfo.FullName}\"\n{exception}");
            }
            catch (IOException exception)
            {
                Debug.LogError($"I/O Exception : {fileInfo.FullName}\n{exception}");
            }
            catch (Exception exception)
            {
                Debug.LogError(exception);
            }
    
            return outputString;
        }
    
        public static void WriteFile(FileInfo fileInfo, string content)
        {
            if (fileInfo.Directory is { Exists: false })
            {
                fileInfo.Directory.Create();
            }
    
            string[] split = content.Split("\n");
            using (StreamWriter streamWriter = new StreamWriter(fileInfo.FullName))
            {
                foreach (string value in split)
                {
                    streamWriter.WriteLine(value);
                }
            }
        }

        #endregion
        
        #region Output & Debug

        public static void Log(object self, string content)
        {
            Debug.Log($"{self.GetType().Name} -> {content}");
        }
        
        public static void Error(object self, string content)
        {
            Debug.LogError($"{self.GetType().Name} -> {content}");
        }
        
        public static void LogWarning(object self, string content)
        {
            Debug.LogWarning($"{self.GetType().Name} -> {content}");
        }

        #endregion
        
        #region DrawInspector

        public static void DrawInspector()
        {
            
        }

        public static void DrawCagrsLibStyle(Color color)
        {
            // GUI.backgroundColor = color;
            // GUI.contentColor = new Color(0.92f, 0.99f, 1f);
            // GUI.contentColor = color;
        }

        #endregion
        
        #region List To Dictionary

        public static Dictionary<TK, TV> List2Dic<TK, TV>(List<TK> keys, List<TV> values)
        {
            int count = Math.Min(keys.Count, values.Count);
            Dictionary<TK, TV> dic = new Dictionary<TK, TV>();
            
            for (int i = 0; i < count; i++)
            {
                dic.Add(keys[i], values[i]);
            }

            return dic;
        }

        public static void Dic2List<TK, TV>(Dictionary<TK, TV> dictionary, out List<TK> keys, out List<TV> values)
        {
            keys = dictionary.Keys.ToList();
            values = dictionary.Values.ToList();
        }

        public static List<KeyValuePair<TK, TV>> Dic2KeyValuePairs<TK, TV>(Dictionary<TK, TV> dictionary)
        {
            List<KeyValuePair<TK, TV>> keyValuePairs = new();
            foreach (var kvp in dictionary)
            {
                keyValuePairs.Add(kvp);
            }

            return keyValuePairs;
        }

        #endregion
    }
}
