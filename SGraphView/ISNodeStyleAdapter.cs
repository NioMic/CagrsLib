#if UNITY_EDITOR 
using UnityEditor.Experimental.GraphView;

namespace CagrsLib.SGraphView
{
    public interface ISNodeStyleAdapter
    {
        void OnChangeNodeStyle(Node node);
    }
}
#endif