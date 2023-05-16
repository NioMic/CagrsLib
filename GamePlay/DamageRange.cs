using System;
using System.Collections.Generic;
using UnityEngine;

namespace CagrsLib.GamePlay
{
    [RequireComponent(typeof(Collider))]
    [AddComponentMenu("CagrsLib/Gameplay/DamageRange")]
    public class DamageRange : MonoBehaviour
    {
        public List<Health> healths;

        private void Awake()
        {
            healths = new List<Health>();
        }

        private void OnTriggerEnter(Collider other)
        {
            Health health = other.gameObject.GetComponent<Health>();
            
            if (health == null) return;
            
            healths.Add(health);
        }

        private void OnTriggerExit(Collider other)
        {
            Health health = other.gameObject.GetComponent<Health>();
            
            if (health == null) return;

            if (healths.Contains(health))
            {
                healths.Remove(health);
            }
        }

        public void Damage(float damage)
        {
            List<int> nullReferenceIndex = new List<int>();
            if (nullReferenceIndex == null) throw new ArgumentNullException(nameof(nullReferenceIndex));

            int i = 0;
            
            foreach (var health in healths)
            {
                try
                {
                    health.Damage(damage, gameObject);
                }
                catch (NullReferenceException)
                {
                    nullReferenceIndex.Add(i);
                }
                
                i++;
            }
            
            RemoveNullReference(nullReferenceIndex);
        }

        private void RemoveNullReference(List<int> nullReferenceIndex)
        {
            foreach (var index in nullReferenceIndex)
            {
                healths.RemoveAt(index);
            }
        }
    }
}