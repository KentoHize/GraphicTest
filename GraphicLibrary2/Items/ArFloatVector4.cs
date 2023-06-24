using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GraphicLibrary2.Items
{
    //DirectX Compatible Float Vector4
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public class ArFloatVector4 : IDisposable, ICloneable, IEqualityOperators<ArFloatVector4, ArFloatVector4, bool>, IComparable<ArFloatVector4>, IEquatable<ArFloatVector4>, IAdditionOperators<ArFloatVector4, ArFloatVector4, ArFloatVector4>,
        ISubtractionOperators<ArFloatVector4, ArFloatVector4, ArFloatVector4>, IMultiplyOperators<ArFloatVector4, ArFloatVector4, float>, IUnaryNegationOperators<ArFloatVector4, ArFloatVector4>,
        IParsable<ArFloatVector4>
    {
        float _x, _y, _z, _w;
        public static ArFloatVector4 Zero => new ArFloatVector4();
        public static ArFloatVector4 One => new ArFloatVector4(1, 1, 1, 1);
        public static ArFloatVector4 UnitX { get => new ArFloatVector4(1, 0, 0, 0); }
        public static ArFloatVector4 UnitY { get => new ArFloatVector4(0, 1, 0, 0); }
        public static ArFloatVector4 UnitZ { get => new ArFloatVector4(0, 0, 1, 0); }
        public static ArFloatVector4 UnitW { get => new ArFloatVector4(0, 0, 0, 1); }

        public ArFloatVector4()
        { }

        public ArFloatVector4(float x, float y, float z, float w)
        {
            _x = x;
            _y = y;
            _z = z;
            _w = w;
        }

        public ArFloatVector4(double x, double y, double z, double w)
           : this((float)x, (float)y, (float)z, (float)w)
        { }

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

        public void Dispose()
            => Dispose();
        public object Clone()
            => new ArFloatVector4(_x, _y, _z, _w);
        public override string ToString()
            => ToString("G");
        public string ToString(string format)
            => $"({_x.ToString(format)}, {_y.ToString(format)}, {_z.ToString(format)}, {_w.ToString(format)})";
        public bool Equals(ArFloatVector4? other)
            => ReferenceEquals(other, null) ? false : _x == other._x && _y == other._y && _z == other._z && _w == other._w;
        public int CompareTo(ArFloatVector4? other)
            => ReferenceEquals(other, null) ? 1 : Equals(other) ? 0 : _x > other._x ? 1 : _x < other._x ? -1 : _y > other._y ? 1 : _y < other._y ? -1 : _z > other._z ? 1 : _z < other._z ? -1 : _w > other._w ? 1 : -1;
        public static bool operator ==(ArFloatVector4? left, ArFloatVector4? right)
            => ReferenceEquals(left, right) ? true : ReferenceEquals(left, null) ? false : left.Equals(right);
        public static bool operator !=(ArFloatVector4? left, ArFloatVector4? right)
            => !(left == right);
        public static ArFloatVector4 operator +(ArFloatVector4 left, ArFloatVector4 right)
            => new ArFloatVector4(left._x + right._x, left._y + right._y, left._z + right._z, left._w + right._w);
        public static ArFloatVector4 operator -(ArFloatVector4 left, ArFloatVector4 right)
            => new ArFloatVector4(left._x - right._x, left._y - right._y, left._z - right._z, left._w - right._w);
        public static ArFloatVector4 operator *(ArFloatVector4 a, int b)
            => new ArFloatVector4(a._x * b, a._y * b, a._z * b, a._w * b);
        public static ArFloatVector4 operator /(ArFloatVector4 a, double b)
            => new ArFloatVector4((float)(a._x / b), (float)(a._y / b), (float)(a._z / b), (float)(a._w / b));
        public double GetLength() => Math.Sqrt(_x * _x + _y * _y + _z * _z + _w * _w);
        public double AngleBetween(ArFloatVector4 a)
            => Math.Acos(DotProduct(a) / (GetLength() * a.GetLength()));
        public ArFloatVector4 Normalize()
        {
            double l = GetLength();
            return new ArFloatVector4((float)(_x / l), (float)(_y / l), (float)(_z / l), (float)(_w / l));
        }
        public static float operator *(ArFloatVector4 left, ArFloatVector4 right)
            => left.DotProduct(right);
        public float DotProduct(ArFloatVector4 a)
            => _x * a._x + _y * a._y + _z * a._z + _w * a._w;
        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(this, obj))
                return true;
            else if (!(obj is ArFloatVector4))
                return false;
            return Equals((ArFloatVector4)obj);
        }
        public override int GetHashCode()
            => (_x, _y, _z, _w).GetHashCode();

        /// <summary>
        /// 將字串轉為ArFloatVector4
        /// </summary>
        /// <param name="s">(x,y,z,w)或x,y,z,w</param>
        /// <param name="provider">Null</param>
        /// <returns>值</returns>
        public static ArFloatVector4 Parse(string s, IFormatProvider? provider = null)
        {
            string[] n;
            if (s.StartsWith('('))
                n = s.Substring(1, s.Length - 2).Replace(" ", "").Split(',');
            else
                n = s.Replace(" ", "").Split(',');
            return new ArFloatVector4(float.Parse(n[0]), float.Parse(n[1]), float.Parse(n[2]), float.Parse(n[3]));
        }

        public static bool TryParse([NotNullWhen(true)] string? s, [MaybeNullWhen(false)] out ArFloatVector4 result)
            => TryParse(s, null, out result);

        public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out ArFloatVector4 result)
        {
            try
            {
                result = Parse(s, provider);
                return true;
            }
            catch
            {
                result = null;
                return false;
            }
        }

        public static bool operator <(ArFloatVector4 left, ArFloatVector4 right)
            => ReferenceEquals(left, null) ? !ReferenceEquals(right, null) : left.CompareTo(right) < 0;
        public static bool operator <=(ArFloatVector4 left, ArFloatVector4 right)
            => ReferenceEquals(left, null) || left.CompareTo(right) <= 0;
        public static bool operator >(ArFloatVector4 left, ArFloatVector4 right)
            => !ReferenceEquals(left, null) && left.CompareTo(right) > 0;
        public static bool operator >=(ArFloatVector4 left, ArFloatVector4 right)
            => ReferenceEquals(left, null) ? ReferenceEquals(right, null) : left.CompareTo(right) >= 0;
        public static ArFloatVector4 operator -(ArFloatVector4 value)
            => new ArFloatVector4(value._x * -1, value._y * -1, value._z * -1, value._w * -1);

        public static implicit operator ArFloatVector4(ArFloatVector2 a)
            => new ArFloatVector4(a[0], a[1], 0, 0);
        public static implicit operator ArFloatVector4(ArFloatVector3 a)
            => new ArFloatVector4(a[0], a[1], a[2], 0);
        public static implicit operator ArFloatVector4(ArIntVector3 a)
            => new ArFloatVector4(a[0], a[1], a[2], 0);

    }
}
