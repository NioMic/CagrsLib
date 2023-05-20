#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace CagrsLib.MeshBackpack.Editor
{
    [CustomEditor(typeof(MeshBackPackView))]
    public class MeshBackPackViewEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            MeshBackPackView backPackView = (MeshBackPackView)target;

            GUILayout.Label("w" + backPackView.GetComponent<RectTransform>().rect.width);
            GUILayout.Label("h" + backPackView.GetComponent<RectTransform>().rect.height);
            GUILayout.Label("ss" + backPackView.slotSize);
            GUILayout.Label("sp" + backPackView.spacing);

            if (backPackView.slotPrefab == null || 
                backPackView.gridLayoutGroup == null ||
                backPackView.contentView == null)
            {
                EditorGUILayout.HelpBox("Can't found slotPrefab, gridLayoutGroup or contentView", MessageType.Warning);
                return;
            }
            
            if (!EditorApplication.isPlaying)
            {
                GUILayout.BeginHorizontal();
                backPackView.autoRefreshUI = GUILayout.Toggle(backPackView.autoRefreshUI, "Auto Refresh UI");
                if (! backPackView.autoRefreshUI)
                {
                    if (GUILayout.Button("Refresh UI"))
                    {
                        backPackView.RefreshUI();
                    }
                }
                else
                {
                    backPackView.RefreshUI();
                }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                
                if (GUILayout.Button("Generate Preview"))
                {
                    backPackView.RegenerateGirdItems();
                }

                if (GUILayout.Button("Clear Preview"))
                {
                    backPackView.ClearGirdItems();
                }
                
                GUILayout.EndHorizontal();
            }
        }
    }
}

#endif 