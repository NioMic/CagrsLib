using System.Collections.Generic;

namespace CagrsLib.SGraphView
{
    public interface ISNodeAdapter
    {
        bool CanDestroy(SGraphData.NodeData data);

        string GetNodeTitle(SGraphData.NodeData data);
        
        List<SGraphData.NodeData.PortData> BuildPorts(SGraphData.NodeData data, SGraphView graphView);
        
        string GetSearchProviderWindowPath();
        
        string GetRegisterName();
    }
}