using System;
using CagrsLib.LibCore;
using UnityEngine;
using UnityEngine.Events;

namespace CagrsLib.GamePlay
{
    [AddComponentMenu("CagrsLib/Gameplay/Health")]
    public class Health : MonoBehaviour
    {
        public UnityEvent onDamageSignal;
        public UnityEvent onHealthUpdate;
        
        [HideInInspector]
        public float health;
        
        [HideInInspector]
        public bool range = true;

        [HideInInspector]
        public HealthRange healthRange;

        public void Add(float add)
        {
            Set(health + add);

            onHealthUpdate.Invoke();
        }
        
        public void Remove(float remove)
        {
            Set(health - remove);
            
            onHealthUpdate.Invoke();
        }

        public void Set(float set)
        {
            if (range)
            {
                health = LibUtil.InRange(set, healthRange.min, healthRange.max);
                return;
            }

            health = set;
            onHealthUpdate.Invoke();
        }

        public void Damage(float damage, GameObject attacker = null)
        {
            onDamageSignal.Invoke();
            Remove(damage);
        }
        
        [Serializable]
        public class HealthRange
        {
            public float min, max = 100;
        }
    }
}