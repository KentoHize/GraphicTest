using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GraphicTest
{
    public struct ArrVertex
    {
        public ArrVertex() { }
        public ArrVertex(Vector3 position, Vector4 color)
        {
            //Position = position;
            //X = (int)position.X;
            //Y = (int)position.Y;
            //Z = (int)position.Z;
            //Color1 = 0;
            Color2 = color;
        }
        //public int X;
        //public int Y;
        //public int Z;

        //private int _a = 0;
        //public int a { get => _a; }
        public Vector3 Position { get; set; }
        public Vector4 Color2 { get; set; }        
        //public Color Color { get; set; }
    }
}
