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

        #region KeyBoard

        private static readonly string[] Row1 = {"`", "1", "2", "3", "4", "5", "6", "7", "8", "9", "0", "-", "=", "Back"};
        private static readonly string[] Row2 = {"Tab", "Q", "W", "E", "R", "T", "Y", "U", "I", "O", "P", "[", "]", "\\"};
        private static readonly string[] Row3 = {"Caps", "A", "S", "D", "F", "G", "H", "J", "K", "L", ";", "'", "Enter"};
        private static readonly string[] Row4 = {"Shift", "Z", "X", "C", "V", "B", "N", "M", ",", ".", "/", "Shift"};
        private static readonly string[] Row5 = {"Ctrl","Fn","Alt","Space","Alt","Fn","Ctrl","←","↓","↑","→"};
    
        private static readonly float[] RowLength1 = {50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 138};
        private static readonly float[] RowLength2 = {120, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 70};
        private static readonly float[] RowLength3 = {130, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 110};
        private static readonly float[] RowLength4 = {140, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 150};
        private static readonly float[] RowLength5 = {120, 60, 60, 170, 60, 60, 60, 50, 50, 50, 50};
    
        private static readonly KeyCode[] RowKeyCode1 = {KeyCode.BackQuote, KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4, KeyCode.Alpha5, KeyCode.Alpha6, KeyCode.Alpha7, KeyCode.Alpha8, KeyCode.Alpha9, KeyCode.Alpha0, KeyCode.Minus, KeyCode.Equals, KeyCode.Backspace};
        private static readonly KeyCode[] RowKeyCode2 = {KeyCode.Tab, KeyCode.Q, KeyCode.W, KeyCode.E, KeyCode.R, KeyCode.T, KeyCode.Y, KeyCode.U, KeyCode.I, KeyCode.O, KeyCode.P, KeyCode.LeftBracket, KeyCode.RightBracket, KeyCode.Backslash};
        private static readonly KeyCode[] RowKeyCode3 = {KeyCode.CapsLock, KeyCode.A, KeyCode.S, KeyCode.D, KeyCode.F, KeyCode.G, KeyCode.H, KeyCode.J, KeyCode.K, KeyCode.L, KeyCode.Semicolon, KeyCode.Quote, KeyCode.KeypadEnter};
        private static readonly KeyCode[] RowKeyCode4 = {KeyCode.LeftShift, KeyCode.Z, KeyCode.X, KeyCode.C, KeyCode.V, KeyCode.B, KeyCode.N, KeyCode.M, KeyCode.Comma, KeyCode.Period, KeyCode.Slash, KeyCode.RightShift};
        private static readonly KeyCode[] RowKeyCode5 = {KeyCode.LeftControl,KeyCode.Menu,KeyCode.LeftAlt,KeyCode.Space,KeyCode.RightAlt,KeyCode.Menu,KeyCode.RightControl,KeyCode.LeftArrow,KeyCode.DownArrow,KeyCode.UpArrow,KeyCode.RightArrow};
    
        #endregion

        private InputConfigure _editConfigure;
        private InputOperation _editOperation;

        private bool _isShowAllOperations;

        private List<InputOperation> _removeOperations;

        private readonly Color[] _colors =
        {
            new(1f, 0.1f, 0.07f, 1f),
            new (0.12f, 1f, 0.13f, 1f),
            new (0.16f, 0.31f, 1f, 1f),
            new (0.91f, 1f, 0.24f, 1f),
            new (0.75f, 0.31f, 1f, 1f),
            new (0.32f, 1f, 0.74f, 1f),
            new (1f, 0.52f, 0.05f, 1f),
            new (0.39f, 0.14f, 1f, 1f),
            new (0.06f, 0.79f, 1f, 1f),
            new (0.76f, 1f, 0.61f, 1f),
            new (0.58f, 0.06f, 0.04f, 1f),
            new (0.07f, 0.62f, 0.08f, 1f),
            new (0.09f, 0.18f, 0.57f, 1f),
            new (0.59f, 0.65f, 0.15f, 1f),
            new (0.44f, 0.18f, 0.58f, 1f),
            new (0.2f, 0.64f, 0.47f, 1f),
            new (0.65f, 0.33f, 0.03f, 1f),
            new (0.25f, 0.09f, 0.66f, 1f),
            new (0.04f, 0.48f, 0.62f, 1f),
            new (0.47f, 0.61f, 0.38f, 1f)
        };

        public static void OpenConfigure(InputConfigure configure)
        {
            InputConfigureEditorWindow window = GetWindow<InputConfigureEditorWindow>("Keyboard");

            window._editConfigure = configure;

            window.minSize = new Vector2(1100, 290);
            window.maxSize = new Vector2(1100, 290);

            window._removeOperations = new List<InputOperation>();
        }

        private void OnGUI()
        {
            _editOperation ??= FindOrCreateFirst();

            titleContent = new GUIContent("Operation Editor - " + _editConfigure.configureName);
            
            foreach (var operation in _removeOperations)
            {
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
            
            DrawRow(Row1, RowLength1, RowKeyCode1);
            DrawRow(Row2, RowLength2, RowKeyCode2);
            DrawRow(Row3, RowLength3, RowKeyCode3);
            DrawRow(Row4, RowLength4, RowKeyCode4);
            DrawRow(Row5, RowLength5, RowKeyCode5);

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
                
                GUI.backgroundColor = new Color(1f, 0f, 0.18f);
                GUI.contentColor = new Color(0.92f, 0.99f, 1f);

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

                GUI.backgroundColor = GetColorByIndex(i);
                GUI.contentColor = GetColorByIndex(i);

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
                _editConfigure.operations.Add(new InputOperation
                {
                    name = "MyOperation",
                    bindKeyCodes = new List<KeyCode>().ToArray()
                });
                    
                EditorUtility.SetDirty(_editConfigure);
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
                    
                    if (_editOperation.bindKeyCodes.Contains(rowKeyCode[i]))
                    {
                        keyCodes.Remove(rowKeyCode[i]);
                        _editOperation.bindKeyCodes = keyCodes.ToArray();
                    }
                    else
                    {
                        keyCodes.Add(rowKeyCode[i]);
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
            if (_editOperation != null)
            {
                if (_isShowAllOperations)
                {
                    int i = 0;
                    foreach (var operation in _editConfigure.operations)
                    {
                        if (operation.bindKeyCodes.Contains(rowKeyCode))
                        {
                            GUI.backgroundColor = GetColorByIndex(i);
                            GUI.contentColor = GetColorByIndex(i);
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

            bool buttonResult = GUILayout.Button(key, GUILayout.Width(rowLength), GUILayout.Height(ButtonHeight));
        
            DefaultColor();

            return buttonResult;
        }

        public void SelectedColor()
        {
            GUI.backgroundColor = new Color(0f, 1f, 0.07f);
            GUI.contentColor = new Color(0.92f, 0.99f, 1f);
        }

        private void DefaultColor()
        {
            GUI.contentColor = Color.white;
            GUI.backgroundColor = Color.gray;
        }

        private Color GetColorByIndex(int index)
        {
            if (index < _colors.Length)
            {
                return _colors[index];
            }
            
            return Color.gray;
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
    }
}
#endif