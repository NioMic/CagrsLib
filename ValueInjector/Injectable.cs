using System;
using System.Collections.Generic;
using CagrsLib.LibCore;
using UnityEngine;

namespace CagrsLib.ValueInjector
{
    public class Injectable : MonoBehaviour
    {
        private Dictionary<string, InjectorScript> _injectorScripts;

        private readonly List<string> _unloaded = new();

        private void OnEnable()
        {
            _injectorScripts = new Dictionary<string, InjectorScript>();
        }

        public string BindInjectorScript(InjectorScript script)
        {
            var guid = Guid.NewGuid().ToString();
            _injectorScripts.Add(guid, script);
            script.OnBind();

            return guid;
        }

        public void UnbindInjectorScript(string guid)
        {
            _unloaded.Add(guid);
        }

        public void InvokeScript(string invokeName, object[] parameters = null)
        {
            foreach (var injectorScript in _injectorScripts.Values)
            {
                if (injectorScript.IsActive()) injectorScript.OnInvoke(invokeName, parameters);
            }
        }

        public object GetInjectedValue(string injectName, object obj)
        {
            var objType = obj.GetType();
            if (objType.IsValueType)
            {
                foreach (var injectorScript in _injectorScripts.Values)
                {
                    if (injectorScript.IsActive())
                    {
                        var result = injectorScript.OnInject(injectName, obj);
                        if (result != null)
                        {
                            obj = result;
                        }
                    }
                }
            }
            else
            {
                LibUtil.LogWarning(this, 
                    "The object is not a structure, which means that the content of the object will be changed. To avoid this problem, use a structure as a formal parameter");
            }

            return obj;
        }

        private void Update()
        {
            foreach (var injectors in _injectorScripts.Values)
            {
                injectors.InjectorUpdate(Time.deltaTime);
            }
        }

        private void FixedUpdate()
        {
            foreach (var injectors in _injectorScripts.Values)
            {
                injectors.InjectorFixedUpdate(Time.fixedDeltaTime);
            }
        }

        private void LateUpdate()
        {
            foreach (var injectors in _injectorScripts.Values)
            {
                injectors.InjectorLateUpdate(Time.deltaTime);
            }

            foreach (var unload in _unloaded)
            {
                _injectorScripts.Remove(unload);
            }
            _unloaded.Clear();
        }

        public Dictionary<string, InjectorScript> GetInjectorScripts()
        {
            return _injectorScripts;
        }
    }
}
