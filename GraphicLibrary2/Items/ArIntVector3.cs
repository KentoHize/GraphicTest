using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.InteropServices;

namespace GraphicLibrary2.Items
{
    //DirectX Compatible Int Vector3
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct ArIntVector3 : IDisposable, ICloneable, IEqualityOperators<ArIntVector3, ArIntVector3, bool>, IComparable<ArIntVector3>, IEquatable<ArIntVector3>, IAdditionOperators<ArIntVector3, ArIntVector3, ArIntVector3>,
        ISubtractionOperators<ArIntVector3, ArIntVector3, ArIntVector3>, IMultiplyOperators<ArIntVector3, ArIntVector3, float>, IUnaryNegationOperators<ArIntVector3, ArIntVector3>,
        IParsable<ArIntVector3>
    {
        int _x, _y, _z;
        public static ArIntVector3 Zero => new ArIntVector3();
        public static ArIntVector3 One => new ArIntVector3(1, 1, 1);
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

        public ArIntVector3(long x, long y, long z)
           : this((int)x, (int)y, (int)z)
        { }

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

        public void Dispose()
            => Dispose();
        public object Clone()
            => new ArIntVector3(_x, _y, _z);
        public override string ToString()
            => ToString("G");
        public string ToString(string format)
            => $"({_x.ToString(format)}, {_y.ToString(format)}, {_z.ToString(format)})";
        public bool Equals(ArIntVector3 other)
            => _x == other._x && _y == other._y && _z == other._z;
        public int CompareTo(ArIntVector3 other)
            => Equals(other) ? 0 : _x > other._x ? 1 : _x < other._x ? -1 : _y > other._y ? 1 : _y < other._y ? -1 : _z > other._z ? 1 : -1;
        public static bool operator ==(ArIntVector3 left, ArIntVector3 right)
            => left.Equals(right);
        public static bool operator !=(ArIntVector3 left, ArIntVector3 right)
            => !(left == right);
        public static ArIntVector3 operator +(ArIntVector3 left, ArIntVector3 right)
            => new ArIntVector3(left._x + right._x, left._y + right._y, left._z + right._z);
        public static ArIntVector3 operator -(ArIntVector3 left, ArIntVector3 right)
            => new ArIntVector3(left._x - right._x, left._y - right._y, left._z - right._z);
        public static ArIntVector3 operator *(ArIntVector3 a, int b)
            => new ArIntVector3(a._x * b, a._y * b, a._z * b);
        public static ArIntVector3 operator /(ArIntVector3 a, double b)
            => new ArIntVector3((int)(a._x / b), (int)(a._y / b), (int)(a._z / b));
        public double GetLength() => Math.Sqrt(_x * _x + _y * _y + _z * _z);

        public double AngleBetween(ArIntVector3 a)
            => Math.Acos(DotProduct(a) / (GetLength() * a.GetLength()));
        public ArIntVector3 Normalize()
        {
            double l = GetLength();
            return new ArIntVector3((int)(_x / l), (int)(_y / l), (int)(_z / l));
        }

        public static float operator *(ArIntVector3 left, ArIntVector3 right)
            => left.DotProduct(right);
        public ArIntVector3 CrossProduct(ArIntVector3 a)
            => new ArIntVector3(_y * a._z - _z * a._y,
                _z * a._x - _x * a._z,
                _x * a._y - _y * a._x);
        public float DotProduct(ArIntVector3 a)
            => _x * a._x + _y * a._y + _z * a._z;
        public override bool Equals(object? obj)
        {
            if (!(obj is ArIntVector3))
                return false;
            return Equals((ArIntVector3)obj);
        }
        public override int GetHashCode()
            => (_x, _y, _z).GetHashCode();

        /// <summary>
        /// 將字串轉為ArIntVector3
        /// </summary>
        /// <param name="s">(x,y,z)或x,y,z</param>
        /// <param name="provider">Null</param>
        /// <returns>值</returns>
        public static ArIntVector3 Parse(string s, IFormatProvider? provider = null)
        {
            string[] n;
            if (s.StartsWith('('))
                n = s.Substring(1, s.Length - 2).Replace(" ", "").Split(',');
            else
                n = s.Replace(" ", "").Split(',');
            return new ArIntVector3(int.Parse(n[0]), int.Parse(n[1]), int.Parse(n[2]));
        }

        public static bool TryParse([NotNullWhen(true)] string? s, [MaybeNullWhen(false)] out ArIntVector3 result)
            => TryParse(s, null, out result);

        public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out ArIntVector3 result)
        {
            try
            {
                result = Parse(s, provider);
                return true;
            }
            catch
            {
                result = default;
                return false;
            }
        }

        public static bool operator <(ArIntVector3 left, ArIntVector3 right)
            => left.CompareTo(right) < 0;
        public static bool operator <=(ArIntVector3 left, ArIntVector3 right)
            => left.CompareTo(right) <= 0;
        public static bool operator >(ArIntVector3 left, ArIntVector3 right)
            => left.CompareTo(right) > 0;
        public static bool operator >=(ArIntVector3 left, ArIntVector3 right)
            => left.CompareTo(right) >= 0;
        public static ArIntVector3 operator -(ArIntVector3 value)
            => new ArIntVector3(value._x * -1, value._y * -1, value._z * -1);

        public static explicit operator ArIntVector3(ArFloatVector2 a)
            => new ArIntVector3((int)a[0], (int)a[1], 0);
        public static explicit operator ArIntVector3(ArFloatVector3 a)
            => new ArIntVector3((int)a[0], (int)a[1], (int)a[2]);
        public static explicit operator ArIntVector3(ArFloatVector4 a)
            => new ArIntVector3((int)a[0], (int)a[1], (int)a[2]);
    }
}
