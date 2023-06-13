using System.Drawing;

namespace GraphicLibrary.Items
{
    public struct ArMixVertex : IArVertex
    {
        public ArIntVector3 Position { get; set; }
        public ArFloatVector4 Color { get; set; }
        public ArFloatVector2 TextureCroodinate { get; set; }
        public static ArMixVertex Empty => new ArMixVertex();
        public static int ByteSize => 36;
        public ArMixVertex()
            : this(0, 0, 0, 0, 0, 0, 0, 0, 0)
        { }

        public ArMixVertex(ArIntVector3 position, ArFloatVector4 color, ArFloatVector2 texCrood)
            : this(position.X, position.Y, position.Z, color[0], color[1], color[2], color[3], texCrood[0], texCrood[1])
        { }

        public ArMixVertex(ArIntVector3 position, Color color)
            : this(position.X, position.Y, position.Z, color.ToArFloatVector4())
        { }

        public ArMixVertex(ArIntVector3 position, ArFloatVector2 texCrood)
            : this(position.X, position.Y, position.Z, 0, 0, 0, 0, texCrood[0], texCrood[1])
        { }


        public ArMixVertex(int x, int y, int z, float red, float green, float blue, float alpha, float u, float v)
        {
            Position = new ArIntVector3(x, y, z);
            Color = new ArFloatVector4(red, green, blue, alpha);
            TextureCroodinate = new ArFloatVector2(u, v);
        }

        public ArMixVertex(int x, int y, int z, ArFloatVector4 color)
            : this(x, y, z, color[0], color[1], color[2], color[3], 0, 0)
        { }

        public ArMixVertex(int x, int y, int z, Color color)
            : this(x, y, z, color.ToArFloatVector4())
        { }

        public ArMixVertex(int x, int y, int z)
            : this(x, y, z, ArFloatVector4.Zero)
        { }

        public ArMixVertex(int x, int y, int z, float u, float v)
           : this(x, y, z, 0, 0, 0, 0, u, v)
        { }
    }
}
