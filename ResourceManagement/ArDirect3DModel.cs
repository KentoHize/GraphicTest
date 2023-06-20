using GraphicLibrary.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ResourceManagement
{
    //[StructLayout(LayoutKind.Explicit, Size = 36, CharSet = CharSet.Ansi)]
    [StructLayout(LayoutKind.Sequential)]
    public class ArDirect3DVertex
    {
        //[FieldOffset(0)]
        public ArIntVector3 _position;
        //[FieldOffset(12)]
        public ArFloatVector3 _textureCoordinate;
        //[FieldOffset(24)]
        public ArFloatVector3 _shadowCoordinate;
        public ArIntVector3 Position { get => _position; set => _position = value; }        
        public ArFloatVector3 TextureCoordinate { get => _textureCoordinate; set => _textureCoordinate = value; }
        public ArFloatVector3 ShadowCoordinate { get => _shadowCoordinate; set => _shadowCoordinate = value; }
    }

    [StructLayout(LayoutKind.Sequential)]
    public class ArDirect3DModel
    {
        public ArDirect3DVertex[] _vertices;
        public int[] _indices;
        public ArDirect3DVertex[] Vertices { get => _vertices; set => _vertices = value; }
        public int[] Indices { get => _indices; set => _indices = value; }

    }
}
