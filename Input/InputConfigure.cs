using System.Collections.Generic;
using UnityEngine;

namespace CagrsLib.Input
{
    [CreateAssetMenu(fileName = "InputConfigure", menuName = "CagrsLib/Objects/InputConfigure", order = 1)]
    public class InputConfigure : ScriptableObject
    {
        public string configureName = "Untitled";
        
        [HideInInspector]
        public List<InputOperation> operations;
        public List<InputTrigger> triggers;
        public List<InputAxis> axes;
        
        public Dictionary<string, InputOperation> RemapOperations = new();

        public Dictionary<string, InputOperation> OperationsDictionary;

        public Dictionary<string, InputOperation> GetRemapOperations()
        {
            return RemapOperations;
        }
        
        public void SetRemapOperations(Dictionary<string, InputOperation> remap)
        {
            RemapOperations = remap;
        }
    }
}