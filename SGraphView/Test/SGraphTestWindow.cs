#if UNITY_EDITOR 
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Button = UnityEngine.UIElements.Button;

namespace CagrsLib.SGraphView.Test
{
    public class SGraphTestWindow : EditorWindow
    {
        private SGraphTestGraphData _currentData;

        private SGraphView _graphView;
        
        public static void OpenWindow(SGraphTestGraphData data)
        {
            SGraphTestWindow window = CreateInstance<SGraphTestWindow>();

            window._currentData = data;
            
            window.Show();
            
            window.GenerateView();
            
            window.LoadData();

            window.titleContent = new GUIContent("Dialogue Editor - " + data.name);
        }

        #region Override

        public override void SaveChanges()
        {
            WriteData();
            base.SaveChanges();
        }

        private void OnGUI()
        {
            var windowRoot = rootVisualElement;
            var windowMousePosition = Event.current.mousePosition;
            var graphMousePosition = _graphView.WorldToLocal(windowMousePosition);
            
            Event inputEvent = Event.current;
            
            if (inputEvent.type == EventType.KeyDown && inputEvent.keyCode == KeyCode.S)
            {
                Node node = _graphView.BuildNodeAsNew("entry");

                node.SetPosition(new Rect(graphMousePosition, _graphView.DefaultNodeSize));

                _graphView.AddElement(node);
            }
            
            if (inputEvent.type == EventType.KeyDown && inputEvent.keyCode == KeyCode.D)
            {
                Node node = _graphView.BuildNodeAsNew("dialogue");

                node.SetPosition(new Rect(graphMousePosition, _graphView.DefaultNodeSize));
                    
                _graphView.AddElement(node);
            }
            
            if (inputEvent.type == EventType.KeyDown && inputEvent.keyCode == KeyCode.F)
            {
                Node node = _graphView.BuildNodeAsNew("options");

                node.SetPosition(new Rect(graphMousePosition, _graphView.DefaultNodeSize));
                    
                _graphView.AddElement(node);
            }

            hasUnsavedChanges = _graphView.Changed;
            saveChangesMessage = "Your current changes have not been saved. Do you want to save them before exiting?";
        }

        private void OnDestroy()
        {
            _graphView.ClearAdapters();
            
            _graphView.ClearData();
        }

        #endregion
        
        #region UI 

        private void GenerateView()
        {
            _graphView = new SGraphView(this);
            
            rootVisualElement.Add(_graphView);
            
            _graphView.StretchToParentSize();
            _graphView.SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
            
            _graphView.AddManipulator(new ContentDragger());
            _graphView.AddManipulator(new SelectionDragger());
            _graphView.AddManipulator(new RectangleSelector());

            GridBackground gridBackground = new GridBackground();
            _graphView.Insert(0, gridBackground);
            
            _graphView.ClearAdapters();
            _graphView.ClearData();

            rootVisualElement.Add(_graphView);
            rootVisualElement.Add(BuildToolBar());

            _graphView.RigisterAdapter(new DialogueNode());
            _graphView.RigisterAdapter(new OptionNode());
            _graphView.RigisterAdapter(new EntryNode());
            
            _graphView.RigisterAdapter(new LinkDialogue());
            _graphView.RigisterAdapter(new LinkOptions());
            
            _graphView.BuildSearchWindow();
            _graphView.BuildNodeInspector();
        }

        private Toolbar BuildToolBar()
        {
            Toolbar toolbar = new Toolbar
            {
                style =
                {
                    position = Position.Absolute
                }
            };
            
            Button saveButton = new Button(clickEvent: WriteData)
            {
                text = "Save Assets"
            };
            
            toolbar.Add(saveButton);

            return toolbar;
        }

        #endregion

        #region Data Load & Write

        private void LoadData()
        {
            if (_currentData.graphData == null)
            {
                _currentData.graphData = new SGraphData();
                EditorUtility.SetDirty(_currentData);
            }

            _graphView.SetData(_currentData.graphData);
        }

        private void WriteData()
        {
            _currentData.graphData = _graphView.GetData();
            
            EditorUtility.SetDirty(_currentData);
        }

        #endregion
    }
}
#endif