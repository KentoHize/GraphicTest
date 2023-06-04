using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphicLibrary.Items
{
    public struct ArTextureVertex : IArVertex
    {
        public ArIntVector3 Position { get; set; }
        public ArFloatVector2 TextureCroodinate { get; set; }
    }
}
