using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace GraphicLibrary2.Items
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct ArLight
    {
        public ArIntVector3 Position { get; set; }
        public ArFloatVector3 Direction { get; set; }
        public ArFloatVector3 LightColor { get; set; }
        public float Slope { get; set; }
        public ArLightType LightType { get; set; }

        public ArLight(ArLightType lightType, ArFloatVector3 lightColor, ArIntVector3? position = null, ArFloatVector3? direction = null, float slope = 0.5f)
        {
            Position = position ?? new ArIntVector3();
            Direction = direction ?? new ArFloatVector3();
            LightColor = lightColor;
            Slope = slope;
            LightType = lightType;
        }
    }

    public enum ArLightType
    {
        Ambient = 0,
        Point,
        Directional,
        SpotLight,
    }
}
