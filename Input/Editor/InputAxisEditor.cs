#if UNITY_EDITOR
using CagrsLib.LibCore;
using UnityEditor;
using UnityEngine;

namespace CagrsLib.Input.Editor
{
    [CustomEditor(typeof(InputAxis))]
    public class InputAxisEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            LibUtil.DrawCagrsLibStyle(new Color(0.24f, 0.67f, 0.07f));
            
            InputAxis inputAxis = (InputAxis)target;
            
            if (string.IsNullOrEmpty(inputAxis.name) || inputAxis.name.Length <= 0)
            {
                EditorGUILayout.HelpBox("Please enter a name", MessageType.Error);
            }
            
            if (string.IsNullOrEmpty(inputAxis.positiveOperationName) || 
                string.IsNullOrEmpty(inputAxis.negativeOperationName))
            {
                EditorGUILayout.HelpBox("Invalid axis configure", MessageType.Error);
            }
            
            base.OnInspectorGUI();
        }
    }
}
#endif