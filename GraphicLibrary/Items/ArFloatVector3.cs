using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace GraphicLibrary.Items
{
    //DirectX Shader Compatible Float Vector3
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct ArFloatVector3 : IEquatable<ArFloatVector3>, IFormattable
    {
        float _x, _y, _z;
        public static ArFloatVector3 Zero { get => new ArFloatVector3(); }
        public static ArFloatVector3 One { get => new ArFloatVector3(1, 1, 1); }
        public static ArFloatVector3 UnitX { get => new ArFloatVector3(1, 0, 0); }
        public static ArFloatVector3 UnitY { get => new ArFloatVector3(0, 1, 0); }
        public static ArFloatVector3 UnitZ { get => new ArFloatVector3(0, 0, 1); }
        public ArFloatVector3()
        { }

        public ArFloatVector3(float x, float y, float z)
        {
            _x = x;
            _y = y;
            _z = z;
        }

        public float this[int index]
        {
            get => index switch { 0 => _x, 1 => _y, 2 => _z, _ => throw new IndexOutOfRangeException(nameof(index)) };
            set
            {
                switch (index)
                {
                    case 0:
                        _x = value;
                        break;
                    case 1:
                        _y = value;
                        break;
                    case 2:
                        _z = value;
                        break;                    
                    default:
                        throw new IndexOutOfRangeException(nameof(index));
                }
            }
        }
        public float X { get => _x; set => _x = value; }
        public float Y { get => _y; set => _y = value; }
        public float Z { get => _z; set => _z = value; }
        public static ArFloatVector3 operator +(ArFloatVector3 a, ArFloatVector3 b)
            => new ArFloatVector3(a._x + b._x, a._y + b._y, a._z + b._z);
        public static ArFloatVector3 operator -(ArFloatVector3 a, ArFloatVector3 b)
            => new ArFloatVector3(a._x - b._x, a._y - b._y, a._z - b._z);
        public static ArFloatVector3 operator *(ArFloatVector3 a, double b)
            => new ArFloatVector3((float)(a._x * b), (float)(a._y * b), (float)(a._z * b));
        public static ArFloatVector3 operator *(ArFloatVector3 a, int b)
            => new ArFloatVector3(a._x * b, a._y * b, a._z * b);
        public static ArFloatVector3 operator /(ArFloatVector3 a, double b)
            => new ArFloatVector3((float)(a._x / b), (float)(a._y / b), (float)(a._z / b));
        public static ArFloatVector3 operator /(ArFloatVector3 a, int b)
            => new ArFloatVector3(a._x / b, a._y / b, a._z / b);
        public static bool operator ==(ArFloatVector3 a, ArFloatVector3 b)
            => a.Equals(b);
        public static bool operator !=(ArFloatVector3 a, ArFloatVector3 b)
            => !a.Equals(b);

        public ArFloatVector3 CrossProduct(ArFloatVector3 a)
            => new ArFloatVector3(_y * a._z - _z * a._y,
                _z * a._x - _x * a._z,
                _x * a._y - _y * a._x);
        public float DotProduct(ArFloatVector3 a)
            => _x * a._x + _y * a._y + _z * a._z;
        public double GetLength()
            => Math.Sqrt(_x * _x + _y * _y + _z * _z);
        public override string ToString()
            => $"({_x}, {_y}, {_z})";
        public bool Equals(ArFloatVector3 other)
            => _x == other._x && _y == other._y && _z == other._z;
        public string ToString(string? format, IFormatProvider? formatProvider)
            => string.Format(formatProvider, "({0}, {1}, {2})", _x, _y, _z);
        public override int GetHashCode()
            => (_x,  _y,  _z).GetHashCode();
        public override bool Equals(object obj)
            => obj is ArFloatVector3 && Equals((ArFloatVector3)obj);
    }
}
