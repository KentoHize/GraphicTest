using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace GraphicLibrary2.Items
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct ArCamera
    {
        public ArIntVector3 Position { get; set; }        
        public ArFloatVector3 Direction { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int Depth { get; set; }        
        public float Slope { get; set; }
        public bool IsPerspective { get; set; }
        public ArCamera(int width, int height, int depth, ArIntVector3? position = null, ArFloatVector3? direction = null, bool isPerspective = false, float slope = 0.5f)
        {
            Position = position ?? new ArIntVector3();
            Direction = direction ?? new ArFloatVector3();
            Width = width;
            Height = height;
            Depth = depth;
            Slope = slope;
            IsPerspective = isPerspective;
        }
    }
}
