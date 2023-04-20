using System;
using System.ComponentModel;
using System.Windows.Markup;
//using ParserLibrary.Models.Media;



namespace ParserLibrary.Models.Media
{
    [Serializable]
    [TypeConverter(typeof(Vector3DConverter))]
    [ValueSerializer(typeof(Vector3DValueSerializer))]
    public struct Vector3D : IFormattable
    {
        internal double _x;

        internal double _y;

        internal double _z;

        public double Length => Math.Sqrt(_x * _x + _y * _y + _z * _z);

        public double LengthSquared => _x * _x + _y * _y + _z * _z;

        public double X
        {
            get
            {
                return _x;
            }
            set
            {
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
                _z = value;
            }
        }

        public Vector3D(double x, double y, double z)
        {
            _x = x;
            _y = y;
            _z = z;
        }

        public void Normalize()
        {
            double num = Math.Abs(_x);
            double num2 = Math.Abs(_y);
            double num3 = Math.Abs(_z);
            if (num2 > num)
            {
                num = num2;
            }

            if (num3 > num)
            {
                num = num3;
            }

            _x /= num;
            _y /= num;
            _z /= num;
            double num4 = Math.Sqrt(_x * _x + _y * _y + _z * _z);
            this /= num4;
        }

        public static double AngleBetween(Vector3D vector1, Vector3D vector2)
        {
            vector1.Normalize();
            vector2.Normalize();
            double num = DotProduct(vector1, vector2);
            double radians = ((!(num < 0.0)) ? (2.0 * Math.Asin((vector1 - vector2).Length / 2.0)) : (Math.PI - 2.0 * Math.Asin((-vector1 - vector2).Length / 2.0)));
            return M3DUtil.RadiansToDegrees(radians);
        }

        public static Vector3D operator -(Vector3D vector)
        {
            return new Vector3D(0.0 - vector._x, 0.0 - vector._y, 0.0 - vector._z);
        }

        public void Negate()
        {
            _x = 0.0 - _x;
            _y = 0.0 - _y;
            _z = 0.0 - _z;
        }

        public static Vector3D operator +(Vector3D vector1, Vector3D vector2)
        {
            return new Vector3D(vector1._x + vector2._x, vector1._y + vector2._y, vector1._z + vector2._z);
        }

        public static Vector3D Add(Vector3D vector1, Vector3D vector2)
        {
            return new Vector3D(vector1._x + vector2._x, vector1._y + vector2._y, vector1._z + vector2._z);
        }

        public static Vector3D operator -(Vector3D vector1, Vector3D vector2)
        {
            return new Vector3D(vector1._x - vector2._x, vector1._y - vector2._y, vector1._z - vector2._z);
        }

        public static Vector3D Subtract(Vector3D vector1, Vector3D vector2)
        {
            return new Vector3D(vector1._x - vector2._x, vector1._y - vector2._y, vector1._z - vector2._z);
        }

        public static Point3D operator +(Vector3D vector, Point3D point)
        {
            return new Point3D(vector._x + point._x, vector._y + point._y, vector._z + point._z);
        }

        public static Point3D Add(Vector3D vector, Point3D point)
        {
            return new Point3D(vector._x + point._x, vector._y + point._y, vector._z + point._z);
        }

        public static Point3D operator -(Vector3D vector, Point3D point)
        {
            return new Point3D(vector._x - point._x, vector._y - point._y, vector._z - point._z);
        }

        public static Point3D Subtract(Vector3D vector, Point3D point)
        {
            return new Point3D(vector._x - point._x, vector._y - point._y, vector._z - point._z);
        }

        public static Vector3D operator *(Vector3D vector, double scalar)
        {
            return new Vector3D(vector._x * scalar, vector._y * scalar, vector._z * scalar);
        }

        public static Vector3D Multiply(Vector3D vector, double scalar)
        {
            return new Vector3D(vector._x * scalar, vector._y * scalar, vector._z * scalar);
        }

        public static Vector3D operator *(double scalar, Vector3D vector)
        {
            return new Vector3D(vector._x * scalar, vector._y * scalar, vector._z * scalar);
        }

        public static Vector3D Multiply(double scalar, Vector3D vector)
        {
            return new Vector3D(vector._x * scalar, vector._y * scalar, vector._z * scalar);
        }

        public static Vector3D operator /(Vector3D vector, double scalar)
        {
            return vector * (1.0 / scalar);
        }

        public static Vector3D Divide(Vector3D vector, double scalar)
        {
            return vector * (1.0 / scalar);
        }

        public static Vector3D operator *(Vector3D vector, Matrix3D matrix)
        {
            return matrix.Transform(vector);
        }

        public static Vector3D Multiply(Vector3D vector, Matrix3D matrix)
        {
            return matrix.Transform(vector);
        }

        public static double DotProduct(Vector3D vector1, Vector3D vector2)
        {
            return DotProduct(ref vector1, ref vector2);
        }

        internal static double DotProduct(ref Vector3D vector1, ref Vector3D vector2)
        {
            return vector1._x * vector2._x + vector1._y * vector2._y + vector1._z * vector2._z;
        }

        public static Vector3D CrossProduct(Vector3D vector1, Vector3D vector2)
        {
            CrossProduct(ref vector1, ref vector2, out var result);
            return result;
        }

        internal static void CrossProduct(ref Vector3D vector1, ref Vector3D vector2, out Vector3D result)
        {
            result._x = vector1._y * vector2._z - vector1._z * vector2._y;
            result._y = vector1._z * vector2._x - vector1._x * vector2._z;
            result._z = vector1._x * vector2._y - vector1._y * vector2._x;
        }

        public static explicit operator Point3D(Vector3D vector)
        {
            return new Point3D(vector._x, vector._y, vector._z);
        }

        //public static explicit operator Size3D(Vector3D vector)
        //{
        //    return new Size3D(Math.Abs(vector._x), Math.Abs(vector._y), Math.Abs(vector._z));
        //}

        public static bool operator ==(Vector3D vector1, Vector3D vector2)
        {
            if (vector1.X == vector2.X && vector1.Y == vector2.Y)
            {
                return vector1.Z == vector2.Z;
            }

            return false;
        }

        public static bool operator !=(Vector3D vector1, Vector3D vector2)
        {
            return !(vector1 == vector2);
        }

        public static bool Equals(Vector3D vector1, Vector3D vector2)
        {
            if (vector1.X.Equals(vector2.X) && vector1.Y.Equals(vector2.Y))
            {
                return vector1.Z.Equals(vector2.Z);
            }

            return false;
        }

        public override bool Equals(object o)
        {
            if (o == null || !(o is Vector3D))
            {
                return false;
            }

            Vector3D vector = (Vector3D)o;
            return Equals(this, vector);
        }

        public bool Equals(Vector3D value)
        {
            return Equals(this, value);
        }

        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode() ^ Z.GetHashCode();
        }

        public static Vector3D Parse(string source)
        {
            IFormatProvider invariantEnglishUS = TypeConverterHelper.InvariantEnglishUS;//   System.Windows.Markup.TypeConverterHelper.InvariantEnglishUS;
            TokenizerHelper tokenizerHelper = new TokenizerHelper(source, invariantEnglishUS);
            string value = tokenizerHelper.NextTokenRequired();
            Vector3D result = new Vector3D(Convert.ToDouble(value, invariantEnglishUS), Convert.ToDouble(tokenizerHelper.NextTokenRequired(), invariantEnglishUS), Convert.ToDouble(tokenizerHelper.NextTokenRequired(), invariantEnglishUS));
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
            char numericListSeparator = TokenizerHelper.GetNumericListSeparator(provider);
            return string.Format(provider, "{1:" + format + "}{0}{2:" + format + "}{0}{3:" + format + "}", numericListSeparator, _x, _y, _z);
        }
    }
}

