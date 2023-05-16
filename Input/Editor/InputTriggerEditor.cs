#if UNITY_EDITOR
using System.Text;
using CagrsLib.LibCore;
using UnityEditor;
using UnityEngine;

namespace CagrsLib.Input.Editor
{
    [CustomEditor(typeof(InputTrigger))]
    public class InputTriggerEditor : UnityEditor.Editor
    {
        string content = "";

        public override void OnInspectorGUI()
        {
            LibUtil.DrawCagrsLibStyle(new Color(0.24f, 0.67f, 0.07f));
            
            InputTrigger inputTrigger = (InputTrigger)target;

            bool stepCheck = inputTrigger.steps == null || inputTrigger.steps.Count == 0;
            if (stepCheck)
            {
                StringBuilder hint = new StringBuilder();
                hint.AppendLine("Format : <Operation Name> <Active Type> <Wait Seconds> <In Time> <Out Time>");
                hint.AppendLine("Active Type : Down / Up");
                hint.AppendLine("Wait Seconds : One seconds -> 1");
                hint.AppendLine("In/Out Time : None / Next / End / Trigger / Return / Error");
                hint.AppendLine("");
                hint.AppendLine("Example : (Double Click Jump)");
                hint.AppendLine("Jump Down 1 None None");
                hint.AppendLine("Jump Up 1 Next End");
                hint.AppendLine("Jump Down 1 Return End");
            
                EditorGUILayout.HelpBox(hint.ToString(), MessageType.None);
                EditorGUILayout.HelpBox("Invalid trigger configure", MessageType.Warning);
            }
            
            if (string.IsNullOrEmpty(inputTrigger.triggerName))
            {
                EditorGUILayout.HelpBox("Please enter a name", MessageType.Error);
            }
            
            content = EditorGUILayout.TextArea(content);

            EditorGUILayout.BeginHorizontal();
            
            if (GUILayout.Button("READ ↑"))
            {
                content = inputTrigger.StepsToString();
            }

            GUI.enabled = !string.IsNullOrEmpty(content);
            if (GUILayout.Button("WRITE ↓"))
            {
                inputTrigger.StringToSteps(content);
                EditorUtility.SetDirty(inputTrigger);
                LibUtil.Log(this, "Written !");
            }
            
            EditorGUILayout.EndHorizontal();

            GUI.enabled = true;
            base.OnInspectorGUI();
            
            if (!stepCheck)
            {
                EditorGUILayout.HelpBox(inputTrigger.StepsToString(), MessageType.None);
            }
        }
    }
}
#endif