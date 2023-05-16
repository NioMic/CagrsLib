#if UNITY_EDITOR
using CagrsLib.LibCore;
using UnityEditor;
using UnityEngine;

namespace CagrsLib.GamePlay.Editor
{
    [CustomEditor(typeof(DamageRange))]
    public class DamageRangeEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            LibUtil.DrawCagrsLibStyle(new Color(1f, 0.92f, 0f));

            base.OnInspectorGUI();
        }
    }
}
#endif