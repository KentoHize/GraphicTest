using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphicLibrary.Items
{
    //DirectX Compatible
    public struct ArColorVertex : IArVertex
    {
        public ArIntVector3 Position { get; set; }
        public ArFloatVector4 Color { get; set; }
      
        public static ArColorVertex Empty => new ArColorVertex();
        public static int ByteSize => 28;
        public ArColorVertex()
            : this(0, 0, 0, 0, 0, 0, 0)
        { }

        public ArColorVertex(ArIntVector3 position, ArFloatVector4 color)
            : this(position.X, position.Y, position.Z, color[0], color[1], color[2], color[3])
        { }

        public ArColorVertex(ArIntVector3 position, Color color)
            : this(position.X, position.Y, position.Z, color.ToArFloatVector4())
        { }

        public ArColorVertex(int x, int y, int z, float red, float green, float blue, float alpha)
        {
            Position = new ArIntVector3(x, y, z);
            Color = new ArFloatVector4(red, green, blue, alpha);           
        }
        public ArColorVertex(int x, int y, int z, ArFloatVector4 color)
            : this(x, y, z, color[0], color[1], color[2], color[3])
        { }

        public ArColorVertex(int x, int y, int z, Color color)
            : this(x, y, z, color.ToArFloatVector4())
        { }

        public ArColorVertex(int x, int y, int z)
            : this(x, y, z, ArFloatVector4.Zero)
        { }
    }
}
