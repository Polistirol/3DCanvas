using ParserLibrary.Helpers;
using ParserLibrary.Interfaces;
using System;
using System.Security.Policy;
using System.Windows.Media;
using ParserLibrary.Models.Media;

namespace ParserLibrary.Models
{
    public abstract class ToolpathEntity : ViewModelBase, IToolpathEntity
    {
        private Point3D _endPoint;
        private Point3D _startPoint;
        public Point3D StartPoint
        { get { return _startPoint; } set { _startPoint = value; } }
        public Point3D EndPoint
        { get { return _endPoint; } set { _endPoint = value; } }
        //public PathGeometry GeometryPath { get; set; } = new PathGeometry();
        public object GeometryPath { get; set; } 

        //public virtual Tuple<double, double, double, double> BoundingBox  => new Tuple<double, double, double, double>(GeometryPath.Bounds.Left, GeometryPath.Bounds.Right, GeometryPath.Bounds.Bottom, GeometryPath.Bounds.Top);
        public BoundingBox BoundingBox { get; set; } = new BoundingBox();
        public TechnoHelper.ELineType LineColor { get; set; }

        public abstract TechnoHelper.EEntityType EntityType { get; }

        public int SourceLine { get; set; }
        public bool IsBeamOn { get; set; }
        public bool Is2DProgram { get; set; }
        public string OriginalLine { get; set; }
        public bool IsLeadIn { get; set; }

        public object Tag { get; set; }
        public Point3D OriginalEndPoint { get; set; }
        public Point3D OriginalStartPoint { get; set; }

        public abstract void Render(Matrix3D U, Matrix3D Un, bool isRot, double Zradius);
    }
}