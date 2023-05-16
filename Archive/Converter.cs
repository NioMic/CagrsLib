using System;
using UnityEngine;

namespace CagrsLib.Archive
{
    public class Converter
    {
        public static Vector2Data Convert(Vector2 vector2)
        {
            return new Vector2Data
            {
                x = vector2.x,
                y = vector2.y
            };
        }
        
        public static Vector2 Convert(Vector2Data vector2)
        {
            return new Vector2
            {
                x = vector2.x,
                y = vector2.y
            };
        }
        
        public static Vector3Data Convert(Vector3 vector3)
        {
            return new Vector3Data
            {
                x = vector3.x,
                y = vector3.y,
                z = vector3.z
            };
        }
        
        public static Vector3 Convert(Vector3Data vector3)
        {
            return new Vector3
            {
                x = vector3.x,
                y = vector3.y,
                z = vector3.z
            };
        }
        
        public static QuaternionData Convert(Quaternion quaternion)
        {
            return new QuaternionData
            {
                x = quaternion.x,
                y = quaternion.y,
                z = quaternion.z,
                w = quaternion.w
            };
        }
        
        public static Quaternion Convert(QuaternionData quaternion)
        {
            return new Quaternion
            {
                x = quaternion.x,
                y = quaternion.y,
                z = quaternion.z,
                w = quaternion.w
            };
        }
    }

    [Serializable]
    public class QuaternionData
    {
        public float x, y, z, w;
    }

    [Serializable]
    public class Vector3Data
    {
        public float x, y, z;
    }

    [Serializable]
    public class Vector2Data
    {
        public float x, y;
    }
}