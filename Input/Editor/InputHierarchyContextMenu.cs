#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace CagrsLib.Input.Editor
{
    public class InputHierarchyContextMenu
    {
        [MenuItem("GameObject/CagrsLib/InputManager", false, 10)]
        static void CreateInputManagerGameObject(MenuCommand menuCommand)
        {
            GameObject inputManagerObject = new GameObject("Input System");
            GameObject cursorControllerObject = new GameObject("Cursor Controller");
            
            cursorControllerObject.AddComponent<CursorController>();
            inputManagerObject.AddComponent<InputManager>();

            InputManager inputManager = inputManagerObject.GetComponent<InputManager>();
            inputManager.configure = AssetDatabase.LoadAssetAtPath<InputConfigure>("Assets/CagrsLib/Input/Example/DefaultInput.asset");
            
            GameObjectUtility.SetParentAndAlign(inputManagerObject, menuCommand.context as GameObject);
            GameObjectUtility.SetParentAndAlign(cursorControllerObject, inputManagerObject);
            
            Undo.RegisterCreatedObjectUndo(inputManagerObject, "Create " + inputManagerObject.name);
            Selection.activeObject = inputManagerObject;
        }
    }
}
#endif