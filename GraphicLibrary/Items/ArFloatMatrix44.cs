using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphicLibrary.Items
{
    //DirectX Shader Compatible Int Vector3
    public struct ArFloatMatrix44 : IEquatable<ArFloatMatrix44>
    {
        float _11, _12, _13, _14, _21, _22, _23, _24, _31, _32, _33, _34, _41, _42, _43, _44;
        public static ArFloatMatrix44 Zero { get => new ArFloatMatrix44(); }
        public static ArFloatMatrix44 One { get => new ArFloatMatrix44(1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1); }

        public ArFloatMatrix44()
            : this(0, 0, 0, 0, 0, 0, 0, 0,0, 0, 0, 0, 0, 0, 0, 0)
        { }

        public ArFloatMatrix44(float _11, float _12, float _13, float _14, float _21, float _22, float _23, float _24, float _31, float _32, float _33, float _34, float _41, float _42, float _43, float _44)
        {
            this._11 = _11;
            this._12 = _12;
            this._13 = _13;
            this._14 = _14;
            this._21 = _21;
            this._22 = _22;
            this._23 = _23;
            this._24 = _24;
            this._31 = _31;
            this._32 = _32;
            this._33 = _33;
            this._34 = _34;
            this._41 = _41;
            this._42 = _42;
            this._43 = _43;
            this._44 = _44;
        }

        public ArFloatMatrix44(float[,] matrix)
        {
            if (matrix.GetLength(0) != 4 || matrix.GetLength(1) != 4)
                throw new IndexOutOfRangeException(nameof(matrix));
            _11 = matrix[0, 0];
            _12 = matrix[0, 1];
            _13 = matrix[0, 2];
            _14 = matrix[0, 3];
            _21 = matrix[1, 0];
            _22 = matrix[1, 1];
            _23 = matrix[1, 2];
            _24 = matrix[1, 3];
            _31 = matrix[2, 0];
            _32 = matrix[2, 1];
            _33 = matrix[2, 2];
            _34 = matrix[2, 3];
            _41 = matrix[3, 0];
            _42 = matrix[3, 1];
            _43 = matrix[3, 2];
            _44 = matrix[3, 3];
        }

        public static bool operator ==(ArFloatMatrix44 a, ArFloatMatrix44 b)
            => a.Equals(b);
        public static bool operator !=(ArFloatMatrix44 a, ArFloatMatrix44 b)
            => !a.Equals(b);
        public bool Equals(ArFloatMatrix44 other)
            => _11 == other._11 && _12 == other._12 && _13 == other._13 && _14 == other._14 &&
            _21 == other._21 && _22 == other._22 && _23 == other._23 && _24 == other._24 &&
            _31 == other._31 && _32 == other._32 && _33 == other._33 && _34 == other._34 &&
            _41 == other._41 && _42 == other._42 && _43 == other._43 && _44 == other._44;

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("{{{0} {1} {2} {3}}}\n", _11, _12, _13, _14);
            sb.AppendFormat("{{{0} {1} {2} {3}}}\n", _21, _22, _23, _24);
            sb.AppendFormat("{{{0} {1} {2} {3}}}\n", _31, _32, _33, _34);
            sb.AppendFormat("{{{0} {1} {2} {3}}}", _41, _42, _43, _44);
            return sb.ToString();
        }
    }
}
