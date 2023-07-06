using GraphicLibrary.Items;
using System.Drawing;

namespace GraphicLibrary
{
    public static class Extensions
    {
        public static ArFloatVector4 ToArFloatVector4(this Color a)
            => new ArFloatVector4((float)a.R / 255, (float)a.G / 255, (float)a.B / 255, (float)a.A / 255);


        public static ArTextureVertex[] ToArTextureVertices(this ArIntVector3[] geoVertices)
        {
            ArTextureVertex[] result = new ArTextureVertex[geoVertices.Length];
            for (int i = 0; i < geoVertices.Length; i++)
                result[i] = new ArTextureVertex(geoVertices[i], ArFloatVector2.Zero);
            return result;
        }

      
    }
}
