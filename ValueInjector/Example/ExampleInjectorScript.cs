using System;
using UnityEngine;

namespace CagrsLib.ValueInjector.Example
{
    
    public class ExampleInjectorScript : SimpleInjectorScript
    {
        // The following example shows how to add 20 to the value named “speed” using the InjectorScript class.
        public override object OnInject(string name, object obj)
        {
            if (name == "speed")
            {
                float speed = Convert.ToSingle(obj);

                return speed + 20;
            }

            // In other MonoBehaviours of this GameObject, the following content needs to be called : 
            //   float playerSpeed = _injectable.GetInjectedValue("speed", speed);
            
            return null;
        }

        // The following example shows how to output “Jump!” when the character jumps.
        public override void OnInvoke(string invokeName, object[] parameters)
        {
            if (invokeName == "jump")
            {
                Debug.Log("Jump !");
            }
            
            // In the code for the character jump in this GameObject, the following content needs to be called:
            //   _injectable.InvokeScript("jump");
        }
    }
}