using ParserLib.Helpers;
using ParserLib.Interfaces.Macros;
using System;
using System.Windows.Media.Media3D;

namespace ParserLib.Models.Macros
{
    public class KeyholeMoves : Macro, IKeyhole
    {


        public override TechnoHelper.EEntityType EntityType => TechnoHelper.EEntityType.Keyhole;

        public Point3D Center1 { get; set; }
        public Point3D Center2 { get; set; }
        public Point3D Normal { get; set; }


        public double BigRadius { get; set; }
        public double SmallRadius { get; set; }


    }
}
