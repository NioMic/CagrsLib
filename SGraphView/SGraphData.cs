#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace CagrsLib.SGraphView
{
    /*
     * 该类为SGraphView的内部数据，用于序列化到储存和反序列化到内存，
     * 内部分为四种数据 GraphData、PortData、NodeData、LinkData，除LinkData外其他数据都有一个用于绑定适配器的ExtraData和AdapterName
     */
    [Serializable]
    public class SGraphData
    {
        public List<NodeData> nodeDatas;
        
        public List<LinkData> linkDatas;

        public string graphExtraData;
        
        public SGraphData()
        {
            nodeDatas = new List<NodeData>();
            linkDatas = new List<LinkData>();
        }

        [Serializable]
        public class NodeData
        {
            public string nodeAdapterName;
            
            public string nodeGuid;

            public float nodePositionX;
            public float nodePositionY;
            
            public List<PortData> portDatas;
            
            public string nodeExtraData;

            public NodeData()
            {
                portDatas = new List<PortData>();
            }

            [Serializable]
            public class PortData
            {
                public string portAdapterName;
                
                public string portGuid;

                public Direction portDirection;
                
                public Orientation portOrientation;

                public Port.Capacity portCapacity;
                
                public Color portColor;
                
                public string portExtraData;
            }
        }

        [Serializable]
        public class LinkData
        {
            public string nodeGuid;
            
            public string portGuid;
            
            public string targetNodeGuid;
            
            public string targetPortGuid;
        }
    }
}
#endif