using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphicLibrary.Items
{
    public struct ArTextureVertex : IArVertex
    {
        public ArIntVector3 Position { get; set; }
        public ArFloatVector2 TextureCroodinate { get; set; }
        public static ArTextureVertex Empty => new ArTextureVertex();

        public ArTextureVertex()
           : this(0, 0, 0, 0, 0)
        { }

        public ArTextureVertex(ArIntVector3 position, ArFloatVector2 textCrood)
            : this(position.X, position.Y, position.Z, textCrood[0], textCrood[1])
        { }    

        public ArTextureVertex(int x, int y, int z, float u, float v)
        {
            Position = new ArIntVector3(x, y, z);
            TextureCroodinate = new ArFloatVector2(u, v);
        }
        public ArTextureVertex(int x, int y, int z, ArFloatVector2 textCrood)
            : this(x, y, z, textCrood[0], textCrood[1])
        { }

        public ArTextureVertex(int x, int y, int z)
            : this(x, y, z, ArFloatVector2.Zero)
        { }
    }
}
