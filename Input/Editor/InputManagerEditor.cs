#if UNITY_EDITOR
using CagrsLib.LibCore;
using UnityEditor;
using UnityEngine;

namespace CagrsLib.Input.Editor
{
    [CustomEditor(typeof(InputManager))]
    public class InputManagerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            LibUtil.DrawCagrsLibStyle(new Color(0.24f, 0.67f, 0.07f));
            
            InputManager manager = (InputManager) target;

            base.OnInspectorGUI();
        }
    }
}
#endif