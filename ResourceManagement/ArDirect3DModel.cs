using GraphicLibrary.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ResourceManagement
{
    [StructLayout(LayoutKind.Sequential)]
    public class ArDirect3DVertex
    {
        public ArIntVector3 Position { get; set; }        
        public ArFloatVector3 TextureCoordinate { get; set; }
        public ArFloatVector3 ShadowCoordinate { get; set; }
    }

    public class ArDirect3DModel
    {   
        public ArDirect3DVertex[] Vertices { get; set; }
        public int[] Indices { get; set; }

    }
}
