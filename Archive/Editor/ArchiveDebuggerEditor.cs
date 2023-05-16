#if UNITY_EDITOR
using CagrsLib.LibCore;
using UnityEditor;
using UnityEngine;

namespace CagrsLib.Archive.Editor
{
    [CustomEditor(typeof(ArchiveDebugger))]
    public class ArchiveDebuggerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            LibUtil.DrawCagrsLibStyle(new Color(0.67f, 0f, 0.12f));
            
            ArchiveDebugger debugger = (ArchiveDebugger)target;

            EditorGUI.BeginDisabledGroup(!EditorApplication.isPlaying);
            
            GUILayout.BeginHorizontal();
            
            if (GUILayout.Button("Write"))
            {
                debugger.manager.WriteAll();
            }
            
            if (GUILayout.Button("Load"))
            {
                debugger.manager.LoadAll();
            }
            
            GUILayout.EndHorizontal();

            EditorGUI.EndDisabledGroup();
            
            base.OnInspectorGUI();
        }
    }
}
#endif