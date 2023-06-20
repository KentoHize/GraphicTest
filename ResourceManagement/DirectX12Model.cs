using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX.Direct3D12;

namespace ResourceManagement
{
    public class DirectX12Model
    {
        public int VertericesCount { get; set; }
        public Resource VertexBuffer { get; set; }
        public Resource IndexBuffer { get; set; }
        public IndexBufferView IndexBufferView { get; set; }
        public VertexBufferView VertexBufferView { get; set; }
        

        //必要Bytes
        //必要Texture
        //Shade數

    }
}
