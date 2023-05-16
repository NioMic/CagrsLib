using System;
using UnityEngine;

namespace CagrsLib.Input
{
    public class CursorController : MonoBehaviour
    {
        public static CursorController GetInstance()
        {
            return FindObjectOfType<CursorController>();
        }

        [Header("Operation Bind")]
        public string callCursorOperation = "CallCursor";
        public string focusOperation = "Focus";
        public string switchOperation = "SwitchCursor";

        [Space]
        [Header("Lock Mode")]
        public CursorLockMode focusLockMode = CursorLockMode.Locked;
        public bool focusVisibility;
        
        public CursorLockMode outOfFocusLockMode = CursorLockMode.None;
        public bool outOfFocusVisibility = true;

        [Space]
        [Header("Info")]
        public bool focus;

        private InputManager _inputManager;

        private void Awake()
        {
            _inputManager = InputManager.GetInstance();
        }

        private void Update()
        {
            if (focus)
            {
                Cursor.lockState = focusLockMode;
                Cursor.visible = focusVisibility;
            }
            else
            {
                Cursor.lockState = outOfFocusLockMode;
                Cursor.visible = outOfFocusVisibility;
            }

            if (_inputManager.GetOperationState(callCursorOperation) == InputManager.OperationState.Down)
            {
                CallCursor();
            }
            
            if (_inputManager.GetOperationState(focusOperation) == InputManager.OperationState.Down)
            {
                Focus();
            }
            
            if (_inputManager.GetOperationState(switchOperation) == InputManager.OperationState.Down)
            {
                Switch();
            }
        }

        public void Focus()
        {
            focus = true;
        }

        public void CallCursor()
        {
            focus = false;
        }

        public void Switch()
        {
            focus = !focus;
        }
    }
}