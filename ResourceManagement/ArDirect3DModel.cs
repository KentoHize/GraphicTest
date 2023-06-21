using GraphicLibrary.Items;
using SharpDX.DirectWrite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

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
        public ArDirect3DVertex[] Vertices { get => _vertices; set => _vertices = value; }
        public int[] Indices { get => _indices; set => _indices = value; }        
        public int[] MaterialIndices { get; set; }

    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ArMaterial
    {   
        public int TextureIndex { get; set; }


        //float4 DiffuseAlbedo;
        //float3 FresnelR0;
        //float Roughness;
    }
}
