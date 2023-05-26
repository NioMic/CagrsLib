#if UNITY_EDITOR
using System.Collections.Generic;
using CagrsLib.Input.Windows;
using CagrsLib.LibCore;
using UnityEditor;
using UnityEngine;
using Color = UnityEngine.Color;

namespace CagrsLib.Input.Editor
{
    [CustomEditor(typeof(InputConfigure))]
    public class InputConfigureEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            LibUtil.DrawCagrsLibStyle(new Color(0.24f, 0.67f, 0.07f));

            InputConfigure configure = (InputConfigure)target;
            
            if (GUILayout.Button("Operation Editor", GUILayout.Height(30)))
            {
                if (configure.operations == null)
                {
                    configure.operations = new List<InputOperation>();
                }

                InputConfigureEditorWindow.OpenConfigure(configure);
            }

            base.OnInspectorGUI();
        }
    }
}
#endif