using ParserLib.Models;
using System.Collections.Generic;
using System.Windows.Media.Media3D;

namespace ParserLib.Interfaces.Macros
{
    public interface IRect : IMacro
    {
        Point3D SidePoint { get; set; }
        Point3D CenterPoint { get; set; }
        Point3D VertexPoint { get; set; }

    }
}