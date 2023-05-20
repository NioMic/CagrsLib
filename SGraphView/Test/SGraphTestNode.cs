#if UNITY_EDITOR 
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace CagrsLib.SGraphView.Test
{
    public class EntryNode : ISNodeAdapter
    {
        public bool CanDestroy(SGraphData.NodeData data)
        {
            return false;
        }

        public string GetNodeTitle(SGraphData.NodeData data)
        {
            return "Entry";
        }

        public List<SGraphData.NodeData.PortData> BuildPorts(SGraphData.NodeData data, SGraphView graphView)
        {
            List<SGraphData.NodeData.PortData> portDatas = new List<SGraphData.NodeData.PortData>();
            
            portDatas.Add(graphView.CreatePortData("link_dialogue", Direction.Output));

            return portDatas;
        }

        public string GetSearchProviderWindowPath()
        {
            // TODO :: 空值异常
            // return null;
            return "Create";
        }

        public string GetRegisterName()
        {
            return "entry";
        }
    }
    
    public class DialogueNode : ISNodeAdapter, ISNodeStyleAdapter
    {
        public bool CanDestroy(SGraphData.NodeData data)
        {
            return true;
        }

        public string GetNodeTitle(SGraphData.NodeData data)
        {
            return "Dialogue";
        }

        public List<SGraphData.NodeData.PortData> BuildPorts(SGraphData.NodeData data, SGraphView graphView)
        {
            List<SGraphData.NodeData.PortData> portDatas = new List<SGraphData.NodeData.PortData>();
            
            portDatas.Add(graphView.CreatePortData("link_dialogue", Direction.Input, Port.Capacity.Multi));
            portDatas.Add(graphView.CreatePortData("link_options", Direction.Output, Port.Capacity.Multi));

            return portDatas;
        }

        public string GetSearchProviderWindowPath()
        {
            return "Create";
        }

        public string GetRegisterName()
        {
            return "dialogue";
        }

        public void OnChangeNodeStyle(Node node)
        {
            node.titleContainer.style.backgroundColor = new StyleColor(new Color(0f, 0.6f, 1f));
            node.titleContainer[0].style.color = new StyleColor(new Color(0f, 0.12f, 0.2f));
        }
    }
    
    public class OptionNode : ISNodeAdapter, ISNodeStyleAdapter
    {
        public bool CanDestroy(SGraphData.NodeData data)
        {
            return true;
        }

        public string GetNodeTitle(SGraphData.NodeData data)
        {
            return "Options";
        }

        public List<SGraphData.NodeData.PortData> BuildPorts(SGraphData.NodeData data, SGraphView graphView)
        {
            List<SGraphData.NodeData.PortData> portDatas = new List<SGraphData.NodeData.PortData>();
            
            portDatas.Add(graphView.CreatePortData("link_options", Direction.Input));
            portDatas.Add(graphView.CreatePortData("link_dialogue", Direction.Output));

            return portDatas;
        }

        public string GetSearchProviderWindowPath()
        {
            return "Create";
        }

        public string GetRegisterName()
        {
            return "option";
        }

        
        public void OnChangeNodeStyle(Node node)
        {
            node.titleContainer.style.backgroundColor = new StyleColor(new Color(0f, 1f, 0.83f));
            node.titleContainer[0].style.color = new StyleColor(new Color(0f, 0.31f, 0.25f));
        }
    }

    public class LinkDialogue : ISPortAdapter
    {
        public Color GetPortColor(SGraphData.NodeData.PortData data)
        {
            return new Color(0f, 0.6f, 1f);
        }

        public string GetPortName(SGraphData.NodeData.PortData data)
        {
            return "Dialogue";
        }

        public string GetRegisterName()
        {
            return "link_dialogue";
        }
    }
    
    public class LinkOptions : ISPortAdapter
    {
        public Color GetPortColor(SGraphData.NodeData.PortData data)
        {
            return new Color(0f, 1f, 0.83f);
        }

        public string GetPortName(SGraphData.NodeData.PortData data)
        {
            return "Options";
        }

        public string GetRegisterName()
        {
            return "link_options";
        }
    }
}
#endif