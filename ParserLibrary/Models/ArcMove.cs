﻿using ParserLibrary.Interfaces;
using ParserLibrary.Models.Media;
using System;
using System.Windows;
//using System.Windows.Media;

using static ParserLibrary.Helpers.TechnoHelper;

namespace ParserLibrary.Models
{
    public class ArcMove : ToolpathEntity, IArc
    {

        private double strokeThickness = 1;
        private double degreeToRad = Math.PI / 180;
        private Vector3D vpn = new Vector3D(0, 0, 1);
        private Vector VectorForRotationAngleCalculation = new Vector(1, 0);


        public bool IsStroked { get; set; }

        public bool IsRotating { get; set; }

        public bool IsLargeArc { get; set; }

        public double Radius { get; set; }// raggio della circonferenza a cui appartiene l'arco di circonferenza

        public double RotationAngle { get; set; }

        public Vector3D Normal { get; set; }// vettore normale al piano su cui giace l'arco di circonferenze

        public Point3D ViaPoint { get; set; }
        public Point3D OriginalViaPoint { get; set; }
        public Point3D NormalPoint { get; set; }

        public Point3D CenterPoint { get; set; }

        public Size ArcSize { get; set; }

        public SweepDirection ArcSweepDirection { get; set; }


        public override EEntityType EntityType { get => EEntityType.Arc; }


        public override void Render(Matrix3D U, Matrix3D Un, bool isRot, double Zradius)
        {
            Normal = Un.Transform(Normal);
            Normal = Vector3D.Multiply(1 / Normal.Length, Normal);

            CenterPoint = U.Transform(CenterPoint);
            StartPoint = U.Transform(StartPoint);
            EndPoint = U.Transform(EndPoint);
            Radius *= Zradius;

            var angleBetweenNormalAndViewPlaneNormal = Vector3D.AngleBetween(vpn, Normal);

            Vector3D intersection = Vector3D.CrossProduct(vpn, Normal);
            intersection = Vector3D.Multiply(1 / intersection.Length, intersection);

            double e = Radius * degreeToRad * Math.Abs(angleBetweenNormalAndViewPlaneNormal - 90);
            RotationAngle = -Vector.AngleBetween(new Vector(intersection.X, intersection.Y), VectorForRotationAngleCalculation);

            if (e < strokeThickness / 1000)
            {
                Point3D StartPointDump = new Point3D(StartPoint.X, StartPoint.Y, StartPoint.Z);
                Point3D EndPointDump = new Point3D(EndPoint.X, EndPoint.Y, EndPoint.Z);

                Point3D SP = new Point3D(StartPoint.X, StartPoint.Y, StartPoint.Z);
                Point3D EP = new Point3D(EndPoint.X, EndPoint.Y, EndPoint.Z);

                bool IsLargeDump = IsLargeArc;

                Vector3D vZ = Normal;
                Vector3D vY = intersection;
                Vector3D vX = Vector3D.CrossProduct(Normal, intersection);
                vX = Vector3DMultiply(vX);

                Point3D Px = new Point3D(1, 0, 0);
                Point3D Py = new Point3D(0, 1, 0);
                Point3D Pz = new Point3D(0, 0, 1);

                Point3D revCenterPoint = new Point3D(-CenterPoint.X, -CenterPoint.Y, -CenterPoint.Z);
                revCenterPoint = Point3DDotProduct(revCenterPoint, vX, vY, vZ);

                Px = Point3DSubtract(Px, CenterPoint);
                Px = Point3DDotProduct(Px, vX, vY, vZ);

                Py = Point3DSubtract(Py, CenterPoint);
                Py = Point3DDotProduct(Py, vX, vY, vZ);

                Pz = Point3DSubtract(Pz, CenterPoint);
                Pz = Point3DDotProduct(Pz, vX, vY, vZ);

                SP = Point3DSubtract(SP, CenterPoint);
                SP = Point3DDotProduct(SP, vX, vY, vZ);

                EP = Point3DSubtract(EP, CenterPoint);
                EP = Point3DDotProduct(EP, vX, vY, vZ);

                double SP_ang = Math.Atan2(SP.Y, SP.X);
                double EP_ang = Math.Atan2(EP.Y, EP.X);

                if (SP_ang < 0) { SP_ang = Math.PI * 2 + SP_ang; }
                if (EP_ang < 0) { EP_ang = Math.PI * 2 + EP_ang; }

                double maxY = Radius;
                double minY = -Radius;

                int qIni = (int)(SP_ang / (Math.PI / 2)) + 1;
                int qEnd = (int)(EP_ang / (Math.PI / 2)) + 1;

                if (((qIni == qEnd) && (!IsLargeArc)) || (qIni == 1 && qEnd == 4) || (qIni == 3 && qEnd == 2)) { maxY = Math.Max(SP.Y, EP.Y); minY = Math.Min(SP.Y, EP.Y); }
                else if ((qIni == 2 && qEnd == 1) || (qIni == 2 && qEnd == 4) || (qIni == 3 && qEnd == 1) || (qIni == 3 && qEnd == 4)) { maxY = Radius; minY = Math.Min(SP.Y, EP.Y); }
                else if ((qIni == 1 && qEnd == 3) || (qIni == 1 && qEnd == 2) || (qIni == 4 && qEnd == 3) || (qIni == 4 && qEnd == 2)) { maxY = Math.Max(SP.Y, EP.Y); minY = -Radius; }

                SP = new Point3D(0, minY, 0);
                EP = new Point3D(0, maxY, 0);

                Vector3D vX_ = Point3D.Subtract(Px, revCenterPoint);
                vX_ = Vector3DMultiply(vX_);

                Vector3D vY_ = Point3D.Subtract(Py, revCenterPoint);
                vY_ = Vector3DMultiply(vY_);

                Vector3D vZ_ = Point3D.Subtract(Pz, revCenterPoint);
                vZ_ = Vector3DMultiply(vZ_);

                SP = Point3DSubtract(SP, revCenterPoint);
                SP = Point3DDotProduct(SP, vX_, vY_, vZ_);

                EP = Point3DSubtract(EP, revCenterPoint);
                EP = Point3DDotProduct(EP, vX_, vY_, vZ_);

                StartPoint = SP;
                EndPoint = EP;
                IsLargeArc = false;
                ArcSize = new Size(1000, 1);
                ArcSweepDirection = Normal.Z >= 0 ? SweepDirection.Clockwise : SweepDirection.Counterclockwise;
                RefreshArcProperties();

                StartPoint = StartPointDump;
                EndPoint = EndPointDump;
                IsLargeArc = IsLargeDump;
            }
            else
            {
                ArcSize = new Size(Radius, Math.Abs(Radius * Math.Cos(angleBetweenNormalAndViewPlaneNormal * degreeToRad)));
                ArcSweepDirection = Normal.Z >= 0 ? SweepDirection.Clockwise : SweepDirection.Counterclockwise;

                RefreshArcProperties();
            }
        }

        private void RefreshArcProperties()
        {
            OnPropertyChanged("StartPoint");
            OnPropertyChanged("EndPoint");
            OnPropertyChanged("RotationAngle");
            OnPropertyChanged("ArcSize");
            OnPropertyChanged("ArcSweepDirection");
            OnPropertyChanged("IsLargeArc");
        }

        private Vector3D Vector3DMultiply(Vector3D v)
        {
            return Vector3D.Multiply(1 / v.Length, v);
        }

        private Point3D Point3DDotProduct(Point3D p, Vector3D vX, Vector3D vY, Vector3D vZ)
        {
            return new Point3D(Vector3D.DotProduct((Vector3D)p, vX), Vector3D.DotProduct((Vector3D)p, vY), Vector3D.DotProduct((Vector3D)p, vZ));
        }

        private Point3D Point3DSubtract(Point3D p, Point3D pointCenter)
        {
            return (Point3D)Point3D.Subtract(p, pointCenter);
        }

        public override string ToString()
        {
            return $"Arc sp: {StartPoint} vp: {ViaPoint} ep: {EndPoint}";
        }
    }
}