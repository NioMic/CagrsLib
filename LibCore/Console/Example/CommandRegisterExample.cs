using System.Collections.Generic;
using UnityEngine;

namespace CagrsLib.LibCore.Console.Example
{
    public class CommandRegisterExample : MonoBehaviour
    {
        [CommandRegister]
        private CommandLine CommandRegister()
        {
            CommandLine myCommandLine = CommandLine.GenerateAsRoot("my_command")
                .Paramater("name")
                .Paramater("age")
                .Branch("gender_select", new[]
                {
                    CommandLine.Generate("male"),
                    CommandLine.Generate("female"),
                    CommandLine.Generate("other")
                        .Paramater("other_type")
                })
                .Paramater("want");
        
            return myCommandLine;
        }

        [HintRegister("my_command")]
        private string[] CommandHint(string parameterName)
        {
            switch (parameterName)
            {
                case "name":
                    return new[] { "ZhangSan", "LiSi", "WangWu" };
                case "age":
                    return new[] { "16", "18", "24" };
                case "want":
                    return new[] { "money", "life", "time", "YOU" };
                case "other_type":
                    return new[] { "robot", "ai", "rabbit", "fox", "other_animal" };
                    
            }

            return new[] { "" };
        }

        [CommandInvoke]
        private void CommandInvoke(Dictionary<string,string> input)
        {
        
        }
    }
}