namespace GraphicLibrary.Items
{
    //DirectX Shader Compatible Float Vector4
    public struct ArFloatVector4 : IEquatable<ArFloatVector4>, IFormattable
    {
        float _x, _y, _z, _w;
        public static ArFloatVector4 Zero { get => new ArFloatVector4(); }
        public static ArFloatVector4 One { get => new ArFloatVector4(1, 1, 1, 1); }
        public static ArFloatVector4 UnitW { get => new ArFloatVector4(1, 0, 0, 0); }
        public static ArFloatVector4 UnitX { get => new ArFloatVector4(0, 1, 0, 0); }
        public static ArFloatVector4 UnitY { get => new ArFloatVector4(0, 0, 1, 0); }
        public static ArFloatVector4 UnitZ { get => new ArFloatVector4(0, 0, 0, 1); }
        public ArFloatVector4()
        { }

        public ArFloatVector4(float x, float y, float z, float w)
        {
            _x = x;
            _y = y;
            _z = z;
            _w = w;
        }

        public float this[int index]
        {
            get => index switch { 0 => _x, 1 => _y, 2 => _z, 3 => _w, _ => throw new IndexOutOfRangeException(nameof(index)) };
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
                    case 3:
                        _w = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException(nameof(index));
                }
            }
        }
        public float X { get => _x; set => _x = value; }
        public float Y { get => _y; set => _y = value; }
        public float Z { get => _z; set => _z = value; }
        public float W { get => _w; set => _w = value; }
        public static ArFloatVector4 operator +(ArFloatVector4 a, ArFloatVector4 b)
            => new ArFloatVector4(a._x + b._x, a._y + b._y, a._z + b._z, a._w + b._w);
        public static ArFloatVector4 operator -(ArFloatVector4 a, ArFloatVector4 b)
            => new ArFloatVector4(a._x - b._x, a._y - b._y, a._z - b._z, a._w - b._w);
        public static ArFloatVector4 operator *(ArFloatVector4 a, int b)
            => new ArFloatVector4(a._x * b, a._y * b, a._z * b, a._w * b);
        public static ArFloatVector4 operator /(ArFloatVector4 a, int b)
            => new ArFloatVector4(a._x / b, a._y / b, a._z / b, a._w / b);
        public static bool operator ==(ArFloatVector4 a, ArFloatVector4 b)
            => a.Equals(b);
        public static bool operator !=(ArFloatVector4 a, ArFloatVector4 b)
            => !a.Equals(b);
        public override string ToString()
            => $"({_x}, {_y}, {_z}, {_w})";
        public bool Equals(ArFloatVector4 other)
            => _x == other._x && _y == other._y && _z == other._z && _w == other._w;
        public string ToString(string? format, IFormatProvider? formatProvider)
            => string.Format(formatProvider, "({0}, {1}, {2}, {3})", _x, _y, _z, _w);
        public override int GetHashCode()
            => (_x, _y, _z, _w).GetHashCode();
        public override bool Equals(object obj)
            => obj is ArFloatVector4 && Equals((ArFloatVector4)obj);
    }
}
