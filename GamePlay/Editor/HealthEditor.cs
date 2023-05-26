#if UNITY_EDITOR
using CagrsLib.LibCore;
using UnityEditor;
using UnityEngine;

namespace CagrsLib.GamePlay.Editor
{
    [CustomEditor(typeof(Health))]
    public class HealthEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            LibUtil.DrawCagrsLibStyle(new Color(1f, 0.92f, 0f));
            
            Health health = (Health)target;

            float min = health.inRangeSetting.min;
            float max = health.inRangeSetting.max;

            if (max < min || min > max)
            {
                EditorGUILayout.HelpBox("Wrong Value !", MessageType.Error);
                LibUtil.Error(this, "Wrong value !");
            }

            base.OnInspectorGUI();

            health.inRange = GUILayout.Toggle(health.inRange, "Enable Range");
            
            if (health.inRange)
            {
                health.health = EditorGUILayout.Slider("Current Health", health.health, 
                    min, max);
                
                EditorGUILayout.Space();
                
                GUILayout.Label("Range");
                health.inRangeSetting.min = EditorGUILayout.FloatField("Min", min);
                health.inRangeSetting.max = EditorGUILayout.FloatField("Max", max);
            }
            else
            {
                health.health = EditorGUILayout.FloatField("Current Health", health.health);
            }
            
            EditorUtility.SetDirty(health);
        }
    }
}
#endif