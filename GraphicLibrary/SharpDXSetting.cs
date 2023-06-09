﻿using SharpDX;
using SharpDX.DXGI;

namespace GraphicLibrary
{
    public class SharpDXSetting
    {
        public IntPtr Handle { get; set; }
        public SwapEffect SwapEffect { get; set; } = SwapEffect.FlipDiscard;
        public ViewportF Viewport { get; set; }
        public bool CullTwoFace { get; set; } = false;
        public bool DrawClockwise { get; set; } = false;
        public int FrameCount { get; set; } = 2;
    }
}
