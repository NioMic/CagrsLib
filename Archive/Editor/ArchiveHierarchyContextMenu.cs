#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace CagrsLib.Archive.Editor
{
    public class ArchiveHierarchyContextMenu : MonoBehaviour
    {
        [MenuItem("GameObject/CagrsLib/ArchiveManager", false, 10)]
        static void CreateArchiveManagerGameObject(MenuCommand menuCommand)
        {
            GameObject go = new GameObject("Archive Manager");
            
            go.AddComponent<ArchiveManager>();
            
            GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
            
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
            
            Selection.activeObject = go;
        }
        
        [MenuItem("GameObject/CagrsLib/ArchiveManager (Debugger)", false, 10)]
        static void CreateArchiveManagerWithDebuggerGameObject(MenuCommand menuCommand)
        {
            GameObject go = new GameObject("Archive Manager");
            
            go.AddComponent<ArchiveManager>();
            go.AddComponent<ArchiveDebugger>();
            
            GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
            
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
            
            Selection.activeObject = go;
        }
    }
}
#endif