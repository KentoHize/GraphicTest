using GraphicLibrary.Items;
using System.Runtime.InteropServices;

namespace ResourceManagement
{
    [StructLayout(LayoutKind.Sequential)]
    public struct ArDirect3DVertex
    {
        ArIntVector3 _position;
        int _modelMaterialIndex;
        ArFloatVector2 _textureCoordinate;
        ArFloatVector3 _shadowCoordinate;
        public ArIntVector3 Position { get => _position; set => _position = value; }
        public int MaterialIndex { get => _modelMaterialIndex; set => _modelMaterialIndex = value; }
        public ArFloatVector2 TextureCoordinate { get => _textureCoordinate; set => _textureCoordinate = value; }
        public ArFloatVector3 ShadowCoordinate { get => _shadowCoordinate; set => _shadowCoordinate = value; }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ArDirect3DModel
    {
        ArDirect3DVertex[] _vertices;
        int[] _indices;
        PrimitiveTopology _primitiveTopology;
        public ArDirect3DVertex[] Vertices { get => _vertices; set => _vertices = value; }
        public int[] Indices { get => _indices; set => _indices = value; }
        public PrimitiveTopology PrimitiveTopology { get => _primitiveTopology; set => _primitiveTopology = value; }

        public ArDirect3DModel()
        {
            _primitiveTopology = PrimitiveTopology.TriangleList;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ArMaterial
    {
        public int TextureIndex { get; set; }


        //float4 DiffuseAlbedo;
        //float3 FresnelR0;
        //float Roughness;
    }

    public enum PrimitiveTopology
    {
        Undefined = 0,
        PointList = 1,
        LineList = 2,
        LineStrip = 3,
        TriangleList = 4,
        TriangleStrip = 5
    }
}
