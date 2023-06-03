using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphicLibrary.Items
{
    //DirectX Shader Compatible Int Vector3
    public struct ArIntVector3 : IEquatable<ArIntVector3>, IFormattable
    {
        int[] _data = new int[3];
        public static ArIntVector3 Zero { get => new ArIntVector3(); }
        public static ArIntVector3 One { get => new ArIntVector3(1, 1, 1); }
        public static ArIntVector3 UnitX { get => new ArIntVector3(1, 0, 0); }
        public static ArIntVector3 UnitY { get => new ArIntVector3(0, 1, 0); }
        public static ArIntVector3 UnitZ { get => new ArIntVector3(0, 0, 1); }
        public ArIntVector3()
        { }

        public ArIntVector3(int x, int y, int z)
        {
            _data[0] = x;
            _data[1] = y;
            _data[2] = z;
        }

        public int this[int index]
        {
            get => _data[index];
            set => _data[index] = value;
        }
        public int X { get => _data[0]; set => _data[0] = value; }
        public int Y { get => _data[1]; set => _data[1] = value; }
        public int Z { get => _data[2]; set => _data[2] = value; }
        public static ArIntVector3 operator +(ArIntVector3 a, ArIntVector3 b)
            => new ArIntVector3(a._data[0] + b._data[0], a._data[1] + b._data[1], a._data[2] + b._data[2]);
        public static ArIntVector3 operator -(ArIntVector3 a, ArIntVector3 b)
            => new ArIntVector3(a._data[0] - b._data[0], a._data[1] - b._data[1], a._data[2] - b._data[2]);
        public static ArIntVector3 operator *(ArIntVector3 a, double b)
            => new ArIntVector3((int)(a._data[0] * b), (int)(a._data[1] * b), (int)(a._data[2] * b));
        public static ArIntVector3 operator *(ArIntVector3 a, int b)
            => new ArIntVector3(a._data[0] * b, a._data[1] * b, a._data[2] * b);
        public static ArIntVector3 operator /(ArIntVector3 a, double b)
            => new ArIntVector3((int)(a._data[0] / b), (int)(a._data[1] / b), (int)(a._data[2] / b));
        public static ArIntVector3 operator /(ArIntVector3 a, int b)
            => new ArIntVector3(a._data[0] / b, a._data[1] / b, a._data[2] / b);
        public static bool operator ==(ArIntVector3 a, ArIntVector3 b)
            => a.Equals(b);
        public static bool operator !=(ArIntVector3 a, ArIntVector3 b)
            => !a.Equals(b);

        public ArIntVector3 CrossProduct(ArIntVector3 a)
            => new ArIntVector3(_data[1] * a._data[2] - _data[2] * a._data[1],
                _data[2] * a._data[0] - _data[0] * a._data[2],
                _data[0] * a._data[1] - _data[1] * a._data[0]);
        public long DotProduct(ArIntVector3 a)
            => _data[0] * a._data[0] + _data[1] * a._data[1] + _data[2] * a._data[2];
        public double GetLength()
            => Math.Sqrt(_data[0] * _data[0] + _data[1] * _data[1] + _data[2] * _data[2]);
        public override string ToString()
            => $"({_data[0]}, {_data[1]}, {_data[2]})";
        public bool Equals(ArIntVector3 other)
            => _data[0] == other._data[0] && _data[1] == other._data[1] && _data[2] == other._data[2];
        public string ToString(string? format, IFormatProvider? formatProvider)
            => string.Format(formatProvider, "({0}, {1}, {2})", _data[0], _data[1], _data[2]);
        public override int GetHashCode()
            => (_data[0], _data[1], _data[2]).GetHashCode();
        public override bool Equals(object obj)
            => obj is ArIntVector3 && Equals((ArIntVector3)obj);
    }
}
