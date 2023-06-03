using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphicLibrary.Items
{
    public class Ar3DModelGroup
    {
        Ar3DModel[] m_models;
        public Ar3DModel[] Models { get => m_models; set { if (value.Length > int.MaxValue) throw new IndexOutOfRangeException(); m_models = value; } }
    }
}
