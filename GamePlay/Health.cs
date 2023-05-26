using System;
using CagrsLib.LibCore;
using UnityEngine;
using UnityEngine.Events;

namespace CagrsLib.GamePlay
{
    [AddComponentMenu("CagrsLib/Gameplay/Health")]
    public class Health : MonoBehaviour
    {
        public UnityEvent<GameObject> onDamageSignal;
        public UnityEvent<UpdateInfo> onHealthUpdate;
        
        [HideInInspector]
        public float health = 100;
        
        [HideInInspector]
        public bool inRange = true;

        [HideInInspector]
        public InRangeSetting inRangeSetting;

        public void Add(float add)
        {
            float old = health;
            
            Set(health + add);

            onHealthUpdate.Invoke(GetUpdateInfo(old, health));
        }
        
        public void Remove(float remove)
        {
            float old = health;
            
            Set(health - remove);
            
            onHealthUpdate.Invoke(GetUpdateInfo(old, health));
        }

        public void Set(float set)
        {
            float old = health;
            
            if (inRange)
            {
                health = LibUtil.InRange(set, inRangeSetting.min, inRangeSetting.max);
                return;
            }

            health = set;
            
            onHealthUpdate.Invoke(GetUpdateInfo(old, health));
        }

        public void Damage(float damage, GameObject attacker = null)
        {
            Remove(damage);
            
            onDamageSignal.Invoke(attacker);
        }

        private UpdateInfo GetUpdateInfo(float oldHealth, float newHealth)
        {
            if (oldHealth > newHealth)
            {
                return UpdateInfo.Remove;
            }
            
            if (oldHealth < newHealth)
            {
                return UpdateInfo.Add;
            }
            
            return UpdateInfo.NoChange;
        }

        [Serializable]
        public class InRangeSetting
        {
            public float min, max = 100;
        }

        [Serializable]
        public enum UpdateInfo
        {
            Remove, Add, NoChange
        }
    }
}