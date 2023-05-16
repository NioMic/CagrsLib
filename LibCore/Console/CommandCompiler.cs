using System;
using System.Collections.Generic;

namespace CagrsLib.LibCore.Console
{
    public class CommandCompiler
    {
        private readonly Dictionary<string, CommandLine> _registeredCommandLines = new();

        public void Register(CommandLine commandLine)
        {
            _registeredCommandLines.Add(commandLine.GetName(), commandLine);
        }

        public void Remove(CommandLine commandLine)
        {
            _registeredCommandLines.Remove(commandLine.GetName());
        }

        public void Remove(string name)
        {
            _registeredCommandLines.Remove(name);
        }

        /*
         root branch : root/name
         root param : root/name/param
         sub branch : root/name/branch
         sub branch param : root/name/branch_selected/param
         */

        #region Command Compile

        public Dictionary<string, string> Compile(string cmdString)
        {
            string[] split = cmdString.Trim().Split(' ');

            Dictionary<string, string> result = new Dictionary<string, string>();
            if (result == null) throw new ArgumentNullException(nameof(result));

            CommandLine direct = DirectToCommandLine(split[0]);
            
            if (direct == null) return null;

            result.Add(direct.Path(), direct.GetName());

            if (! CompileBranch(direct, 1, result, split, out int index))
            {
                return null;
            }

            return result;
        }

        private bool CompileBranch(CommandLine commandLine, int index, Dictionary<string, string> result, string[] split, out int outIndex)
        {
            foreach (var parameter in commandLine.GetParameters().Values)
            {
                if (index > split.Length - 1)
                {
                    outIndex = 0;
                    return false;
                }

                if (parameter.GetParameterType() == CommandParameterType.ObjectParameter)
                {
                    result.Add(parameter.Path(), split[index]);
                    index ++;
                }

                if (parameter.GetParameterType() == CommandParameterType.BranchParameter)
                {
                    CommandBranchParameter branchParameter = (CommandBranchParameter) parameter;
                    
                    result.Add(parameter.Path(), split[index]);
                    
                    CommandLine subCommandLine = branchParameter.FindCommandLineByName(split[index]);

                    if (subCommandLine == null)
                    {
                        outIndex = 0;
                        return false;
                    }

                    if (!CompileBranch(subCommandLine, index + 1, result, split, out index))
                    {
                        outIndex = 0;
                        return false;
                    }
                }
            }

            outIndex = index;
            return true;
        }

        private CommandLine DirectToCommandLine(string name)
        {
            if (_registeredCommandLines.ContainsKey(name)) 
                return _registeredCommandLines[name];
            
            return null;
        }
        
        #endregion

        #region Realtime Compile

        // [a]| index = 1 select = 'a'
        // |[a] index = 0 select = null
        
        public CommandRealtimeInfo RealtimeCompile(string content, int selectedPos)
        {
            try
            {
                content = content.Trim() + " End";

                SplitBySelectedPos(content, selectedPos,
                    new CommandRealtimeInfo(),
                    out CommandRealtimeInfo realtimeInfo,
                    out string[] split,
                    out string currentSelect,
                    out string left,
                    out string right,
                    out string[] leftSplit,
                    out string[] rightSplit,
                    out int currentSelectSplitIndex);

                #endregion

                #region Get CommandLine

                string commandlineName = GetStringFromSplit(0, leftSplit, rightSplit);
                CommandLine commandLine = DirectToCommandLine(commandlineName);
                realtimeInfo.RootCommandLine = commandLine;

                if (commandLine == null) return realtimeInfo;

                #endregion

                #region Check CurrentParameter

                GetCurrentParameter(commandLine, leftSplit, 1, 0, out int outWrittenPivot,
                    out CommandLine currentCommandLine, out int inBranchPosition);

                CommandParameter currentParameter =
                    currentCommandLine.GetParameterList()
                        [inBranchPosition];

                realtimeInfo.CurrentCommandLine = currentCommandLine;

                realtimeInfo.CurrentParameter = currentParameter;

                #endregion

                #region Check IsBranchRoot

                if (currentParameter == null) return realtimeInfo;

                if (currentParameter.GetParameterType() == CommandParameterType.BranchParameter)
                {
                    realtimeInfo.IsBranchRoot = true;
                }

                #endregion

                #region Get Input
                
                if (leftSplit.Length > 0)
                {
                    realtimeInfo.FirstInput = leftSplit[0];
                    realtimeInfo.Input = leftSplit[^1];
                }

                #endregion

                #region Get Position

                realtimeInfo.ParameterAbsoluteIndex = leftSplit.Length;
                realtimeInfo.ParameterBranchIndex = inBranchPosition;

                #endregion
                
                return realtimeInfo;
            }
            catch (ArgumentOutOfRangeException)
            {
                return new CommandRealtimeInfo();
            }
        }

        private void GetCurrentParameter(CommandLine commandLine,
            string[] written,
            int writtenPivot, // 1
            int parameterPivot, // 0
            out int outWrittenPivot,
            out CommandLine currentCommandLine,
            out int inBranchPosition)
        {
            currentCommandLine = commandLine;
            inBranchPosition = parameterPivot;
            
            try
            {
                foreach (var parameter in commandLine.GetParameterList())
                {
                    if (writtenPivot < written.Length)
                    {
                        currentCommandLine = commandLine;
                        inBranchPosition = parameterPivot;

                        string selected = written[written.Length - 1];

                        string currentCheck = written[writtenPivot];

                        CommandParameterType parameterType = parameter.GetParameterType();
                        if (parameterType == CommandParameterType.ObjectParameter)
                        {
                            writtenPivot++;
                            parameterPivot++;
                        }

                        if (parameterType == CommandParameterType.BranchParameter)
                        {
                            CommandBranchParameter branchParameter = (CommandBranchParameter)parameter;
                            CommandLine childCommandLine = branchParameter.FindCommandLineByName(currentCheck);
                            if (childCommandLine != null)
                            {
                                GetCurrentParameter(childCommandLine, written, writtenPivot + 1, 
                                    0, out int outPivot, out currentCommandLine,
                                    out inBranchPosition);

                                writtenPivot = outPivot;
                                parameterPivot++;
                            }
                        }
                    }
                }
            }
            catch (IndexOutOfRangeException e)
            {
                currentCommandLine = commandLine;
                inBranchPosition = parameterPivot;
            }

            outWrittenPivot = writtenPivot;
        }

        private void SplitBySelectedPos(
            string content,
            int selectedPos,
            CommandRealtimeInfo realtimeInfo,
            out CommandRealtimeInfo outRealtimeInfo,
            out string[] split,
            out string currentSelect,
            out string left,
            out string right,
            out string[] leftSplit,
            out string[] rightSplit,
            out int currentSelectSplitIndex
            )
        {
            List<string> splitList = new List<string>();
            foreach (var @char in content)
            {
                splitList.Add(@char + "");
            }

            split = splitList.ToArray();

            currentSelect = split[selectedPos - 1];

            currentSelectSplitIndex = 0; // Include CommandLineName

            left = "";
            right = "";
            leftSplit = null;
            rightSplit = null;
            outRealtimeInfo = realtimeInfo;
            
            #region Length Check & Position Check

            if (selectedPos > content.Length) return;
            if (selectedPos == content.Length) realtimeInfo.InEnd = true;
            if (selectedPos == 0) realtimeInfo.InStart = true;

            if (currentSelect != " ")
            {
                realtimeInfo.InSpace = true;
            }
            else
            {
                realtimeInfo.InParam = true;
            }

            #endregion

            #region Split

            // AAA B|BB CCC -> L: AAA R: BBB CCC
            // AAA BBB| CCC -> L: AAA R: BBB CCC
            // AAA| BBB CCC -> L: --- R: AAA BBB CCC

            left = content.Substring(0, selectedPos);
            right = content.Substring(selectedPos);
            
            if (realtimeInfo.InSpace)
            {
                int spacePosition = right.IndexOf(' ');
                string rLeft = right.Substring(0, spacePosition).Trim();
                string rRight = right.Substring(spacePosition).Trim();

                left += rLeft;
                right = rRight;
            }

            leftSplit = left.Split();
            rightSplit = right.Split();

            currentSelectSplitIndex = leftSplit.Length + 1;

            outRealtimeInfo = realtimeInfo;
        }

        private static string GetStringFromSplit(int index, string[] left, string[] right)
        {
            int leftEndIndex = left.Length - 1;
            int rightStartIndex = index - leftEndIndex - 1;
            if (index <= leftEndIndex)
            {
                return left[index];
            }

            return right[rightStartIndex];
        }

        public Dictionary<string, CommandLine> GetRegisteredCommandLine()
        {
            return _registeredCommandLines;
        }

        public struct CommandRealtimeInfo
        {
            public CommandLine RootCommandLine;
            
            public CommandLine CurrentCommandLine;

            public CommandParameter CurrentParameter;

            public bool IsBranchRoot;

            public bool InSpace;

            public bool InEnd;

            public bool InStart;

            public bool InParam;

            public int ParameterAbsoluteIndex;
            
            public int ParameterBranchIndex;

            public string FirstInput;
            
            public string Input;

            public string[] Hints;//

            public string[] ShowHints;//
        }
        
        #endregion
    }
}