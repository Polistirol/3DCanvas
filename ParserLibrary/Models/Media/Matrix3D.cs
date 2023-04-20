using System;
using System.ComponentModel;
using System.Windows.Markup;



namespace ParserLibrary.Models.Media
{
    [Serializable]
    [TypeConverter(typeof(Matrix3DConverter))]
    [ValueSerializer(typeof(Matrix3DValueSerializer))]
    public struct Matrix3D : IFormattable
    {
        private double _m11;

        private double _m12;

        private double _m13;

        private double _m14;

        private double _m21;

        private double _m22;

        private double _m23;

        private double _m24;

        private double _m31;

        private double _m32;

        private double _m33;

        private double _m34;

        private double _offsetX;

        private double _offsetY;

        private double _offsetZ;

        private double _m44;

        private bool _isNotKnownToBeIdentity;

        private static readonly Matrix3D s_identity = CreateIdentity();

        private const int c_identityHashCode = 0;

        public static Matrix3D Identity => s_identity;

        public bool IsIdentity
        {
            get
            {
                if (IsDistinguishedIdentity)
                {
                    return true;
                }

                if (_m11 == 1.0 && _m12 == 0.0 && _m13 == 0.0 && _m14 == 0.0 && _m21 == 0.0 && _m22 == 1.0 && _m23 == 0.0 && _m24 == 0.0 && _m31 == 0.0 && _m32 == 0.0 && _m33 == 1.0 && _m34 == 0.0 && _offsetX == 0.0 && _offsetY == 0.0 && _offsetZ == 0.0 && _m44 == 1.0)
                {
                    IsDistinguishedIdentity = true;
                    return true;
                }

                return false;
            }
        }

        public bool IsAffine
        {
            get
            {
                if (!IsDistinguishedIdentity)
                {
                    if (_m14 == 0.0 && _m24 == 0.0 && _m34 == 0.0)
                    {
                        return _m44 == 1.0;
                    }

                    return false;
                }

                return true;
            }
        }

        public double Determinant
        {
            get
            {
                if (IsDistinguishedIdentity)
                {
                    return 1.0;
                }

                if (IsAffine)
                {
                    return GetNormalizedAffineDeterminant();
                }

                double num = _m13 * _m24 - _m23 * _m14;
                double num2 = _m13 * _m34 - _m33 * _m14;
                double num3 = _m13 * _m44 - _offsetZ * _m14;
                double num4 = _m23 * _m34 - _m33 * _m24;
                double num5 = _m23 * _m44 - _offsetZ * _m24;
                double num6 = _m33 * _m44 - _offsetZ * _m34;
                double num7 = _m22 * num2 - _m32 * num - _m12 * num4;
                double num8 = _m12 * num5 - _m22 * num3 + _offsetY * num;
                double num9 = _m32 * num3 - _offsetY * num2 - _m12 * num6;
                double num10 = _m22 * num6 - _m32 * num5 + _offsetY * num4;
                return _offsetX * num7 + _m31 * num8 + _m21 * num9 + _m11 * num10;
            }
        }

        //public bool HasInverse => !DoubleUtil.IsZero(Determinant);

        public double M11
        {
            get
            {
                if (IsDistinguishedIdentity)
                {
                    return 1.0;
                }

                return _m11;
            }
            set
            {
                if (IsDistinguishedIdentity)
                {
                    this = s_identity;
                    IsDistinguishedIdentity = false;
                }

                _m11 = value;
            }
        }

        public double M12
        {
            get
            {
                return _m12;
            }
            set
            {
                if (IsDistinguishedIdentity)
                {
                    this = s_identity;
                    IsDistinguishedIdentity = false;
                }

                _m12 = value;
            }
        }

        public double M13
        {
            get
            {
                return _m13;
            }
            set
            {
                if (IsDistinguishedIdentity)
                {
                    this = s_identity;
                    IsDistinguishedIdentity = false;
                }

                _m13 = value;
            }
        }

        public double M14
        {
            get
            {
                return _m14;
            }
            set
            {
                if (IsDistinguishedIdentity)
                {
                    this = s_identity;
                    IsDistinguishedIdentity = false;
                }

                _m14 = value;
            }
        }

        public double M21
        {
            get
            {
                return _m21;
            }
            set
            {
                if (IsDistinguishedIdentity)
                {
                    this = s_identity;
                    IsDistinguishedIdentity = false;
                }

                _m21 = value;
            }
        }

        public double M22
        {
            get
            {
                if (IsDistinguishedIdentity)
                {
                    return 1.0;
                }

                return _m22;
            }
            set
            {
                if (IsDistinguishedIdentity)
                {
                    this = s_identity;
                    IsDistinguishedIdentity = false;
                }

                _m22 = value;
            }
        }

        public double M23
        {
            get
            {
                return _m23;
            }
            set
            {
                if (IsDistinguishedIdentity)
                {
                    this = s_identity;
                    IsDistinguishedIdentity = false;
                }

                _m23 = value;
            }
        }

        public double M24
        {
            get
            {
                return _m24;
            }
            set
            {
                if (IsDistinguishedIdentity)
                {
                    this = s_identity;
                    IsDistinguishedIdentity = false;
                }

                _m24 = value;
            }
        }

        public double M31
        {
            get
            {
                return _m31;
            }
            set
            {
                if (IsDistinguishedIdentity)
                {
                    this = s_identity;
                    IsDistinguishedIdentity = false;
                }

                _m31 = value;
            }
        }

        public double M32
        {
            get
            {
                return _m32;
            }
            set
            {
                if (IsDistinguishedIdentity)
                {
                    this = s_identity;
                    IsDistinguishedIdentity = false;
                }

                _m32 = value;
            }
        }

        public double M33
        {
            get
            {
                if (IsDistinguishedIdentity)
                {
                    return 1.0;
                }

                return _m33;
            }
            set
            {
                if (IsDistinguishedIdentity)
                {
                    this = s_identity;
                    IsDistinguishedIdentity = false;
                }

                _m33 = value;
            }
        }

        public double M34
        {
            get
            {
                return _m34;
            }
            set
            {
                if (IsDistinguishedIdentity)
                {
                    this = s_identity;
                    IsDistinguishedIdentity = false;
                }

                _m34 = value;
            }
        }

        public double OffsetX
        {
            get
            {
                return _offsetX;
            }
            set
            {
                if (IsDistinguishedIdentity)
                {
                    this = s_identity;
                    IsDistinguishedIdentity = false;
                }

                _offsetX = value;
            }
        }

        public double OffsetY
        {
            get
            {
                return _offsetY;
            }
            set
            {
                if (IsDistinguishedIdentity)
                {
                    this = s_identity;
                    IsDistinguishedIdentity = false;
                }

                _offsetY = value;
            }
        }

        public double OffsetZ
        {
            get
            {
                return _offsetZ;
            }
            set
            {
                if (IsDistinguishedIdentity)
                {
                    this = s_identity;
                    IsDistinguishedIdentity = false;
                }

                _offsetZ = value;
            }
        }

        public double M44
        {
            get
            {
                if (IsDistinguishedIdentity)
                {
                    return 1.0;
                }

                return _m44;
            }
            set
            {
                if (IsDistinguishedIdentity)
                {
                    this = s_identity;
                    IsDistinguishedIdentity = false;
                }

                _m44 = value;
            }
        }

        private bool IsDistinguishedIdentity
        {
            get
            {
                return !_isNotKnownToBeIdentity;
            }
            set
            {
                _isNotKnownToBeIdentity = !value;
            }
        }

        public Matrix3D(double m11, double m12, double m13, double m14, double m21, double m22, double m23, double m24, double m31, double m32, double m33, double m34, double offsetX, double offsetY, double offsetZ, double m44)
        {
            _m11 = m11;
            _m12 = m12;
            _m13 = m13;
            _m14 = m14;
            _m21 = m21;
            _m22 = m22;
            _m23 = m23;
            _m24 = m24;
            _m31 = m31;
            _m32 = m32;
            _m33 = m33;
            _m34 = m34;
            _offsetX = offsetX;
            _offsetY = offsetY;
            _offsetZ = offsetZ;
            _m44 = m44;
            _isNotKnownToBeIdentity = true;
        }

        public void SetIdentity()
        {
            this = s_identity;
        }

        public void Prepend(Matrix3D matrix)
        {
            this = matrix * this;
        }

        public void Append(Matrix3D matrix)
        {
            this *= matrix;
        }

        public void Rotate(Quaternion quaternion)
        {
            Point3D center = default(Point3D);
            this *= CreateRotationMatrix(ref quaternion, ref center);
        }

        public void RotatePrepend(Quaternion quaternion)
        {
            Point3D center = default(Point3D);
            this = CreateRotationMatrix(ref quaternion, ref center) * this;
        }

        public void RotateAt(Quaternion quaternion, Point3D center)
        {
            this *= CreateRotationMatrix(ref quaternion, ref center);
        }

        public void RotateAtPrepend(Quaternion quaternion, Point3D center)
        {
            this = CreateRotationMatrix(ref quaternion, ref center) * this;
        }

        public void Scale(Vector3D scale)
        {
            if (IsDistinguishedIdentity)
            {
                SetScaleMatrix(ref scale);
                return;
            }

            _m11 *= scale.X;
            _m12 *= scale.Y;
            _m13 *= scale.Z;
            _m21 *= scale.X;
            _m22 *= scale.Y;
            _m23 *= scale.Z;
            _m31 *= scale.X;
            _m32 *= scale.Y;
            _m33 *= scale.Z;
            _offsetX *= scale.X;
            _offsetY *= scale.Y;
            _offsetZ *= scale.Z;
        }

        public void ScalePrepend(Vector3D scale)
        {
            if (IsDistinguishedIdentity)
            {
                SetScaleMatrix(ref scale);
                return;
            }

            _m11 *= scale.X;
            _m12 *= scale.X;
            _m13 *= scale.X;
            _m14 *= scale.X;
            _m21 *= scale.Y;
            _m22 *= scale.Y;
            _m23 *= scale.Y;
            _m24 *= scale.Y;
            _m31 *= scale.Z;
            _m32 *= scale.Z;
            _m33 *= scale.Z;
            _m34 *= scale.Z;
        }

        public void ScaleAt(Vector3D scale, Point3D center)
        {
            if (IsDistinguishedIdentity)
            {
                SetScaleMatrix(ref scale, ref center);
                return;
            }

            double num = _m14 * center.X;
            _m11 = num + scale.X * (_m11 - num);
            num = _m14 * center.Y;
            _m12 = num + scale.Y * (_m12 - num);
            num = _m14 * center.Z;
            _m13 = num + scale.Z * (_m13 - num);
            num = _m24 * center.X;
            _m21 = num + scale.X * (_m21 - num);
            num = _m24 * center.Y;
            _m22 = num + scale.Y * (_m22 - num);
            num = _m24 * center.Z;
            _m23 = num + scale.Z * (_m23 - num);
            num = _m34 * center.X;
            _m31 = num + scale.X * (_m31 - num);
            num = _m34 * center.Y;
            _m32 = num + scale.Y * (_m32 - num);
            num = _m34 * center.Z;
            _m33 = num + scale.Z * (_m33 - num);
            num = _m44 * center.X;
            _offsetX = num + scale.X * (_offsetX - num);
            num = _m44 * center.Y;
            _offsetY = num + scale.Y * (_offsetY - num);
            num = _m44 * center.Z;
            _offsetZ = num + scale.Z * (_offsetZ - num);
        }

        public void ScaleAtPrepend(Vector3D scale, Point3D center)
        {
            if (IsDistinguishedIdentity)
            {
                SetScaleMatrix(ref scale, ref center);
                return;
            }

            double num = center.X - center.X * scale.X;
            double num2 = center.Y - center.Y * scale.Y;
            double num3 = center.Z - center.Z * scale.Z;
            _offsetX += _m11 * num + _m21 * num2 + _m31 * num3;
            _offsetY += _m12 * num + _m22 * num2 + _m32 * num3;
            _offsetZ += _m13 * num + _m23 * num2 + _m33 * num3;
            _m44 += _m14 * num + _m24 * num2 + _m34 * num3;
            _m11 *= scale.X;
            _m12 *= scale.X;
            _m13 *= scale.X;
            _m14 *= scale.X;
            _m21 *= scale.Y;
            _m22 *= scale.Y;
            _m23 *= scale.Y;
            _m24 *= scale.Y;
            _m31 *= scale.Z;
            _m32 *= scale.Z;
            _m33 *= scale.Z;
            _m34 *= scale.Z;
        }

        public void Translate(Vector3D offset)
        {
            if (IsDistinguishedIdentity)
            {
                SetTranslationMatrix(ref offset);
                return;
            }

            _m11 += _m14 * offset.X;
            _m12 += _m14 * offset.Y;
            _m13 += _m14 * offset.Z;
            _m21 += _m24 * offset.X;
            _m22 += _m24 * offset.Y;
            _m23 += _m24 * offset.Z;
            _m31 += _m34 * offset.X;
            _m32 += _m34 * offset.Y;
            _m33 += _m34 * offset.Z;
            _offsetX += _m44 * offset.X;
            _offsetY += _m44 * offset.Y;
            _offsetZ += _m44 * offset.Z;
        }

        public void TranslatePrepend(Vector3D offset)
        {
            if (IsDistinguishedIdentity)
            {
                SetTranslationMatrix(ref offset);
                return;
            }

            _offsetX += _m11 * offset.X + _m21 * offset.Y + _m31 * offset.Z;
            _offsetY += _m12 * offset.X + _m22 * offset.Y + _m32 * offset.Z;
            _offsetZ += _m13 * offset.X + _m23 * offset.Y + _m33 * offset.Z;
            _m44 += _m14 * offset.X + _m24 * offset.Y + _m34 * offset.Z;
        }

        public static Matrix3D operator *(Matrix3D matrix1, Matrix3D matrix2)
        {
            if (matrix1.IsDistinguishedIdentity)
            {
                return matrix2;
            }

            if (matrix2.IsDistinguishedIdentity)
            {
                return matrix1;
            }

            return new Matrix3D(matrix1._m11 * matrix2._m11 + matrix1._m12 * matrix2._m21 + matrix1._m13 * matrix2._m31 + matrix1._m14 * matrix2._offsetX, matrix1._m11 * matrix2._m12 + matrix1._m12 * matrix2._m22 + matrix1._m13 * matrix2._m32 + matrix1._m14 * matrix2._offsetY, matrix1._m11 * matrix2._m13 + matrix1._m12 * matrix2._m23 + matrix1._m13 * matrix2._m33 + matrix1._m14 * matrix2._offsetZ, matrix1._m11 * matrix2._m14 + matrix1._m12 * matrix2._m24 + matrix1._m13 * matrix2._m34 + matrix1._m14 * matrix2._m44, matrix1._m21 * matrix2._m11 + matrix1._m22 * matrix2._m21 + matrix1._m23 * matrix2._m31 + matrix1._m24 * matrix2._offsetX, matrix1._m21 * matrix2._m12 + matrix1._m22 * matrix2._m22 + matrix1._m23 * matrix2._m32 + matrix1._m24 * matrix2._offsetY, matrix1._m21 * matrix2._m13 + matrix1._m22 * matrix2._m23 + matrix1._m23 * matrix2._m33 + matrix1._m24 * matrix2._offsetZ, matrix1._m21 * matrix2._m14 + matrix1._m22 * matrix2._m24 + matrix1._m23 * matrix2._m34 + matrix1._m24 * matrix2._m44, matrix1._m31 * matrix2._m11 + matrix1._m32 * matrix2._m21 + matrix1._m33 * matrix2._m31 + matrix1._m34 * matrix2._offsetX, matrix1._m31 * matrix2._m12 + matrix1._m32 * matrix2._m22 + matrix1._m33 * matrix2._m32 + matrix1._m34 * matrix2._offsetY, matrix1._m31 * matrix2._m13 + matrix1._m32 * matrix2._m23 + matrix1._m33 * matrix2._m33 + matrix1._m34 * matrix2._offsetZ, matrix1._m31 * matrix2._m14 + matrix1._m32 * matrix2._m24 + matrix1._m33 * matrix2._m34 + matrix1._m34 * matrix2._m44, matrix1._offsetX * matrix2._m11 + matrix1._offsetY * matrix2._m21 + matrix1._offsetZ * matrix2._m31 + matrix1._m44 * matrix2._offsetX, matrix1._offsetX * matrix2._m12 + matrix1._offsetY * matrix2._m22 + matrix1._offsetZ * matrix2._m32 + matrix1._m44 * matrix2._offsetY, matrix1._offsetX * matrix2._m13 + matrix1._offsetY * matrix2._m23 + matrix1._offsetZ * matrix2._m33 + matrix1._m44 * matrix2._offsetZ, matrix1._offsetX * matrix2._m14 + matrix1._offsetY * matrix2._m24 + matrix1._offsetZ * matrix2._m34 + matrix1._m44 * matrix2._m44);
        }

        public static Matrix3D Multiply(Matrix3D matrix1, Matrix3D matrix2)
        {
            return matrix1 * matrix2;
        }

        public Point3D Transform(Point3D point)
        {
            MultiplyPoint(ref point);
            return point;
        }

        public void Transform(Point3D[] points)
        {
            if (points != null)
            {
                for (int i = 0; i < points.Length; i++)
                {
                    MultiplyPoint(ref points[i]);
                }
            }
        }

        //public Point4D Transform(Point4D point)
        //{
        //    MultiplyPoint(ref point);
        //    return point;
        //}

        //public void Transform(Point4D[] points)
        //{
        //    if (points != null)
        //    {
        //        for (int i = 0; i < points.Length; i++)
        //        {
        //            MultiplyPoint(ref points[i]);
        //        }
        //    }
        //}

        public Vector3D Transform(Vector3D vector)
        {
            MultiplyVector(ref vector);
            return vector;
        }

        public void Transform(Vector3D[] vectors)
        {
            if (vectors != null)
            {
                for (int i = 0; i < vectors.Length; i++)
                {
                    MultiplyVector(ref vectors[i]);
                }
            }
        }

        public void Invert()
        {
            if (!InvertCore())
            {
                throw new InvalidOperationException();// MS.Internal.PresentationCore.SR.Get("Matrix3D_NotInvertible", null));
            }
        }

        internal void SetScaleMatrix(ref Vector3D scale)
        {
            _m11 = scale.X;
            _m22 = scale.Y;
            _m33 = scale.Z;
            _m44 = 1.0;
            IsDistinguishedIdentity = false;
        }

        internal void SetScaleMatrix(ref Vector3D scale, ref Point3D center)
        {
            _m11 = scale.X;
            _m22 = scale.Y;
            _m33 = scale.Z;
            _m44 = 1.0;
            _offsetX = center.X - center.X * scale.X;
            _offsetY = center.Y - center.Y * scale.Y;
            _offsetZ = center.Z - center.Z * scale.Z;
            IsDistinguishedIdentity = false;
        }

        internal void SetTranslationMatrix(ref Vector3D offset)
        {
            _m11 = (_m22 = (_m33 = (_m44 = 1.0)));
            _offsetX = offset.X;
            _offsetY = offset.Y;
            _offsetZ = offset.Z;
            IsDistinguishedIdentity = false;
        }

        internal static Matrix3D CreateRotationMatrix(ref Quaternion quaternion, ref Point3D center)
        {
            Matrix3D result = s_identity;
            result.IsDistinguishedIdentity = false;
            double num = quaternion.X + quaternion.X;
            double num2 = quaternion.Y + quaternion.Y;
            double num3 = quaternion.Z + quaternion.Z;
            double num4 = quaternion.X * num;
            double num5 = quaternion.X * num2;
            double num6 = quaternion.X * num3;
            double num7 = quaternion.Y * num2;
            double num8 = quaternion.Y * num3;
            double num9 = quaternion.Z * num3;
            double num10 = quaternion.W * num;
            double num11 = quaternion.W * num2;
            double num12 = quaternion.W * num3;
            result._m11 = 1.0 - (num7 + num9);
            result._m12 = num5 + num12;
            result._m13 = num6 - num11;
            result._m21 = num5 - num12;
            result._m22 = 1.0 - (num4 + num9);
            result._m23 = num8 + num10;
            result._m31 = num6 + num11;
            result._m32 = num8 - num10;
            result._m33 = 1.0 - (num4 + num7);
            if (center.X != 0.0 || center.Y != 0.0 || center.Z != 0.0)
            {
                result._offsetX = (0.0 - center.X) * result._m11 - center.Y * result._m21 - center.Z * result._m31 + center.X;
                result._offsetY = (0.0 - center.X) * result._m12 - center.Y * result._m22 - center.Z * result._m32 + center.Y;
                result._offsetZ = (0.0 - center.X) * result._m13 - center.Y * result._m23 - center.Z * result._m33 + center.Z;
            }

            return result;
        }

        internal void MultiplyPoint(ref Point3D point)
        {
            if (!IsDistinguishedIdentity)
            {
                double x = point.X;
                double y = point.Y;
                double z = point.Z;
                point.X = x * _m11 + y * _m21 + z * _m31 + _offsetX;
                point.Y = x * _m12 + y * _m22 + z * _m32 + _offsetY;
                point.Z = x * _m13 + y * _m23 + z * _m33 + _offsetZ;
                if (!IsAffine)
                {
                    double num = x * _m14 + y * _m24 + z * _m34 + _m44;
                    point.X /= num;
                    point.Y /= num;
                    point.Z /= num;
                }
            }
        }

        //internal void MultiplyPoint(ref Point4D point)
        //{
        //    if (!IsDistinguishedIdentity)
        //    {
        //        double x = point.X;
        //        double y = point.Y;
        //        double z = point.Z;
        //        double w = point.W;
        //        point.X = x * _m11 + y * _m21 + z * _m31 + w * _offsetX;
        //        point.Y = x * _m12 + y * _m22 + z * _m32 + w * _offsetY;
        //        point.Z = x * _m13 + y * _m23 + z * _m33 + w * _offsetZ;
        //        point.W = x * _m14 + y * _m24 + z * _m34 + w * _m44;
        //    }
        //}

        internal void MultiplyVector(ref Vector3D vector)
        {
            if (!IsDistinguishedIdentity)
            {
                double x = vector.X;
                double y = vector.Y;
                double z = vector.Z;
                vector.X = x * _m11 + y * _m21 + z * _m31;
                vector.Y = x * _m12 + y * _m22 + z * _m32;
                vector.Z = x * _m13 + y * _m23 + z * _m33;
            }
        }

        internal double GetNormalizedAffineDeterminant()
        {
            double num = _m12 * _m23 - _m22 * _m13;
            double num2 = _m32 * _m13 - _m12 * _m33;
            double num3 = _m22 * _m33 - _m32 * _m23;
            return _m31 * num + _m21 * num2 + _m11 * num3;
        }

        public  bool IsZero(double value)
        {
            return Math.Abs(value) < 2.2204460492503131E-15;
        }

        internal bool NormalizedAffineInvert()
        {
            double num = _m12 * _m23 - _m22 * _m13;
            double num2 = _m32 * _m13 - _m12 * _m33;
            double num3 = _m22 * _m33 - _m32 * _m23;
            double num4 = _m31 * num + _m21 * num2 + _m11 * num3;
            if (IsZero(num4))
            {
                return false;
            }

            double num5 = _m21 * _m13 - _m11 * _m23;
            double num6 = _m11 * _m33 - _m31 * _m13;
            double num7 = _m31 * _m23 - _m21 * _m33;
            double num8 = _m11 * _m22 - _m21 * _m12;
            double num9 = _m11 * _m32 - _m31 * _m12;
            double num10 = _m11 * _offsetY - _offsetX * _m12;
            double num11 = _m21 * _m32 - _m31 * _m22;
            double num12 = _m21 * _offsetY - _offsetX * _m22;
            double num13 = _m31 * _offsetY - _offsetX * _m32;
            double num14 = _m23 * num10 - _offsetZ * num8 - _m13 * num12;
            double num15 = _m13 * num13 - _m33 * num10 + _offsetZ * num9;
            double num16 = _m33 * num12 - _offsetZ * num11 - _m23 * num13;
            double num17 = num8;
            double num18 = 0.0 - num9;
            double num19 = num11;
            double num20 = 1.0 / num4;
            _m11 = num3 * num20;
            _m12 = num2 * num20;
            _m13 = num * num20;
            _m21 = num7 * num20;
            _m22 = num6 * num20;
            _m23 = num5 * num20;
            _m31 = num19 * num20;
            _m32 = num18 * num20;
            _m33 = num17 * num20;
            _offsetX = num16 * num20;
            _offsetY = num15 * num20;
            _offsetZ = num14 * num20;
            return true;
        }

        internal bool InvertCore()
        {
            if (IsDistinguishedIdentity)
            {
                return true;
            }

            if (IsAffine)
            {
                return NormalizedAffineInvert();
            }

            double num = _m13 * _m24 - _m23 * _m14;
            double num2 = _m13 * _m34 - _m33 * _m14;
            double num3 = _m13 * _m44 - _offsetZ * _m14;
            double num4 = _m23 * _m34 - _m33 * _m24;
            double num5 = _m23 * _m44 - _offsetZ * _m24;
            double num6 = _m33 * _m44 - _offsetZ * _m34;
            double num7 = _m22 * num2 - _m32 * num - _m12 * num4;
            double num8 = _m12 * num5 - _m22 * num3 + _offsetY * num;
            double num9 = _m32 * num3 - _offsetY * num2 - _m12 * num6;
            double num10 = _m22 * num6 - _m32 * num5 + _offsetY * num4;
            double num11 = _offsetX * num7 + _m31 * num8 + _m21 * num9 + _m11 * num10;
            if (IsZero(num11))
            {
                return false;
            }

            double num12 = _m11 * num4 - _m21 * num2 + _m31 * num;
            double num13 = _m21 * num3 - _offsetX * num - _m11 * num5;
            double num14 = _m11 * num6 - _m31 * num3 + _offsetX * num2;
            double num15 = _m31 * num5 - _offsetX * num4 - _m21 * num6;
            num = _m11 * _m22 - _m21 * _m12;
            num2 = _m11 * _m32 - _m31 * _m12;
            num3 = _m11 * _offsetY - _offsetX * _m12;
            num4 = _m21 * _m32 - _m31 * _m22;
            num5 = _m21 * _offsetY - _offsetX * _m22;
            num6 = _m31 * _offsetY - _offsetX * _m32;
            double num16 = _m13 * num4 - _m23 * num2 + _m33 * num;
            double num17 = _m23 * num3 - _offsetZ * num - _m13 * num5;
            double num18 = _m13 * num6 - _m33 * num3 + _offsetZ * num2;
            double num19 = _m33 * num5 - _offsetZ * num4 - _m23 * num6;
            double num20 = _m24 * num2 - _m34 * num - _m14 * num4;
            double num21 = _m14 * num5 - _m24 * num3 + _m44 * num;
            double num22 = _m34 * num3 - _m44 * num2 - _m14 * num6;
            double num23 = _m24 * num6 - _m34 * num5 + _m44 * num4;
            double num24 = 1.0 / num11;
            _m11 = num10 * num24;
            _m12 = num9 * num24;
            _m13 = num8 * num24;
            _m14 = num7 * num24;
            _m21 = num15 * num24;
            _m22 = num14 * num24;
            _m23 = num13 * num24;
            _m24 = num12 * num24;
            _m31 = num23 * num24;
            _m32 = num22 * num24;
            _m33 = num21 * num24;
            _m34 = num20 * num24;
            _offsetX = num19 * num24;
            _offsetY = num18 * num24;
            _offsetZ = num17 * num24;
            _m44 = num16 * num24;
            return true;
        }

        private static Matrix3D CreateIdentity()
        {
            Matrix3D result = new Matrix3D(1.0, 0.0, 0.0, 0.0, 0.0, 1.0, 0.0, 0.0, 0.0, 0.0, 1.0, 0.0, 0.0, 0.0, 0.0, 1.0);
            result.IsDistinguishedIdentity = true;
            return result;
        }

        public static bool operator ==(Matrix3D matrix1, Matrix3D matrix2)
        {
            if (matrix1.IsDistinguishedIdentity || matrix2.IsDistinguishedIdentity)
            {
                return matrix1.IsIdentity == matrix2.IsIdentity;
            }

            if (matrix1.M11 == matrix2.M11 && matrix1.M12 == matrix2.M12 && matrix1.M13 == matrix2.M13 && matrix1.M14 == matrix2.M14 && matrix1.M21 == matrix2.M21 && matrix1.M22 == matrix2.M22 && matrix1.M23 == matrix2.M23 && matrix1.M24 == matrix2.M24 && matrix1.M31 == matrix2.M31 && matrix1.M32 == matrix2.M32 && matrix1.M33 == matrix2.M33 && matrix1.M34 == matrix2.M34 && matrix1.OffsetX == matrix2.OffsetX && matrix1.OffsetY == matrix2.OffsetY && matrix1.OffsetZ == matrix2.OffsetZ)
            {
                return matrix1.M44 == matrix2.M44;
            }

            return false;
        }

        public static bool operator !=(Matrix3D matrix1, Matrix3D matrix2)
        {
            return !(matrix1 == matrix2);
        }

        public static bool Equals(Matrix3D matrix1, Matrix3D matrix2)
        {
            if (matrix1.IsDistinguishedIdentity || matrix2.IsDistinguishedIdentity)
            {
                return matrix1.IsIdentity == matrix2.IsIdentity;
            }

            if (matrix1.M11.Equals(matrix2.M11) && matrix1.M12.Equals(matrix2.M12) && matrix1.M13.Equals(matrix2.M13) && matrix1.M14.Equals(matrix2.M14) && matrix1.M21.Equals(matrix2.M21) && matrix1.M22.Equals(matrix2.M22) && matrix1.M23.Equals(matrix2.M23) && matrix1.M24.Equals(matrix2.M24) && matrix1.M31.Equals(matrix2.M31) && matrix1.M32.Equals(matrix2.M32) && matrix1.M33.Equals(matrix2.M33) && matrix1.M34.Equals(matrix2.M34) && matrix1.OffsetX.Equals(matrix2.OffsetX) && matrix1.OffsetY.Equals(matrix2.OffsetY) && matrix1.OffsetZ.Equals(matrix2.OffsetZ))
            {
                return matrix1.M44.Equals(matrix2.M44);
            }

            return false;
        }

        public override bool Equals(object o)
        {
            if (o == null || !(o is Matrix3D))
            {
                return false;
            }

            Matrix3D matrix = (Matrix3D)o;
            return Equals(this, matrix);
        }

        public bool Equals(Matrix3D value)
        {
            return Equals(this, value);
        }

        public override int GetHashCode()
        {
            if (IsDistinguishedIdentity)
            {
                return 0;
            }

            return M11.GetHashCode() ^ M12.GetHashCode() ^ M13.GetHashCode() ^ M14.GetHashCode() ^ M21.GetHashCode() ^ M22.GetHashCode() ^ M23.GetHashCode() ^ M24.GetHashCode() ^ M31.GetHashCode() ^ M32.GetHashCode() ^ M33.GetHashCode() ^ M34.GetHashCode() ^ OffsetX.GetHashCode() ^ OffsetY.GetHashCode() ^ OffsetZ.GetHashCode() ^ M44.GetHashCode();
        }

        public static Matrix3D Parse(string source)
        {
            IFormatProvider invariantEnglishUS = TypeConverterHelper.InvariantEnglishUS;//   System.Windows.Markup.TypeConverterHelper.InvariantEnglishUS;
            TokenizerHelper tokenizerHelper = new TokenizerHelper(source, invariantEnglishUS);
            string text = tokenizerHelper.NextTokenRequired();
            Matrix3D result = ((!(text == "Identity")) ? new Matrix3D(Convert.ToDouble(text, invariantEnglishUS), Convert.ToDouble(tokenizerHelper.NextTokenRequired(), invariantEnglishUS), Convert.ToDouble(tokenizerHelper.NextTokenRequired(), invariantEnglishUS), Convert.ToDouble(tokenizerHelper.NextTokenRequired(), invariantEnglishUS), Convert.ToDouble(tokenizerHelper.NextTokenRequired(), invariantEnglishUS), Convert.ToDouble(tokenizerHelper.NextTokenRequired(), invariantEnglishUS), Convert.ToDouble(tokenizerHelper.NextTokenRequired(), invariantEnglishUS), Convert.ToDouble(tokenizerHelper.NextTokenRequired(), invariantEnglishUS), Convert.ToDouble(tokenizerHelper.NextTokenRequired(), invariantEnglishUS), Convert.ToDouble(tokenizerHelper.NextTokenRequired(), invariantEnglishUS), Convert.ToDouble(tokenizerHelper.NextTokenRequired(), invariantEnglishUS), Convert.ToDouble(tokenizerHelper.NextTokenRequired(), invariantEnglishUS), Convert.ToDouble(tokenizerHelper.NextTokenRequired(), invariantEnglishUS), Convert.ToDouble(tokenizerHelper.NextTokenRequired(), invariantEnglishUS), Convert.ToDouble(tokenizerHelper.NextTokenRequired(), invariantEnglishUS), Convert.ToDouble(tokenizerHelper.NextTokenRequired(), invariantEnglishUS)) : Identity);
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
            return string.Format(provider, "{1:" + format + "}{0}{2:" + format + "}{0}{3:" + format + "}{0}{4:" + format + "}{0}{5:" + format + "}{0}{6:" + format + "}{0}{7:" + format + "}{0}{8:" + format + "}{0}{9:" + format + "}{0}{10:" + format + "}{0}{11:" + format + "}{0}{12:" + format + "}{0}{13:" + format + "}{0}{14:" + format + "}{0}{15:" + format + "}{0}{16:" + format + "}", numericListSeparator, _m11, _m12, _m13, _m14, _m21, _m22, _m23, _m24, _m31, _m32, _m33, _m34, _offsetX, _offsetY, _offsetZ, _m44);
        }
    }
}

