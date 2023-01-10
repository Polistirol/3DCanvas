using ParserLib.Helpers;
using ParserLib.Interfaces.Macros;
using System;
using System.Collections.Generic;
using System.Windows.Media.Media3D;

namespace ParserLib.Models.Macros
{
    public class PolyMoves : Macro, IPoly
    {
        public int Sides { get; set; }

        public Point3D VertexPoint { get; set; }

        public Point3D NormalPoint { get; set; }
        public Point3D CenterPoint { get; set; }


        public override TechnoHelper.EEntityType EntityType => TechnoHelper.EEntityType.Poly;





    }
}