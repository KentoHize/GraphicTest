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
        public static (ArTextureVertex[] vertices, int[] indices) GetTextureSphere(int raidus = 1, int sliceCount = 12)
        {

            return new(null, null);

        }
        //public static (ArTextureVertex[] vertices, int[] indices) GetTextureIcosahedron(int raidus = 1)
        //{


        //}

        public static int[] GetTriangleFromPolygon(ArTextureVertex[] vertices)
        {
            if (vertices.Length < 3)
                throw new ArgumentOutOfRangeException(nameof(vertices));
            else if (vertices.Length == 3)
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

        public static (ArMixVertex[] vertices, int[] indices) GetMixCube(int size = 1)
        {
            ArMixVertex[] vertices = new ArMixVertex[8];

            vertices[0] = new ArMixVertex(0, 0, 0, 1, 0, 0, 1, 0, 1);
            vertices[1] = new ArMixVertex(0, 1, 0, 0, 1, 0, 1, 0, 0);
            vertices[2] = new ArMixVertex(1, 1, 0, 0, 0, 1, 1, 1, 0);
            vertices[3] = new ArMixVertex(1, 0, 0, 1, 1, 0, 1, 1, 1);

            vertices[4] = new ArMixVertex(0, 0, 1, 0, 1, 1, 1, 1, 1);
            vertices[5] = new ArMixVertex(0, 1, 1, 1, 0, 1, 1, 1, 0);
            vertices[6] = new ArMixVertex(1, 1, 1, 1, 1, 1, 1, 0, 0);
            vertices[7] = new ArMixVertex(1, 0, 1, 0, 0, 0, 1, 0, 1);

            int[] indices = new int[]
            { 2, 1, 0, 3, 2, 0,
              1, 5, 4, 1, 4, 0,
              5, 6, 7, 5, 7, 4,
              2, 3, 7, 2, 7, 6,
              0, 4 ,3, 4, 7, 3,
              1, 2, 5, 2, 6, 5
            };

            if (size != 1)
                for (int i = 0; i < vertices.Length; i++)
                    vertices[i] = new ArMixVertex(vertices[i].Position[0] * size, vertices[i].Position[1] * size, vertices[i].Position[2] * size, vertices[i].Color);
            return (vertices, indices);
        }

        public static (ArTextureVertex[] vertices, int[] indices) GetTextureCube(int size = 1)
        {
            ArTextureVertex[] vertices = new ArTextureVertex[]
            {
                new ArTextureVertex(0, 1, 1, 0, 1),
                new ArTextureVertex(1, 1, 1, 0, 0),
                new ArTextureVertex(1, 1, 0, 1, 0),
                new ArTextureVertex(0, 1, 0, 1, 1),

                new ArTextureVertex(0, 0, 1, 0, 1),
                new ArTextureVertex(1, 0, 1, 0, 0),
                new ArTextureVertex(1, 0, 0, 1, 0),
                new ArTextureVertex(0, 0, 0, 1, 1),

                new ArTextureVertex(0, 0, 1, 0, 1),
                new ArTextureVertex(0, 1, 1, 0, 0),
                new ArTextureVertex(0, 1, 0, 1, 0),
                new ArTextureVertex(0, 0, 0, 1, 1),

                new ArTextureVertex(1, 0, 1, 0, 1),
                new ArTextureVertex(1, 1, 1, 0, 0),
                new ArTextureVertex(1, 1, 0, 1, 0),
                new ArTextureVertex(1, 0, 0, 1, 1),

                new ArTextureVertex(0, 1, 1, 0, 1),
                new ArTextureVertex(1, 1, 1, 0, 0),
                new ArTextureVertex(1, 0, 1, 1, 0),
                new ArTextureVertex(0, 0, 1, 1, 1),

                new ArTextureVertex(0, 1, 0, 0, 1),
                new ArTextureVertex(1, 1, 0, 0, 0),
                new ArTextureVertex(1, 0, 0, 1, 0),
                new ArTextureVertex(0, 0, 0, 1, 1),
            };

            int[] indices = new int[]
            {
                0,1,2,0,2,3,
                4,6,5,4,7,6,
                8,9,10,8,10,11,
                12,14,13,12,15,14,
                16,18,17,16,19,18,
                20,21,22,20,22,23
            };

            if (size != 1)
                for (int i = 0; i < vertices.Length; i++)
                    vertices[i] = new ArTextureVertex(vertices[i].Position[0] * size, vertices[i].Position[1] * size, vertices[i].Position[2] * size, vertices[i].TextureCroodinate);
            return (vertices, indices);
        }

        public static (ArTextureVertex[] vertices, int[] indices) Get8VerticesCube(int size = 1)
        {
            ArTextureVertex[] vertices = new ArTextureVertex[8];

            vertices[0] = new ArTextureVertex(0, 0, 0, 0, 1);
            vertices[1] = new ArTextureVertex(0, 1, 0, 0, 0);
            vertices[2] = new ArTextureVertex(1, 1, 0, 1, 0);
            vertices[3] = new ArTextureVertex(1, 0, 0, 1, 1);

            vertices[4] = new ArTextureVertex(0, 0, 1, 1, 1);
            vertices[5] = new ArTextureVertex(0, 1, 1, 0, 1);
            vertices[6] = new ArTextureVertex(1, 1, 1, 0, 0);
            vertices[7] = new ArTextureVertex(1, 0, 1, 1, 0);

            int[] indices = new int[]
            { 2, 1, 0, 3, 2, 0,
              1, 5, 4, 1, 4, 0,
              5, 6, 7, 5, 7, 4,
              2, 3, 7, 2, 7, 6,
              0, 4 ,3, 4, 7, 3,
              1, 2, 5, 2, 6, 5
            };

            if (size != 1)
                for (int i = 0; i < vertices.Length; i++)
                    vertices[i] = new ArTextureVertex(vertices[i].Position[0] * size, vertices[i].Position[1] * size, vertices[i].Position[2] * size, vertices[i].TextureCroodinate);
            return (vertices, indices);
        }
    }
}
