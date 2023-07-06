using System.Runtime.InteropServices;

namespace GraphicLibrary.Items
{
    //DirectX Shader Compatible Int Vector3
    //[StructLayout(LayoutKind.Explicit, Size = 12, CharSet = CharSet.Ansi)]
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    //[Serializable]
    public struct ArIntVector3 : IEquatable<ArIntVector3>, IFormattable
    {
        public int _x, _y, _z;
        //[FieldOffset(0)] int _x;
        //[FieldOffset(4)] int _y;
        //[FieldOffset(8)] int _z;
        public static ArIntVector3 Zero { get => new ArIntVector3(); }
        public static ArIntVector3 One { get => new ArIntVector3(1, 1, 1); }
        public static ArIntVector3 UnitX { get => new ArIntVector3(1, 0, 0); }
        public static ArIntVector3 UnitY { get => new ArIntVector3(0, 1, 0); }
        public static ArIntVector3 UnitZ { get => new ArIntVector3(0, 0, 1); }
        public ArIntVector3()
        { }

        public ArIntVector3(int x, int y, int z)
        {
            _x = x;
            _y = y;
            _z = z;
        }

        public int this[int index]
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
        public int X { get => _x; set => _x = value; }
        public int Y { get => _y; set => _y = value; }
        public int Z { get => _z; set => _z = value; }
        public static ArIntVector3 operator +(ArIntVector3 a, ArIntVector3 b)
            => new ArIntVector3(a._x + b._x, a._y + b._y, a._z + b._z);
        public static ArIntVector3 operator -(ArIntVector3 a, ArIntVector3 b)
            => new ArIntVector3(a._x - b._x, a._y - b._y, a._z - b._z);
        public static ArIntVector3 operator *(ArIntVector3 a, double b)
            => new ArIntVector3((int)(a._x * b), (int)(a._y * b), (int)(a._z * b));
        public static ArIntVector3 operator *(ArIntVector3 a, int b)
            => new ArIntVector3(a._x * b, a._y * b, a._z * b);
        public static ArIntVector3 operator /(ArIntVector3 a, double b)
            => new ArIntVector3((int)(a._x / b), (int)(a._y / b), (int)(a._z / b));
        public static ArIntVector3 operator /(ArIntVector3 a, int b)
            => new ArIntVector3(a._x / b, a._y / b, a._z / b);
        public static bool operator ==(ArIntVector3 a, ArIntVector3 b)
            => a.Equals(b);
        public static bool operator !=(ArIntVector3 a, ArIntVector3 b)
            => !a.Equals(b);

        public ArIntVector3 CrossProduct(ArIntVector3 a)
            => new ArIntVector3(_y * a._z - _z * a._y,
                _z * a._x - _x * a._z,
                _x * a._y - _y * a._x);
        public long DotProduct(ArIntVector3 a)
            => _x * a._x + _y * a._y + _z * a._z;
        public double GetLength()
            => Math.Sqrt(_x * _x + _y * _y + _z * _z);
        public ArFloatVector3 Normalize()
        {
            double l = GetLength();
            return new ArFloatVector3((float)(_x / l), (float)(_y / l), (float)(_z / l));
        }
        public override string ToString()
            => $"({_x}, {_y}, {_z})";
        public bool Equals(ArIntVector3 other)
            => _x == other._x && _y == other._y && _z == other._z;
        public string ToString(string? format, IFormatProvider? formatProvider)
            => string.Format(formatProvider, "({0}, {1}, {2})", _x, _y, _z);
        public override int GetHashCode()
            => (_x, _y, _z).GetHashCode();
        public override bool Equals(object obj)
            => obj is ArIntVector3 && Equals((ArIntVector3)obj);

        public static explicit operator ArIntVector3(ArFloatVector2 a)
          => new ArIntVector3((int)a[0], (int)a[1], 0);
        public static explicit operator ArIntVector3(ArFloatVector3 a)
            => new ArIntVector3((int)a[0], (int)a[1], (int)a[2]);
        public static explicit operator ArIntVector3(ArFloatVector4 a)
            => new ArIntVector3((int)a[0], (int)a[1], (int)a[2]);
    }
}
