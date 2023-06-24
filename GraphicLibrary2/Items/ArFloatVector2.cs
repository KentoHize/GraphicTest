using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.InteropServices;

namespace GraphicLibrary2.Items
{
    //DirectX Compatible Float Vector2
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct ArFloatVector2 : IDisposable, ICloneable, IEqualityOperators<ArFloatVector2, ArFloatVector2, bool>, IComparable<ArFloatVector2>, IEquatable<ArFloatVector2>, IAdditionOperators<ArFloatVector2, ArFloatVector2, ArFloatVector2>,
    ISubtractionOperators<ArFloatVector2, ArFloatVector2, ArFloatVector2>, IMultiplyOperators<ArFloatVector2, ArFloatVector2, float>, IUnaryNegationOperators<ArFloatVector2, ArFloatVector2>,
        IParsable<ArFloatVector2>
    {
        float _x, _y;
        public static ArFloatVector2 Zero => new ArFloatVector2();
        public static ArFloatVector2 One => new ArFloatVector2(1, 1);
        public static ArFloatVector2 UnitX { get => new ArFloatVector2(1, 0); }
        public static ArFloatVector2 UnitY { get => new ArFloatVector2(0, 1); }

        public ArFloatVector2()
        { }

        public ArFloatVector2(float x, float y)
        {
            _x = x;
            _y = y;
        }

        public ArFloatVector2(double x, double y)
            : this((float)x, (float)y)
        { }

        public float this[int index]
        {
            get => index switch { 0 => _x, 1 => _y, _ => throw new IndexOutOfRangeException(nameof(index)) };
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
                    default:
                        throw new IndexOutOfRangeException(nameof(index));
                }
            }
        }

        public object Clone()
            => new ArFloatVector2(_x, _y);
        public void Dispose()
            => Dispose();
        public override string ToString()
            => ToString("G");
        public string ToString(string format)
            => $"({_x.ToString(format)}, {_y.ToString(format)})";
        public bool Equals(ArFloatVector2 other)
            => _x == other._x && _y == other._y;
        public int CompareTo(ArFloatVector2 other)
            => Equals(other) ? 0 : _x > other._x ? 1 : _x < other._x ? -1 : _y > other._y ? 1 : -1;
        public static bool operator ==(ArFloatVector2 left, ArFloatVector2 right)
            => left.Equals(right);
        public static bool operator !=(ArFloatVector2 left, ArFloatVector2 right)
            => !(left == right);
        public static ArFloatVector2 operator +(ArFloatVector2 left, ArFloatVector2 right)
            => new ArFloatVector2(left._x + right._x, left._y + right._y);
        public static ArFloatVector2 operator -(ArFloatVector2 left, ArFloatVector2 right)
            => new ArFloatVector2(left._x - right._x, left._y - right._y);
        public static ArFloatVector2 operator *(ArFloatVector2 a, int b)
            => new ArFloatVector2(a._x * b, a._y * b);
        public static ArFloatVector2 operator /(ArFloatVector2 a, double b)
            => new ArFloatVector2((float)(a._x / b), (float)(a._y / b));
        public double GetLength() => Math.Sqrt(_x * _x + _y * _y);
        public double AngleBetween(ArFloatVector2 a)
            => Math.Acos(DotProduct(a) / (GetLength() * a.GetLength()));
        public ArFloatVector2 Normalize()
        {
            double l = GetLength();
            return new ArFloatVector2((float)(_x / l), (float)(_y / l));
        }
        public double Determinant(ArFloatVector2 a)
            => CrossProduct(a);
        public static float operator *(ArFloatVector2 left, ArFloatVector2 right)
            => left.DotProduct(right);
        public float CrossProduct(ArFloatVector2 a)
            => _x * a._y - _y * a._x;
        public float DotProduct(ArFloatVector2 a)
            => _x * a._x + _y * a._y;
        public override bool Equals(object? obj)
        {
            if (!(obj is ArFloatVector2))
                return false;
            return Equals((ArFloatVector2)obj);
        }
        public override int GetHashCode()
            => (_x, _y).GetHashCode();

        /// <summary>
        /// 將字串轉為ArFloatVector2
        /// </summary>
        /// <param name="s">(x,y)或x,y</param>
        /// <param name="provider">Null</param>
        /// <returns>值</returns>
        public static ArFloatVector2 Parse(string s, IFormatProvider? provider = null)
        {
            string[] n;
            if (s.StartsWith('('))
                n = s.Substring(1, s.Length - 2).Replace(" ", "").Split(',');
            else
                n = s.Replace(" ", "").Split(',');
            return new ArFloatVector2(float.Parse(n[0]), float.Parse(n[1]));
        }

        public static bool TryParse([NotNullWhen(true)] string? s, out ArFloatVector2 result)
            => TryParse(s, null, out result);

        public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out ArFloatVector2 result)
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

        public static bool operator <(ArFloatVector2 left, ArFloatVector2 right)
            => left.CompareTo(right) < 0;
        public static bool operator <=(ArFloatVector2 left, ArFloatVector2 right)
            => left.CompareTo(right) <= 0;
        public static bool operator >(ArFloatVector2 left, ArFloatVector2 right)
            => left.CompareTo(right) > 0;
        public static bool operator >=(ArFloatVector2 left, ArFloatVector2 right)
            => left.CompareTo(right) >= 0;
        public static ArFloatVector2 operator -(ArFloatVector2 value)
            => new ArFloatVector2(value._x * -1, value._y * -1);

        public static explicit operator ArFloatVector2(ArFloatVector3 a)
            => new ArFloatVector2(a[0], a[1]);
        public static explicit operator ArFloatVector2(ArFloatVector4 a)
            => new ArFloatVector2(a[0], a[1]);
    }
}
