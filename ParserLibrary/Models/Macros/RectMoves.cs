using ParserLibrary.Helpers;
using ParserLibrary.Interfaces.Macros;
using System;
using System.Collections.Generic;
using ParserLibrary.Models.Media;

namespace ParserLibrary.Models.Macros
{
    public class RectMoves : Macro, IRect
    {
        public Point3D SidePoint { get; set; }
        public Point3D CenterPoint { get; set; }
        public Point3D VertexPoint { get; set; }

        public override TechnoHelper.EEntityType EntityType => TechnoHelper.EEntityType.Rect;


    }
}