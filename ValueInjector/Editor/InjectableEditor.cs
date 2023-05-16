#if UNITY_EDITOR
using System.Collections.Generic;
using CagrsLib.LibCore;
using UnityEditor;
using UnityEngine;
using Color = UnityEngine.Color;

namespace CagrsLib.ValueInjector.Editor
{
    [CustomEditor(typeof(Injectable))]
    public class InjectableEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            LibUtil.DrawCagrsLibStyle(new Color(0.92f, 0.87f, 0f));
            
            base.OnInspectorGUI();

            Injectable injectable = (Injectable)target;

            Dictionary<string, InjectorScript> injectorScripts = injectable.GetInjectorScripts();

            int i = 1;
            
            if (injectorScripts == null) return;
            
            foreach (var injectorScript in injectorScripts)
            {
                EditorGUILayout.BeginHorizontal();

                GUILayout.Label(i + ". " + injectorScript.Value.GetType().Name);
                
                if (GUILayout.Button("REMOVE"))
                {
                    injectable.UnbindInjectorScript(injectorScript.Key);
                }

                EditorGUILayout.EndHorizontal();
                
                EditorGUILayout.HelpBox(injectorScript.Value.GetDestruction(), MessageType.Info);
                
                EditorGUILayout.Space();
                
               i++;
            }
        }
    }
}
#endif