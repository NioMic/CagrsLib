using UnityEngine;
using UnityEngine.Events;

namespace CagrsLib.GamePlay
{
    [DisallowMultipleComponent]
    [AddComponentMenu("CagrsLib/Gameplay/PickupItem")]
    public class PickupItem : MonoBehaviour
    {
        public bool autoDestroyAfterPickup;
        
        public string itemName;
        
        public UnityEvent onPickup;

        private bool _isDestroy;
        
        public void Pickup()
        {
            onPickup.Invoke();
            if (autoDestroyAfterPickup)
            {
                _isDestroy = true;
            }
        }

        public void DestroyItem()
        {
            _isDestroy = true;
        }

        private void LateUpdate()
        {
            if (_isDestroy)
            {
                Destroy(gameObject);
            }
        }
    }
}