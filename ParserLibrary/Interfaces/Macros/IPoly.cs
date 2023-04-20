using ParserLibrary.Models;
using System.Collections.Generic;
using ParserLibrary.Models.Media;

namespace ParserLibrary.Interfaces.Macros
{
    public interface IPoly : IMacro
    {
        int Sides { get; set; }
        Point3D NormalPoint { get; set; }
        Point3D CenterPoint { get; set; }
        Point3D VertexPoint { get; set; }

    }
}