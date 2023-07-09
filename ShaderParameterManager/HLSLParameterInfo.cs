using SharpDX.Direct3D12;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShaderParameterManager
{
    public struct HLSLParameterInfo
    {
        public string Name { get; set; }
        public Type Type { get; set; }
        public int Count { get; set; } // -1 符
        public RootParameterType RootParameterType { get; set; }
        public ShaderVisibility Visibility { get; set; }
        public bool IsDescriptor { get; set; }
    }
    
}
