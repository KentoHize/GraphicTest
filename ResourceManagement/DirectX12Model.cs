﻿using SharpDX.Direct3D12;

namespace ResourceManagement
{
    public class DirectX12Model
    {
        public bool Base { get; set; }
        public SharpDX.Direct3D.PrimitiveTopology PrimitiveTopology { get; set; }
        public int IndicesCount { get; set; }
        public Resource VertexBuffer { get; set; }
        public Resource IndexBuffer { get; set; }
        public IndexBufferView IndexBufferView { get; set; }
        public VertexBufferView VertexBufferView { get; set; }


        //public int[] MaterialIndices { get; set; }

        //public ArFloatMatrix44 TransformMatrix { get; set; }
        //必要Bytes
        //必要Texture
        //Shade數

    }


}
