using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphicLibrary.Items
{
    public class ArColorPlane : ArPlane
    {
        //目前轉點順序需要正確
        ArColorVertex[] m_vertices;
        public ArColorVertex[] Vertices { get => m_vertices; set { if (value.Length > int.MaxValue) throw new IndexOutOfRangeException(); m_vertices = value; } }
        public override bool IsPlane => m_vertices.Length > 2;
        public override bool IsLine => m_vertices.Length == 2;
        public override bool IsPoint => m_vertices.Length == 1;
        public ArColorPlane(List<ArColorVertex> vertices)
            : this(vertices.ToArray())
        { }
        public ArColorPlane(ArColorVertex[] vertices)
        {
            Vertices = vertices;
        }
        public ArColorPlane() { }
    }
}
