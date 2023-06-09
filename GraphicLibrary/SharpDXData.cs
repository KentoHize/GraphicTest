using SharpDX.Direct3D12;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using GraphicLibrary.Items;

namespace GraphicLibrary
{
    public class SharpDXData
    {
        public SharpDXBundleData[] VerticesData { get; set; }        
        public ArFloatVector4 BackgroundColor { get; set; }
        public ArFloatMatrix44 TransformMartrix { get; set; }
    }
}
