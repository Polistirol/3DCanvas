using ParserLibrary.Helpers;
using ParserLibrary.Interfaces.Macros;
using ParserLibrary.Models.Media;

namespace ParserLibrary.Models.Macros
{
    public class HoleMoves : Macro, IHole
    {

        public double Radius { get; set; }

        public override TechnoHelper.EEntityType EntityType => TechnoHelper.EEntityType.Hole;

        public Point3D Center { get; set; }
        public Point3D Normal { get; set; }
        public int Smooth { get; set; }
        
    }


}
