#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace CagrsLib.MeshBackpack.Editor
{
    public class MeshBackPackViewHierarchyContextMenu
    {
        [MenuItem("GameObject/CagrsLib/MeshBackPackView", false, 10)]
        static void CreateMeshBackPackView(MenuCommand menuCommand)
        {
            GameObject backPackView = new GameObject("Backpack View");
            GameObject slotPrefabView = new GameObject("Slot Prefab View");
            GameObject slotPrefabBackgroundView = new GameObject("Slot Prefab Background");
            GameObject backPackGird = new GameObject("Backpack Gird");
            
            GameObjectUtility.SetParentAndAlign(backPackGird, menuCommand.context as GameObject);
            GameObjectUtility.SetParentAndAlign(slotPrefabView, backPackView);
            GameObjectUtility.SetParentAndAlign(backPackGird, backPackView);
            GameObjectUtility.SetParentAndAlign(slotPrefabBackgroundView, slotPrefabView);
            
            backPackGird.layer = LayerMask.NameToLayer("UI");
            slotPrefabView.layer = LayerMask.NameToLayer("UI");
            slotPrefabBackgroundView.layer = LayerMask.NameToLayer("UI");
            backPackGird.layer = LayerMask.NameToLayer("UI");
            
            Undo.RegisterCreatedObjectUndo(backPackView, "Create " + backPackView.name);
            Selection.activeObject = backPackView;
            
            RectTransform backPackRect = backPackView.AddComponent<RectTransform>();
            MeshBackPackView meshBackPackView = backPackView.AddComponent<MeshBackPackView>();
            backPackRect.pivot = new Vector2(0.5f, 0.5f);
            backPackRect.anchorMax = new Vector2(0.5f, 0.5f);
            backPackRect.anchorMin = new Vector2(0.5f, 0.5f);

            RectTransform slotPrefabRect = slotPrefabView.AddComponent<RectTransform>();
            slotPrefabRect.pivot = new Vector2(0.5f, 0.5f);
            slotPrefabRect.anchorMax = new Vector2(0, 1);
            slotPrefabRect.anchorMin = new Vector2(0, 1);
            
            // TODO :: 这里不知道为什么在构建的时候总是不会自动填充父对象
            Image slotPrefabBackgroundImage = slotPrefabBackgroundView.AddComponent<Image>();
            RectTransform slotPreviewBackgroundRect = slotPrefabBackgroundView.GetComponent<RectTransform>();
            slotPrefabBackgroundImage.color = new Color(0,0,0,0.5f);
            slotPreviewBackgroundRect.pivot = new Vector2(0.5f, 0.5f);
            slotPreviewBackgroundRect.anchorMax = new Vector2(0, 0);
            slotPreviewBackgroundRect.anchorMin = new Vector2(1, 1);
            slotPreviewBackgroundRect.localScale = Vector3.one;
            slotPreviewBackgroundRect.localPosition = Vector3.zero;
            slotPreviewBackgroundRect.sizeDelta = slotPrefabRect.sizeDelta;

            GridLayoutGroup backPackGirdLayoutGroup = backPackGird.AddComponent<GridLayoutGroup>();
            RectTransform backPackGirdRect = backPackGird.GetComponent<RectTransform>();
            backPackGirdLayoutGroup.childAlignment = TextAnchor.MiddleCenter;
            backPackGirdRect.anchorMax = new Vector2(0, 0);
            backPackGirdRect.anchorMin = new Vector2(1, 1);
            backPackGirdRect.localScale = Vector3.one;
            backPackGirdRect.localPosition = Vector3.zero;
            backPackGirdRect.sizeDelta = backPackRect.sizeDelta;

            meshBackPackView.slotPrefab = slotPrefabView;
            meshBackPackView.gridLayoutGroup = backPackGirdLayoutGroup;
            meshBackPackView.contentView = backPackGird;
        }
    }
}
#endif