﻿using System.Windows;
//using System.Windows.Media;
using ParserLibrary.Models.Media;

namespace ParserLibrary.Interfaces
{
    public interface IArc : IToolpathEntity
    {
        Point3D ViaPoint { get; set; }
        Point3D OriginalViaPoint { get; set; }

        Point3D NormalPoint { get; set; }

        Point3D CenterPoint { get; set; }

        Size ArcSize { get; set; }

        Vector3D Normal { get; set; }// vettore normale al piano su cui giace l'arco di circonferenze

        SweepDirection ArcSweepDirection { get; set; }

        double Radius { get; set; }// raggio della circonferenza a cui appartiene l'arco di circonferenza

        double RotationAngle { get; set; }

        bool IsLargeArc { get; set; }

        bool IsStroked { get; set; }

        bool IsRotating { get; set; }
    }
}