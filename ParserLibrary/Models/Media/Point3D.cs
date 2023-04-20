using System;
using System.ComponentModel;
using System.Windows.Markup;


namespace ParserLibrary.Models.Media
{
    [Serializable]
    [ValueSerializer(typeof(Point3DValueSerializer))]
    [TypeConverter(typeof(Point3DConverter))]
    public struct Point3D : IFormattable
    {
        internal double _x;

        internal double _y;

        internal double _z;

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

        public Point3D(double x, double y, double z)
        {
            _x = x;
            _y = y;
            _z = z;
        }

        public void Offset(double offsetX, double offsetY, double offsetZ)
        {
            _x += offsetX;
            _y += offsetY;
            _z += offsetZ;
        }

        public static Point3D operator +(Point3D point, Vector3D vector)
        {
            return new Point3D(point._x + vector._x, point._y + vector._y, point._z + vector._z);
        }

        public static Point3D Add(Point3D point, Vector3D vector)
        {
            return new Point3D(point._x + vector._x, point._y + vector._y, point._z + vector._z);
        }

        public static Point3D operator -(Point3D point, Vector3D vector)
        {
            return new Point3D(point._x - vector._x, point._y - vector._y, point._z - vector._z);
        }

        public static Point3D Subtract(Point3D point, Vector3D vector)
        {
            return new Point3D(point._x - vector._x, point._y - vector._y, point._z - vector._z);
        }

        public static Vector3D operator -(Point3D point1, Point3D point2)
        {
            return new Vector3D(point1._x - point2._x, point1._y - point2._y, point1._z - point2._z);
        }

        public static Vector3D Subtract(Point3D point1, Point3D point2)
        {
            Vector3D result = default(Vector3D);
            Subtract(ref point1, ref point2, out result);
            return result;
        }

        internal static void Subtract(ref Point3D p1, ref Point3D p2, out Vector3D result)
        {
            result._x = p1._x - p2._x;
            result._y = p1._y - p2._y;
            result._z = p1._z - p2._z;
        }

        public static Point3D operator *(Point3D point, Matrix3D matrix)
        {
            return matrix.Transform(point);
        }

        public static Point3D Multiply(Point3D point, Matrix3D matrix)
        {
            return matrix.Transform(point);
        }

        public static explicit operator Vector3D(Point3D point)
        {
            return new Vector3D(point._x, point._y, point._z);
        }

        //public static explicit operator Point4D(Point3D point)
        //{
        //    return new Point4D(point._x, point._y, point._z, 1.0);
        //}

        public static bool operator ==(Point3D point1, Point3D point2)
        {
            if (point1.X == point2.X && point1.Y == point2.Y)
            {
                return point1.Z == point2.Z;
            }

            return false;
        }

        public static bool operator !=(Point3D point1, Point3D point2)
        {
            return !(point1 == point2);
        }

        public static bool Equals(Point3D point1, Point3D point2)
        {
            if (point1.X.Equals(point2.X) && point1.Y.Equals(point2.Y))
            {
                return point1.Z.Equals(point2.Z);
            }

            return false;
        }

        public override bool Equals(object o)
        {
            if (o == null || !(o is Point3D))
            {
                return false;
            }

            Point3D point = (Point3D)o;
            return Equals(this, point);
        }

        public bool Equals(Point3D value)
        {
            return Equals(this, value);
        }

        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode() ^ Z.GetHashCode();
        }

        public static Point3D Parse(string source)
        {
            IFormatProvider invariantEnglishUS = TypeConverterHelper.InvariantEnglishUS;//   System.Windows.Markup.TypeConverterHelper.InvariantEnglishUS;
            TokenizerHelper tokenizerHelper = new TokenizerHelper(source, invariantEnglishUS);
            string value = tokenizerHelper.NextTokenRequired();
            Point3D result = new Point3D(Convert.ToDouble(value, invariantEnglishUS), Convert.ToDouble(tokenizerHelper.NextTokenRequired(), invariantEnglishUS), Convert.ToDouble(tokenizerHelper.NextTokenRequired(), invariantEnglishUS));
            tokenizerHelper.LastTokenRequired();
            return result;
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            throw new NotImplementedException();
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


