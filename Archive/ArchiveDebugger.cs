using System;
using UnityEngine;

namespace CagrsLib.Archive
{
    [DisallowMultipleComponent]
    [AddComponentMenu("CagrsLib/Archive/ArchiveDebugger")]
    [RequireComponent(typeof(ArchiveManager))]
    public class ArchiveDebugger : MonoBehaviour
    {
        [HideInInspector] 
        public ArchiveManager manager;

        public bool autoLoad;

        public ArchiveAutoLoadSetting setting = ArchiveAutoLoadSetting.Awake;

        private bool isLoaded;
        
        private void Awake()
        {
            manager = GetComponent<ArchiveManager>();
            if (autoLoad && setting == ArchiveAutoLoadSetting.Awake && !isLoaded)
            {
                manager.LoadAll();

                isLoaded = true;
            }
        }

        private void OnEnable()
        {
            if (autoLoad && setting == ArchiveAutoLoadSetting.Enable && !isLoaded)
            {
                manager.LoadAll();

                isLoaded = true;
            }
        }

        private void Start()
        {
            if (autoLoad && setting == ArchiveAutoLoadSetting.Start && !isLoaded)
            {
                manager.LoadAll();

                isLoaded = true;
            }
        }
        
        private void Update()
        {
            if (autoLoad && setting == ArchiveAutoLoadSetting.FirstUpdate && !isLoaded)
            {
                manager.LoadAll();

                isLoaded = true;
            }
        }
        
        private void FixedUpdate()
        {
            if (autoLoad && setting == ArchiveAutoLoadSetting.FirstFixedUpdate && !isLoaded)
            {
                manager.LoadAll();

                isLoaded = true;
            }
        }
        
        private void LateUpdate()
        {
            if (autoLoad && setting == ArchiveAutoLoadSetting.FirstLateUpdate && !isLoaded)
            {
                manager.LoadAll();

                isLoaded = true;
            }
        }

        [Serializable]
        public enum ArchiveAutoLoadSetting
        {
            Awake, Enable, Start, FirstUpdate, FirstFixedUpdate, FirstLateUpdate
        }
    }
}
