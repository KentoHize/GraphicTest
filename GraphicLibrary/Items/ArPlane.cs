using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphicLibrary.Items
{
    public abstract class ArPlane
    {   
        public abstract bool IsPlane { get; }
        public abstract bool IsLine { get;  }
        public abstract bool IsPoint { get; }
    }
}
