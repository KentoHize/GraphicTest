using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace GraphicLibrary.Items
{
    public class Ar3DModel : ICloneable
    {
        ArPlane[] m_planes;
        public ArPlane[] Planes { get => m_planes; set { if (value.Length > int.MaxValue) throw new IndexOutOfRangeException(); m_planes = value; } }

        public Ar3DModel(List<ArPlane> planes)
            : this(planes.ToArray())
        { }
        public Ar3DModel(ArPlane[] planes)
        {
            Planes = planes;
        }

        public Ar3DModel()
        { }

        public object Clone()
        {
            Ar3DModel result = new Ar3DModel();
            if (Planes == null)
                return result;
            
            result.m_planes = (ArPlane[])m_planes.Clone();
            return result;
        }
    }
}
