using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphicLibrary.Items
{
    public class ArPlane
    {
        //目前轉點順序需要正確
        ArVertex[] m_vertices;
        public ArVertex[] Vertices { get => m_vertices; set { if (value.Length > int.MaxValue) throw new IndexOutOfRangeException(); m_vertices = value; } }
        public Stream TextureResource { get; set; }
        public bool IsPlane { get => m_vertices.Length > 2; }
        public bool IsLine { get => m_vertices.Length == 2; }
        public bool IsPoint { get => m_vertices.Length == 1; }
        public ArPlane(List<ArVertex> vertices)
            : this(vertices.ToArray())
        { }
        public ArPlane(ArVertex[] vertices)
        {
            Vertices = vertices;
        }
        public ArPlane() { }
    }
}
