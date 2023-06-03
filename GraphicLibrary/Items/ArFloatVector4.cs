using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphicLibrary.Items
{
    //DirectX Shader Compatible Float Vector4
    public struct ArFloatVector4 : IEquatable<ArFloatVector4>, IFormattable
    {
        float[] _data = new float[4];
        public static ArFloatVector4 Zero { get => new ArFloatVector4(); }
        public static ArFloatVector4 One { get => new ArFloatVector4(1, 1, 1, 1); }
        public static ArFloatVector4 UnitW { get => new ArFloatVector4(1, 0, 0, 0); }
        public static ArFloatVector4 UnitX { get => new ArFloatVector4(0, 1, 0, 0); }
        public static ArFloatVector4 UnitY { get => new ArFloatVector4(0, 0, 1 ,0); }
        public static ArFloatVector4 UnitZ { get => new ArFloatVector4(0, 0, 0, 1); }
        public ArFloatVector4()
        { }

        public ArFloatVector4(float w, float x, float y, float z)
        {
            _data[0] = w;
            _data[1] = x;
            _data[2] = y;
            _data[3] = z;
        }

        public float this[int index]
        {
            get => _data[index];
            set => _data[index] = value;
        }
        public float W { get => _data[0]; set => _data[0] = value; }
        public float X { get => _data[1]; set => _data[1] = value; }
        public float Y { get => _data[2]; set => _data[2] = value; }
        public float Z { get => _data[3]; set => _data[3] = value; }
        public static ArFloatVector4 operator +(ArFloatVector4 a, ArFloatVector4 b)
            => new ArFloatVector4(a._data[0] + b._data[0], a._data[1] + b._data[1], a._data[2] + b._data[2], a._data[3] + b._data[3]);
        public static ArFloatVector4 operator -(ArFloatVector4 a, ArFloatVector4 b)
            => new ArFloatVector4(a._data[0] - b._data[0], a._data[1] - b._data[1], a._data[2] - b._data[2], a._data[3] - b._data[3]);
        public static ArFloatVector4 operator *(ArFloatVector4 a, int b)
            => new ArFloatVector4(a._data[0] * b, a._data[1] * b, a._data[2] * b, a._data[3] * b);
        public static ArFloatVector4 operator /(ArFloatVector4 a, int b)
            => new ArFloatVector4(a._data[0] / b, a._data[1] / b, a._data[2] / b, a._data[3] / b);
        public static bool operator ==(ArFloatVector4 a, ArFloatVector4 b)
            => a.Equals(b);
        public static bool operator !=(ArFloatVector4 a, ArFloatVector4 b)
            => !a.Equals(b);
        public override string ToString()
            => $"({_data[0]}, {_data[1]}, {_data[2]}, {_data[3]})";
        public bool Equals(ArFloatVector4 other)
            => _data[0] == other._data[0] && _data[1] == other._data[1] && _data[2] == other._data[2] && _data[3] == other._data[3];
        public string ToString(string? format, IFormatProvider? formatProvider)
            => string.Format(formatProvider, "({0}, {1}, {2}, {3})", _data[0], _data[1], _data[2], _data[3]);
        public override int GetHashCode()
            => (_data[0], _data[1], _data[2], _data[3]).GetHashCode();
        public override bool Equals(object obj)
            => obj is ArFloatVector4 && Equals((ArFloatVector4)obj);
    }
}
