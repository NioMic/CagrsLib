#if UNITY_EDITOR
using CagrsLib.LibCore;
using UnityEditor;
using Color = UnityEngine.Color;

namespace CagrsLib.Input.Editor
{
    [CustomEditor(typeof(CursorController))]
    public class CursorControllerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            LibUtil.DrawCagrsLibStyle(new Color(0.24f, 0.67f, 0.07f));

            base.OnInspectorGUI();
        }
    }
}
#endif