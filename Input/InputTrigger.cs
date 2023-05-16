using System;
using System.Collections.Generic;
using System.Text;
using CagrsLib.LibCore;
using UnityEngine;
using UnityEngine.Serialization;

namespace CagrsLib.Input
{
    [CreateAssetMenu(fileName = "InputTrigger", menuName = "CagrsLib/Objects/InputTrigger", order = 1)]
    public class InputTrigger : ScriptableObject
    {
        [FormerlySerializedAs("name")] 
        public string triggerName;
        public bool endWhenInputWrongOperation;
        
        [HideInInspector]
        public List<Step> steps;
        
        [HideInInspector]
        public List<string> includeOperations;

        public void SetupInputTrigger()
        {
            includeOperations = new List<string>();
            
            foreach (var step in steps)
            {
                string operationName = step.operationName;
                
                if (! includeOperations.Contains(operationName)) 
                    includeOperations.Add(operationName);
            }
        }

        public bool IsInputWrongOperation(string should, List<string> actual)
        {
            foreach (var include in includeOperations)
            {
                if (actual.Contains(include) && include != should)
                {
                    return true;
                }
            }

            return false;
        }

        [Serializable]
        public class Step
        {
            public string operationName;
            public ActiveType activeType;
            public float waitSeconds;
            public Execute inTimePressed = Execute.None;
            public Execute outTimePressed = Execute.None;

            public static Step FirstStep(string operationName, float waitSeconds = 0)
            {
                return new Step
                {
                    operationName = operationName,
                    waitSeconds = waitSeconds
                };
            }
        }
        
        public void StringToSteps(string str)
        {
            string standardFormat = "OperationName Down/Up WaitSeconds InTime OutTime";

            steps = new List<Step>();
            
            int currentLine = 1;
            foreach (var line in str.Split('\n'))
            {
                string[] lineParams = line.Trim().Split(' ');

                if (lineParams.Length < 5) return;
                
                Step step = new Step();

                step.operationName = lineParams[0];
                
                if (lineParams[1].ToLower() == "down")
                {
                    step.activeType = ActiveType.Down;
                } 
                else if (lineParams[1].ToLower() == "up")
                {
                    step.activeType = ActiveType.Up;
                }
                else
                {
                    LibUtil.LogWarning(this, "Unknown active type at " + currentLine + " :\n\t" + standardFormat);
                    return;
                }

                if (float.TryParse(lineParams[2], out float waitSeconds))
                {
                    step.waitSeconds = waitSeconds;
                }
                else
                {
                    LibUtil.LogWarning(this, "Invalid argument at " + currentLine + " :\n\t" + standardFormat);
                    return;
                }

                Execute inTime = StringToExecute(lineParams[3]);
                Execute outTime = StringToExecute(lineParams[4]);

                if (inTime != Execute.Error && outTime != Execute.Error)
                {
                    step.inTimePressed = inTime;
                    step.outTimePressed = outTime;
                }
                else
                {
                    LibUtil.LogWarning(this, "Unknown Execute Type at " + currentLine + " :\n\t" + standardFormat);
                    return;
                }

                steps.Add(step);
                currentLine++;
            }
        }

        public string StepsToString()
        {
            StringBuilder stringBuilder = new();
            foreach (var step in steps)
            {
                stringBuilder.Append(
                    $"{step.operationName} {step.activeType.ToString()} {step.waitSeconds} {step.inTimePressed.ToString()} {step.outTimePressed.ToString()} \n"
                );
            }

            return stringBuilder.ToString().Trim();
        }

        private static Execute StringToExecute(string str)
        {
            switch (str.Trim().ToLower())
            {
                case "null":
                    return Execute.None;
                case "no":
                    return Execute.None;
                case "none":
                    return Execute.None;
                
                case "next":
                    return Execute.Next;
                case "go":
                    return Execute.Next;
                
                case "finish":
                    return Execute.End;
                case "end":
                    return Execute.End;
                
                case "trigger":
                    return Execute.Trigger;
                case "execute":
                    return Execute.Trigger;
                case "exe":
                    return Execute.Trigger;
                
                case "return":
                    return Execute.Return;
                case "done":
                    return Execute.Return;
                    
            }
            return Execute.Error;
        }

        [Serializable]
        public enum ActiveType
        {
            Down = -1,
            Up = 1
        }

        [Serializable]
        public enum Execute
        {
            None = 0,
            Next = 1,
            End = -1,
            Trigger = 2,
            Return = 3,
            Error
        }
    }
}