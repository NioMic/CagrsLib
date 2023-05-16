using System.Collections.Generic;
using CagrsLib.Archive;
using CagrsLib.Input;
using UnityEngine;

namespace CagrsLib.Links.KeyRemap
{
    [RequireComponent(typeof(InputManager))]
    public class KeyRemapData : MonoBehaviour
    {
        private InputManager _manager;

        public ArchiveSetting setting;

        private void OnEnable()
        {
            _manager = InputManager.GetInstance();
        }

        [ArchiveData.Write]
        private ArchivePack Write()
        {
            ArchivePack pack = ArchivePack.CreatePack("remap_info");

            if (setting.write) pack.Write("info", _manager.configure.GetRemapOperations());
            
            return pack;
        }

        [ArchiveData.Load("remap_info")]
        private void Load(ArchivePack archivePack)
        {
            if (setting.load)
            {
                Dictionary<string, InputOperation> info = (Dictionary<string, InputOperation>)
                    archivePack.Load("info", new Dictionary<string, InputOperation>());

                _manager.configure.RemapOperations = info;
            }
        }
    }
}
