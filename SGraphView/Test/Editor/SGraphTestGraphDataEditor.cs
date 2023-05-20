#if UNITY_EDITOR 
using UnityEditor;
using UnityEngine;

namespace CagrsLib.SGraphView.Test.Editor
{
    [CustomEditor(typeof(SGraphTestGraphData))]
    public class SGraphTestGraphDataEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Open Graph Editor"))
            {
                SGraphTestWindow.OpenWindow((SGraphTestGraphData) target);
            }
        }
    }
}
#endif