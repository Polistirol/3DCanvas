using ParserLib.Helpers;
using ParserLib.Interfaces.Macros;
using System;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace ParserLib.Models.Macros
{
    public class SlotMove : Macro, ISlot
    {

        public override TechnoHelper.EEntityType EntityType => TechnoHelper.EEntityType.Slot;

        public Point3D Center1 { get; set; }
        public Point3D Center2 { get; set; }
        public double Radius { get; set; }
        public Point3D Normal { get; set; }

    }
}