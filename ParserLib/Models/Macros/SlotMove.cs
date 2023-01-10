using ParserLib.Helpers;
using ParserLib.Interfaces.Macros;
using System;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace ParserLib.Models.Macros
{
    public class SlotMove : Macro, ISlot
    {
        //public CircularEntity Arc1 { get; set; }
        //public CircularEntity Arc2 { get; set; }
        //public ToolpathEntity Line1 { get; set; }
        //public ToolpathEntity Line2 { get; set; }

        public Point3D EndPoint { get; set; }
        public Point3D StartPoint { get; set; }
        public PathGeometry GeometryPath { get; set; }



        public override TechnoHelper.EEntityType EntityType => TechnoHelper.EEntityType.Slot;

        public Point3D Center1 { get; set; }
        public Point3D Center2 { get; set; }
        public double Radius { get; set; }


        //public override TechnoHelper.EEntityType EntityType { get; set; }
    }
}