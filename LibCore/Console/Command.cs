using System.Collections.Generic;

namespace CagrsLib.LibCore.Console
{
    public interface Command
    {
        [CommandRegister]
        CommandLine CommandRegister();

        [HintRegister("command_name")]
        string[] CommandHint(string parameterName);

        [CommandInvoke]
        void CommandInvoke(Dictionary<string, string> input);
    }
}