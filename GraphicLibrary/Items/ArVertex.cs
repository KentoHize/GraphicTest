using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphicLibrary.Items
{
    public struct ArVertex
    {
        ArIntVector3 Position { get; set; }
        ArFloatVector4 Color { get; set; }
        public static ArVertex Empty => new ArVertex();
        public ArVertex()
            : this(0, 0, 0, 0, 0, 0, 0)
        { }

        public ArVertex(ArIntVector3 position, ArFloatVector4 color)
            : this(position.X, position.Y, position.Z, color.W, color.X, color.Y, color.Z)
        { }

        public ArVertex(ArIntVector3 position, Color color)
            : this(position.X, position.Y, position.Z, color.ToArFloatVector4())
        { }

        public ArVertex(int x, int y, int z, float red, float green, float blue, float alpha)
        {
            Position = new ArIntVector3(x, y, z);
            Color = new ArFloatVector4(red, green, blue, alpha);
        }
        public ArVertex(int x, int y, int z, ArFloatVector4 color)
            : this(x, y, z, color.W, color.X, color.Y, color.Z)
        { }

        public ArVertex(int x, int y, int z, Color color)
            : this(x, y, z, (float)color.R / 255, (float)color.G / 255, (float)color.B / 255, (float)color.A / 255)
        { }

        public ArVertex(int x, int y, int z)
            : this(x, y, z, ArFloatVector4.Zero)
        { }
    }
}
