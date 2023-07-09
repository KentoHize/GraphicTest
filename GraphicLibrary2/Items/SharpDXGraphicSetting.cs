using SharpDX.Direct3D12;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace GraphicLibrary2.Items
{
    public class SharpDXGraphicSetting
    {
        public bool CullTwoFace { get; set; }
        public FillMode FillMode { get; set; }
        public bool DrawClockwise { get; set; }
        public bool MultisampleEnabled { get; set; }
        public bool IsBlendEnabled { get; set; }

        public SharpDXGraphicSetting(bool cullTwoFace = false, FillMode fillMode = FillMode.Solid, 
            bool drawClockwise = false, bool multisampleEnabled = false, bool isBlendEnabled = true)
        {
            CullTwoFace = cullTwoFace;
            FillMode = fillMode;
            DrawClockwise = drawClockwise;
            MultisampleEnabled = multisampleEnabled;
            IsBlendEnabled = isBlendEnabled;
        }
    }

    public enum FillMode
    {
        Wireframe = 2,
        Solid = 3
    }

}
