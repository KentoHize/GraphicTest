using GraphicLibrary.Items;
using SharpDX.Direct3D12;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResourceManagement
{
    public struct DirectX12FrameVariables
    {
        public Dictionary<int, int> ReplaceMaterialIndices { get; set; }
        //public ArFloatMatrix44 TransformMatrix { get; set; }
        public Resource TransformMatrix { get; set; }
    }
}
