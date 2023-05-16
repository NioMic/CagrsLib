using System;
using System.Collections.Generic;
using CagrsLib.LibCore;
using UnityEngine;

namespace CagrsLib.Input
{
    [DisallowMultipleComponent]
    [AddComponentMenu("CagrsLib/Input/InputManager")]
    public class InputManager : MonoBehaviour
    {
        public static InputManager GetInstance()
        {
            return FindObjectOfType<InputManager>();
        }

        public InputConfigure configure;

        private Dictionary<string, OperationState> _operationState;
        private Dictionary<string, bool> _operationActive;
        private Dictionary<string, bool> _triggerState;
        private Dictionary<string, InputAxisInstance> _inputAxisInstances;

        private List<InputTriggerInstance> _inputTriggerInstances;

        public List<InputTriggerInstance> GetTriggers()
        {
            return _inputTriggerInstances;
        }

        #region Initialize
        public void Awake()
        {
            if (configure == null)
            {
                LibUtil.LogWarning(this, "Cannot found configure");
            }

            InitializeFromConfigure();
        }

        private void InitializeFromConfigure()
        {
            _operationState = new Dictionary<string, OperationState>();
            _operationActive = new Dictionary<string, bool>();
            _triggerState = new Dictionary<string, bool>();
            
            _inputTriggerInstances = new List<InputTriggerInstance>();
            _inputAxisInstances = new Dictionary<string, InputAxisInstance>();

            configure.OperationsDictionary = new Dictionary<string, InputOperation>();

            foreach (var operation in configure.operations)
            {
                _operationState.Add(operation.name, OperationState.None);
                _operationActive.Add(operation.name, false);
                configure.OperationsDictionary.Add(operation.name, operation);
            }

            foreach (var trigger in configure.triggers)
            {
                _triggerState.Add(trigger.triggerName, false);
                
                _inputTriggerInstances.Add(new InputTriggerInstance(trigger, this));
            }

            foreach (var axis in configure.axes)
            {
                _inputAxisInstances.Add(axis.axisName, new InputAxisInstance(axis, this));
            }
        }
        #endregion

        #region Update

        private void Update()
        {
            UpdateOperationState();
            
            UpdateTriggerState(Time.deltaTime);
            
            UpdateAxesDirection();
        }

        private void FixedUpdate()
        {
            FixedUpdateAxesDirection();
        }

        private void UpdateAxesDirection()
        {
            foreach (var inputAxisInstance in _inputAxisInstances.Values)
            {
                if (!inputAxisInstance.GetInputAxis().isFixedUpdate)
                {
                    inputAxisInstance.OnUpdate();
                }
            }
        }

        private void FixedUpdateAxesDirection()
        {
            foreach (var inputAxisInstance in _inputAxisInstances.Values)
            {
                if (inputAxisInstance.GetInputAxis().isFixedUpdate)
                {
                    inputAxisInstance.OnUpdate();
                }
            }
        }

        private void UpdateOperationState()
        {
            foreach (var operation in configure.OperationsDictionary.Values)
            {
                OperationState state = GetKeysState(operation.bindKeyCodes);
                _operationState[operation.name] = state;
                if (state != OperationState.None)
                {
                    _operationActive[operation.name] = true;
                }
                else
                {
                    _operationActive[operation.name] = false;
                }
            }
        }

        private void UpdateTriggerState(float deltaTime)
        {
            foreach (var inputTrigger in _inputTriggerInstances)
            {
                inputTrigger.UpdateTrigger(deltaTime);
            }
        }

        #endregion

        #region Get Status

        private OperationState GetKeysState(KeyCode[] keys)
        {
            foreach (var key in keys)
            {
                if (UnityEngine.Input.GetKeyDown(key))
                {
                    return OperationState.Down;
                }

                if (UnityEngine.Input.GetKeyUp(key))
                {
                    return OperationState.Up;
                }

                if (UnityEngine.Input.GetKey(key))
                {
                    return OperationState.Press;
                }
            }
            
            return OperationState.None;
        }
        
        private OperationState GetOperationState(InputOperation operation)
        {
            return GetKeysState(operation.bindKeyCodes);
        }

        public OperationState GetOperationState(string operationName)
        {
            Dictionary<string, InputOperation> remapOperations = configure.GetRemapOperations();
            if (remapOperations.ContainsKey(operationName))
            {
                return GetKeysState(remapOperations[operationName].bindKeyCodes);
            }

            if (configure.OperationsDictionary.ContainsKey(operationName))
            {
                return GetKeysState(configure.OperationsDictionary[operationName].bindKeyCodes);
            }

            LibUtil.LogWarning(this, "Can't Found Operation : " + operationName);
            return OperationState.None;
        }

        private List<string> GetActiveOperations()
        {
            List<string> list = new List<string>();

            foreach (var active in _operationActive)
            {
                if (active.Value)
                {
                    list.Add(active.Key);
                }
            }

            return list;
        }

        public bool GetTrigger(string triggerName)
        {
            try
            {
                return _triggerState[triggerName];
            }
            catch (KeyNotFoundException)
            {
                LibUtil.Error(this, "Can't found trigger : " + triggerName);
                return false;
            }
        }

        public float GetAxis(string axisName)
        {
            try
            {
                return _inputAxisInstances[axisName].GetAxis();
            }
            catch (KeyNotFoundException)
            {
                LibUtil.Error(this, "Can't found axis : " + axisName);
                return 0;
            }
        }

        #endregion

        // TODO: 这里的代码可能会存在优先级问题，需要评估
        #region Eval

        public void EvalOperation(string operationName, OperationState state)
        {
            _operationState[operationName] = state;

            if (state != OperationState.None)
            {
                _operationActive[operationName] = true;
            }
            else
            {
                _operationActive[operationName] = false;
            }
        }

        public void EvalTrigger(string triggerName)
        {
            _triggerState[triggerName] = true;
        }

        #endregion

        #region Operation Remap

        public void AddRemapOperation(InputOperation operation)
        {
            Dictionary<string, InputOperation> remapOperations = configure.GetRemapOperations();
            remapOperations.Add(operation.name, operation);
        }

        public void AddRemapOperation(string remapName, KeyCode[] keyCodes)
        {
            Dictionary<string, InputOperation> remapOperations = configure.GetRemapOperations();

            InputOperation remapOperation = new InputOperation
            {
                name = remapName,
                bindKeyCodes = keyCodes
            };
            
            if (remapOperations.ContainsKey(remapName))
            {
                remapOperations[remapName] = remapOperation;
                return;
            }

            remapOperations.Add(remapName, remapOperation);
        }

        public void ClearRemapOperation(string operationName)
        {
            Dictionary<string, InputOperation> remapOperations = configure.GetRemapOperations();
            remapOperations.Remove(operationName);
        }

        #endregion

        public class InputTriggerInstance
        {
            private readonly Clock _clock;

            private int _step;
            
            private int _stepCount;
            
            private InputTrigger _inputTrigger;

            private InputManager _manager;

            public int GetStep()
            {
                return _step;
            }

            public int GetStepCount()
            {
                return _stepCount;
            }

            public string GetName()
            {
                return _inputTrigger.triggerName;
            }

            public InputTriggerInstance(InputTrigger inputTrigger, InputManager manager)
            {
                _clock = new Clock();
                
                _inputTrigger = inputTrigger;
                
                _stepCount = inputTrigger.steps.Count;

                _manager = manager;
                
                _inputTrigger.SetupInputTrigger();
            }

            public void UpdateTrigger(float deltaTime)
            {
                _manager._triggerState[_inputTrigger.triggerName] = false;
                
                _clock.Update(deltaTime);

                InputTrigger.Step currentStep = _inputTrigger.steps[_step];

                if (_inputTrigger.endWhenInputWrongOperation &&
                    _inputTrigger.IsInputWrongOperation(currentStep.operationName, _manager.GetActiveOperations()))
                {
                    ExecuteStep(InputTrigger.Execute.End);
                }
                else
                {
                    if ((_manager.GetOperationState(currentStep.operationName) == OperationState.Down &&
                         currentStep.activeType == InputTrigger.ActiveType.Down)
                        ||
                        (_manager.GetOperationState(currentStep.operationName) == OperationState.Up &&
                         currentStep.activeType == InputTrigger.ActiveType.Up))
                    {
                        if (_step > 0)
                        {
                            if (_clock.IsOutTime() == false)
                            {
                                ExecuteStep(currentStep.inTimePressed);
                            }
                            else
                            {
                                ExecuteStep(currentStep.outTimePressed);
                            }
                        }
                        else
                        {
                            _step++;
                        }

                        if (currentStep.waitSeconds > 0)
                        {
                            _clock.StartWithTimer(currentStep.waitSeconds, Clock.TimerMode.Keep);
                        }
                    }
                }
            }

            private void ExecuteStep(InputTrigger.Execute execute)
            {
                switch (execute)
                {
                    case InputTrigger.Execute.None:
                        return;
                    
                    case InputTrigger.Execute.Next:
                        _step++;
                        _clock.Reset();
                        break;
                    case InputTrigger.Execute.End:
                        _step = 0;
                        _clock.Stop();
                        _clock.Reset();
                        break;
                    case InputTrigger.Execute.Trigger:
                        _step++;
                        _manager._triggerState[_inputTrigger.triggerName] = true;
                        _clock.Reset();
                        break;
                    case InputTrigger.Execute.Return:
                        _manager._triggerState[_inputTrigger.triggerName] = true;
                        _clock.Stop();
                        _clock.Reset();
                        _step = 0;
                        break;
                }
            }
        }

        public class InputAxisInstance
        {
            private InputAxis _inputAxis;

            private InputManager _manager;

            private float _direction;

            public InputAxisInstance(InputAxis axis, InputManager manager)
            {
                _manager = manager;

                _inputAxis = axis;

                _direction = 0;
            }

            public InputAxis GetInputAxis()
            {
                return _inputAxis;
            }

            public void OnUpdate()
            {
                var isNegative = _manager.GetOperationState(_inputAxis.negativeOperationName) != OperationState.None;
                var isPositive = _manager.GetOperationState(_inputAxis.positiveOperationName) != OperationState.None;

                if (isPositive)
                {
                    _direction = LibUtil.InRange(_direction + _inputAxis.sensitivity, -1, 1);
                }

                if (isNegative)
                {
                    _direction = LibUtil.InRange(_direction - _inputAxis.sensitivity, -1, 1);
                }
            
                if (!isNegative && !isPositive)
                {
                    _direction = LibUtil.StrictTo(_direction, 0, _inputAxis.gravity);
                }

                if (_inputAxis.snap)
                {
                    if (_direction > 0 && isNegative)
                    {
                        _direction = 0;
                    }

                    if (_direction < 0 && isPositive)
                    {
                        _direction = 0;
                    }
                }
            }
            
            public float GetAxis()
            {
                if (Math.Abs(_direction) <= _inputAxis.dead)
                {
                    return 0;
                }

                if (_inputAxis.invert)
                {
                    return -_direction;
                }

                return _direction;
            }
        }

        public enum OperationState
        {
            Down = -1,
            Up = 1,
            Press = 2,
            None = 0
        }
    }
}