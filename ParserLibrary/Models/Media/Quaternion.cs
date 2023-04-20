using System;
using System.ComponentModel;
using System.Windows.Markup;

namespace ParserLibrary.Models.Media
{
    [Serializable]
    [TypeConverter(typeof(QuaternionConverter))]
    [ValueSerializer(typeof(QuaternionValueSerializer))]
    public struct Quaternion : IFormattable
    {
        internal double _x;

        internal double _y;

        internal double _z;

        internal double _w;

        private bool _isNotDistinguishedIdentity;

        private static int c_identityHashCode = GetIdentityHashCode();

        private static Quaternion s_identity = GetIdentity();

        public static Quaternion Identity => s_identity;

        public Vector3D Axis
        {
            get
            {
                if (IsDistinguishedIdentity || (_x == 0.0 && _y == 0.0 && _z == 0.0))
                {
                    return new Vector3D(0.0, 1.0, 0.0);
                }

                Vector3D result = new Vector3D(_x, _y, _z);
                result.Normalize();
                return result;
            }
        }

        public double Angle
        {
            get
            {
                if (IsDistinguishedIdentity)
                {
                    return 0.0;
                }

                double num = Math.Sqrt(_x * _x + _y * _y + _z * _z);
                double x = _w;
                if (!(num <= double.MaxValue))
                {
                    double num2 = Math.Max(Math.Abs(_x), Math.Max(Math.Abs(_y), Math.Abs(_z)));
                    double num3 = _x / num2;
                    double num4 = _y / num2;
                    double num5 = _z / num2;
                    num = Math.Sqrt(num3 * num3 + num4 * num4 + num5 * num5);
                    x = _w / num2;
                }

                return Math.Atan2(num, x) * (360.0 / Math.PI);
            }
        }

        public bool IsNormalized
        {
            get
            {
                if (IsDistinguishedIdentity)
                {
                    return true;
                }

                double value = _x * _x + _y * _y + _z * _z + _w * _w;
                return IsOne(value);
            }
        }
        public  bool IsOne(double value)
        {
            return Math.Abs(value - 1.0) < 2.2204460492503131E-15;
        }


        public bool IsIdentity
        {
            get
            {
                if (!IsDistinguishedIdentity)
                {
                    if (_x == 0.0 && _y == 0.0 && _z == 0.0)
                    {
                        return _w == 1.0;
                    }

                    return false;
                }

                return true;
            }
        }

        public double X
        {
            get
            {
                return _x;
            }
            set
            {
                if (IsDistinguishedIdentity)
                {
                    this = s_identity;
                    IsDistinguishedIdentity = false;
                }

                _x = value;
            }
        }

        public double Y
        {
            get
            {
                return _y;
            }
            set
            {
                if (IsDistinguishedIdentity)
                {
                    this = s_identity;
                    IsDistinguishedIdentity = false;
                }

                _y = value;
            }
        }

        public double Z
        {
            get
            {
                return _z;
            }
            set
            {
                if (IsDistinguishedIdentity)
                {
                    this = s_identity;
                    IsDistinguishedIdentity = false;
                }

                _z = value;
            }
        }

        public double W
        {
            get
            {
                if (IsDistinguishedIdentity)
                {
                    return 1.0;
                }

                return _w;
            }
            set
            {
                if (IsDistinguishedIdentity)
                {
                    this = s_identity;
                    IsDistinguishedIdentity = false;
                }

                _w = value;
            }
        }

        private bool IsDistinguishedIdentity
        {
            get
            {
                return !_isNotDistinguishedIdentity;
            }
            set
            {
                _isNotDistinguishedIdentity = !value;
            }
        }

        public Quaternion(double x, double y, double z, double w)
        {
            _x = x;
            _y = y;
            _z = z;
            _w = w;
            _isNotDistinguishedIdentity = true;
        }

        public Quaternion(Vector3D axisOfRotation, double angleInDegrees)
        {
            angleInDegrees %= 360.0;
            double num = angleInDegrees * (Math.PI / 180.0);
            double length = axisOfRotation.Length;
            if (length == 0.0)
            {
                throw new InvalidOperationException();// MS.Internal.PresentationCore.SR.Get("Quaternion_ZeroAxisSpecified"));
            }

            Vector3D vector3D = axisOfRotation / length * Math.Sin(0.5 * num);
            _x = vector3D.X;
            _y = vector3D.Y;
            _z = vector3D.Z;
            _w = Math.Cos(0.5 * num);
            _isNotDistinguishedIdentity = true;
        }

        public void Conjugate()
        {
            if (!IsDistinguishedIdentity)
            {
                _x = 0.0 - _x;
                _y = 0.0 - _y;
                _z = 0.0 - _z;
            }
        }

        public void Invert()
        {
            if (!IsDistinguishedIdentity)
            {
                Conjugate();
                double num = _x * _x + _y * _y + _z * _z + _w * _w;
                _x /= num;
                _y /= num;
                _z /= num;
                _w /= num;
            }
        }

        public void Normalize()
        {
            if (!IsDistinguishedIdentity)
            {
                double num = _x * _x + _y * _y + _z * _z + _w * _w;
                if (num > double.MaxValue)
                {
                    double num2 = 1.0 / Max(Math.Abs(_x), Math.Abs(_y), Math.Abs(_z), Math.Abs(_w));
                    _x *= num2;
                    _y *= num2;
                    _z *= num2;
                    _w *= num2;
                    num = _x * _x + _y * _y + _z * _z + _w * _w;
                }

                double num3 = 1.0 / Math.Sqrt(num);
                _x *= num3;
                _y *= num3;
                _z *= num3;
                _w *= num3;
            }
        }

        public static Quaternion operator +(Quaternion left, Quaternion right)
        {
            if (right.IsDistinguishedIdentity)
            {
                if (left.IsDistinguishedIdentity)
                {
                    return new Quaternion(0.0, 0.0, 0.0, 2.0);
                }

                left._w += 1.0;
                return left;
            }

            if (left.IsDistinguishedIdentity)
            {
                right._w += 1.0;
                return right;
            }

            return new Quaternion(left._x + right._x, left._y + right._y, left._z + right._z, left._w + right._w);
        }

        public static Quaternion Add(Quaternion left, Quaternion right)
        {
            return left + right;
        }

        public static Quaternion operator -(Quaternion left, Quaternion right)
        {
            if (right.IsDistinguishedIdentity)
            {
                if (left.IsDistinguishedIdentity)
                {
                    return new Quaternion(0.0, 0.0, 0.0, 0.0);
                }

                left._w -= 1.0;
                return left;
            }

            if (left.IsDistinguishedIdentity)
            {
                return new Quaternion(0.0 - right._x, 0.0 - right._y, 0.0 - right._z, 1.0 - right._w);
            }

            return new Quaternion(left._x - right._x, left._y - right._y, left._z - right._z, left._w - right._w);
        }

        public static Quaternion Subtract(Quaternion left, Quaternion right)
        {
            return left - right;
        }

        public static Quaternion operator *(Quaternion left, Quaternion right)
        {
            if (left.IsDistinguishedIdentity)
            {
                return right;
            }

            if (right.IsDistinguishedIdentity)
            {
                return left;
            }

            double x = left._w * right._x + left._x * right._w + left._y * right._z - left._z * right._y;
            double y = left._w * right._y + left._y * right._w + left._z * right._x - left._x * right._z;
            double z = left._w * right._z + left._z * right._w + left._x * right._y - left._y * right._x;
            double w = left._w * right._w - left._x * right._x - left._y * right._y - left._z * right._z;
            return new Quaternion(x, y, z, w);
        }

        public static Quaternion Multiply(Quaternion left, Quaternion right)
        {
            return left * right;
        }

        private void Scale(double scale)
        {
            if (IsDistinguishedIdentity)
            {
                _w = scale;
                IsDistinguishedIdentity = false;
                return;
            }

            _x *= scale;
            _y *= scale;
            _z *= scale;
            _w *= scale;
        }

        private double Length()
        {
            if (IsDistinguishedIdentity)
            {
                return 1.0;
            }

            double num = _x * _x + _y * _y + _z * _z + _w * _w;
            if (!(num <= double.MaxValue))
            {
                double num2 = Math.Max(Math.Max(Math.Abs(_x), Math.Abs(_y)), Math.Max(Math.Abs(_z), Math.Abs(_w)));
                double num3 = _x / num2;
                double num4 = _y / num2;
                double num5 = _z / num2;
                double num6 = _w / num2;
                double num7 = Math.Sqrt(num3 * num3 + num4 * num4 + num5 * num5 + num6 * num6);
                return num7 * num2;
            }

            return Math.Sqrt(num);
        }

        public static Quaternion Slerp(Quaternion from, Quaternion to, double t)
        {
            return Slerp(from, to, t, useShortestPath: true);
        }

        public static Quaternion Slerp(Quaternion from, Quaternion to, double t, bool useShortestPath)
        {
            if (from.IsDistinguishedIdentity)
            {
                from._w = 1.0;
            }

            if (to.IsDistinguishedIdentity)
            {
                to._w = 1.0;
            }

            double num = from.Length();
            double num2 = to.Length();
            from.Scale(1.0 / num);
            to.Scale(1.0 / num2);
            double num3 = from._x * to._x + from._y * to._y + from._z * to._z + from._w * to._w;
            if (useShortestPath)
            {
                if (num3 < 0.0)
                {
                    num3 = 0.0 - num3;
                    to._x = 0.0 - to._x;
                    to._y = 0.0 - to._y;
                    to._z = 0.0 - to._z;
                    to._w = 0.0 - to._w;
                }
            }
            else if (num3 < -1.0)
            {
                num3 = -1.0;
            }

            if (num3 > 1.0)
            {
                num3 = 1.0;
            }

            double num4;
            double num5;
            if (num3 > 0.999999)
            {
                num4 = 1.0 - t;
                num5 = t;
            }
            else if (num3 < -0.9999999999)
            {
                to = new Quaternion(0.0 - from.Y, from.X, 0.0 - from.W, from.Z);
                double num6 = t * Math.PI;
                num4 = Math.Cos(num6);
                num5 = Math.Sin(num6);
            }
            else
            {
                double num7 = Math.Acos(num3);
                double num8 = Math.Sqrt(1.0 - num3 * num3);
                num4 = Math.Sin((1.0 - t) * num7) / num8;
                num5 = Math.Sin(t * num7) / num8;
            }

            double num9 = num * Math.Pow(num2 / num, t);
            num4 *= num9;
            num5 *= num9;
            return new Quaternion(num4 * from._x + num5 * to._x, num4 * from._y + num5 * to._y, num4 * from._z + num5 * to._z, num4 * from._w + num5 * to._w);
        }

        private static double Max(double a, double b, double c, double d)
        {
            if (b > a)
            {
                a = b;
            }

            if (c > a)
            {
                a = c;
            }

            if (d > a)
            {
                a = d;
            }

            return a;
        }

        private static int GetIdentityHashCode()
        {
            return 0.0.GetHashCode() ^ 1.0.GetHashCode();
        }

        private static Quaternion GetIdentity()
        {
            Quaternion result = new Quaternion(0.0, 0.0, 0.0, 1.0);
            result.IsDistinguishedIdentity = true;
            return result;
        }

        public static bool operator ==(Quaternion quaternion1, Quaternion quaternion2)
        {
            if (quaternion1.IsDistinguishedIdentity || quaternion2.IsDistinguishedIdentity)
            {
                return quaternion1.IsIdentity == quaternion2.IsIdentity;
            }

            if (quaternion1.X == quaternion2.X && quaternion1.Y == quaternion2.Y && quaternion1.Z == quaternion2.Z)
            {
                return quaternion1.W == quaternion2.W;
            }

            return false;
        }

        public static bool operator !=(Quaternion quaternion1, Quaternion quaternion2)
        {
            return !(quaternion1 == quaternion2);
        }

        public static bool Equals(Quaternion quaternion1, Quaternion quaternion2)
        {
            if (quaternion1.IsDistinguishedIdentity || quaternion2.IsDistinguishedIdentity)
            {
                return quaternion1.IsIdentity == quaternion2.IsIdentity;
            }

            if (quaternion1.X.Equals(quaternion2.X) && quaternion1.Y.Equals(quaternion2.Y) && quaternion1.Z.Equals(quaternion2.Z))
            {
                return quaternion1.W.Equals(quaternion2.W);
            }

            return false;
        }

        public override bool Equals(object o)
        {
            if (o == null || !(o is Quaternion))
            {
                return false;
            }

            Quaternion quaternion = (Quaternion)o;
            return Equals(this, quaternion);
        }

        public bool Equals(Quaternion value)
        {
            return Equals(this, value);
        }

        public override int GetHashCode()
        {
            if (IsDistinguishedIdentity)
            {
                return c_identityHashCode;
            }

            return X.GetHashCode() ^ Y.GetHashCode() ^ Z.GetHashCode() ^ W.GetHashCode();
        }

        public static Quaternion Parse(string source)
        {
            IFormatProvider invariantEnglishUS = TypeConverterHelper.InvariantEnglishUS;//  System.Windows.Markup.TypeConverterHelper.InvariantEnglishUS;
            TokenizerHelper tokenizerHelper = new TokenizerHelper(source, invariantEnglishUS);
            string text = tokenizerHelper.NextTokenRequired();
            Quaternion result = ((!(text == "Identity")) ? new Quaternion(Convert.ToDouble(text, invariantEnglishUS), Convert.ToDouble(tokenizerHelper.NextTokenRequired(), invariantEnglishUS), Convert.ToDouble(tokenizerHelper.NextTokenRequired(), invariantEnglishUS), Convert.ToDouble(tokenizerHelper.NextTokenRequired(), invariantEnglishUS)) : Identity);
            tokenizerHelper.LastTokenRequired();
            return result;
        }

        public override string ToString()
        {
            return ConvertToString(null, null);
        }

        public string ToString(IFormatProvider provider)
        {
            return ConvertToString(null, provider);
        }

        string IFormattable.ToString(string format, IFormatProvider provider)
        {
            return ConvertToString(format, provider);
        }

        internal string ConvertToString(string format, IFormatProvider provider)
        {
            if (IsIdentity)
            {
                return "Identity";
            }

            char numericListSeparator = TokenizerHelper.GetNumericListSeparator(provider);
            return string.Format(provider, "{1:" + format + "}{0}{2:" + format + "}{0}{3:" + format + "}{0}{4:" + format + "}", numericListSeparator, _x, _y, _z, _w);
        }
    }
}


