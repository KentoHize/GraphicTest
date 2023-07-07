using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace GraphicLibrary2.Items
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct ArMaterial
    {   
        public ArFloatVector3 Albedo { get; set; } //反射
        public float Opacity { get; set; } //不透明度
        public float FresnelR0 { get; set; } //can be controled by roughness
        public float Roughness {get; set;}
        public int TextureMapIndex { get; set; }
        public int NormalMapIndex { get; set; }
        public ArMaterial(float roughness = 1, int textureMapIndex = -1, float opacity = 1, float fresnelR0 = 1, ArFloatVector3? albedo = null, int normalMapIndex = -1)
        {
            Albedo = albedo ?? ArFloatVector3.One;
            Opacity = opacity;
            FresnelR0 = fresnelR0;
            Roughness = roughness;
            TextureMapIndex = textureMapIndex;
            NormalMapIndex = normalMapIndex;
        }
    }
}
