using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphicLibrary.Items
{
    //DirectX Shader Compatible Int Vector3
    public struct ArFloatVector3 : IEquatable<ArFloatVector3>, IFormattable
    {
        float[] _data = new float[3];
        public static ArFloatVector3 Zero { get => new ArFloatVector3(); }
        public static ArFloatVector3 One { get => new ArFloatVector3(1, 1, 1); }
        public static ArFloatVector3 UnitX { get => new ArFloatVector3(1, 0, 0); }
        public static ArFloatVector3 UnitY { get => new ArFloatVector3(0, 1, 0); }
        public static ArFloatVector3 UnitZ { get => new ArFloatVector3(0, 0, 1); }
        public ArFloatVector3()
        { }

        public ArFloatVector3(float x, float y, float z)
        {
            _data[0] = x;
            _data[1] = y;
            _data[2] = z;
        }

        public float this[int index]
        {
            get => _data[index];
            set => _data[index] = value;
        }
        public float X { get => _data[0]; set => _data[0] = value; }
        public float Y { get => _data[1]; set => _data[1] = value; }
        public float Z { get => _data[2]; set => _data[2] = value; }
        public static ArFloatVector3 operator +(ArFloatVector3 a, ArFloatVector3 b)
            => new ArFloatVector3(a._data[0] + b._data[0], a._data[1] + b._data[1], a._data[2] + b._data[2]);
        public static ArFloatVector3 operator -(ArFloatVector3 a, ArFloatVector3 b)
            => new ArFloatVector3(a._data[0] - b._data[0], a._data[1] - b._data[1], a._data[2] - b._data[2]);
        public static ArFloatVector3 operator *(ArFloatVector3 a, double b)
            => new ArFloatVector3((float)(a._data[0] * b), (float)(a._data[1] * b), (float)(a._data[2] * b));
        public static ArFloatVector3 operator *(ArFloatVector3 a, int b)
            => new ArFloatVector3(a._data[0] * b, a._data[1] * b, a._data[2] * b);
        public static ArFloatVector3 operator /(ArFloatVector3 a, double b)
            => new ArFloatVector3((float)(a._data[0] / b), (float)(a._data[1] / b), (float)(a._data[2] / b));
        public static ArFloatVector3 operator /(ArFloatVector3 a, int b)
            => new ArFloatVector3(a._data[0] / b, a._data[1] / b, a._data[2] / b);
        public static bool operator ==(ArFloatVector3 a, ArFloatVector3 b)
            => a.Equals(b);
        public static bool operator !=(ArFloatVector3 a, ArFloatVector3 b)
            => !a.Equals(b);

        public ArFloatVector3 CrossProduct(ArFloatVector3 a)
            => new ArFloatVector3(_data[1] * a._data[2] - _data[2] * a._data[1],
                _data[2] * a._data[0] - _data[0] * a._data[2],
                _data[0] * a._data[1] - _data[1] * a._data[0]);
        public float DotProduct(ArFloatVector3 a)
            => _data[0] * a._data[0] + _data[1] * a._data[1] + _data[2] * a._data[2];
        public double GetLength()
            => Math.Sqrt(_data[0] * _data[0] + _data[1] * _data[1] + _data[2] * _data[2]);
        public override string ToString()
            => $"({_data[0]}, {_data[1]}, {_data[2]})";
        public bool Equals(ArFloatVector3 other)
            => _data[0] == other._data[0] && _data[1] == other._data[1] && _data[2] == other._data[2];
        public string ToString(string? format, IFormatProvider? formatProvider)
            => string.Format(formatProvider, "({0}, {1}, {2})", _data[0], _data[1], _data[2]);
        public override int GetHashCode()
            => (_data[0],  _data[1],  _data[2]).GetHashCode();
        public override bool Equals(object obj)
            => obj is ArFloatVector3 && Equals((ArFloatVector3)obj);
    }
}
