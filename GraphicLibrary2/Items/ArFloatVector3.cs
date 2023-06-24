using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.InteropServices;

namespace GraphicLibrary2.Items
{
    //DirectX Compatible Float Vector3
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct ArFloatVector3 : IDisposable, ICloneable, IEqualityOperators<ArFloatVector3, ArFloatVector3, bool>, IComparable<ArFloatVector3>, IEquatable<ArFloatVector3>, IAdditionOperators<ArFloatVector3, ArFloatVector3, ArFloatVector3>,
        ISubtractionOperators<ArFloatVector3, ArFloatVector3, ArFloatVector3>, IMultiplyOperators<ArFloatVector3, ArFloatVector3, float>, IUnaryNegationOperators<ArFloatVector3, ArFloatVector3>,
        IParsable<ArFloatVector3>
    {
        float _x, _y, _z;
        public static ArFloatVector3 Zero => new ArFloatVector3();
        public static ArFloatVector3 One => new ArFloatVector3(1, 1, 1);
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

        public ArFloatVector3(double x, double y, double z)
           : this((float)x, (float)y, (float)z)
        { }

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
        public void Dispose()
            => Dispose();
        public object Clone()
            => new ArFloatVector3(_x, _y, _z);
        public override string ToString()
            => ToString("G");
        public string ToString(string format)
            => $"({_x.ToString(format)}, {_y.ToString(format)}, {_z.ToString(format)})";
        public bool Equals(ArFloatVector3 other)
            => _x == other._x && _y == other._y && _z == other._z;
        public int CompareTo(ArFloatVector3 other)
            => Equals(other) ? 0 : _x > other._x ? 1 : _x < other._x ? -1 : _y > other._y ? 1 : _y < other._y ? -1 : _z > other._z ? 1 : -1;
        public static bool operator ==(ArFloatVector3 left, ArFloatVector3 right)
            => left.Equals(right);
        public static bool operator !=(ArFloatVector3 left, ArFloatVector3 right)
            => !(left == right);
        public static ArFloatVector3 operator +(ArFloatVector3 left, ArFloatVector3 right)
            => new ArFloatVector3(left._x + right._x, left._y + right._y, left._z + right._z);
        public static ArFloatVector3 operator -(ArFloatVector3 left, ArFloatVector3 right)
            => new ArFloatVector3(left._x - right._x, left._y - right._y, left._z - right._z);
        public static ArFloatVector3 operator *(ArFloatVector3 a, int b)
            => new ArFloatVector3(a._x * b, a._y * b, a._z * b);
        public static ArFloatVector3 operator /(ArFloatVector3 a, double b)
            => new ArFloatVector3((float)(a._x / b), (float)(a._y / b), (float)(a._z / b));
        public double GetLength() => Math.Sqrt(_x * _x + _y * _y + _z * _z);

        public double AngleBetween(ArFloatVector3 a)
            => Math.Acos(DotProduct(a) / (GetLength() * a.GetLength()));
        public ArFloatVector3 Normalize()
        {
            double l = GetLength();
            return new ArFloatVector3((float)(_x / l), (float)(_y / l), (float)(_z / l));
        }

        public static float operator *(ArFloatVector3 left, ArFloatVector3 right)
            => left.DotProduct(right);
        public ArFloatVector3 CrossProduct(ArFloatVector3 a)
            => new ArFloatVector3(_y * a._z - _z * a._y,
                _z * a._x - _x * a._z,
                _x * a._y - _y * a._x);
        public float DotProduct(ArFloatVector3 a)
            => _x * a._x + _y * a._y + _z * a._z;
        public override bool Equals(object? obj)
        {
            if (!(obj is ArFloatVector3))
                return false;
            return Equals((ArFloatVector3)obj);
        }
        public override int GetHashCode()
            => (_x, _y, _z).GetHashCode();

        /// <summary>
        /// 將字串轉為ArFloatVector3
        /// </summary>
        /// <param name="s">(x,y,z)或x,y,z</param>
        /// <param name="provider">Null</param>
        /// <returns>值</returns>
        public static ArFloatVector3 Parse(string s, IFormatProvider? provider = null)
        {
            string[] n;
            if (s.StartsWith('('))
                n = s.Substring(1, s.Length - 2).Replace(" ", "").Split(',');
            else
                n = s.Replace(" ", "").Split(',');
            return new ArFloatVector3(float.Parse(n[0]), float.Parse(n[1]), float.Parse(n[2]));
        }

        public static bool TryParse([NotNullWhen(true)] string? s, [MaybeNullWhen(false)] out ArFloatVector3 result)
            => TryParse(s, null, out result);

        public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out ArFloatVector3 result)
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

        public static bool operator <(ArFloatVector3 left, ArFloatVector3 right)
            => left.CompareTo(right) < 0;
        public static bool operator <=(ArFloatVector3 left, ArFloatVector3 right)
            => left.CompareTo(right) <= 0;
        public static bool operator >(ArFloatVector3 left, ArFloatVector3 right)
            => left.CompareTo(right) > 0;
        public static bool operator >=(ArFloatVector3 left, ArFloatVector3 right)
            => left.CompareTo(right) >= 0;
        public static ArFloatVector3 operator -(ArFloatVector3 value)
            => new ArFloatVector3(value._x * -1, value._y * -1, value._z * -1);

        public static implicit operator ArFloatVector3(ArFloatVector2 a)
            => new ArFloatVector3(a[0], a[1], 0);
        public static implicit operator ArFloatVector3(ArIntVector3 a)
            => new ArFloatVector3(a[0], a[1], a[2]);
        public static explicit operator ArFloatVector3(ArFloatVector4 a)
            => new ArFloatVector3(a[0], a[1], a[2]);
    }
}
