using GraphicLibrary.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace GraphicLibrary
{
    public static class Extensions
    {
        public static ArFloatVector4 ToArFloatVector4(this Color a)
            => new ArFloatVector4((float)a.R / 255, (float)a.G / 255, (float)a.B / 255, (float)a.A / 255);
    }
}
