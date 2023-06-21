using GraphicLibrary.Items;
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
        int _modelTextureIndex;
        ArFloatVector2 _textureCoordinate;        
        ArFloatVector3 _shadowCoordinate;
        public ArIntVector3 Position { get => _position; set => _position = value; }        
        public int ModelTextureIndex { get => _modelTextureIndex; set => _modelTextureIndex = value; }
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
        public int[] TextureIndices { get; set; }

    }
}
