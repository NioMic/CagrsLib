using System;
using System.Collections.Generic;
using System.Linq;
using CagrsLib.LibCore;
using Newtonsoft.Json;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace CagrsLib.SGraphView
{
    /*
     * 该类为GraphView的派生类，用于简化构建GraphView
     *
     * 包含功能：
     * 一、保存
     *  SGraphData导入其中，经由NodeAdapter与PortAdapter构建具体的Node和Port
     *
     * 二、读取
     *  通过GraphView中的信息，生成对应的SGraphData将其存储到ScriptableObject中
     *
     * 三、生成搜索窗口
     *  通过NodeAdapter中的GetSearchProviderPath获得该Node的位置、自动生成SearchProviderWindow
     */
    
    //TODO :: 当Node被移除的时候，存储的数据并不会对应消失，待修正

    public class SGraphView : UnityEditor.Experimental.GraphView.GraphView
    {
        public bool Changed; // 数据是否被更改过
        
        private EditorWindow _window;
        private SSearchProviderWindow _searchWindowProvider;
        private Blackboard _nodeInspector;

        private object _graphExtraData;
        
        private Dictionary<string, ISNodeAdapter> _nodeAdapters;
        private Dictionary<string, ISPortAdapter> _portAdapters;

        private Dictionary<Node, string> _nodeGuid;
        private Dictionary<string, Node> _nodeGuidString;
        private Dictionary<string, string> _nodeAdapterName; // Guid - AdapterName
        private Dictionary<string, object> _nodeExtraDatas;
        
        private Dictionary<Port, string> _portGuid;
        private Dictionary<string, Port> _portGuidString;
        private Dictionary<string, string> _portAdapterName; // Guid - AdapterName
        private Dictionary<string, object> _portExtraDatas;
        private Dictionary<string, Node> _portFrom; // Guid - Node

        private readonly JsonSerializerSettings _jsonSerializerSetting = new()
        {
            TypeNameHandling = TypeNameHandling.All
        };

        public readonly Vector2 DefaultNodeSize = new(200, 150);

        public SGraphView(EditorWindow from)
        {
            _window = from;
            
            ClearAdapters();
            
            ClearData();
        }

        public void ClearData()
        {
            _nodeExtraDatas = new Dictionary<string, object>();
            _portExtraDatas = new Dictionary<string, object>();

            _nodeGuid = new Dictionary<Node, string>();
            _portGuid = new Dictionary<Port, string>();

            _nodeGuidString = new Dictionary<string, Node>();
            _portGuidString = new Dictionary<string, Port>();
            
            _nodeAdapterName = new Dictionary<string, string>();
            _portAdapterName = new Dictionary<string, string>();

            _portFrom = new Dictionary<string, Node>();
        }

        public void ClearAdapters()
        {
            _nodeAdapters = new Dictionary<string, ISNodeAdapter>();
            _portAdapters = new Dictionary<string, ISPortAdapter>();
        }

        #region Adapter Manage

        public SGraphView RigisterAdapter(ISNodeAdapter nodeAdapter)
        {
            _nodeAdapters.Add(nodeAdapter.GetRegisterName(), nodeAdapter);
            return this;
        }

        public SGraphView RigisterAdapter(ISPortAdapter portAdapter)
        {
            _portAdapters.Add(portAdapter.GetRegisterName(), portAdapter);
            return this;
        }

        public bool TryFindPortAdapter(string adapterName, out ISPortAdapter portAdapter)
        {
            if (adapterName != null && _portAdapters.ContainsKey(adapterName))
            {
                portAdapter = _portAdapters[adapterName];
                return true;
            }

            portAdapter = null;
            return false;
        }

        public bool TryFindNodeAdapter(string adapterName, out ISNodeAdapter nodeAdapter)
        {
            if (adapterName != null && _nodeAdapters.ContainsKey(adapterName))
            {
                nodeAdapter = _nodeAdapters[adapterName];
                return true;
            }

            nodeAdapter = null;
            return false;
        }

        #endregion

        #region Build

        public void AddNode(Node node)
        {
            AddElement(node);
            Changed = true;
        }

        public void AddNodes(List<Node> nodes)
        {
            foreach (var node in nodes)
            {
                AddNode(node);
            }
            Changed = true;
        }

        public List<Node> BuildNodes(SGraphData data)
        {
            List<Node> nodes = new List<Node>();

            foreach (var nodeData in data.nodeDatas)
            {
                nodes.Add(BuildNode(nodeData));
            }

            return nodes;
        }

        public Node BuildNode(SGraphData.NodeData data)
        {
            Node node = new Node();
            
            if (!TryFindNodeAdapter(data.nodeAdapterName, out ISNodeAdapter adapter))
            {
                LibUtil.LogWarning(this, "Can't found node adapter : " + data.nodeAdapterName);
                return null;
            }
            
            node.SetPosition(new Rect(
                new Vector2(data.nodePositionX, data.nodePositionY),
                DefaultNodeSize));

            node.title = adapter.GetNodeTitle(data);

            if (! adapter.CanDestroy(data))
            {
                node.capabilities -= Capabilities.Deletable;
            }
            
            foreach (var portData in data.portDatas)
            {
                Port port = BuildPort(portData);

                if (port != null)
                {
                    if (portData.portDirection == Direction.Input) node.inputContainer.Add(port);
                    if (portData.portDirection == Direction.Output) node.outputContainer.Add(port);
                    
                    _portFrom.Add(portData.portGuid, node);
                }
            }

            _nodeExtraDatas.Add(data.nodeGuid, Deserialize(data.nodeExtraData));
            _nodeAdapterName.Add(data.nodeGuid, data.nodeAdapterName);
            _nodeGuid.Add(node, data.nodeGuid);
            _nodeGuidString.Add(data.nodeGuid, node);

            try
            {
                ISNodeStyleAdapter styleAdapter = (ISNodeStyleAdapter)adapter;
                
                styleAdapter.OnChangeNodeStyle(node);
            }
            catch (InvalidCastException)
            {
                
            }
            
            return node;
        }

        public SGraphData.NodeData CreateNodeData(string adapterName)
        {
            SGraphData.NodeData nodeData = new SGraphData.NodeData();
            
            if (!TryFindNodeAdapter(adapterName, out ISNodeAdapter adapter))
            {
                LibUtil.LogWarning(this, "Can't found node adapter : " + adapterName);
                return null;
            }
            
            nodeData.nodeGuid = Guid.NewGuid().ToString();

            nodeData.nodeAdapterName = adapterName;
            
            nodeData.nodePositionX = 0;
            nodeData.nodePositionY = 0;

            nodeData.nodeExtraData = null;
            
            foreach (var portData in adapter.BuildPorts(null, this))
            {
                nodeData.portDatas.Add(portData);
            }

            return nodeData;
        }

        public Node BuildNodeAsNew(string adapterName)
        {
            return BuildNode(CreateNodeData(adapterName));
        }

        public Port BuildPort(SGraphData.NodeData.PortData data)
        {
            if (!TryFindPortAdapter(data.portAdapterName, out ISPortAdapter adapter))
            {
                LibUtil.LogWarning(this, "Can't found port adapter : " + data.portAdapterName);
                return null;
            }
            
            Port port = Port.Create<Edge>(data.portOrientation, data.portDirection, data.portCapacity, null);

            port.portColor = adapter.GetPortColor(data);
            
            port.portName = adapter.GetPortName(data);

            _portExtraDatas.Add(data.portGuid, Deserialize(data.portExtraData));
            _portAdapterName.Add(data.portGuid, data.portAdapterName);
            _portGuid.Add(port, data.portGuid);
            _portGuidString.Add(data.portGuid, port);
            
            return port;
        }

        public SGraphData.NodeData.PortData CreatePortData(string portAdapterName, Direction direction,
            Port.Capacity capacity = Port.Capacity.Single, Orientation orientation = Orientation.Horizontal)
        {
            SGraphData.NodeData.PortData portData = new SGraphData.NodeData.PortData();
            
            if (!TryFindPortAdapter(portAdapterName, out ISPortAdapter adapter))
            {
                LibUtil.LogWarning(this, "Can't found port adapter : " + portAdapterName);
                return null;
            }

            portData.portGuid = Guid.NewGuid().ToString();
            
            portData.portCapacity = capacity;

            portData.portAdapterName = portAdapterName;

            portData.portDirection = direction;

            portData.portColor = adapter.GetPortColor(portData);

            portData.portOrientation = orientation;

            return portData;
        }

        public Port BuildPortAsNew(string portAdapterName, Direction direction,
            Port.Capacity capacity = Port.Capacity.Single, Orientation orientation = Orientation.Horizontal)
        {
            return BuildPort(CreatePortData(portAdapterName, direction, capacity, orientation));
        }

        public void BuildSearchWindow()
        {
            _searchWindowProvider = ScriptableObject.CreateInstance<SSearchProviderWindow>();

            DirectoryObject<string> directory = DirectoryObject<string>.Create();

            foreach (var adapter in _nodeAdapters)
            {
                Debug.Log($"{adapter.Value.GetSearchProviderWindowPath()}, {adapter.Key}");

                directory.QuickPut(adapter.Value.GetSearchProviderWindowPath(), adapter.Key, adapter.Key);
            }
            
            _searchWindowProvider.Initialize(directory, _window, this);
            
            nodeCreationRequest += context =>
            {
                SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), _searchWindowProvider);
            };
        }

        public void BuildNodeInspector()
        {
            if (_window == null) return;

            _nodeInspector = new Blackboard();

            _window.rootVisualElement.Add(_nodeInspector);

            InspectorElement inspectorElement = new InspectorElement();

            _window.rootVisualElement.Add(inspectorElement);
        }

        private string Serialize(object obj)
        {
            return JsonConvert.SerializeObject(obj, _jsonSerializerSetting);
        }
        
        private object Deserialize(string str)
        {
            if (str == null)
            {
                return null;
            }

            return JsonConvert.DeserializeObject(str, _jsonSerializerSetting);
        }

        #endregion
        
        #region Get & Set Data

        public void SetData(SGraphData data)
        {
            ClearData();

            foreach (var node in nodes)
            {
                RemoveNode(node);
            }

            AddNodes(BuildNodes(data));

            _graphExtraData = Deserialize(data.graphExtraData);

            foreach (var linkData in data.linkDatas)
            {
                ConnectPort(_portGuidString[linkData.portGuid],
                    _portGuidString[linkData.targetPortGuid]);
            }

            Changed = false;
        }

        public SGraphData GetData()
        {
            SGraphData data = new SGraphData();
            
            foreach (var node in nodes)
            {
                string nodeGuid = _nodeGuid[node];
                
                SGraphData.NodeData nodeData = new SGraphData.NodeData();

                nodeData.nodeAdapterName = _nodeAdapterName[nodeGuid];

                nodeData.nodeGuid = nodeGuid;

                Vector2 pos = node.GetPosition().position;

                nodeData.nodePositionX = pos.x;
                nodeData.nodePositionY = pos.y;

                nodeData.nodeExtraData = Serialize(_nodeExtraDatas[nodeGuid]);

                foreach (var portGuid in _portGuid)
                {
                    if (_nodeGuid[_portFrom[portGuid.Value]] == nodeGuid)
                    {
                        SGraphData.NodeData.PortData portData = new SGraphData.NodeData.PortData();

                        portData.portAdapterName = _portAdapterName[portGuid.Value];

                        portData.portGuid = portGuid.Value;

                        portData.portColor = portGuid.Key.portColor;

                        portData.portDirection = portGuid.Key.direction;

                        portData.portOrientation = portGuid.Key.orientation;

                        portData.portCapacity = portGuid.Key.capacity;

                        portData.portExtraData = Serialize(_portExtraDatas[portGuid.Value]);

                        nodeData.portDatas.Add(portData);
                    }
                }

                Changed = false;

                data.nodeDatas.Add(nodeData);
            }

            foreach (var edge in edges)
            {
                SGraphData.LinkData linkData = new SGraphData.LinkData();

                linkData.nodeGuid = _nodeGuid[edge.output.node];
                linkData.portGuid = _portGuid[edge.output];
                
                linkData.targetNodeGuid = _nodeGuid[edge.input.node];
                linkData.targetPortGuid = _portGuid[edge.input];
                
                data.linkDatas.Add(linkData);
            }

            data.graphExtraData = Serialize(_graphExtraData);

            return data;
        }
        
        private void ConnectPort(Port outputPort, Port inputPort)
        {
            var edge = new Edge
            {
                output = outputPort,
                input = inputPort
            };
                
            if (inputPort == null || outputPort == null) return;
                
            edge.input.Connect(edge);
            edge.output.Connect(edge);
            Add(edge);
        }

        #endregion
        
        #region Remove

        public void RemoveNode(Node node)
        {
            if (_nodeGuid.ContainsKey(node))
            {
                string guid = _nodeGuid[node];
                _nodeAdapters.Remove(guid);
                _nodeExtraDatas.Remove(guid);

                foreach (var from in _portFrom)
                {
                    if (from.Value == node)
                    {
                        _portAdapterName.Remove(from.Key);
                        _portExtraDatas.Remove(from.Key);
                    }
                    _portFrom.Remove(from.Key);
                    
                    foreach (var guidValue in _portGuid)
                    {
                        if (guidValue.Value.Equals(from.Key)) _portGuid.Remove(guidValue.Key);
                    }
                }

                _nodeGuid.Remove(node);
                
                edges.Where(x => x.input.node == node).ToList()
                    .ForEach(RemoveElement);
                
                RemoveElement(node);
            }
        }

        #endregion

        #region Overrides

        /*
         * 这里实现了NodePort连接的功能，并且能
         * 防止 同节点、同方向、不同颜色的Port可以相连 的功能
         */
        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            var compatiblePorts = new List<Port>();
            foreach (var port in ports.ToList())
            {
                if (startPort.node == port.node ||
                    startPort.direction == port.direction ||
                    startPort.portColor != port.portColor)
                {
                    continue;
                }

                compatiblePorts.Add(port);
                Changed = true;
            }

            return compatiblePorts;
        }

        #endregion Search Provider Window

        public class SSearchProviderWindow : ScriptableObject, ISearchWindowProvider
        {
            private DirectoryObject<string> _providerData;

            private EditorWindow _window;

            private UnityEditor.Experimental.GraphView.GraphView _graphView;

            public void Initialize(DirectoryObject<string> data, EditorWindow window, UnityEditor.Experimental.GraphView.GraphView graphView)
            {
                _providerData = data;

                _graphView = graphView;
                
                _window = window;
            }

            public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
            {
                List<SearchTreeEntry> entries = new List<SearchTreeEntry>();

                _providerData.ListTree(objectFirst: true,
                    IdirectoryTreeEvent: new SearchWindowProviderTreeEvent(entries));

                return entries;
            }

            public bool OnSelectEntry(SearchTreeEntry searchTreeEntry, SearchWindowContext context)
            {
                var windowRoot = _window.rootVisualElement;
                var windowMousePosition = windowRoot.ChangeCoordinatesTo(windowRoot.parent, context.screenMousePosition - _window.position.position);
                var graphMousePosition = _graphView.contentViewContainer.WorldToLocal(windowMousePosition);

                if (searchTreeEntry.userData != null)
                {
                    SGraphView graphView = (SGraphView)_graphView;

                    Debug.Log((string)searchTreeEntry.userData);
                    
                    Node node = graphView.BuildNodeAsNew((string)searchTreeEntry.userData);

                    node.SetPosition(new Rect(graphMousePosition, Vector2.zero));
                    
                    graphView.AddNode(node);
                }

                return true;
            }
            
            private class SearchWindowProviderTreeEvent : IDirectoryTreeEvent<string>
            {
                private readonly List<SearchTreeEntry> _searchTreeEntries;

                public SearchWindowProviderTreeEvent(List<SearchTreeEntry> searchTreeEntries)
                {
                    _searchTreeEntries = searchTreeEntries;
                }

                public string OnDirectory(int depth, string dicName, string fromDic, char split)
                {
                    _searchTreeEntries.Add(new SearchTreeGroupEntry(
                        new GUIContent(dicName),
                        depth));
                    return "";
                }

                public string OnObject(int depth, string objName, string fromDic, char split, string str)
                {
                    if (str == null) return "";
                    
                    _searchTreeEntries.Add(new SearchTreeEntry(new GUIContent(objName))
                    {
                        level = depth,
                        userData = str
                    });
                    return "";
                }
            }
        }
    }
}