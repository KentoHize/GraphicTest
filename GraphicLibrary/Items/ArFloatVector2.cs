using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphicLibrary.Items
{
    //DirectX Shader Compatible Float Vector2
    public struct ArFloatVector2 : IEquatable<ArFloatVector2>, IFormattable
    {
        float _x, _y;
        public static ArFloatVector2 Zero { get => new ArFloatVector2(); }
        public static ArFloatVector2 One { get => new ArFloatVector2(1, 1); }
        public static ArFloatVector2 UnitX { get => new ArFloatVector2(1, 0); }
        public static ArFloatVector2 UnitY { get => new ArFloatVector2(0, 1); }
        
        public ArFloatVector2()
        { }

        public ArFloatVector2(float x, float y)
        {
            _x = x;
            _y = y;
            
        }

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
        public float X { get => _x; set => _x = value; }
        public float Y { get => _y; set => _y = value; }        
        public static ArFloatVector2 operator +(ArFloatVector2 a, ArFloatVector2 b)
            => new ArFloatVector2(a._x + b._x, a._y + b._y);
        public static ArFloatVector2 operator -(ArFloatVector2 a, ArFloatVector2 b)
            => new ArFloatVector2(a._x - b._x, a._y - b._y);
        public static ArFloatVector2 operator *(ArFloatVector2 a, double b)
            => new ArFloatVector2((float)(a._x * b), (float)(a._y * b));
        public static ArFloatVector2 operator *(ArFloatVector2 a, int b)
            => new ArFloatVector2(a._x * b, a._y * b);
        public static ArFloatVector2 operator /(ArFloatVector2 a, double b)
            => new ArFloatVector2((float)(a._x / b), (float)(a._y / b));
        public static ArFloatVector2 operator /(ArFloatVector2 a, int b)
            => new ArFloatVector2(a._x / b, a._y / b);
        public static bool operator ==(ArFloatVector2 a, ArFloatVector2 b)
            => a.Equals(b);
        public static bool operator !=(ArFloatVector2 a, ArFloatVector2 b)
            => !a.Equals(b);

        public double GetLength()
            => Math.Sqrt(_x * _x + _y * _y);
        public override string ToString()
            => $"({_x}, {_y})";
        public bool Equals(ArFloatVector2 other)
            => _x == other._x && _y == other._y;
        public string ToString(string? format, IFormatProvider? formatProvider)
            => string.Format(formatProvider, "({0}, {1})", _x, _y);
        public override int GetHashCode()
            => (_x, _y).GetHashCode();
        public override bool Equals(object obj)
            => obj is ArFloatVector2 && Equals((ArFloatVector2)obj);
    }
}
