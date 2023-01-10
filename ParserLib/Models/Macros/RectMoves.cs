using ParserLib.Helpers;
using ParserLib.Interfaces.Macros;
using System;
using System.Collections.Generic;
using System.Windows.Media.Media3D;

namespace ParserLib.Models.Macros
{
    public class RectMoves : Macro, IRect
    {
        public Point3D SidePoint { get; set; }
        public Point3D CenterPoint { get; set; }
        public Point3D VertexPoint { get; set; }

        public override TechnoHelper.EEntityType EntityType => TechnoHelper.EEntityType.Rect;


    }
}