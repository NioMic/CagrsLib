using System;

namespace CagrsLib.LibCore.Console
{

    [AttributeUsage(AttributeTargets.Method)]
    public class CommandRegister : Attribute
    {
        
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class HintRegister : Attribute
    {
        public string rootCommandName;
        
        public HintRegister(string name)
        {
            rootCommandName = name;
        }
    }
    
    [AttributeUsage(AttributeTargets.Method)]
    public class CommandInvoke : Attribute
    {
        
    }
}