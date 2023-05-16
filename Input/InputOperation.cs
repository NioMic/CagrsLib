using System;
using UnityEngine;

namespace CagrsLib.Input
{
    [Serializable]
    public class InputOperation
    {
        public string name;
        public KeyCode[] bindKeyCodes;
    }
}