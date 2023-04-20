using ParserLibrary.Helpers;
using ParserLibrary.Interfaces.Macros;
using System;
using System.Windows.Media;
using ParserLibrary.Models.Media;

namespace ParserLibrary.Models.Macros
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