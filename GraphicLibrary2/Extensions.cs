using GraphicLibrary2.Items;
using System.Drawing;

namespace GraphicLibrary2
{
    public static class Extensions
    {
        public static ArFloatVector4 ToArFloatVector4(this Color a)
            => new ArFloatVector4((float)a.R / 255, (float)a.G / 255, (float)a.B / 255, (float)a.A / 255);
    }
}
