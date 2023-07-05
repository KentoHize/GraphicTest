using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics.X86;
using System.Text;

namespace GraphicLibrary2.Items
{
    //DirectX Shader Compatible Int Vector3
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct ArFloatMatrix33 : IEquatable<ArFloatMatrix33>
    {
        float _11, _12, _13, _21, _22, _23, _31, _32, _33;
        public static ArFloatMatrix33 Zero { get => new ArFloatMatrix33(); }
        public static ArFloatMatrix33 One { get => new ArFloatMatrix33(1, 0, 0, 0, 1, 0, 0, 0, 1); }

        public float this[int x, int y]
        {
            get
            {
                return x switch
                {
                    0 => y switch
                    {
                        0 => _11,
                        1 => _12,
                        2 => _13,
                        _ => throw new IndexOutOfRangeException(nameof(y))
                    },
                    1 => y switch
                    {
                        0 => _21,
                        1 => _22,
                        2 => _23,
                        _ => throw new IndexOutOfRangeException(nameof(y))
                    },
                    2 => y switch
                    {
                        0 => _31,
                        1 => _32,
                        2 => _33,
                        _ => throw new IndexOutOfRangeException(nameof(y))
                    },
                    _ => throw new IndexOutOfRangeException(nameof(x))
                };
            }
            set
            {
                switch (x)
                {
                    case 0:
                        switch (y)
                        {
                            case 0:
                                _11 = value;
                                break;
                            case 1:
                                _12 = value;
                                break;
                            case 2:
                                _13 = value;
                                break;
                            default:
                                throw new IndexOutOfRangeException(nameof(y));
                        }
                        break;
                    case 1:
                        switch (y)
                        {
                            case 0:
                                _21 = value;
                                break;
                            case 1:
                                _22 = value;
                                break;
                            case 2:
                                _23 = value;
                                break;
                            default:
                                throw new IndexOutOfRangeException(nameof(y));
                        }
                        break;
                    case 2:
                        switch (y)
                        {
                            case 0:
                                _31 = value;
                                break;
                            case 1:
                                _32 = value;
                                break;
                            case 2:
                                _33 = value;
                                break;
                            default:
                                throw new IndexOutOfRangeException(nameof(y));
                        }
                        break;
                    default:
                        throw new IndexOutOfRangeException(nameof(x));
                }
            }
        }

        public ArFloatMatrix33()
            : this(0, 0, 0, 0, 0, 0, 0, 0, 0)
        { }

        public ArFloatMatrix33(float _11, float _12, float _13, float _21, float _22, float _23, float _31, float _32, float _33)
        {
            this._11 = _11;
            this._12 = _12;
            this._13 = _13;

            this._21 = _21;
            this._22 = _22;
            this._23 = _23;

            this._31 = _31;
            this._32 = _32;
            this._33 = _33;

        }

        public ArFloatMatrix33(float[,] matrix)
        {
            if (matrix.GetLength(0) != 4 || matrix.GetLength(1) != 4)
                throw new IndexOutOfRangeException(nameof(matrix));
            _11 = matrix[0, 0];
            _12 = matrix[0, 1];
            _13 = matrix[0, 2];
            _21 = matrix[1, 0];
            _22 = matrix[1, 1];
            _23 = matrix[1, 2];
            _31 = matrix[2, 0];
            _32 = matrix[2, 1];
            _33 = matrix[2, 2];
        }

        public static ArFloatMatrix33 Product(ArFloatVector3 a, ArFloatVector3 b)
        {
            return new ArFloatMatrix33(a[0] * b[0], a[0] * b[1], a[0] * b[2],
                a[1] * b[0], a[1] * b[1], a[1] * b[2],
                a[2] * b[0], a[2] * b[1], a[2] * b[2]);
        }

        public static ArFloatVector3 operator *(ArFloatMatrix33 a, ArFloatVector3 b)
        {
            return new ArFloatVector3(
                a[0, 0] * b[0] + a[0, 1] * b[1] + a[0, 2] * b[2],
                a[1, 0] * b[0] + a[1, 1] * b[1] + a[1, 2] * b[2],
                a[2, 0] * b[0] + a[2, 1] * b[1] + a[2, 2] * b[2]);
        }

        public static ArFloatMatrix33 operator *(ArFloatMatrix33 a, ArFloatMatrix33 b)
        {
            ArFloatMatrix33 result = new ArFloatMatrix33();
            //result[0, 0] = a[0, 0] * b[0, 0] + a[0, 1] * b[1, 0] + a[0, 2] * b[2, 0] + a[0, 3] * b[3, 0];
            //result[0, 1] = a[0, 0] * b[0, 1] + a[0, 1] * b[1, 1] + a[0, 2] * b[2, 1] + a[0, 3] * b[3, 1];
            //result[0, 2] = a[0, 0] * b[0, 2] + a[0, 1] * b[1, 2] + a[0, 2] * b[2, 2] + a[0, 3] * b[3, 2];
            //result[0, 3] = a[0, 0] * b[0, 3] + a[0, 1] * b[1, 3] + a[0, 2] * b[2, 3] + a[0, 3] * b[3, 3];
            //result[1, 0] = a[1, 0] * b[0, 0] + a[1, 1] * b[1, 0] + a[1, 2] * b[2, 0] + a[1, 3] * b[3, 0];
            //result[1, 1] = a[1, 0] * b[0, 1] + a[1, 1] * b[1, 1] + a[1, 2] * b[2, 1] + a[1, 3] * b[3, 1];
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    result[i, j] = a[i, 0] * b[0, j] + a[i, 1] * b[1, j] + a[i, 2] * b[2, j];
            return result;
        }

        public static bool operator ==(ArFloatMatrix33 a, ArFloatMatrix33 b)
            => a.Equals(b);
        public static bool operator !=(ArFloatMatrix33 a, ArFloatMatrix33 b)
            => !a.Equals(b);

        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            if (!(obj is ArFloatMatrix33))
                return false;
            return Equals((ArFloatMatrix33)obj);
        }

        public override int GetHashCode()
         => (_11, _12, _13, _21, _22, _23, _31, _32, _33).GetHashCode();

        public bool Equals(ArFloatMatrix33 other)
            => _11 == other._11 && _12 == other._12 && _13 == other._13 &&
            _21 == other._21 && _22 == other._22 && _23 == other._23 &&
            _31 == other._31 && _32 == other._32 && _33 == other._33;


        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("{{{0} {1} {2}}}\n", _11, _12, _13);
            sb.AppendFormat("{{{0} {1} {2}}}\n", _21, _22, _23);
            sb.AppendFormat("{{{0} {1} {2}}}\n", _31, _32, _33);
            return sb.ToString();
        }
    }
}
