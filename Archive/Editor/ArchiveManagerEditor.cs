#if UNITY_EDITOR
using CagrsLib.LibCore;
using UnityEditor;
using UnityEngine;

namespace CagrsLib.Archive.Editor
{
    [CustomEditor(typeof(ArchiveManager))]
    public class ArchiveManagerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            ArchiveManager manager = (ArchiveManager)target;
            
            LibUtil.DrawCagrsLibStyle(new Color(0.67f, 0f, 0.12f));
            
            base.OnInspectorGUI();

            if (EditorGUILayout.LinkButton("Copy Archive Path ↓"))
            {
                GUIUtility.systemCopyBuffer = manager.GetFolder().FullName;
            }
            
            EditorGUILayout.SelectableLabel(manager.GetFolder().FullName);
        }
    }
}
#endif