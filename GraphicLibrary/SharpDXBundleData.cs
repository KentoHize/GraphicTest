using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Direct3D;

namespace GraphicLibrary
{
    public struct Vertex
    {
        public Vector3 pos { get; set;}
        public Vector4 color { get; set; }
    }

    public class SharpDXBundleData
    {
        public Vertex[] Data { get; set; }
        public PrimitiveTopology PrimitiveTopology { get; set; }
    }
}
