using System.Collections.Generic;

namespace CagrsLib.LibCore.Console
{
    public class CommandLine
    {
        private bool _isRoot;

        private CommandLine _parent;

        private string _path;
        
        private string _commandName;

        private Dictionary<string, CommandParameter> _parameters;

        #region Generate

        public static CommandLine Generate(string name)
        {
            CommandLine commandLine = new CommandLine();
            commandLine._commandName = name;
            commandLine._parameters = new Dictionary<string, CommandParameter>();

            return commandLine;
        }
        
        public static CommandLine GenerateAsRoot(string name)
        {
            CommandLine commandLine = Generate(name);

            commandLine._path = "root/" + name;

            return commandLine;
        }
        
        #endregion

        #region Add Parameter

        public CommandLine Paramater(string name, string defaultValue = "unknown")
        {
            _parameters.Add(name, new CommandObjectParameter(this, name, defaultValue));

            return this;
        }

        public CommandLine Branch(string branchName, CommandLine[] commandLines)
        {
            Dictionary<string, CommandLine> resultCommands = new Dictionary<string, CommandLine>();
            foreach (var commandLine in commandLines)
            {
                commandLine._path = _path + "/" + commandLine.GetName();
                resultCommands.Add(commandLine._commandName, commandLine.AddToBranch(this));
            }
            
            _parameters.Add(branchName, new CommandBranchParameter(this, branchName, resultCommands));

            return this;
        }
        
        #endregion

        #region CommandLine Operations

        private CommandLine AddToBranch(CommandLine parent)
        {
            _parent = parent;
            _isRoot = true;

            return this;
        }
        
        #endregion

        #region Value Inquire

        public bool IsRoot()
        {
            return _isRoot;
        }

        public int Length()
        {
            return _parameters.Count;
        }

        public string GetName()
        {
            return _commandName;
        }

        public Dictionary<string, CommandParameter> GetParameters()
        {
            return _parameters;
        }
        
        public List<CommandParameter> GetParameterList()
        {
            List<CommandParameter> parameters = new List<CommandParameter>();

            foreach (var value in _parameters.Values)
            {
                parameters.Add(value);
            }
            
            return parameters;
        }

        public string Path()
        {
            return _path;
        }

        public CommandObjectParameter FindObjectParameterByName(string name)
        {
            CommandParameter parameter;

            if (!_parameters.ContainsKey(name)) return null;

            parameter = _parameters[name];

            if (parameter == null) return null;

            if (parameter.GetParameterType() != CommandParameterType.ObjectParameter) return null;
            
            return (CommandObjectParameter) parameter;
        }
        
        public CommandBranchParameter FindBranchByName(string name)
        {
            CommandParameter parameter;

            if (!_parameters.ContainsKey(name)) return null;

            parameter = _parameters[name];

            if (parameter == null) return null;

            if (parameter.GetParameterType() != CommandParameterType.BranchParameter) return null;
            
            return (CommandBranchParameter) parameter;
        }
        
        #endregion
    }

    public abstract class CommandParameter
    {
        protected readonly string ParameterName;

        protected readonly CommandLine Parent;

        public abstract string Path();

        protected CommandParameter(CommandLine commandLine, string name)
        {
            Parent = commandLine;
            ParameterName = name;
        }

        public string GetParentPath()
        {
            if (Parent == null) return "";
            return Parent.Path();
        }

        public string GetName()
        {
            return ParameterName;
        }

        public abstract CommandParameterType GetParameterType();
    }
    
    public class CommandObjectParameter : CommandParameter
    {
        public string ParamDefault;

        public CommandObjectParameter(CommandLine commandLine, string name, string defaultValue) : base(commandLine, name)
        {
            ParamDefault = defaultValue;
        }

        public override string Path()
        {
            return GetParentPath() + "/" + ParameterName;
        }

        public override CommandParameterType GetParameterType()
        {
            return CommandParameterType.ObjectParameter;
        }
    }

    public class CommandBranchParameter : CommandParameter
    {
        public Dictionary<string, CommandLine> CommandLines;

        public CommandBranchParameter(CommandLine commandLine, string name, Dictionary<string, CommandLine> commandLines) : base(commandLine, name)
        {
            CommandLines = commandLines;
        }

        public override string Path()
        {
            return GetParentPath() + "/" + ParameterName;
        }

        public override CommandParameterType GetParameterType()
        {
            return CommandParameterType.BranchParameter;
        }

        public CommandLine FindCommandLineByName(string name)
        {
            if (!CommandLines.ContainsKey(name)) return null;
            
            return CommandLines[name];
        }
    }

    public enum CommandParameterType
    {
        ObjectParameter, BranchParameter
    }
}