using GraphicLibrary.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResourceManagement
{
    public class ArDirect3DVertex
    {
        public ArFloatVector3 Position { get; set; }        
        public ArFloatVector3 TextureCoordinate { get; set; }
        public ArFloatVector3 ShadowCoordinate { get; set; }
    }

    public class ArDirect3DModel
    {   
        public ArDirect3DVertex[] Vertices { get; set; }
        public int[] indices { get; set; }

    }
}
