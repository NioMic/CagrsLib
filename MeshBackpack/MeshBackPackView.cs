using UnityEngine;
using UnityEngine.UI;

namespace CagrsLib.MeshBackpack
{
    public class MeshBackPackView : MonoBehaviour
    {
        [Header("UI Components")]
        public GridLayoutGroup gridLayoutGroup;
        public GameObject slotPrefab;
        public GameObject contentView;

        [Header("Size")] 
        [Range(1, 16)] public int meshSizeX = 5;
        [Range(1, 16)] public int meshSizeY = 5;

        [Header("Spacing")]
        [Range(0, 100)] public float horizontalSpacing = 5;

        [HideInInspector]
        public float slotSize;
        
        [HideInInspector]
        public float spacing;

        [HideInInspector] 
        public bool autoRefreshUI;
        public void RefreshUI()
        {
            if (slotPrefab == null) return;
            if (gridLayoutGroup == null) return;
            if (contentView == null) return;
            
            RectTransform rectTransform = GetComponent<RectTransform>();

            slotSize = rectTransform.rect.width / meshSizeX;

            spacing = slotSize / 100 * horizontalSpacing;
            
            slotSize -= spacing - 0.1f;
            
            RectTransform prefabRectTransform = slotPrefab.GetComponent<RectTransform>();

            prefabRectTransform.sizeDelta = new Vector2(slotSize, slotSize);

            gridLayoutGroup.spacing = new Vector2(spacing, spacing);
            var rect = prefabRectTransform.rect;
            gridLayoutGroup.cellSize = new Vector2(rect.width, rect.width);
            
            foreach (RectTransform child in contentView.GetComponentsInChildren<RectTransform>())
            {
                child.sizeDelta = new Vector2(slotSize, slotSize);
            }
        }
        
        public void RegenerateGirdItems()
        {
            ClearGirdItems();

            for (int i = 0; i < meshSizeX * meshSizeY; i++)
            {
                GameObject newUIElement = Instantiate(slotPrefab, Vector3.zero, Quaternion.Euler(Vector3.zero));

                newUIElement.name = $"Slot UI_{i}";
                
                RectTransform elementRectTransform = newUIElement.GetComponentInChildren<RectTransform>();
                
                elementRectTransform.sizeDelta = new Vector2(slotSize, slotSize);
                
                elementRectTransform.SetParent(contentView.transform);
            }

            foreach (RectTransform child in contentView.GetComponentInChildren<RectTransform>())
            {
                child.sizeDelta = new Vector2(slotSize, slotSize);
            }
        }
        
        public void ClearGirdItems()
        {
            for(int i = 0; i < contentView.transform.childCount; i++)
            {
                if (contentView.transform.GetChild(i).transform == transform) continue;
                
                DestroyImmediate(contentView.transform.GetChild(i).gameObject);
            }
        }
    }
}
