using UnityEngine;

namespace CagrsLib.Input
{
    [CreateAssetMenu(fileName = "InputAxis", menuName = "CagrsLib/Objects/InputAxis", order = 1)]
    public class InputAxis : ScriptableObject
    {
        public string axisName;

        public string positiveOperationName;
        public string negativeOperationName;

        public bool isFixedUpdate = true;

        [Range(0, 1)] public float gravity = 0.5f;

        [Range(0, 1)] public float dead = 0.1f;

        [Range(0, 1)] public float sensitivity = 0.5f;

        public bool snap = true;

        public bool invert;
    }
}