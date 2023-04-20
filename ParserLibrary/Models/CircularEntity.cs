using ParserLibrary.Interfaces;
using System;
using System.Windows;
//using System.Windows.Media;
using ParserLibrary.Models.Media;
using static ParserLibrary.Helpers.TechnoHelper;

namespace ParserLibrary.Models
{
    public class CircularEntity  
    {
        public bool IsStroked { get; set; }

        public bool IsRotating { get; set; }

        public bool IsLargeArc { get; set; }

        public double Radius { get; set; }// raggio della circonferenza a cui appartiene l'arco di circonferenza

        public double RotationAngle { get; set; }

        public Vector3D Normal { get; set; }// vettore normale al piano su cui giace l'arco di circonferenze

        public Point3D ViaPoint { get; set; }

        public Point3D NormalPoint { get; set; }

        public Point3D CenterPoint { get; set; }

        public Size ArcSize { get; set; }

        public SweepDirection ArcSweepDirection { get; set; }



    }
}