using System;
using System.Collections.Generic;
using System.Reflection;
using CagrsLib.LibCore;
using UnityEngine;

namespace CagrsLib.GamePlay
{
    [AddComponentMenu("CagrsLib/Gameplay/Pickup")]
    [DisallowMultipleComponent]
    public class Pickup : MonoBehaviour
    {
        public Collider checkTrigger;

        public List<PickupItem> found;

        public List<Component> linkComponents;
        
        public void Awake()
        {
            if (! checkTrigger.isTrigger)
            {
                LibUtil.LogWarning(this, "'checkTrigger' is not a Trigger !");
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            PickupItem pickupItem = other.gameObject.GetComponent<PickupItem>();
            if (pickupItem != null)
            {
                if (!found.Contains(pickupItem)) found.Add(pickupItem);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            PickupItem pickupItem = other.gameObject.GetComponent<PickupItem>();
            if (pickupItem != null)
            {
                if (found.Contains(pickupItem)) found.Remove(pickupItem);
            }
        }

        public void PickupNearest()
        {
            List<Transform> transforms = new List<Transform>();
            if (transforms == null) throw new ArgumentNullException(nameof(transforms));

            foreach (var pickupItem in found)
            {
                transforms.Add(pickupItem.transform);
            }

            Transform nearestTransform = LibUtil.FindNearest(transform, transforms.ToArray());
            
            if (nearestTransform == null) return; 
            
            PickupItem nearest = nearestTransform.gameObject.GetComponent<PickupItem>();

            if (nearest == null) return; 
            
            InvokeLinks(nearest);
            nearest.Pickup();
        }

        public void PickupAll()
        {
            foreach (var pickupItem in found)
            {
                InvokeLinks(pickupItem);
                pickupItem.Pickup();
            }
        }

        public void RequirePickup(PickupItem pickupItem)
        {
            found.Remove(pickupItem);
        }

        /*
         
         [PickupLink]
         void OnPickup (PickupItem item)
         {
            ...
         }
         
         */
        public void InvokeLinks(PickupItem pickupItem)
        {
            foreach (var component in linkComponents)
            {
                Type currentType = component.GetType();
                MethodInfo[] methods = currentType.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                foreach (MethodInfo method in methods)
                {
                    PickupLink attribute = method.GetCustomAttribute<PickupLink>();
                    if (attribute != null)
                    {
                        ParameterInfo[] parameters = method.GetParameters();
                        if (parameters.Length == 1 && parameters[0].ParameterType == typeof(PickupItem))
                        {
                            method.Invoke(component, new object[]{pickupItem});
                        }
                    }
                }
            }
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class PickupLink : Attribute
    {
    }
}