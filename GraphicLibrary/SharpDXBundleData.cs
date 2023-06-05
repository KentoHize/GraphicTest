using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GraphicLibrary.Items;
using SharpDX;
using SharpDX.Direct3D;

namespace GraphicLibrary
{
    public class SharpDXBundleData
    {
        //public IArVertex[] Verteices { get; set; }
        public ArTextureVertex[] TextureVertices { get; set; }
        public ArColorVertex[] ColorVertices { get; set; }
        public ArMixVertex[] MixVertices { get; set; }
        public int[] Indices { get; set; }

        public ArFloatMatrix44 TransformMartrix { get; set; }
        //public ArIntVector3 TraslateVector { get; set; }
        //public ArFloatVector3 RotateVector { get; set; }
        //public ArFloatVector3 ScaleVector { get; set; }
        public PrimitiveTopology PrimitiveTopology { get; set; }
    }
}
