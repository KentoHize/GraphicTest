using GraphicLibrary.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace GraphicLibrary
{
    public enum VertexType
    {
        Texture = 0,
        Color
    }

    public static class Ar3DGeometry
    {
        //得到Cube
        //public static Ar3DModel GetCube(int size = 1, VertexType type = VertexType.Texture)
        //{
        //    Ar3DModel model = new Ar3DModel();
        //    if(type == VertexType.Texture)
        //    {
        //        model.Planes = new ArTexturePlane[6];
        //        model.Planes[0] = new ArTexturePlane
        //        {
        //            Vertices = new ArTextureVertex[]
        //            {

        //            }
        //        };
        //        //new ArTexturePlane { }
        //    }
        //    return model;
        //}

        public static int[] GetTriangleFromPolygon(ArTextureVertex[] vertices)
        {
            if (vertices.Length < 3)
                throw new ArgumentOutOfRangeException(nameof(vertices));
            else if(vertices.Length == 3)
                    return new int[] { 0, 1, 2 };
            else
            {
                int[] result = new int[(vertices.Length - 2) * 3];
                int index = 0;
                int k = 0, l = vertices.Length - 1;
                while (l != k + 1)
                {
                    result[index++] = k;
                    result[index++] = l;
                    result[index++] = k + 1;
                    k++;
                    if (k == l - 1)
                        break;
                    result[index++] = l;
                    result[index++] = l - 1;
                    result[index++] = k;
                    l--;
                }
                return result;
            }
        }

        public static (ArTextureVertex[] vertices, int[] indices) GetCube(int size = 1)
        {
            ArTextureVertex[] vertices = new ArTextureVertex[8];
            
            vertices[0] = new ArTextureVertex(0, 0, 0, 0, 1);
            vertices[1] = new ArTextureVertex(0, 1, 0, 0, 0);
            vertices[2] = new ArTextureVertex(1, 1, 0, 1, 0);
            vertices[3] = new ArTextureVertex(1, 0, 0, 1, 1);
            vertices[4] = new ArTextureVertex(0, 0, 1, 0, 1);
            vertices[5] = new ArTextureVertex(0, 1, 1, 0, 0);
            vertices[6] = new ArTextureVertex(1, 1, 1, 1, 0);
            vertices[7] = new ArTextureVertex(1, 0, 1, 1, 1);

            int[] indices = new int[]
            { 2, 1, 0, 3, 2, 0,
              1, 5, 4, 1, 4, 0, 
              5, 6, 7, 5, 7, 4,
              2, 3, 7, 2, 7, 6,
              0, 4 ,3, 4, 7, 3,
              1, 2, 5, 2, 6, 5
            };

            if(size != 1)
                for (int i = 0; i < vertices.Length; i++)
                    vertices[i] = new ArTextureVertex(vertices[i].Position[0] * size, vertices[i].Position[1] * size, vertices[i].Position[2] * size, vertices[i].TextureCroodinate);
            return (vertices, indices);
        }
    }
}
