using ParserLib.Models;
using System.Windows.Media.Media3D;

namespace ParserLib.Interfaces.Macros
{
    public interface ISlot : IMacro
    {

        Point3D Center1 { get; set; }
        Point3D Center2 { get; set; }
        Point3D Normal { get; set; }

        double Radius { get; set; }

    }
}