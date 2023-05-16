#if UNITY_EDITOR
using CagrsLib.LibCore;
using UnityEditor;
using UnityEngine;

namespace CagrsLib.Archive.Editor
{
    [CustomEditor(typeof(ArchiveData))]
    public class ArchiveDataEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            LibUtil.DrawCagrsLibStyle(new Color(0.67f, 0f, 0.12f));
            
            ArchiveData data = (ArchiveData)target;
            
            bool lockWidget = GUILayout.Toggle(data.@lock, "Lock");
            
            if (lockWidget != data.@lock)
            {
                data.@lock = lockWidget;
                EditorUtility.SetDirty(target);
            }

            EditorGUI.BeginDisabledGroup(data.@lock);
            
            base.OnInspectorGUI();
            
            EditorGUI.EndDisabledGroup();
        }
    }
}
#endif