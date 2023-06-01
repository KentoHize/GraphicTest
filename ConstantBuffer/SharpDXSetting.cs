using SharpDX.Direct3D12;
using SharpDX.DXGI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConstantBuffer
{
    public class SharpDXSetting
    {
        public IntPtr Handle { get; set; }
        public SwapEffect SwapEffect { get; set; } = SwapEffect.FlipDiscard;
        public Viewport Viewport { get; set; }
        public bool CullTwoFace { get; set; } = false;
    }
}
