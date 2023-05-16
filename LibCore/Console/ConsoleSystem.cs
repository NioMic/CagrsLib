using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

namespace CagrsLib.LibCore.Console
{
    public class ConsoleSystem : MonoBehaviour
    {
        [Header("Console Configure")]
        public InputField inputField;

        [Tooltip("Placeholder")]
        public Text hintText;

        public KeyCode callConsole;
        public KeyCode enterCommand;
        public KeyCode autoCompletion;
        public KeyCode lastHint;
        public KeyCode nextHint;

        private int _historyPosition = -1;
        
        private List<string> _history;
        
        private List<string> _insertHint;

        private Dictionary<string, HintInfo> _hintInfos;

        private CommandCompiler.CommandRealtimeInfo _realtimeInfo;
        
        public static CommandCompiler GetCompilerInstance()
        {
            return FindObjectOfType<ConsoleSystem>()._commandCompiler;
        }

        private CommandCompiler _commandCompiler;

        private void Awake()
        {
            _commandCompiler = new CommandCompiler();

            _hintInfos = new Dictionary<string, HintInfo>();

            _insertHint = new List<string>();

            _history = new List<string>();
            
            AutoRegister();
        }

        private void Update()
        {
            try
            {
                Control();
            }
            catch (ArgumentOutOfRangeException e)
            {
            }
            catch (ArgumentNullException e)
            {
            }
        }

        public void UpdatePlaceholder()
        {
            try
            {
                RealtimeHint();
            }
            catch (IndexOutOfRangeException e)
            {
                hintText.text = "";
            }
            catch (NullReferenceException e)
            {
                hintText.text = "";
            }
            catch (KeyNotFoundException e)
            {
                hintText.text = "";
            }
            catch (ArgumentNullException e)
            {
                hintText.text = "";
            }
        }

        private void Control()
        {
            if (Input.GetKeyDown(autoCompletion))
            {
                string insert = _insertHint[^1];
                Regex regex = new Regex(_realtimeInfo.Input);
                insert = regex.Replace(insert,"",1);

                string setText = inputField.text.Insert(inputField.selectionFocusPosition, insert);

                inputField.text = setText;
                inputField.selectionAnchorPosition += insert.Length;
                inputField.selectionFocusPosition += insert.Length;
            }

            if (Input.GetKeyDown(enterCommand))
            {
                InvokeCommand(inputField.text);
                
                _history.Add(inputField.text);

                _historyPosition = -1;
                
                inputField.text = "";
                
                inputField.ActivateInputField();
            }

            if (inputField.text.Trim().Length < 1 || _historyPosition != -1)
            {
                if (Input.GetKeyDown(nextHint))
                {
                    if (_historyPosition == -1)
                    {
                        _historyPosition = _history.Count - 1;
                    }
                    else
                    {
                        _historyPosition = LibUtil.InRange(_historyPosition + 1, 0, _history.Count - 1);
                    }
                    inputField.text = _history[_historyPosition];
                }
                
                if (Input.GetKeyDown(lastHint))
                {
                    if (_historyPosition == -1)
                    {
                        _historyPosition = _history.Count - 1;
                    }
                    else
                    {
                        _historyPosition = LibUtil.InRange(_historyPosition - 1, 0, _history.Count - 1);
                    }
                    inputField.text = _history[_historyPosition];
                }
                
                inputField.selectionAnchorPosition = inputField.text.Length;
                inputField.selectionFocusPosition = inputField.text.Length;
            }
        }

        private void RealtimeHint()
        {
            int selectPos = inputField.selectionFocusPosition;
            
            _realtimeInfo = _commandCompiler.RealtimeCompile(inputField.text, selectPos);
            
            StringBuilder builder = new StringBuilder();

            CommandLine currentCommandLine = _realtimeInfo.CurrentCommandLine;
            CommandLine rootCommandLine = _realtimeInfo.RootCommandLine;
            CommandParameter currentParameter = _realtimeInfo.CurrentParameter;

            string currentInput = _realtimeInfo.Input;

            string[] hints = _hintInfos[rootCommandLine.GetName()]
                .GetHint(currentParameter.GetName());

            _insertHint.Clear();
            
            if (currentParameter.GetParameterType() == CommandParameterType.ObjectParameter)
            {
                foreach (var hint in hints)
                {
                    if (hint.Trim().StartsWith(currentInput))
                    {
                        builder.AppendLine(hint);
                        _insertHint.Add(hint);
                    }
                }
            }

            if (currentParameter.GetParameterType() == CommandParameterType.BranchParameter)
            {
                CommandBranchParameter branchParameter = (CommandBranchParameter)currentParameter;
                foreach (var commandLine in branchParameter.CommandLines.Values)
                {
                    string commandLineName = commandLine.GetName();
                    if (commandLineName.Trim().StartsWith(currentInput))
                    {
                        builder.AppendLine(commandLineName);
                        _insertHint.Add(commandLineName);
                    }
                }
            }
            
            builder.AppendLine("<" + currentParameter.GetName() + "> : " + currentParameter.Path());
            hintText.text = builder.ToString();
        }

        private void AutoRegister()
        {
            FindRigisteredCommandLinesAndHintInfo(out List<CommandLine> commandLines, out List<HintInfo> hintInfos);

            foreach (var commandLine in commandLines)
            {
                _commandCompiler.Register(commandLine);
                LibUtil.Log(this, $"Registered command \"{commandLine.GetName()}\" ( Total Parameter : {commandLine.Length()} )");
            }

            foreach (var hintInfo in hintInfos)
            {
                string commandLineName = hintInfo.GetName();
                LibUtil.Log(this, $"Registered hint for \"{commandLineName}\"");
                
                _hintInfos.Add(commandLineName, hintInfo);
            }
        }

        public void FindRigisteredCommandLinesAndHintInfo(out List<CommandLine> commandLines, out List<HintInfo> hintInfos)
        {
            commandLines = new List<CommandLine>();
            hintInfos = new List<HintInfo>();
            foreach (GameObject gameObject in FindObjectsOfType(typeof(GameObject)))
            {
                foreach (Component component in gameObject.GetComponents<Component>())
                {
                    MethodInfo[] methods = component.GetType().GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    foreach (MethodInfo method in methods)
                    {
                        if (method.GetCustomAttributes(typeof(CommandRegister), true).Length > 0)
                        {
                            if (method.ReturnType == typeof(CommandLine))
                            {
                                CommandLine result = (CommandLine)method.Invoke(component, null);
                                commandLines.Add(result);
                            }
                        }

                        object[] attributes = method.GetCustomAttributes(typeof(HintRegister), true);
                        if (attributes.Length > 0)
                        {
                            HintRegister[] hintRegisters = (HintRegister[])attributes;

                            foreach (var hintRegister in hintRegisters)
                            {
                                ParameterInfo[] parameters = method.GetParameters();
                                if (parameters.Length == 1 && parameters[0].ParameterType == typeof(string))
                                {
                                    if (method.ReturnType == typeof(string[]))
                                    {
                                        HintInfo hintInfo = new HintInfo(hintRegister.rootCommandName, component, method);
                                        hintInfos.Add(hintInfo);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        
        public void InvokeCommand(string command)
        {
            foreach (GameObject gameObject in FindObjectsOfType(typeof(GameObject)))
            {
                foreach (Component comp in gameObject.GetComponents<Component>())
                {
                    MethodInfo[] methods = comp.GetType().GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    foreach (MethodInfo method in methods)
                    {
                        if (method.GetCustomAttributes(typeof(CommandInvoke), true).Length > 0)
                        {
                            ParameterInfo[] parameters = method.GetParameters();
                            if (parameters.Length == 1 && parameters[0].ParameterType == typeof(Dictionary<string, string>))
                            {
                                Dictionary<string, string> commandDict = _commandCompiler.Compile(command);
                                method.Invoke(comp, new object[] { commandDict });
                            }
                        }
                    }
                }
            }
        }
        
        public class HintInfo
        {
            private readonly Component _component;
            private readonly MethodInfo _methodInfo;
            private readonly string _hintName;

            public HintInfo(string name, Component component, MethodInfo methodInfo)
            {
                _component = component;
                _methodInfo = methodInfo;

                _hintName = name;
            }

            public string GetName()
            {
                return _hintName;
            }

            public Component GetComponent()
            {
                return _component;
            }

            public MethodInfo GetMethodInfo()
            {
                return _methodInfo;
            }

            public string[] GetHint(string parameterName)
            {
                if (_component != null)
                {
                    return (string[])_methodInfo.Invoke(_component, new object[]{parameterName});
                }

                return null;
            }
        }
    }
}
