#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace CagrsLib.Input.Windows
{
    public class InputConfigureEditorWindow : EditorWindow
    {
        private const int ButtonHeight = 50;
        private const int ButtonSpacing = 0;

        private const int NameInputHeight = 45;

        #region KeyBoard

        private static readonly string[] Row0 = {"ESC", "F1", "F2", "F3", "F4", "F5", "F6", "F7", "F8", "F9", "F10", "F11", "F12"};
        private static readonly string[] Row1 = {"`", "1", "2", "3", "4", "5", "6", "7", "8", "9", "0", "-", "=", "Back"};
        private static readonly string[] Row2 = {"Tab", "Q", "W", "E", "R", "T", "Y", "U", "I", "O", "P", "[", "]", "\\"};
        private static readonly string[] Row3 = {"Caps", "A", "S", "D", "F", "G", "H", "J", "K", "L", ";", "'", "Enter"};
        private static readonly string[] Row4 = {"Shift", "Z", "X", "C", "V", "B", "N", "M", ",", ".", "/", "Shift"};
        private static readonly string[] Row5 = {"Ctrl","Fn","Alt","Space","Alt","Fn","Ctrl","←","↓","↑","→"};
        private static readonly string[] Row6 = {"Insert","Home","PgUp","K7","K8","K9","K-","KEnter"};
        private static readonly string[] Row7 = {"Delete","End","PgDown","K4","K5","K6","K+", "K*"};
        private static readonly string[] Row8 = {"←ML","MC","MR→","K1","K2","K3","K0", "K/"};
        private static readonly string[] Row9 = {"M3","M4","M5","M6"};
    
        // 一行788
        
        private static readonly float[] RowLength0 = {71, 60, 60, 60, 60, 60, 60, 60, 60, 60, 60, 60, 60};
        private static readonly float[] RowLength1 = {50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 138};
        private static readonly float[] RowLength2 = {120, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 68};
        private static readonly float[] RowLength3 = {130, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 111};
        private static readonly float[] RowLength4 = {140, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 154};
        private static readonly float[] RowLength5 = {126, 60, 60, 170, 60, 60, 60, 50, 50, 50, 50};
        private static readonly float[] RowLength6 = {60, 60, 60, 50, 50, 50, 50, 80, 120};
        private static readonly float[] RowLength7 = {60, 60, 60, 50, 50, 50, 50, 80, 120};
        private static readonly float[] RowLength8 = {60, 60, 60, 50, 50, 50, 50, 80, 120};
        private static readonly float[] RowLength9 = {60, 60, 60, 50};
    
        private static readonly KeyCode[] RowKeyCode0 = {KeyCode.Escape, KeyCode.F1, KeyCode.F2, KeyCode.F3, KeyCode.F4, KeyCode.F5, KeyCode.F6, KeyCode.F7, KeyCode.F8, KeyCode.F9, KeyCode.F10, KeyCode.F11, KeyCode.F12};
        private static readonly KeyCode[] RowKeyCode1 = {KeyCode.BackQuote, KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4, KeyCode.Alpha5, KeyCode.Alpha6, KeyCode.Alpha7, KeyCode.Alpha8, KeyCode.Alpha9, KeyCode.Alpha0, KeyCode.Minus, KeyCode.Equals, KeyCode.Backspace};
        private static readonly KeyCode[] RowKeyCode2 = {KeyCode.Tab, KeyCode.Q, KeyCode.W, KeyCode.E, KeyCode.R, KeyCode.T, KeyCode.Y, KeyCode.U, KeyCode.I, KeyCode.O, KeyCode.P, KeyCode.LeftBracket, KeyCode.RightBracket, KeyCode.Backslash};
        private static readonly KeyCode[] RowKeyCode3 = {KeyCode.CapsLock, KeyCode.A, KeyCode.S, KeyCode.D, KeyCode.F, KeyCode.G, KeyCode.H, KeyCode.J, KeyCode.K, KeyCode.L, KeyCode.Semicolon, KeyCode.Quote, KeyCode.KeypadEnter};
        private static readonly KeyCode[] RowKeyCode4 = {KeyCode.LeftShift, KeyCode.Z, KeyCode.X, KeyCode.C, KeyCode.V, KeyCode.B, KeyCode.N, KeyCode.M, KeyCode.Comma, KeyCode.Period, KeyCode.Slash, KeyCode.RightShift};
        private static readonly KeyCode[] RowKeyCode5 = {KeyCode.LeftControl,KeyCode.Menu,KeyCode.LeftAlt,KeyCode.Space,KeyCode.RightAlt,KeyCode.Menu,KeyCode.RightControl,KeyCode.LeftArrow,KeyCode.DownArrow,KeyCode.UpArrow,KeyCode.RightArrow};
        private static readonly KeyCode[] RowKeyCode6 = {KeyCode.Insert, KeyCode.Home, KeyCode.PageUp, KeyCode.Keypad7, KeyCode.Keypad8, KeyCode.Keypad9, KeyCode.KeypadMinus, KeyCode.KeypadEnter};
        private static readonly KeyCode[] RowKeyCode7 = {KeyCode.Delete, KeyCode.End, KeyCode.PageDown, KeyCode.Keypad4, KeyCode.Keypad5, KeyCode.Keypad6, KeyCode.KeypadPlus, KeyCode.KeypadMultiply};
        private static readonly KeyCode[] RowKeyCode8 = {KeyCode.Mouse0, KeyCode.Mouse1, KeyCode.Mouse2, KeyCode.Keypad1, KeyCode.Keypad2, KeyCode.Keypad3, KeyCode.Keypad0, KeyCode.KeypadDivide};
        private static readonly KeyCode[] RowKeyCode9 = {KeyCode.Mouse3, KeyCode.Mouse4, KeyCode.Mouse5, KeyCode.Mouse6};
    
        #endregion

        private InputConfigure _editConfigure;
        private InputOperation _editOperation;

        private List<KeyCode> _selected;

        private bool _isShowAllOperations;

        private List<InputOperation> _removeOperations;

        private int _maxOperation = 20;

        public static void OpenConfigure(InputConfigure configure)
        {
            InputConfigureEditorWindow window = GetWindow<InputConfigureEditorWindow>("Keyboard");

            window._editConfigure = configure;
            
            window.RefreshSelectedKeyCodes();

            window.minSize = new Vector2(1100, 525);
            window.maxSize = new Vector2(1100, 525);

            window._removeOperations = new List<InputOperation>();
        }

        private void OnGUI()
        {
            _editOperation ??= FindOrCreateFirst();

            titleContent = new GUIContent("Operation Editor - " + _editConfigure.configureName);
            
            foreach (var operation in _removeOperations)
            {
                foreach (var keyCode in operation.bindKeyCodes)
                {
                    RemoveFromSelectedKeyCodes(keyCode);
                }
                _editConfigure.operations.Remove(operation);
            }
            _removeOperations.Clear();

            DefaultColor();
            
            GUILayout.BeginHorizontal();
            
            GUILayout.BeginVertical();
            
            _isShowAllOperations = GUILayout.Toggle(_isShowAllOperations, "Show All");

            GUILayout.BeginScrollView(new Vector2(300, 280));
            
            DrawList();
            
            GUILayout.EndScrollView();
            
            GUILayout.EndVertical();
            
            GUILayout.BeginVertical();
            
            DrawRow(Row0, RowLength0, RowKeyCode0);
            DrawRow(Row1, RowLength1, RowKeyCode1);
            DrawRow(Row2, RowLength2, RowKeyCode2);
            DrawRow(Row3, RowLength3, RowKeyCode3);
            DrawRow(Row4, RowLength4, RowKeyCode4);
            DrawRow(Row5, RowLength5, RowKeyCode5);
            DrawRow(Row6, RowLength6, RowKeyCode6);
            DrawRow(Row7, RowLength7, RowKeyCode7);
            DrawRow(Row8, RowLength8, RowKeyCode8);
            DrawRow(Row9, RowLength9, RowKeyCode9);

            GUILayout.EndVertical();
            
            GUILayout.EndHorizontal();
        }

        private void DrawList()
        {
            int i = 0;
            foreach (var operation in _editConfigure.operations)
            {
                GUILayout.BeginHorizontal();

                if (operation.Equals(_editOperation))
                    SelectedColor();
                
                if (GUILayout.Button("Select"))
                {
                    _editOperation = operation;
                    
                    EditorUtility.SetDirty(_editConfigure);
                }

                if (GUILayout.Button("X"))
                {
                    _removeOperations.Add(operation);
                    
                    EditorUtility.SetDirty(_editConfigure);

                    GUILayout.EndHorizontal();

                    _editOperation = FindOrCreateFirst();

                    return;
                }
                
                if (operation.Equals(_editOperation))
                    SelectedColor();

                string editOperationName = EditorGUILayout.TextField(operation.name);

                if (editOperationName != operation.name)
                {
                    operation.name = editOperationName;

                    _editOperation = operation;
                }
                
                DefaultColor();
                
                i++;
                
                GUILayout.EndHorizontal();
            }

            if (GUILayout.Button("+ Operation"))
            {
                if (_editConfigure.operations.Count < _maxOperation)
                {
                    _editConfigure.operations.Add(new InputOperation
                    {
                        name = "MyOperation",
                        bindKeyCodes = new List<KeyCode>().ToArray()
                    });
                    
                    EditorUtility.SetDirty(_editConfigure);
                    
                    RefreshSelectedKeyCodes();
                }
                else
                {
                    EditorUtility.DisplayDialog("Warning!",
                        $"The number of operations is full, the maximum is {_maxOperation}.",
                        "OK");
                }
            }
        }

        private void DrawRow(string[] row, float[] rowLength, KeyCode[] rowKeyCode)
        {
            GUILayout.BeginHorizontal();

            int i = 0;
            foreach (string key in row)
            {
                if (DrawButton(key, rowLength[i], rowKeyCode[i]))
                {
                    List<KeyCode> keyCodes = new List<KeyCode>(_editOperation.bindKeyCodes);

                    bool selected = IsSelected(rowKeyCode[i]);
                    bool isOperationSelected = _editOperation.bindKeyCodes.Contains(rowKeyCode[i]);
                    
                    if (selected)
                    {
                        if (isOperationSelected)
                        {
                            keyCodes.Remove(rowKeyCode[i]);
                            RemoveFromSelectedKeyCodes(rowKeyCode[i]);
                            _editOperation.bindKeyCodes = keyCodes.ToArray();
                        }
                        else
                        {
                            EditorUtility.DisplayDialog("Repeated binding!",
                                "You cannot bind the same key to different operations, please try using other keys.",
                                "OK");
                        }
                    }
                    else
                    {
                        keyCodes.Add(rowKeyCode[i]);
                        AddToSelectedKeyCodes(rowKeyCode[i]);
                        _editOperation.bindKeyCodes = keyCodes.ToArray();
                    }
                }
                GUILayout.Space(ButtonSpacing);

                i++;
            }
            GUILayout.EndHorizontal();
        }

        private bool DrawButton(string key, float rowLength, KeyCode rowKeyCode)
        {
            string bindOperationName = "";
            
            if (_editOperation != null)
            {
                if (_isShowAllOperations)
                {
                    int i = 0;
                    foreach (var operation in _editConfigure.operations)
                    {
                        if (operation.bindKeyCodes.Contains(rowKeyCode))
                        {
                            SelectedColor();
                            bindOperationName = "\n" + operation.name;
                        }

                        i++;
                    }
                }
                else
                {
                    if (_editOperation.bindKeyCodes.Contains(rowKeyCode))
                    {
                        SelectedColor();
                    }
                }
            }

            bool buttonResult = GUILayout.Button(key + bindOperationName, GUILayout.Width(rowLength), GUILayout.Height(ButtonHeight));
        
            DefaultColor();

            return buttonResult;
        }

        public void SelectedColor()
        {
            GUI.backgroundColor = new Color(0.09f, 0.86f, 1f);
            GUI.contentColor = new Color(0.92f, 0.99f, 1f);
        }

        private void DefaultColor()
        {
            GUI.contentColor = Color.white;
            GUI.backgroundColor = Color.gray;
        }

        private InputOperation FindOrCreateFirst()
        {
            if (_editConfigure.operations == null)
            {
                _editConfigure.operations = new List<InputOperation>();
            }

            if (_editConfigure.operations.Count <= 0)
            {
                _editConfigure.operations.Add(new InputOperation
                {
                    name = "MyOperation",
                    bindKeyCodes = new List<KeyCode>().ToArray()
                });
            }
            
            EditorUtility.SetDirty(_editConfigure);

            return _editConfigure.operations[0];
        }

        private void RefreshSelectedKeyCodes()
        {
            _selected = new List<KeyCode>();

            foreach (var operation in _editConfigure.operations)
            {
                foreach (var keyCode in operation.bindKeyCodes)
                {
                    AddToSelectedKeyCodes(keyCode);
                }
            }
        }

        private void AddToSelectedKeyCodes(KeyCode keyCode)
        {
            if (!_selected.Contains(keyCode))
            {
                _selected.Add(keyCode);
            }
        }

        private void RemoveFromSelectedKeyCodes(KeyCode keyCode)
        {
            if (_selected.Contains(keyCode))
            {
                _selected.Remove(keyCode);
            }
        }

        private bool IsSelected(KeyCode keyCode)
        {
            return _selected.Contains(keyCode);
        }
    }
}
#endif