using System;
using System.Collections.Generic;
using System.Reflection;
using CagrsLib.LibCore;
using UnityEngine;

namespace CagrsLib.Archive
{
    [DisallowMultipleComponent]
    [AddComponentMenu("CagrsLib/Archive/ArchiveData")]
    public class ArchiveData : MonoBehaviour
    {
        [HideInInspector]
        public bool @lock;
        
        public string dataGuid;
        
        private void Reset()
        {
            dataGuid = Guid.NewGuid().ToString();
        }

        public List<ArchivePack> WriteArchive()
        {
            List<ArchivePack> archivePacks = new();

            Component[] components = GetComponents<Component>();
            foreach (Component component in components)
            {
                Type currentType = component.GetType();
                MethodInfo[] methods = currentType.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                foreach (MethodInfo method in methods)
                {
                    Write attribute = method.GetCustomAttribute<Write>();
                    if (attribute != null)
                    {
                        if (method.GetParameters().Length == 0 && method.ReturnType == typeof(ArchivePack))
                        {
                            archivePacks.Add((ArchivePack)method.Invoke(component, null));
                        }
                    }
                }
            }

            return archivePacks;
        }

        public void LoadArchive(Dictionary<string, ArchivePack> packs)
        {
            Component[] components = GetComponents<Component>();
            foreach (Component component in components)
            {
                Type currentType = component.GetType();
                MethodInfo[] methods = currentType.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                foreach (MethodInfo method in methods)
                {
                    Load attribute = method.GetCustomAttribute<Load>();
                    if (attribute != null)
                    {
                        ParameterInfo[] parameters = method.GetParameters();
                        if (parameters.Length == 1 && parameters[0].ParameterType == typeof(ArchivePack))
                        {
                            try
                            {
                                ArchivePack pack = packs[attribute.GetName()];
                                method.Invoke(component, new object[]{pack});
                            }
                            catch (KeyNotFoundException)
                            {
                                LibUtil.LogWarning(this, "Fail to find pack : " + attribute.GetName());
                            }
                        }
                    }
                }
            }
        }

        #region Attributes

        [AttributeUsage(AttributeTargets.Method)]
        public class Write : Attribute {}

        [AttributeUsage(AttributeTargets.Method)]
        public class Load : Attribute
        {
            private string _fromPack;

            public Load(string fromPack)
            {
                _fromPack = fromPack;
            }

            public string GetName()
            {
                return _fromPack;
            }
        }
        
        // TODO :: 具体实现 Write / Load Field 功能
        [AttributeUsage(AttributeTargets.Field)]
        public class WriteField : Attribute
        {
            private string _packName;

            public WriteField(string packName)
            {
                _packName = packName;
            }

            public string GetName()
            {
                return _packName;
            }
        }

        [AttributeUsage(AttributeTargets.Field)]
        public class LoadField : Attribute
        {
            private string _fromPack;

            public LoadField(string fromPack)
            {
                _fromPack = fromPack;
            }

            public string GetName()
            {
                return _fromPack;
            }
        }
        
        #endregion
    }
    
    [Serializable]
    public class ArchivePack
    {
        public string packName;
        public Dictionary<string, object> Values = new();

        public static ArchivePack CreatePack(string name)
        {
            return new ArchivePack
            {
                packName = name
            };
        }

        public void Write(string name, object obj)
        {
            Values.Add(name, obj);
        }
        
        public void Write(Value value)
        {
            Values.Add(value.ValueName, value.ValueObject);
        }

        public object Load(string name, object notFound = null)
        {
            if (!Values.ContainsKey(name))
            {
                return notFound;
            }

            return Values[name];
        }

        public float LoadFloat(string name, float notFound = 0)
        {
            return Convert.ToSingle(Load(name, notFound));
        }

        public int LoadInteger(string name, float notFound = 0)
        {
            return Convert.ToInt32(Load(name, notFound));
        }

        [Serializable]
        public class Value
        {
            public string ValueName;
            public object ValueObject;

            public Value(string valueName, object valueObject)
            { 
                ValueName = valueName;
                ValueObject = valueObject;
            }
        }
    }
    
    [Serializable]
    public class ArchiveSetting
    {
        public bool write, load;
    }
}
