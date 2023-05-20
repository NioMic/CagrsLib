#if UNITY_EDITOR 
using UnityEngine;

namespace CagrsLib.SGraphView
{
    public interface ISPortAdapter
    {
        Color GetPortColor(SGraphData.NodeData.PortData data);
        
        string GetPortName(SGraphData.NodeData.PortData data);

        string GetRegisterName();
    }
}
#endif