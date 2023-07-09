using SharpDX.Direct3D12;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShaderParameterManager
{
    public struct HLSLParameterInfo
    {
        public string Name { get; set; }
        public Type Type { get; set; }
        public SpecificType SpecificType { get; set;}
        public int Count { get; set; } // -1 符
        public ParameterType RootParameterType { get; set; }
        public ShaderVisibility Visibility { get; set; }
        public bool IsDescriptor { get; set; }
    }
    
    public enum SpecificType
    {
        NotSet = 0,
        Texture2D
    }

}
