using ParserLibrary.Models;
using System.Collections.Generic;
using ParserLibrary.Models.Media;

namespace ParserLibrary.Interfaces.Macros
{
    public interface IRect : IMacro
    {
        Point3D SidePoint { get; set; }
        Point3D CenterPoint { get; set; }
        Point3D VertexPoint { get; set; }

    }
}