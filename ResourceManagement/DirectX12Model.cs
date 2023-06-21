using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using GraphicLibrary.Items;
using SharpDX.Direct3D12;

namespace ResourceManagement
{
    public class DirectX12Model
    {
        public bool Base { get; set; }
        public int IndicesCount { get; set; }
        public Resource VertexBuffer { get; set; }
        public Resource IndexBuffer { get; set; }
        public IndexBufferView IndexBufferView { get; set; }
        public VertexBufferView VertexBufferView { get; set; }

        //public ArFloatMatrix44 TransformMatrix { get; set; }
        //必要Bytes
        //必要Texture
        //Shade數

    }
}
