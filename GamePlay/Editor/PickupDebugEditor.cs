#if UNITY_EDITOR
using CagrsLib.LibCore;
using UnityEditor;
using UnityEngine;

namespace CagrsLib.GamePlay.Editor
{
    [CustomEditor(typeof(Pickup))]
    public class PickupDebugEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            LibUtil.DrawCagrsLibStyle(new Color(1f, 0.92f, 0f));
            
            Pickup pickup = (Pickup) target;
            
            EditorGUI.BeginDisabledGroup(!EditorApplication.isPlaying);
            
            if (GUILayout.Button("PickupAll"))
            {
                pickup.PickupAll();
            }

            if (GUILayout.Button("PickupNearest"))
            {
                pickup.PickupNearest();
            }

            EditorGUI.EndDisabledGroup();

            base.OnInspectorGUI();
        }
    }
}
#endif